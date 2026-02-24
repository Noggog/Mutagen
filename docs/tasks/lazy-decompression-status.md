# Lazy Decompression Implementation - Current Status

**Last updated**: 2026-02-13
**Branch**: `lazy-decompression`
**Build status**: 0 errors
**Unit tests**: 3,945 passing (net8.0/net9.0/net10.0)
**Code generation**: Verified (regenerated + built + tested successfully)

## Summary

The lazy decompression feature for major record binary overlays is **functionally complete**. All 4 phases are done. The generator produces correct code, the solution builds cleanly, and all unit tests pass.

**Remaining work**: Run passthrough tests (byte-level round-trip integration tests against real game files).

---

## Architecture

### RecordDataPayload Pattern

Each major record binary overlay now has a **nested partial payload class** that bundles all subrecord-derived state:

```
MajorRecordBinaryOverlay (base)
  ├── protected Lazy<bool>? _payloadInit     ← shared across hierarchy, set by concrete factory
  ├── MajorRecordRecordDataPayload _payload  ← has EditorIDLocation
  └── Payload property                       ← triggers _payloadInit on access

NpcBinaryOverlay (concrete)
  ├── NpcRecordDataPayload _payload          ← has NPC-specific locations, lists, overlays
  ├── Payload property                       ← triggers same _payloadInit
  └── internal partial class NpcRecordDataPayload  ← hand-written extension adds custom fields
```

**What lives on the payload (generated)**:
- Subrecord location fields (`int? NameLocation`, `RangeInt32? DATALocation`)
- Length overrides (`int? NameLengthOverride`)
- Directly-assigned properties (lists, gendered overlays, array2d)
- DataType state enums

**What lives on the payload (hand-written partial extensions)**:
- Custom location fields (`int? BodyTemplateLocation`, `int? FaceFxPhonemesLoc`)
- Custom span fields (`ReadOnlyMemorySlice<byte>? ObjectsSpan`)
- Custom collections (`IReadOnlyList<IAPerkEffectGetter> Effects`)

### Two-Layer Lazy Init

There are two independent lazy layers:

1. **Decompression** (`LazyMajorRecordData.RecordData`): Lazily decompresses compressed record content. Triggered by accessing `_recordData` on `PluginBinaryOverlay`.

2. **Subrecord parsing** (`_payloadInit`): Lazily parses subrecord locations via `FillSubrecordTypes`. Triggered by accessing `Payload` on any overlay in the hierarchy.

For **non-compressed** records, decompression is a no-op (just returns a slice). But subrecord parsing is still deferred until first property access.

### Why `_recordData` and `Payload` Are Separate

- `_recordData` gives raw bytes (decompressed if needed). Used by hand-written overlay code that computes values from known offsets.
- `Payload.SomeLocation` gives parsed subrecord positions. Must trigger subrecord parsing first.

Hand-written custom parse methods (called during `_payloadInit`) can safely read `_recordData` (triggers decompression only) without circular dependency on `Payload` (which would deadlock the `Lazy<bool>`).

### Shared `_payloadInit` Across Hierarchy

Problem it solves: `MajorRecordBinaryOverlay` defines `EditorID` which needs `Payload.EditorIDLocation`. But `EditorIDLocation` is only populated when the concrete class's factory runs `FillSubrecordTypes`. Each abstract class has its own private `_payload`, but only one `_payloadInit` (protected on the base class). All `Payload` getters check the same `_payloadInit`, so accessing `EditorID` on the base class triggers the concrete factory's init.

---

## Files Modified

### Generator Files (Mutagen.Bethesda.Generation/Modules/Binary/)

| File | What Changed |
|------|-------------|
| `PluginTranslationModule.cs` | Core: payload class generation, `AsyncLocal<StructuredStringBuilder?>` for thread-safe field collection, factory lazy init (`_payloadInit`), `_recordData` accessor |
| `BinaryTranslationGeneration.cs` | Base `GenerateWrapperRecordTypeParse`: `_payload.XLocation` for major records |
| `OverflowGenerationHelper.cs` | `isMajorRecord` param for overflow parse/member |
| `PrimitiveBinaryTranslationGeneration.cs` | Payload field redirect, `_recordData` accessor |
| `FormLinkBinaryTranslationGeneration.cs` | Payload field redirect |
| `EnumBinaryTranslationGeneration.cs` | Payload field redirect (DataType sub-field fix) |
| `ByteBinaryTranslationGeneration.cs` | Payload field redirect |
| `ByteArrayBinaryTranslationGeneration.cs` | Payload field redirect + overflow |
| `LoquiBinaryTranslationGeneration.cs` | Payload field redirect + non-nullable wrapper fix |
| `PluginListBinaryTranslationGeneration.cs` | Directly-assigned list properties on payload |
| `DataBinaryTranslationGeneration.cs` | DataType container locations on payload |
| `PluginArrayBinaryTranslationGeneration.cs` | Array locations on payload |
| `GenderedTypeBinaryTranslationGeneration.cs` | Gendered overlay fields on payload |
| `Array2dBinaryTranslationGeneration.cs` | Array2d fields on payload |
| `CustomLogicTranslationGeneration.cs` | `isMajorRecord` passthrough |
| `DictBinaryTranslationGeneration.cs` | `isMajorRecord` passthrough |
| `FormKeyBinaryTranslationGeneration.cs` | DataType parent location fix |

