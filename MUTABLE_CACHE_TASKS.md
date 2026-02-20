# Mutable Cache Optimization - Task Breakdown

**Parent Plan**: [MUTABLE_CACHE_OPTIMIZATION_PLAN.md](./MUTABLE_CACHE_OPTIMIZATION_PLAN.md)
**Branch**: `mutable-caches`
**Scope**: Phase 1 - Top-Level Group Direct Lookup (Tier 1)

---

## Dependency Graph

```
Task 1 ─────┬──► Task 3 ──┬──► Task 5 ──► Task 6 ──► Task 7 ──► Task 8
             │             │
Task 2 ──┐  └──► Task 4 ──┘
         │         │
         └─────────┴──────────► Task 5
```

**Parallel tracks**:
- Tasks 1 + 2 can run simultaneously (no shared deps)
- Tasks 3 + 4 can run simultaneously (both only need Task 1)
- Tasks 5-8 are sequential

**Key constraint**: Tasks 2-5 all modify `MutableModLinkCache.cs`. Agents working on this file need to be coordinated (either sequential, or targeting non-overlapping line ranges).

---

## Task 1: Add EnumerateGroups() to IModGetter

**Status**: ✅ Complete
**Blocked by**: Nothing
**Parallel with**: Task 2

### Goal
Add an `EnumerateGroups()` method to `IModGetter` that yields all top-level `IGroupGetter` instances. This enables untyped FormKey lookups to check each group's dictionary (O(g) ≈ O(60)) instead of enumerating all records (O(n) ≈ O(100k)).

### Key Files
- `Mutagen.Bethesda.Core\Plugins\Records\IMod.cs` — interface definition
- `Mutagen.Bethesda.Core\Plugins\Records\IMajorRecordEnumerable.cs` — possible home for the method
- `Mutagen.Bethesda.Generation\Modules\Plugin\ModModule.cs` — code generation for per-game implementations
- Generated outputs: `SkyrimMod_Generated.cs`, `Fallout4Mod_Generated.cs`, `OblivionMod_Generated.cs`, `StarfieldMod_Generated.cs`

### Implementation Notes
- Follow the pattern of `TryGetTopLevelGroup` which already exists on `IModGetter`
- Should yield `IGroupGetter` instances (the FormKey-indexed groups), not `IListGroupGetter`
- Each generated mod class (SkyrimMod, etc.) will need an implementation that yields its ~60 group properties
- Check if this can be done via code generation or needs manual implementation per game

---

## Task 2: Optimize Typed FormKey TryResolve

**Status**: ✅ Complete
**Blocked by**: Nothing
**Parallel with**: Task 1

### Goal
For `TryResolve<TMajor>(FormKey formKey, ...)`, add a fast path using `TryGetTopLevelGroup` for O(1) dictionary lookup before falling through to enumeration.

### Key File
`Mutagen.Bethesda.Core\Plugins\Cache\Internals\Implementations\MutableModLinkCache.cs`

### Methods to Update (6 total)

**In `MutableModLinkCache` (non-generic class, starts line 17):**

| Method | Line | Lookup Key |
|--------|------|------------|
| `TryResolve<TMajor>(FormKey, ...)` | 117 | FormKey + generic type |
| `TryResolve(FormKey, Type, ...)` | 193 | FormKey + Type param |
| `TryResolve(IFormLinkIdentifier, ...)` | 229 | Delegates to Type version — no change needed |

**In `MutableModLinkCache<TMod, TModGetter>` (generic class, starts line 1102):**

| Method | Line | Lookup Key |
|--------|------|------------|
| `TryResolve<TMajor>(FormKey, ...)` | ~1204 | FormKey + generic type |
| `TryResolve(FormKey, Type, ...)` | ~1290 | FormKey + Type param |
| `TryResolve(IFormLinkIdentifier, ...)` | ~1325 | Delegates — no change needed |

### Pattern
```csharp
// ADD THIS before the existing foreach loop:
var group = _sourceMod.TryGetTopLevelGroup(typeof(TMajor));
if (group != null)
{
    if (group.RecordCache.TryGetValue(formKey, out var rec) && rec is TMajor typed)
    {
        majorRec = typed;
        return true;
    }
    majorRec = default;
    return false; // Definitive miss — record not in its group
}
// EXISTING foreach loop stays as fallback for nested types
```

### Type Constraint Note
`TryGetTopLevelGroup<TMajor>()` requires `TMajor : IMajorRecordGetter`. The `TryResolve<TMajor>` constraint is `TMajor : IMajorRecordQueryableGetter`. Use the `Type`-based overload `TryGetTopLevelGroup(typeof(TMajor))` to avoid constraint mismatch.

---

## Task 3: Optimize Untyped FormKey TryResolve

