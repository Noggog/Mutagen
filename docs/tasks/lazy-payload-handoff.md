# LazyPayload<T> Implementation — Handoff Status

## What Was Done

### 1. Created `LazyPayload<T>` class ✅
- **File**: `Mutagen.Bethesda.Core/Plugins/Binary/Overlay/LazyPayload.cs`
- Immutable wrapper: holds a `Lazy<bool> init` and `T fields`
- `.Fields` for direct access during init callbacks
- `.Value` triggers init then returns fields

### 2. Removed `_payloadInit` from `MajorRecord.cs` ✅
- **File**: `Mutagen.Bethesda.Core/Plugins/Records/MajorRecord.cs`
- Removed the `protected Lazy<bool>? _payloadInit;` field and its doc comment

### 3. Updated `PluginTranslationModule.cs` generator ✅ (with caveat)
- **File**: `Mutagen.Bethesda.Generation/Modules/Plugin/PluginTranslationModule.cs`
- Changed payload field from `private readonly {Type}RecordDataPayload _payload = new()` → `private LazyPayload<{Type}RecordDataPayload> _payload;`
- Changed Payload property from old getter pattern → `=> _payload.Value;`
- Added `InitPayload` method generation (virtual at root, override at derived)
- Changed factory from `ret._payloadInit = new Lazy<bool>(...)` → `var init = new Lazy<bool>(...); ret.InitPayload(init);`

### 4. Updated custom factory files ✅
- **Files**: `Cell.cs` (Skyrim, Fallout4, Starfield), `ImageSpace.cs` (Fallout4)
- Consolidated the if/else (compressed vs non-compressed) into a single `var init = new Lazy<bool>(...)` + `ret.InitPayload(init);`

### 5. Updated hand-written overlay files (Oblivion, Fallout4, Starfield) ✅
- All `_payload.Something` → `_payload.Fields.Something`
- These games verified clean: `_payload.Fields.` with exactly one `.Fields.`

---

## What Needs Fixing (Double `.Fields.` Bug)

The files listed below already had `_payload.Fields.` from a prior session's git changes. The automated replacement ran `_payload.` → `_payload.Fields.` on top, producing `_payload.Fields.Fields.` (or worse).

### Fix needed: Replace `.Fields.Fields.` → `.Fields.` globally in these files

**8 Generator files** (exactly one `.Fields.Fields.` each, or a few):
1. `Mutagen.Bethesda.Generation/Modules/Binary/BinaryTranslationGeneration.cs` — line 129
2. `Mutagen.Bethesda.Generation/Modules/Binary/DataBinaryTranslationGeneration.cs` — lines 203, 204, 307
3. `Mutagen.Bethesda.Generation/Modules/Binary/Array2dBinaryTranslationGeneration.cs` — line 416
4. `Mutagen.Bethesda.Generation/Modules/Binary/GenderedTypeBinaryTranslationGeneration.cs` — line 562
5. `Mutagen.Bethesda.Generation/Modules/Binary/LoquiBinaryTranslationGeneration.cs` — lines 657, 658, 664, 668
6. `Mutagen.Bethesda.Generation/Modules/Binary/OverflowGenerationHelper.cs` — line 17
7. `Mutagen.Bethesda.Generation/Modules/Binary/PluginArrayBinaryTranslationGeneration.cs` — line 136
8. `Mutagen.Bethesda.Generation/Modules/Binary/PluginListBinaryTranslationGeneration.cs` — lines 839, 849, 864

