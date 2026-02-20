# EnumerateGroups Implementation Plan

**Issue**: [#87 - Add EnumerateGroups to IMod](https://github.com/Mutagen-Modding/Mutagen/issues/87)
**Branch**: `mutable-caches`

---

## Goal

Add `EnumerateGroups()` to `IModGetter` so callers can iterate over top-level groups without knowing the specific mod type. This enables the link cache to do `group.RecordCache.TryGetValue(formKey)` per group for O(g) untyped lookups instead of O(n) full record enumeration.

---

## How It Fits Into the Mutable Cache Optimization

`EnumerateGroups` is used in the **untyped lookup path** of `MutableModLinkCache` — the methods that receive a `FormKey` (or `EditorID`) with no type information:

```
TryResolve(FormKey formKey, out IMajorRecordGetter majorRec)
```

Without a type, we can't call `TryGetTopLevelGroup<T>()` (no `T` to provide). Instead, we need to walk all groups and check each one's dictionary:

```csharp
// Current: O(n) — enumerate every record in the entire mod
foreach (var item in _sourceMod.EnumerateMajorRecords())
{
    if (item.FormKey == formKey) { majorRec = item; return true; }
}

// With EnumerateGroups: O(g) — check ~60-80 group dictionaries
foreach (var group in _sourceMod.EnumerateGroups())
{
    if (group.RecordCache.TryGetValue(formKey, out var rec))
    {
        majorRec = rec;
        return true;
    }
}
// Then check nested records as fallback...
```

The **typed** lookups (`TryResolve<TMajor>`, `TryResolve(FormKey, Type)`) don't need this — they already have `TryGetTopLevelGroup(typeof(TMajor))`. `EnumerateGroups` is specifically for when we have zero type information.

---

## Current Architecture

### TryGetTopLevelGroup Pattern (what we're following)

The existing `TryGetTopLevelGroup` works via:

1. **Interface** on `IModGetter` (`IMod.cs:54,68`):
   ```csharp
   IGroupGetter<TMajor>? TryGetTopLevelGroup<TMajor>() where TMajor : IMajorRecordGetter;
   IGroupGetter? TryGetTopLevelGroup(Type type);
   ```

2. **Extension methods** generated per game mod in the mixin class (`ModModule.cs:382-448`):
   ```csharp
   public static IGroupGetter<T>? TryGetTopLevelGroup<T>(this ISkyrimModGetter obj)
       where T : IMajorRecordGetter
   {
       return (IGroupGetter<T>?)((SkyrimModCommon)...).GetGroup(obj: obj, type: typeof(T));
   }
   ```

3. **`GetGroup` method** generated on the Common class (`ModModule.cs:477-545`), which is a big `switch (type.Name)` that maps type names to `obj.GroupProperty`:
   ```csharp
   public object? GetGroup(ISkyrimModGetter obj, Type type)
   {
       switch (type.Name)
       {
           case "Npc": case "INpcGetter": case "INpc": ...
               return obj.Npcs;
           case "CellBlock": case "ICellBlockGetter": ...
               return obj.Cells.Records;  // ListGroup returns .Records
           ...
           default: return null;
       }
   }
   ```

4. **Explicit interface implementations** on the generated mod class (`ModModule.cs:64-67`):
   ```csharp
   IGroupGetter<T>? IModGetter.TryGetTopLevelGroup<T>() => this.TryGetTopLevelGroup<T>();
   IGroupGetter? IModGetter.TryGetTopLevelGroup(Type type) => this.TryGetTopLevelGroup(type);
   ```

5. **`AMod` base class** stub (`AMod.cs:71-72`):
   ```csharp
   IGroupGetter<T>? IModGetter.TryGetTopLevelGroup<T>() => throw new NotImplementedException();
   IGroupGetter? IModGetter.TryGetTopLevelGroup(Type type) => throw new NotImplementedException();
   ```

6. **MultiModOverlay** also implements it (`MultiModOverlayModule.cs:988-1009`).

### Group Types

There are two group types in the mod structure. This matters for what `EnumerateGroups` yields:

| Type | Interface | Storage | FormKey lookup | Example | Count in Skyrim |
|------|-----------|---------|----------------|---------|-----------------|
| **IGroup\<T\>** | `IGroupGetter` | FormKey-indexed cache | O(1) via `RecordCache` | Npcs, Weapons, Spells | ~80 |
| **IListGroup\<T\>** | `IListGroupGetter` | Sequential list | No `RecordCache` | Cells (mod-level) | 1 |

The generated `GetGroup` handles ListGroups specially: `return obj.Cells.Records` (returns the `.Records` list, not the group itself).

**Decision**: `EnumerateGroups` should yield only `IGroupGetter` instances (the ones with `RecordCache`). The sole ListGroup (`Cells`) doesn't have a FormKey-indexed cache, so including it would break the `group.RecordCache.TryGetValue()` pattern. The CellBlock records inside it aren't major records with FormKeys anyway.

---

## Implementation Plan

### Step 1: Add interface method to IModGetter

**File**: `Mutagen.Bethesda.Core\Plugins\Records\IMod.cs`

Add to `IModGetter`:
```csharp
/// <summary>
/// Enumerates all top-level Group getter objects in the mod.
/// Only includes IGroupGetter instances (FormKey-indexed groups),
/// not IListGroupGetter instances.
/// </summary>
IEnumerable<IGroupGetter> EnumerateGroups();
```

### Step 2: Add AMod stub

**File**: `Mutagen.Bethesda.Core\Plugins\Records\AMod.cs`

Add alongside the other `NotImplementedException` stubs (line ~72):
```csharp
IEnumerable<IGroupGetter> IModGetter.EnumerateGroups() => throw new NotImplementedException();
```

### Step 3: Add code generation for the Common class method

**File**: `Mutagen.Bethesda.Generation\Modules\Plugin\ModModule.cs`

Add a new method `GenerateEnumerateGroups` (alongside `GenerateGetGroup` at line 477), and call it from `GenerateInCommon` (line 472).

The generated method should look like:
```csharp
public IEnumerable<IGroupGetter> EnumerateGroups(ISkyrimModGetter obj)
{
    yield return obj.GameSettings;
    yield return obj.Keywords;
    yield return obj.LocationReferenceTypes;
    // ... all IGroup<T> properties
    yield return obj.Worldspaces;
    yield return obj.DialogTopics;
    // ... etc.
    // NOTE: Skip Cells — it's a ListGroup, not an IGroup
}
```

**Code gen logic**: Iterate `obj.IterateFields()`, filter for `LoquiType` fields where `TargetObjectGeneration.GetObjectData().ObjectType == ObjectType.Group`, and **skip** fields where `TargetObjectGeneration.Name.EndsWith("ListGroup")`. For each remaining field, emit `yield return obj.{field.Name};`.

This mirrors the loop in `GenerateGetGroup` (line 490-536) but instead of a switch statement, it yields each group.

### Step 4: Add code generation for the mixin extension method

**File**: `Mutagen.Bethesda.Generation\Modules\Plugin\ModModule.cs`

Add in `GenerateInCommonMixin` (around line 448), following the `TryGetTopLevelGroup` pattern:

```csharp
public static IEnumerable<IGroupGetter> EnumerateGroups(this ISkyrimModGetter obj)
{
    return ((SkyrimModCommon)((ISkyrimModGetter)obj).CommonInstance()!).EnumerateGroups(obj: obj);
}
```

### Step 5: Add explicit interface implementation on generated mod class

**File**: `Mutagen.Bethesda.Generation\Modules\Plugin\ModModule.cs`

Add in `GenerateInClass` (around line 64-67):

```csharp
sb.AppendLine($"IEnumerable<IGroupGetter> {nameof(IModGetter)}.{nameof(IModGetter.EnumerateGroups)}() => this.EnumerateGroups();");
```

### Step 6: Add to MultiModOverlay

**File**: `Mutagen.Bethesda.Generation\Modules\Plugin\MultiModOverlayModule.cs`

The MultiModOverlay classes also implement `IModGetter`. Add `EnumerateGroups` implementation there, following the same pattern as `TryGetTopLevelGroup` (around line 988).

The overlay implementation should yield its merged group wrappers.

### Step 7: Regenerate code

Run the code generator to produce updated `SkyrimMod_Generated.cs`, `Fallout4Mod_Generated.cs`, `OblivionMod_Generated.cs`, `StarfieldMod_Generated.cs`, and their overlay counterparts.

### Step 8: Add unit tests

**File**: `Mutagen.Bethesda.UnitTests\Plugins\Records\GetGroupTests.cs` (extend existing file)

Tests to add:

| Test | Description |
|------|-------------|
| `EnumerateGroups_ReturnsNonEmpty` | `mod.EnumerateGroups()` returns groups |
| `EnumerateGroups_ContainsExpectedGroup` | Result includes `mod.Npcs` |
| `EnumerateGroups_AllAreIGroupGetter` | Every yielded item is `IGroupGetter` |
| `EnumerateGroups_DoesNotContainListGroups` | Result does NOT include Cells (the ListGroup) |
| `EnumerateGroups_CountMatchesExpected` | Count matches the number of top-level IGroup properties |
| `EnumerateGroups_CanLookupFormKeyAcrossGroups` | Add NPC + Weapon, iterate groups, find both by FormKey |
| `EnumerateGroups_EmptyGroupsStillYielded` | Empty groups are included (count > 0 doesn't filter) |

### Step 9: Build and verify

1. `dotnet build` — verify compilation
2. Run `GetGroupTests` — verify new tests pass
3. Run full test suite — verify no regressions

---

## Files Changed Summary

| File | Change |
|------|--------|
| `Mutagen.Bethesda.Core\Plugins\Records\IMod.cs` | Add `EnumerateGroups()` to `IModGetter` |
| `Mutagen.Bethesda.Core\Plugins\Records\AMod.cs` | Add `NotImplementedException` stub |
| `Mutagen.Bethesda.Generation\Modules\Plugin\ModModule.cs` | Code gen: Common class method + mixin + interface impl |
| `Mutagen.Bethesda.Generation\Modules\Plugin\MultiModOverlayModule.cs` | Code gen: overlay implementation |
| `Mutagen.Bethesda.Skyrim\Records\SkyrimMod_Generated.cs` | Regenerated |
| `Mutagen.Bethesda.Fallout4\Records\Fallout4Mod_Generated.cs` | Regenerated |
| `Mutagen.Bethesda.Oblivion\Records\OblivionMod_Generated.cs` | Regenerated |
| `Mutagen.Bethesda.Starfield\Records\StarfieldMod_Generated.cs` | Regenerated |
| `*MultiModOverlay_Generated.cs` (4 files) | Regenerated |
| `Mutagen.Bethesda.UnitTests\Plugins\Records\GetGroupTests.cs` | New tests |

---

## Design Decision: What Does EnumerateGroups Yield?

**Only `IGroupGetter` instances** (not `IListGroupGetter`).

Rationale:
- The consumer pattern is `group.RecordCache.TryGetValue(formKey)` — requires `IGroupGetter` which has `RecordCache`
- `IListGroupGetter` doesn't have `RecordCache` — including it would require callers to type-check
- The only ListGroup in practice is `Cells` at the mod level, which contains `CellBlock` objects (not major records with FormKeys)
- `IGroupGetter` inherits from `IGroupCommonGetter`, which has `.Count`, `.Records`, `.ContainedRecordType` — all useful for the optimization

If a future need arises to also enumerate list groups, a separate `EnumerateListGroups()` could be added.

---

## Verification Checklist

- [ ] `IModGetter.EnumerateGroups()` compiles and is callable
- [ ] Extension method works: `skyrimMod.EnumerateGroups()` via mixin
- [ ] Returns only `IGroupGetter` instances, no `IListGroupGetter`
- [ ] Every top-level IGroup property is yielded (count matches)
- [ ] Works on direct mod objects (SkyrimMod)
- [ ] Works on binary overlays
- [ ] Works on MultiModOverlay
- [ ] Existing tests pass (no regressions)
- [ ] New unit tests pass