**Status**: ✅ Complete
**Blocked by**: Task 1 (needs EnumerateGroups)
**Parallel with**: Task 4

### Goal
For `TryResolve(FormKey formKey, ...)` (no type parameter), iterate groups with dictionary lookups instead of enumerating all records.

### Key File
`Mutagen.Bethesda.Core\Plugins\Cache\Internals\Implementations\MutableModLinkCache.cs`

### Methods to Update (2 total, same pattern in both classes)

| Class | Method | Line |
|-------|--------|------|
| `MutableModLinkCache` | `TryResolve(FormKey, out IMajorRecordGetter, ResolveTarget)` | 53 |
| `MutableModLinkCache<TMod, TModGetter>` | Same signature | ~1140 |

### Pattern
```csharp
// REPLACE the EnumerateMajorRecords loop with:
foreach (var group in _sourceMod.EnumerateGroups())
{
    if (group.RecordCache.TryGetValue(formKey, out var rec))
    {
        majorRec = rec;
        return true;
    }
}
// Fall through: still need to check nested records (cells, placed objects)
// Keep existing EnumerateMajorRecords as fallback for nested types
// TODO: Phase 2/3 will optimize the nested path
```

### Important
Don't just replace the loop — nested records (PlacedObjects, Cells, etc.) won't be in any top-level group. The old enumeration path must remain as a fallback. Consider structuring as: try groups first, then enumerate only non-group records (or just keep the full EnumerateMajorRecords as fallback for now).

---

## Task 4: Optimize EditorID TryResolve

**Status**: ✅ Complete
**Blocked by**: Task 1 (needs EnumerateGroups for untyped variant)
**Parallel with**: Task 3

### Goal
EditorID lookups can't use dictionary lookups (no EditorID index on groups), but we can still scope the search to the relevant group instead of scanning all records.

### Key File
`Mutagen.Bethesda.Core\Plugins\Cache\Internals\Implementations\MutableModLinkCache.cs`

### Methods to Update (6 total across both classes)

**Typed** (`TryResolve<TMajor>(string editorId, ...)`):
| Class | Line |
|-------|------|
| `MutableModLinkCache` | 162 |
| `MutableModLinkCache<TMod, TModGetter>` | ~1250 |

**Untyped** (`TryResolve(string editorId, out IMajorRecordGetter)`):
| Class | Line |
|-------|------|
| `MutableModLinkCache` | 89 |
| `MutableModLinkCache<TMod, TModGetter>` | ~1176 |

**Type-param** (`TryResolve(string editorId, Type type, ...)`):
| Class | Line |
|-------|------|
| `MutableModLinkCache` | 235 |
| `MutableModLinkCache<TMod, TModGetter>` | ~1330 |

### Pattern (typed)
```csharp
// ADD fast path before existing loop:
var group = _sourceMod.TryGetTopLevelGroup(typeof(TMajor));
if (group != null)
{
    foreach (var item in group.Records)
    {
        if (item is IMajorRecordGetter majRec && editorId.Equals(majRec.EditorID))
        {
            majorRec = (TMajor)item;
            return true;
        }
    }
    majorRec = default;
    return false; // Definitive miss — type only exists in this group
}
// Existing loop stays as fallback for nested types
```

### Pattern (untyped — needs EnumerateGroups)
```csharp
foreach (var group in _sourceMod.EnumerateGroups())
{
    foreach (var item in group.Records)
    {
        if (editorId.Equals(item.EditorID))
        {
            majorRec = item;
            return true;
        }
    }
}
// Fall through for nested records
```

### Performance Note
Scanning one group (~2000 NPCs) is far better than scanning all ~100k records, even though it's still O(group_size) per lookup.

---

## Task 5: Optimize SimpleContext TryResolve Methods

**Status**: ✅ Complete
**Blocked by**: Tasks 2, 3, 4

### Goal
Apply the same optimizations to `TryResolveSimpleContext` methods. These follow identical patterns to the non-context methods but wrap results in `IModContext<T>`.

### Key File
`Mutagen.Bethesda.Core\Plugins\Cache\Internals\Implementations\MutableModLinkCache.cs`

### Methods to Update (~12 across both classes)

**In `MutableModLinkCache`:**

| Method | Line | Lookup Type |
|--------|------|-------------|
| `TryResolveSimpleContext(FormKey, ...)` (untyped) | 381 | Untyped FormKey |
| `TryResolveSimpleContext(string, ...)` (untyped) | 417 | Untyped EditorID |
| `TryResolveSimpleContext<TMajor>(FormKey, ...)` | 445 | Typed FormKey |
| `TryResolveSimpleContext<TMajor>(string, ...)` | 482 | Typed EditorID |
| `TryResolveSimpleContext(FormKey, Type, ...)` | 512 | Type-param FormKey |
| `TryResolveSimpleContext(string, Type, ...)` | 554 | Type-param EditorID |