**11 Skyrim hand-written files** (varying levels of `.Fields.` nesting — some have 2, some up to 5):
1. `Mutagen.Bethesda.Skyrim/Records/Major Records/ArmorAddon.cs` — has `.Fields.Fields.Fields.Fields.Fields.` (5 levels!)
2. `Mutagen.Bethesda.Skyrim/Records/Major Records/Armor.cs`
3. `Mutagen.Bethesda.Skyrim/Records/Major Records/GameSettingBool.cs`
4. `Mutagen.Bethesda.Skyrim/Records/Major Records/GlobalUnknown.cs`
5. `Mutagen.Bethesda.Skyrim/Records/Major Records/GlobalShort.cs`
6. `Mutagen.Bethesda.Skyrim/Records/Major Records/GlobalInt.cs`
7. `Mutagen.Bethesda.Skyrim/Records/Major Records/Weather.cs`
8. `Mutagen.Bethesda.Skyrim/Records/Major Records/Region.cs`
9. `Mutagen.Bethesda.Skyrim/Records/Major Records/Race.cs`
10. `Mutagen.Bethesda.Skyrim/Records/Major Records/PlacedObject.cs`
11. `Mutagen.Bethesda.Skyrim/Records/Major Records/Perk.cs`

**Fix approach**: For all 19 files above, repeatedly replace `.Fields.Fields.` with `.Fields.` until no doubles remain. A regex replace loop or multiple passes of `s/\.Fields\.Fields\./\.Fields\./g` will collapse any nesting level down to one.

**1 PluginTranslationModule.cs line** (separate issue):
- Line 2519: `_payload.Fields.Value` should be `_payload.Value`
- The Payload property getter should NOT go through `.Fields` — `.Value` already triggers init and returns the fields object

**1 Generated file (not a bug, just check)**:
- `Mutagen.Bethesda.Oblivion/Records/Major Records/Script_Generated.cs` line 1538 has `_payload.Fields.Fields = ...`
- This is a generated file, so it will be regenerated. But verify after regeneration that the field named `Fields` doesn't produce a confusing `_payload.Fields.Fields` (the first `.Fields` is LazyPayload access, the second `Fields` is the actual field name on the payload class).

---

## What's Not Yet Done

### 6. Regenerate all 4 games
After fixing the double-Fields bug above, run code generation for all 4 games:
```
dotnet build Mutagen.Bethesda.Oblivion.Generator --framework net9.0
cd Mutagen.Bethesda.Oblivion.Generator/bin/Debug/net9.0
dotnet Mutagen.Bethesda.Oblivion.Generator.dll
```
Repeat for Skyrim, Fallout4, Starfield.

### 7. Build and verify
```
dotnet build Mutagen.Records.sln
```
Expect 0 errors.

### 8. Verification checks
1. Grep for `_payloadInit` — should only appear in docs, not in any `.cs` files
2. Grep for `_payload\.(?!Fields|Value|$)` in `.cs` files — should find nothing (no bare `_payload.Something` without `.Fields.` or `.Value`)
3. Grep for `Fields\.Fields\.` — should find nothing (no double nesting)
4. Run passthrough tests for byte-level correctness

---

## Architecture Summary

```
Before:
  MajorRecordBinaryOverlay (base)
    protected Lazy<bool>? _payloadInit;     ← shared, mutable, set post-construction
    private readonly XPayload _payload = new();
    internal XPayload Payload { get { if (_payloadInit != null) _ = _payloadInit.Value; return _payload; } }

After:
  MajorRecordBinaryOverlay (base)
    private LazyPayload<XPayload> _payload;  ← null until InitPayload called
    internal XPayload Payload => _payload.Value;
    protected virtual void InitPayload(Lazy<bool> init) { _payload = new LazyPayload<XPayload>(init, new XPayload()); }

  DerivedBinaryOverlay
    private LazyPayload<YPayload> _payload;  ← own payload, same shared Lazy<bool>
    internal YPayload Payload => _payload.Value;
    protected override void InitPayload(Lazy<bool> init) { base.InitPayload(init); _payload = new LazyPayload<YPayload>(init, new YPayload()); }

  ConcreteFactory:
    var init = new Lazy<bool>(() => { /* fill logic */ return true; }, LazyThreadSafetyMode.ExecutionAndPublication);
    ret.InitPayload(init);   // cascades through hierarchy via virtual dispatch
```

- **Reads** (property getters): `Payload.SomeField` → calls `_payload.Value` → triggers init → returns fields
- **Writes** (inside FillRecordType, during init callback): `_payload.Fields.SomeField = ...` → direct access, no init trigger
