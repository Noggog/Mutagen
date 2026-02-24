# Lazy Decompression for Binary Overlays - Implementation Plan

**Issue:** https://github.com/Mutagen-Modding/Mutagen/issues/74

## Problem Statement

Major Record headers (Type, Length, Flags, FormID) are never compressed — only content after the header is compressed. The goal is lazy, thread-safe, one-time decompression triggered **only** when a subrecord field is first accessed.

This benefits both compressed and non-compressed records:
- **Compressed records:** Skip decompression entirely when only header fields are accessed
- **All major records:** Skip `FillSubrecordTypes` scanning when only header fields are accessed (e.g., iterating FormIDs, filtering by flags, building indexes)

---

## Current Implementation Status

The following infrastructure has been built on the `lazy-decompression` branch:

| Component | Status | Notes |
|-----------|--------|-------|
| `LazyMajorRecordData` | Done | Lazy decompression via `Lazy<byte[]>`, exposes `StructData` (always available) and `RecordData` (lazy decompressed) |
| `PluginBinaryOverlay` base class | Done | `_lazySubrecordInit`, `SetLazySubrecordInit()`, `CreateSubrecordStream()`, modified `ExtractRecordMemoryLazy()` |
| Code generation (factory methods) | Done | `PluginTranslationModule.cs` generates if/else for compressed vs non-compressed |
| Custom overlay files | Done | Cell (Skyrim/FO4/Starfield), ImageSpace (FO4) updated |
| Generated code | Done | All `*_Generated.cs` regenerated with lazy pattern |

---

## Critical Finding: Accessor Short-Circuit Bug

### The Problem

The current implementation relies on `_recordData` being accessed to trigger lazy initialization. However, many generated field accessors **never access `_recordData`** when the field hasn't been populated yet, due to C# short-circuit evaluation:

```csharp
// Generated accessor pattern — BROKEN for lazy init
public ITranslatedStringGetter? Name => _NameLocation.HasValue       // ← Checked FIRST
    ? ...(_recordData...)                                             // ← Only reached if HasValue is true
    : default;                                                        // ← Returns null, _recordData never touched
```