### Base Class (Mutagen.Bethesda.Core)

| File | What Changed |
|------|-------------|
| `PluginBinaryOverlay.cs` | Removed `_lazySubrecordInit`/`SetLazySubrecordInit()`, simplified `_recordData` getter |
| `MajorRecord.cs` | Added `protected Lazy<bool>? _payloadInit` + `EnsurePayloadInit()` (dead code, removable) |

### Hand-Written Overlay Files (34 files - partial payload migration)

Custom overlay files that stored private location fields were migrated to nested partial payload classes:

- **Skyrim (11)**: Race, Perk, Region, Weather, GlobalShort, GlobalInt, GlobalUnknown, GameSettingBool, Armor, ArmorAddon, PlacedObject
- **Oblivion (7)**: Region, PlacedObject, PathGrid, LeveledItem, GlobalUnknown, GlobalShort, GlobalInt
- **Fallout4 (10)**: Region, Npc, Water, Terminal, Holotape, ColorRecord, GlobalInt, GlobalShort, GlobalBool, GameSettingBool
- **Starfield (6)**: Region, Note, GameplayOption, ColorRecord, BoneModifier, GameSettingBool

### Other Hand-Written Overlay Files (12 files - `_recordData` + DataType refs)

- **Worldspace** (Skyrim, Fallout4, Starfield): `Payload.NAM0Location`/`Payload.NAM9Location` + `_recordData`
- **PlacedObject** (Skyrim, Fallout4): `_recordData` for bound data / lighting template / image space
- **Cell** (Skyrim, Fallout4, Starfield): `_payloadInit` instead of `_lazyPayloadInit`
- **ImageSpace** (Fallout4): `Payload.DNAMLocation` + `_payloadInit`
- **Climate** (Skyrim, Fallout4, Starfield): `Payload.TNAMLocation`
- **Book** (Skyrim, Fallout4, Starfield): `Payload.DATALocation`
- **MagicEffect** (Skyrim, Fallout4, Starfield): `Payload.DATALocation`
- **ArmorAddon** (Skyrim, Fallout4): `Payload.DNAMLocation`
- **Race** (Fallout4): `Payload.DATALocation`
- **Npc** (Starfield): `Payload.ACBSLocation`

---

## Key Design Decisions & Reasoning

### 1. Why `partial` payload instead of `EnsurePayloadInit()`?

We initially added an `EnsurePayloadInit()` bridge method. But `partial` payload is cleaner:
- Accessing `Payload.Field` **naturally** triggers init - no need to remember to call a guard method
- Custom fields live alongside generated fields in the same type
- No risk of forgetting the guard call in future code

### 2. Why `_recordData` is NOT on the payload

Originally `RecordData` was a payload field. We removed it because:
- `_recordData` on `PluginBinaryOverlay` already lazily decompresses via `LazyMajorRecordData`
- Having it on the payload created a circular dependency: custom parse methods (inside `_payloadInit`) need `_recordData` to read bytes, but `_recordData` was on `Payload` which triggers `_payloadInit`
- Now: `_recordData` = decompression only (safe inside init), `Payload` = subrecord locations (triggers init)

### 3. Why DataType sub-field locations stay on the overlay class

Fields like `_SizeLocation` inside a `DATA` container are **computed properties** derived from `Payload.DATALocation.Value.Min + offset`. They're generated as `private int _SizeLocation => ...` on the overlay class. Moving them to the payload would be wrong because they don't store independent state.

### 4. Why `AsyncLocal<T>` for `CurrentPayloadFieldsSb`

The code generator runs multiple `ObjectGeneration.Generate()` tasks in parallel. `CurrentPayloadFieldsSb` is set per-object during `GenerateImportWrapper`, so it needs thread-local storage. `AsyncLocal<T>` ensures each parallel generation task has its own payload builder.

---

## Known Issues / Open Items

### 1. Passthrough Tests Not Yet Run (PRIORITY)

The passthrough tests read real game files, write them back, and compare byte-for-byte. These are the definitive correctness check. Run with `/run-passthrough` or the passthrough test suite.

### 2. `EnsurePayloadInit()` is Dead Code

`MajorRecordBinaryOverlay.EnsurePayloadInit()` is no longer called. Can be removed or kept as a safety net for future hand-written overlay code.

### 3. Generator Intermittent Concurrency Error

The Loqui generator has a **pre-existing** race condition (`HashSet` concurrent modification in `ObjectGeneration.Generate()`). Not caused by our changes. Retry usually works. Our `AsyncLocal<T>` usage is correct.

### 4. Generated `_lazyPayloadInit` Warning

The generated `MajorRecord_Generated.cs` produces a CS0649 warning: `Field '_lazyPayloadInit' is never assigned to`. This is because the generated code declares the field but the actual assignment happens via the hand-written `_payloadInit` field on the base class. This warning is harmless but could be cleaned up by having the generator not emit `_lazyPayloadInit` for the base `MajorRecord` type.

---

## How to Resume

```
1. Run passthrough tests
   /run-passthrough

2. Fix any passthrough failures (byte-level serialization differences)

3. Optional cleanup:
   - Remove EnsurePayloadInit() from MajorRecord.cs
   - Suppress or fix the _lazyPayloadInit warning
   - Consider whether _recordData access without Payload is valid for all code paths
```
