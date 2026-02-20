# Mutable Cache Optimization — Next Steps

**Branch**: `mutable-caches`
**Previous work**: Tasks 1–5 complete. Tasks 6–8 remain.
**Key file modified**: `Mutagen.Bethesda.Core\Plugins\Cache\Internals\Implementations\MutableModLinkCache.cs`

---

## What Was Done (Tasks 1–5)

All `TryResolve`, `TryResolveSimpleContext`, and `TryResolveContext` methods in both `MutableModLinkCache` (non-generic) and `MutableModLinkCache<TMod, TModGetter>` (generic) were optimized:

- **Typed FormKey lookups** — O(1) via `TryGetTopLevelGroup(typeof(TMajor))` → `RecordCache.TryGetValue(formKey)`. Definitive miss if group exists but record doesn't.
- **Untyped FormKey lookups** — O(g≈60) via `EnumerateGroups()` → `RecordCache.TryGetValue(formKey)` per group. Falls through to `EnumerateMajorRecords` for nested records.
- **EditorID lookups** — Scoped to the relevant group (typed) or iterated per-group (untyped). Still O(group_size) per group but far better than O(total_records).
- **SimpleContext methods** — Full optimization with `new ModContext<T>(modKey, parent: null, record)` wrapping from group lookup.
- **Full Context methods** (generic class) — Miss fast-path via group check. Hits still need context enumeration due to `getOrAddAsOverride`/`duplicateInto` lambda requirements.

### API Note: `IReadOnlyCache<T, K>.TryGetValue(key)`
Returns `T?` (nullable), **not** `bool` with `out` parameter. Pattern used throughout:
```csharp
var rec = group.RecordCache.TryGetValue(formKey);
if (rec != null) { ... }
```

---

## Task 6: Review MutableLoadOrderLinkCache

**Status**: Ready
**File**: `Mutagen.Bethesda.Core\Plugins\Cache\Internals\Implementations\MutableLoadOrderLinkCache.cs`
**Complexity**: Low (mostly verification)

### Architecture
- Two classes: `MutableLoadOrderLinkCache` and `MutableLoadOrderLinkCache<TMod, TModGetter>`
- Constructor creates a `List<MutableModLinkCache>` from the mutable mods: `mutableMods.Select(m => m.ToUntypedMutableLinkCache()).ToList()`
- TryResolve methods iterate `_mutableMods` forward (Origin) or backward (Winner), calling `mod.TryResolve()` on each
- Also holds a `WrappedImmutableCache` for the non-mutable base

### What to Check
1. **Transitive benefit** — each `_mutableMods[i].TryResolve()` call is now O(1) for top-level types. Confirm the composite cache benefits automatically without additional changes.
2. **No TODO comments** exist in this file — no explicit optimization markers to address.
3. **Possible enhancement**: For `ResolveTarget.Origin`, if `formKey.ModKey` doesn't match any mutable mod's `ModKey`, the mutable lookups could be skipped entirely. This is a minor optimization but could help.
4. **`AllIdentifiers` methods** use `SelectMany` across `_mutableMods` — these enumerate linearly but are not part of the TryResolve hot path.
5. Verify generic `MutableLoadOrderLinkCache<TMod, TModGetter>` delegates correctly to the now-optimized context methods.

### Expected Outcome
Likely no code changes needed — just confirm that delegation works correctly and document any future optimization opportunities.

---

## Task 7: Add Unit Tests

**Status**: Blocked by Task 6
**Complexity**: Medium