For compressed records where `FillSubrecordTypes` hasn't run yet:
1. `_NameLocation` is null (hasn't been populated)
2. `_NameLocation.HasValue` → false
3. Returns `default` (null) → **WRONG**, record may have a Name
4. `_recordData` is never accessed → lazy init **never fires**

### Which Patterns Are Affected

**Pattern A — Always evaluates `_recordData` (WORKS):**
```csharp
// FormLink fields — _recordData is a method argument, always evaluated before the call
public IFormLinkNullableGetter<T> DeathItem =>
    FormLinkBinaryTranslation.Instance.NullableRecordOverlayFactory<T>(
        _package, _recordData, _DeathItemLocation);
```

**Pattern B — Short-circuits past `_recordData` (BROKEN):**
```csharp
// Strings, translated strings, complex sub-objects, primitives with HasValue ternary
public ITranslatedStringGetter? Name => _NameLocation.HasValue
    ? ...(_recordData...) : default;
```

Pattern B affects many common field types: strings, translated strings, complex sub-objects (`ObjectBounds`, `Configuration`, `AIData`), and primitives (`Height`, `Weight`, etc.).

### Root Cause

The fundamental issue is that **location fields and record data live separately**. You can check a location field (`_NameLocation`) without going through the lazy init gate (`_recordData`). No amount of accessor discipline can fully prevent this — the code generator could always produce a pattern that accidentally bypasses the gate.

---

## Proposed Solution: RecordDataPayload

### Core Idea

Bundle all subrecord-related state — location fields, length overrides, directly-assigned properties, AND decompressed record data — into a single per-type payload class. All subrecord accessors must go through the payload, which is the single lazy initialization gate. **Correctness becomes structural — you cannot access any location without triggering init.**

### Architecture

```
Factory Creation (no decompression, no FillSubrecordTypes):
├── Store original compressed bytes in LazyMajorRecordData
├── Pre-allocate empty RecordDataPayload on the overlay
├── Create Lazy<bool> that will decompress + FillSubrecordTypes → populate payload
└── Return overlay immediately

Header Access (e.g., FormID, Flags):
└── Read from _structData (via LazyMajorRecordData.StructData) — no init triggered

First Subrecord Access (e.g., EditorID, Name, Factions):
├── Accessor reads from Payload property
├── Payload getter triggers Lazy<bool> (one-time):
│   ├── Decompress bytes (LazyMajorRecordData.RecordData)
│   ├── Set payload.RecordData = decompressed data
│   ├── Create OverlayStream from decompressed content
│   ├── Call CustomFactoryEnd
│   └── Call FillSubrecordTypes → populates payload location fields + properties
├── Returns pre-allocated payload (now populated)
└── Accessor reads location + data from payload

Subsequent Accesses:
└── Lazy<bool> already evaluated, Payload returns immediately
```

### Generated Code Shape

Per-overlay-type payload class:

```csharp
// Generated for each major record overlay type
internal sealed class NpcRecordDataPayload
{
    // Decompressed content — set during init
    public ReadOnlyMemorySlice<byte> RecordData;

    // Location fields — set by FillRecordType during init
    public RangeInt32? VirtualMachineAdapterLocation;
    public int? VirtualMachineAdapterLengthOverride;
    public RangeInt32? ObjectBoundsLocation;
    public RangeInt32? ConfigurationLocation;
    public int? DeathItemLocation;
    public int? VoiceLocation;
    public int? NameLocation;
    // ... all location/override fields for this type ...

    // Directly-assigned properties — set by FillRecordType during init
    public IReadOnlyList<IRankPlacementGetter>? Factions;
    public IDestructibleGetter? Destructible;
    public IReadOnlyList<IAttackGetter>? Attacks;
    // ... all directly-assigned properties for this type ...
}
```

Overlay class changes:

```csharp
internal partial class NpcBinaryOverlay
{
    // Pre-allocated payload — fields populated during lazy init
    private readonly NpcRecordDataPayload _payload = new();

    // Lazy trigger — evaluates once, populates _payload
    private Lazy<bool>? _lazyPayloadInit;

    // Single access point — ensures init before returning payload
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal NpcRecordDataPayload Payload
    {
        get
        {
            if (_lazyPayloadInit != null)
            {
                _ = _lazyPayloadInit.Value;
            }
            return _payload;
        }
    }
}
```

Accessor generation:

```csharp
// ALL subrecord accessors go through Payload — structurally impossible to bypass

// Pattern B fields (formerly broken) — now correct:
public ITranslatedStringGetter? Name
{
    get
    {
        var p = Payload;
        return p.NameLocation.HasValue
            ? StringBinaryTranslation.Instance.Parse(
                HeaderTranslation.ExtractSubrecordMemory(p.RecordData, p.NameLocation.Value, _package.MetaData.Constants),
                StringsSource.Normal, parsingBundle: _package.MetaData, eager: false)
            : default;
    }
}

// Pattern A fields — also go through Payload for consistency:
public IFormLinkNullableGetter<ILeveledItemGetter> DeathItem =>
    FormLinkBinaryTranslation.Instance.NullableRecordOverlayFactory<ILeveledItemGetter>(
        _package, Payload.RecordData, Payload.DeathItemLocation);

// Directly-assigned properties — also through Payload:
public IReadOnlyList<IRankPlacementGetter>? Factions => Payload.Factions;
```

FillRecordType changes:

```csharp
// FillRecordType writes to _payload instead of this._XyzLocation
public override ParseResult FillRecordType(
    OverlayStream stream, int finalPos, int offset,
    RecordType type, PreviousParse lastParsed,
    Dictionary<RecordType, int>? recordParseCount,
    TypedParseParams translationParams = default)
{
    type = translationParams.ConvertToStandard(type);
    switch (type.TypeInt)
    {
        case RecordTypeInts.INAM:
            _payload.DeathItemLocation = (stream.Position - offset);
            return (int)Npc_FieldIndex.DeathItem;
        case RecordTypeInts.SNAM:
            _payload.Factions = BinaryOverlayList.FactoryByArray<IRankPlacementGetter>(...);
            return (int)Npc_FieldIndex.Factions;
        // ... etc
    }
}
```

Factory generation:

```csharp
public static INpcGetter NpcFactory(
    OverlayStream stream,
    BinaryOverlayFactoryPackage package,
    TypedParseParams translationParams = default)
{
    PluginBinaryOverlay.ExtractRecordMemoryLazy(
        stream, package.MetaData.Constants,
        out var lazyRecordData, out var originalSlice,
        out var offset, out var totalLength);

    var ret = new NpcBinaryOverlay(lazyRecordData, package);
    ret._package.FormVersion = ret;

    // ALL major records get lazy init (compressed AND non-compressed)
    ret._lazyPayloadInit = new Lazy<bool>(() =>
    {
        ret._payload.RecordData = lazyRecordData.RecordData; // Triggers decompression if compressed
        var subStream = lazyRecordData.IsCompressed
            ? PluginBinaryOverlay.CreateSubrecordStream(lazyRecordData, originalSlice, package.MetaData.Constants, package, out var finalPos)
            : CreateNonCompressedStream(originalSlice, package, offset, lazyRecordData, out finalPos);
        ret.CustomFactoryEnd(stream: subStream, finalPos: finalPos, offset: offset);
        ret.FillSubrecordTypes(
            majorReference: ret, stream: subStream, finalPos: finalPos,
            offset: offset, translationParams: translationParams, fill: ret.FillRecordType);
        return true;
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    return ret;
}
```

> **Note:** Both compressed AND non-compressed records use lazy init. For non-compressed records, the init cost is just scanning subrecord headers (no decompression). The benefit is that header-only access patterns (iterating FormIDs, filtering by flags) skip `FillSubrecordTypes` entirely.

---

## Implementation Tasks

### Task 1: Generate RecordDataPayload Classes

**File:** `Mutagen.Bethesda.Generation/Modules/Plugin/PluginTranslationModule.cs`

For each major record overlay, generate an `internal sealed class {Type}RecordDataPayload`:
- A `ReadOnlyMemorySlice<byte> RecordData` field
- All `int?` / `RangeInt32?` location fields (currently instance fields on the overlay)
- All `int?` length override fields
- All directly-assigned properties (lists, sub-overlays set in `FillRecordType`)

### Task 2: Add Payload Infrastructure to Overlays

**File:** `Mutagen.Bethesda.Generation/Modules/Plugin/PluginTranslationModule.cs`

For each major record overlay, generate:
- `private readonly {Type}RecordDataPayload _payload = new();`
- `private Lazy<bool>? _lazyPayloadInit;`
- `internal {Type}RecordDataPayload Payload { get { ... trigger lazy ... return _payload; } }`

### Task 3: Update Accessor Generation

**File:** Code generation modules that produce field accessors

Change all subrecord-based property accessors to read from `Payload` instead of directly from `_recordData` / `this._XyzLocation`:
- Location-checked fields: `Payload.XyzLocation` and `Payload.RecordData`
- FormLink fields: `Payload.RecordData` and `Payload.XyzLocation`
- Directly-assigned properties: `Payload.Xyz`

### Task 4: Update FillRecordType Generation

**File:** `Mutagen.Bethesda.Generation/Modules/Plugin/PluginTranslationModule.cs`

Change `FillRecordType` to write to `_payload.XyzLocation` instead of `_XyzLocation`:
- `_payload.DeathItemLocation = (stream.Position - offset);`
- `_payload.Factions = BinaryOverlayList.FactoryByArray<...>(...);`
- etc.

### Task 5: Update Factory Generation

**File:** `Mutagen.Bethesda.Generation/Modules/Plugin/PluginTranslationModule.cs`

Change the generated factory to always create a `Lazy<bool>` for `_lazyPayloadInit`:
- Remove the if/else compressed vs non-compressed split
- Both paths use lazy init (just with different stream creation)
- Set `_payload.RecordData` inside the Lazy callback

### Task 6: Update Custom Overlay Files

Custom overlay files that have custom `FillRecordType` or factory methods need updating:
- All Cell.cs files (Skyrim, Fallout4, Starfield)
- ImageSpace.cs (Fallout4)
- Any other custom overlay with direct location field access

### Task 7: Clean Up Base Class

**File:** `Mutagen.Bethesda.Core/Plugins/Binary/Overlay/PluginBinaryOverlay.cs`

- Remove `_lazySubrecordInit` and `SetLazySubrecordInit()` (replaced by per-type payload)
- Keep `_recordData` property for non-major-record overlays (groups, subrecords) — they don't use the payload pattern
- Keep `CreateSubrecordStream()` — still needed inside the Lazy callback
- Keep `ExtractRecordMemoryLazy()` — still needed in factories

### Task 8: Regenerate All Code

```bash
dotnet run --project Mutagen.Bethesda.Generation
```

### Task 9: Add Unit Tests

```csharp
[Fact]
public void CompressedRecord_HeaderAccess_DoesNotDecompress()
{
    // Create compressed NPC, write to file
    // Open with overlay
    // Access FormID, Flags (header fields)
    // Verify no decompression occurred
}

[Fact]
public void CompressedRecord_SubrecordAccess_TriggersDecompression()
{
    // Create compressed NPC with EditorID, write to file
    // Open with overlay
    // Access EditorID (or Name, or any subrecord field)
    // Verify decompression occurred and value is correct
}

[Fact]
public void NonCompressedRecord_HeaderOnlyAccess_SkipsFillSubrecordTypes()
{
    // Open non-compressed record with overlay
    // Access only FormID, Flags
    // Verify FillSubrecordTypes was not called
}

[Fact]
public void CompressedRecord_MultipleAccess_DecompressesOnce()
{
    // Verify thread-safe single decompression
}
```

---

## Design Considerations

### Why class (not struct) for the payload?
A struct inside `Lazy<T>` would be copied on every `.Value` access. With 30+ fields per record type, that's significant per-property overhead. A class is allocated once and returned by reference.

However, we're not using `Lazy<RecordDataPayload>` — we pre-allocate the payload and use `Lazy<bool>` as just a trigger. So the payload could theoretically be a struct embedded in the overlay. But class is simpler: `FillRecordType` writes to `_payload.XyzLocation` through a reference, which works cleanly with class semantics.

### Why make non-compressed records lazy too?
Common patterns like iterating all records to check FormIDs or filtering by flags only need header data (`_structData`). Making `FillSubrecordTypes` lazy for ALL major records means these patterns skip subrecord scanning entirely — a significant win for large mod files with thousands of records.

### Thread safety
`Lazy<bool>` with `LazyThreadSafetyMode.ExecutionAndPublication` ensures the init callback runs exactly once, even with concurrent access. The `Payload` property gates all access, so no thread can read partially-initialized locations.

### No reentrancy risk
`FillSubrecordTypes` reads from the `subStream` parameter (not from `Payload`), and `FillRecordType` writes to `_payload` (the backing field, not through `Payload`). So the Lazy callback never re-enters the `Payload` getter.

---

## Verification Checklist

- [ ] Access FormID on compressed record → NO decompression
- [ ] Access Flags on compressed record → NO decompression
- [ ] Access EditorID on compressed record → triggers decompression + FillSubrecordTypes
- [ ] Access Name on compressed record → triggers decompression (was broken before)
- [ ] Access `Factions` on compressed record → triggers init, returns correct list
- [ ] Subsequent field access → uses cached data, no re-init
- [ ] Multi-threaded access → initializes exactly once
- [ ] Non-compressed record header-only access → no FillSubrecordTypes called
- [ ] All existing tests pass
- [ ] CompressedExport test passes on all frameworks (net8.0, net9.0, net10.0)

---

## Execution Plan: Subtasks & Parallelism

### Dependency Graph

```
Phase 1 (sequential — defines patterns everything else depends on):
  1A: PluginTranslationModule.cs — RecordDataPayload class generation
  1B: PluginTranslationModule.cs — Payload property + _lazyPayloadInit generation
  1C: PluginTranslationModule.cs — Factory generation (always-lazy)
  1D: PluginTranslationModule.cs — FillRecordType outer structure (switch preamble)

Phase 2 (parallel — type-specific generators, one agent per file):
  2A: PrimitiveBinaryTranslationGeneration.cs
  2B: FormLinkBinaryTranslationGeneration.cs
  2C: EnumBinaryTranslationGeneration.cs
  2D: ByteBinaryTranslationGeneration.cs
  2E: ByteArrayBinaryTranslationGeneration.cs
  2F: LoquiBinaryTranslationGeneration.cs
  2G: PluginListBinaryTranslationGeneration.cs
  2H: DataBinaryTranslationGeneration.cs
  2I: PluginArrayBinaryTranslationGeneration.cs
  2J: GenderedTypeBinaryTranslationGeneration.cs
  2K: BinaryTranslationGeneration.cs (base class)
  2L: OverflowGenerationHelper.cs
  2M: Array2dBinaryTranslationGeneration.cs
  2N: AssetLinkBinaryTranslationGeneration.cs
  2O: StringBinaryTranslationGeneration.cs (if applicable)

Phase 3 (parallel — custom overlay fixes, after Phase 2):
  3A: Custom overlays with direct _recordData/_Location refs (~8 files)
  3B: Custom overlays with custom factories (Cell, ImageSpace, etc.)
  3C: Base class cleanup (PluginBinaryOverlay.cs)

Phase 4 (sequential):
  4A: Regenerate all code
  4B: Build
  4C: Test
```

---

### Phase 1: Core Code Generation Infrastructure

All changes in `Mutagen.Bethesda.Generation/Modules/Plugin/PluginTranslationModule.cs`.
Must be done **sequentially** as each step builds on the previous.

#### 1A: Generate RecordDataPayload Class

In `GenerateImportWrapper()` (around line 2280), for major record overlays:
- Collect all location fields, length overrides, and directly-assigned properties
- Generate `internal sealed class {Type}RecordDataPayload { ... }` containing:
  - `public ReadOnlyMemorySlice<byte> RecordData;`
  - All `int?` location fields
  - All `RangeInt32?` location fields
  - All `int?` length override fields
  - All directly-assigned properties (lists, sub-overlays)

Key: The payload class must be generated BEFORE the overlay class fields, so generators can reference it.

#### 1B: Generate Payload Property

For each major record overlay, generate:
```csharp
private readonly {Type}RecordDataPayload _payload = new();
private Lazy<bool>? _lazyPayloadInit;
internal {Type}RecordDataPayload Payload
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    get
    {
        if (_lazyPayloadInit != null) _ = _lazyPayloadInit.Value;
        return _payload;
    }
}
```

Also change `recordDataAccessor` for major records:
- Line 2286: `var recordDataAccessor = isMajorRecord ? "Payload.RecordData" : (obj.GetObjectType() == ObjectType.Mod ? "_stream" : "_recordData");`

This single change propagates to ALL type-specific generators that use `recordDataAccessor`.

#### 1C: Update Factory Generation

Lines ~2913-3100. Change from the current compressed/non-compressed split to always-lazy:
```csharp
// Always create lazy init for major records
ret._lazyPayloadInit = new Lazy<bool>(() =>
{
    ret._payload.RecordData = lazyRecordData.RecordData;
    var subStream = lazyRecordData.IsCompressed
        ? CreateSubrecordStream(...)
        : new OverlayStream(originalSlice, ...);
    ret.CustomFactoryEnd(...);
    ret.FillSubrecordTypes(...);
    return true;
}, LazyThreadSafetyMode.ExecutionAndPublication);
```

#### 1D: Update FillRecordType Preamble

In `GenerateOverlayExtras()` (line ~1643), ensure the method can write to `_payload.XyzLocation`.
No structural change to FillRecordType signature needed — it still writes to `this._payload.XyzLocation`.

---

### Phase 2: Type-Specific Generator Updates

Each file in `Mutagen.Bethesda.Generation/Modules/Binary/` needs the same pattern of changes.
**Can be done in parallel** — one agent per file (or batched into 3-4 agents).

#### Per-File Change Pattern

Each generator has two methods to update:

**`GenerateWrapperFields()`** — Currently outputs:
```csharp
// Location field declaration (on overlay class)
sb.AppendLine($"private int? _{typeGen.Name}Location;");
// Accessor property (on overlay class)
sb.AppendLine($"public T? {typeGen.Name} => _{typeGen.Name}Location.HasValue ? ...(_recordData...) : default;");
```

Change to:
```csharp
// Location field declaration → on payload class (use a flag/context to output in right place)
sb.AppendLine($"public int? {typeGen.Name}Location;");  // Note: public, no underscore prefix
// Accessor property → use Payload
sb.AppendLine($"public T? {typeGen.Name} {{ get {{ var p = Payload; return p.{typeGen.Name}Location.HasValue ? ...(p.RecordData...) : default; }} }}");
```

**`GenerateWrapperRecordTypeParse()`** — Currently outputs:
```csharp
sb.AppendLine($"_{typeGen.Name}Location = {locationAccessor};");
```

Change to:
```csharp
sb.AppendLine($"_payload.{typeGen.Name}Location = {locationAccessor};");
```

#### Files to Update

| # | File | What it generates | Key lines |
|---|------|-------------------|-----------|
| 2A | `PrimitiveBinaryTranslationGeneration.cs` | int/float/etc. with HasValue ternary | L252-280 (fields), L265-274 (accessor) |
| 2B | `FormLinkBinaryTranslationGeneration.cs` | FormLink NullableRecordOverlayFactory | L255-258 (field), L264 (accessor) |
| 2C | `EnumBinaryTranslationGeneration.cs` | Enum ParseRecord | L179-182 (field), L221 (accessor) |
| 2D | `ByteBinaryTranslationGeneration.cs` | Single byte fields | L53 (field), L57 (accessor) |
| 2E | `ByteArrayBinaryTranslationGeneration.cs` | Byte arrays | L180 (field), L199 (accessor) |
| 2F | `LoquiBinaryTranslationGeneration.cs` | Complex sub-objects (ObjectBounds, etc.) | L409 (field), L429-436 (accessor) |
| 2G | `PluginListBinaryTranslationGeneration.cs` | Lists (Factions, etc.) — directly assigned | L725/737 (property), L805 (assignment) |
| 2H | `DataBinaryTranslationGeneration.cs` | RangeInt32 data types | L73 (field), L119-124 (accessor) |
| 2I | `PluginArrayBinaryTranslationGeneration.cs` | Fixed-size arrays | L39 (field), L55/60 (accessor) |
| 2J | `GenderedTypeBinaryTranslationGeneration.cs` | Gendered fields | L367 (field) |
| 2K | `BinaryTranslationGeneration.cs` | Base class — location assignment | L126 (assignment in FillRecordType) |
| 2L | `OverflowGenerationHelper.cs` | Length override fields | L28 (field), L17 (assignment) |
| 2M | `Array2dBinaryTranslationGeneration.cs` | 2D arrays — directly assigned | property decl + assignment |
| 2N | `AssetLinkBinaryTranslationGeneration.cs` | Asset links — directly assigned | property decl |
| 2O | `StringBinaryTranslationGeneration.cs` | String fields (if separate from Primitive) | Check if needed |

---

### Phase 3: Manual Updates & Cleanup

**Can be done in parallel** after Phase 2 establishes the patterns.

#### 3A: Custom Overlays with Direct `_recordData`/`_Location` References

Only ~8 files across all games actually reference these fields directly:

| File | What to change |
|------|---------------|
| `Skyrim/.../Worldspace.cs` | `_recordData.Slice(_NAM0Location.Value.Min)` → `Payload.RecordData.Slice(Payload.NAM0Location.Value.Min)` |
| `Skyrim/.../Weather.cs` | `_recordData.Slice(_directionalLoc.Value)` → `Payload.RecordData.Slice(Payload.DirectionalLoc.Value)` |
| `Skyrim/.../PlacedObject.cs` | `_recordData` + `_boundDataLoc`, `_lightingTemplateLoc`, `_imageSpaceLoc` → `Payload.*` |
| `Fallout4/.../Worldspace.cs` | Same as Skyrim Worldspace |
| `Fallout4/.../Weather.cs` | Similar to Skyrim Weather (check if same pattern) |
| `Starfield/.../Worldspace.cs` | Same as Skyrim Worldspace |
| `Starfield/.../Weather.cs` | Similar to Skyrim Weather |
| `Oblivion/.../Worldspace.cs` | Same pattern |

#### 3B: Custom Overlays with Custom Factories

These files have custom factory methods that currently use `SetLazySubrecordInit` or the compressed/non-compressed split. They need updating to the payload pattern:

| File | What to change |
|------|---------------|
| `Skyrim/.../Cell.cs` | Custom `CellFactory` with `SetLazySubrecordInit` → `_lazyPayloadInit` |
| `Fallout4/.../Cell.cs` | Same |
| `Starfield/.../Cell.cs` | Same |
| `Fallout4/.../ImageSpace.cs` | Custom `ImageSpaceFactory` with `SetLazySubrecordInit` → `_lazyPayloadInit` |

#### 3C: Base Class Cleanup

**File:** `Mutagen.Bethesda.Core/Plugins/Binary/Overlay/PluginBinaryOverlay.cs`

- Remove `_lazySubrecordInit` field and `SetLazySubrecordInit()` method
- Keep `_recordData` property (needed by non-major overlays: groups, subrecords)
- Keep `CreateSubrecordStream()` (used inside Lazy callbacks)
- Keep `ExtractRecordMemoryLazy()` (used in factories)

---

### Phase 4: Regenerate, Build, Test

Sequential:
1. `dotnet run --project Mutagen.Bethesda.Generation` — regenerate all `*_Generated.cs`
2. `dotnet build` — verify compilation
3. `dotnet test` — verify all existing tests pass
4. Run CompressedExport passthrough tests

---

## Agent Allocation Strategy

For maximum parallelism, spin up agents as follows:

**Wave 1 (1 agent, sequential):**
- Phase 1A-1D: Core PluginTranslationModule.cs infrastructure

**Wave 2 (4-5 agents, parallel):**
- Agent A: Subtasks 2A-2D (Primitive, FormLink, Enum, Byte)
- Agent B: Subtasks 2E-2G (ByteArray, Loqui, PluginList)
- Agent C: Subtasks 2H-2J (Data, PluginArray, Gendered)
- Agent D: Subtasks 2K-2O (Base class, Overflow, Array2d, AssetLink, String)
- Agent E: Phase 3C (base class cleanup — independent of generators)

**Wave 3 (2-3 agents, parallel):**
- Agent F: Phase 3A (custom overlays with _recordData refs — all games)
- Agent G: Phase 3B (custom overlays with custom factories — all games)

**Wave 4 (1 agent, sequential):**
- Phase 4A-4C: Regenerate + build + test

---

## Files Summary

| File | Action |
|------|--------|
| `PluginTranslationModule.cs` | Generate `RecordDataPayload` class, `Payload` property, update factory, update `FillRecordType`, update all accessors |
| `PrimitiveBinaryTranslationGeneration.cs` | Move location field to payload, update accessor to use `Payload` |
| `FormLinkBinaryTranslationGeneration.cs` | Move location field to payload, update accessor to use `Payload` |
| `EnumBinaryTranslationGeneration.cs` | Move location field to payload, update accessor to use `Payload` |
| `ByteBinaryTranslationGeneration.cs` | Move location field to payload, update accessor to use `Payload` |
| `ByteArrayBinaryTranslationGeneration.cs` | Move location field to payload, update accessor to use `Payload` |
| `LoquiBinaryTranslationGeneration.cs` | Move location field to payload, update accessor to use `Payload` |
| `PluginListBinaryTranslationGeneration.cs` | Move property to payload, update assignment + accessor |
| `DataBinaryTranslationGeneration.cs` | Move RangeInt32 location to payload, update accessor |
| `PluginArrayBinaryTranslationGeneration.cs` | Move location field to payload, update accessor |
| `GenderedTypeBinaryTranslationGeneration.cs` | Move location field to payload |
| `BinaryTranslationGeneration.cs` | Update base location assignment to use `_payload.` prefix |
| `OverflowGenerationHelper.cs` | Move length override to payload, update assignment |
| `Array2dBinaryTranslationGeneration.cs` | Move property to payload |
| `AssetLinkBinaryTranslationGeneration.cs` | Move property to payload |
| `PluginBinaryOverlay.cs` | Remove `_lazySubrecordInit`/`SetLazySubrecordInit()` |
| `LazyMajorRecordData.cs` | No changes needed |
| Worldspace.cs (4 games) | `_recordData`/`_Location` → `Payload.RecordData`/`Payload.Location` |
| Weather.cs (Skyrim, Starfield) | `_recordData`/`_Location` → `Payload.RecordData`/`Payload.Location` |
| PlacedObject.cs (Skyrim) | `_recordData`/`_Location` → `Payload.RecordData`/`Payload.Location` |
| Cell.cs (3 games) | Custom factory → use `_lazyPayloadInit` |
| ImageSpace.cs (FO4) | Custom factory → use `_lazyPayloadInit` |
| `*_Generated.cs` | Regenerate all |