**Same 6 methods in `MutableModLinkCache<TMod, TModGetter>`**, plus the full-context variants:
- `TryResolveContext<TMajor, TMajorGetter>(FormKey, ...)`
- `TryResolveContext<TMajor, TMajorGetter>(string, ...)`
- `TryResolveContext(FormKey, Type, ...)`
- `TryResolveContext(string, Type, ...)`
- `TryResolveUntypedContext(FormKey, ...)`
- `TryResolveUntypedContext(string, ...)`

### Context Wrapping
When using the group fast path, wrap the result in a `ModContext<T>`:
```csharp
majorRec = new ModContext<TMajor>(_sourceMod.ModKey, parent: null, record);
```

For top-level group records, `parent: null` is correct (they have no parent context).

---

## Task 6: Review MutableLoadOrderLinkCache

**Status**: Ready
**Blocked by**: Tasks 2, 3, 4, 5 (all complete)

### Goal
Verify `MutableLoadOrderLinkCache` benefits transitively from the improved `MutableModLinkCache`. Look for additional optimization opportunities.

### Key File
`Mutagen.Bethesda.Core\Plugins\Cache\Internals\Implementations\MutableLoadOrderLinkCache.cs`

### What to Check
1. Each `MutableModLinkCache.TryResolve` call is now O(1) for top-level types — verify the composite cache benefits automatically
2. **Possible optimization**: If `formKey.ModKey` doesn't match any mutable mod's ModKey, skip mutable lookups entirely (only valid for `ResolveTarget.Origin`)
3. Verify generic `MutableLoadOrderLinkCache<TMod, TModGetter>` variant also delegates correctly
4. Check context-based resolution methods

### Scope
This is primarily a review/verification task. The heavy lifting is done by Tasks 2-5. May only need minor tweaks.

---

## Task 7: Add Unit Tests

**Status**: Blocked
**Blocked by**: Tasks 2-6

### Goal
Add targeted tests for the new optimized code paths and verify existing tests still pass.

### Key Files
- `Mutagen.Bethesda.UnitTests\Plugins\Cache\Linking\Implementations\MutableDirectTests.cs`
- `Mutagen.Bethesda.UnitTests\Plugins\Cache\Linking\Implementations\MutableOverlayTests.cs`
- `Mutagen.Bethesda.UnitTests\Plugins\Cache\Linking\Helpers\` (test infrastructure)

### Test Cases

**Regression (existing tests must pass)**:
- All tests in `MutableResolveDirectTests`
- All tests in `MutableContextResolveDirectTests`

**New tests to add**:

| Test | Description |
|------|-------------|
| Top-level typed hit | `TryResolve<INpcGetter>(formKey)` finds NPC in group |
| Top-level typed miss | `TryResolve<INpcGetter>(badFormKey)` returns false |
| Top-level untyped hit | `TryResolve(formKey)` finds record across groups |
| Top-level untyped miss | `TryResolve(badFormKey)` returns false |
| Type-param hit | `TryResolve(formKey, typeof(INpcGetter))` uses group fast path |
| EditorID typed hit | `TryResolve<INpcGetter>(editorId)` scans correct group |
| EditorID untyped hit | `TryResolve(editorId)` finds across groups |
| Nested type fallback | `TryResolve` for PlacedObject still works (falls through) |
| Mutation add + resolve | Add NPC, resolve finds it |
| Mutation remove + resolve | Remove NPC, resolve returns false |
| Context methods | `TryResolveSimpleContext` returns proper `IModContext` |
| Load order integration | `MutableLoadOrderLinkCache` resolves from mutable mod |

---

## Task 8: Final Build + Test Validation

**Status**: Blocked
**Blocked by**: Task 7

### Goal
Full solution build and test run to confirm Phase 1 is complete and correct.

### Steps
1. `dotnet build` from repo root — verify zero errors
2. Run linking tests: `dotnet test --filter "Mutable"` (or appropriate filter)
3. Run full test suite if feasible
4. Fix any compilation errors or test failures
5. Clean up any remaining TODO comments that are now resolved

---

## Summary

| Task | Description | Deps | Estimated Complexity |
|------|-------------|------|---------------------|
| 1 | Add EnumerateGroups() | None | Medium (involves code gen) |
| 2 | Optimize typed FormKey TryResolve | None | Low (straightforward fast path) |
| 3 | Optimize untyped FormKey TryResolve | 1 | Low |
| 4 | Optimize EditorID TryResolve | 1 | Low |
| 5 | Optimize SimpleContext methods | 2,3,4 | Medium (many methods, context wrapping) |
| 6 | Review LoadOrderLinkCache | 2,3,4,5 | Low (mostly review) |
| 7 | Unit tests | 2-6 | Medium |
| 8 | Build + validation | 7 | Low |