### Key Files
- `Mutagen.Bethesda.UnitTests\Plugins\Cache\Linking\Implementations\MutableDirectTests.cs`
- `Mutagen.Bethesda.UnitTests\Plugins\Cache\Linking\Implementations\MutableOverlayTests.cs`
- `Mutagen.Bethesda.UnitTests\Plugins\Cache\Linking\Helpers\` (test infrastructure)

### Regression Tests
Run all existing tests first:
- `MutableResolveDirectTests`
- `MutableContextResolveDirectTests`

### New Test Cases

| Test | What It Validates |
|------|-------------------|
| Top-level typed hit | `TryResolve<INpcGetter>(formKey)` finds NPC via group fast path |
| Top-level typed miss | `TryResolve<INpcGetter>(badFormKey)` returns false without full scan |
| Top-level untyped hit | `TryResolve(formKey)` finds record across groups via EnumerateGroups |
| Top-level untyped miss | `TryResolve(badFormKey)` returns false |
| Type-param hit | `TryResolve(formKey, typeof(INpcGetter))` uses group fast path |
| EditorID typed hit | `TryResolve<INpcGetter>(editorId)` scans correct group only |
| EditorID untyped hit | `TryResolve(editorId)` finds across groups |
| Nested type fallback | `TryResolve` for PlacedObject still works (falls through to enumeration) |
| Mutation add + resolve | Add NPC to mod, resolve finds it immediately |
| Mutation remove + resolve | Remove NPC from mod, resolve returns false |
| SimpleContext wrapping | `TryResolveSimpleContext<INpcGetter>` returns proper `IModContext` with correct `ModKey` and `Record` |
| Load order integration | `MutableLoadOrderLinkCache` resolves from mutable mod correctly |

---

## Task 8: Final Build + Test Validation

**Status**: Blocked by Task 7
**Complexity**: Low

### Steps
1. `dotnet build` from repo root — verify zero errors
2. Run linking tests: `dotnet test --filter "Mutable"` (or appropriate filter)
3. Run full test suite if feasible
4. Fix any compilation errors or test failures
5. Clean up any remaining TODO comments that are now resolved

---

## Concerns & Edge Cases

### 1. Nested Record Fallback Path
The untyped FormKey/EditorID methods check all top-level groups first, then fall through to `EnumerateMajorRecords()` for nested records (Cells, PlacedObjects, etc.). This means:
- **For nested record lookups**: O(g) + O(n) — slightly worse than before (O(n)), but g≈60 is negligible vs n≈100k.
- **For non-existent FormKeys**: Same cost — must check all groups AND enumerate all nested records to confirm miss.
- Phase 2/3 of the optimization plan addresses nested record performance.

### 2. EditorID Case Sensitivity Inconsistency
The non-generic class uses case-sensitive `editorId.Equals(item.EditorID)` while the generic class uses `StringComparison.OrdinalIgnoreCase`. This pre-existing inconsistency was preserved — it's not introduced by our changes.

### 3. Full Context Methods Still Need Enumeration for Hits
The `TryResolveContext` methods in the generic class can only optimize the **miss** path (definitive miss via group check). For hits, they still need `EnumerateMajorRecordContexts` because constructing `GroupModContext` requires game-specific group lambdas (`Func<TMod, IGroup<TMajor>>`) that can't be created from a runtime `Type` alone. This is a known limitation — a future optimization could add a `TryGetTopLevelGroupContext` method that returns context-wrapped results.

### 4. `IReadOnlyCache.TryGetValue` Returns Nullable, Not Bool
The Noggog `IReadOnlyCache<T, K>.TryGetValue(key)` returns `T?` — **not** the `Dictionary<K,V>.TryGetValue(key, out value)` pattern. All code uses `var rec = cache.TryGetValue(key); if (rec != null) ...` or `if (rec is TMajor typed)`.

### 5. Thread Safety
No new threading concerns — the same external locking requirement applies as before. The optimized paths use the same `_sourceMod` group data that was always mutable.

### 6. Meta-Interface Types (e.g., `IPlaceableObjectGetter`)
Types like `IPlaceableObjectGetter` that map to multiple concrete types will return `null` from `TryGetTopLevelGroup()`, correctly falling through to the enumeration path. No special handling needed.

---

## File Change Summary

| File | Status |
|------|--------|
| `Mutagen.Bethesda.Core\Plugins\Cache\Internals\Implementations\MutableModLinkCache.cs` | Modified — all TryResolve/Context methods optimized |
| `MUTABLE_CACHE_TASKS.md` | Updated — Tasks 1–5 marked complete |
| `MUTABLE_CACHE_NEXT_STEPS.md` | New — this file |

### Uncommitted changes
All work is uncommitted on the `mutable-caches` branch. Previous commit was `23b9572bf Some enumerate groups logic`.
