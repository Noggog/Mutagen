# Mutable Cache Optimization Plan

**Issue**: [#229 - Improve MutableModLinkCache performance](https://github.com/Mutagen-Modding/Mutagen/issues/229)
**Depends on**: [#87 - Add EnumerateGroups to IMod](https://github.com/Mutagen-Modding/Mutagen/issues/87)
**Branch**: `mutable-caches`

---

## Problem Statement

The `MutableModLinkCache` currently has O(n) performance for **every** lookup, where n = total major records in the mod. It calls `EnumerateMajorRecords()` / `EnumerateMajorRecords<TMajor>()` and linearly scans for each `TryResolve`. For a mod with 100k records, doing 1000 lookups means ~100 million comparisons.

This is especially painful in `MutableLoadOrderLinkCache`, where mutable mods sit at the end of the load order (highest priority). For `ResolveTarget.Winner`, the mutable mods are checked **first**, so every lookup pays this cost before falling through to the efficient immutable cache.

---

## Current Architecture

### Key Files
- `MutableModLinkCache.cs` - Single mod mutable cache (all O(n) loops)
- `MutableLoadOrderLinkCache.cs` - Composite: immutable base + mutable mods
- `ImmutableModLinkCache.cs` / `InternalImmutableModLinkCache.cs` - Cached O(1) lookups
- `ImmutableModLinkCacheCategory.cs` - Per-type dictionary caching
- `ModContext.cs` - Context wrapping with parent chain + callbacks
- `IGroup.cs` / `AGroup.cs` - Group interfaces with `RecordCache` (FormKey-indexed dictionary)
- `IMod.cs` - Has `TryGetTopLevelGroup<TMajor>()` already

### Three Record Storage Patterns

| Pattern | Storage | FormKey Lookup | Examples |
|---------|---------|----------------|----------|
| **IGroup\<T\>** | FormKey-indexed cache | O(1) via `RecordCache` | Npcs, Weapons, Spells (~60 types) |
| **IListGroup\<T\>** | Sequential list | O(n) scan | Cell blocks at mod level |
| **ExtendedList\<T\>** | Plain list | O(n) scan | PlacedObjects in Cells, WorldspaceBlocks |

### The Core Insight

For **top-level group records** (Npcs, Weapons, etc.), the group itself is already a dictionary. If we know the type, we can go directly to `mod.TryGetTopLevelGroup<INpcGetter>()?.RecordCache.TryGetValue(formKey)` for O(1) lookup - **even on a mutable mod**. The group's internal cache stays in sync because `IGroup<T>` manages its own dictionary.

The hard cases are **nested records** (Cells, PlacedObjects) that live in `ExtendedList<T>` deep in the hierarchy with no dictionary index.

---

## Proposed Solution: Three-Tier Lookup Strategy

### Tier 1: Top-Level Group Direct Lookup (Easy Win)

**For typed lookups** like `TryResolve<INpcGetter>(formKey)`:

1. Use `TryGetTopLevelGroup<TMajor>()` to get the group
2. If group exists, use `RecordCache.TryGetValue(formKey)` for O(1) lookup
3. Done - no enumeration needed

This covers ~60 record types that live in top-level `IGroup<T>` collections. Each group already maintains a FormKey-indexed dictionary that stays in sync with mutations (add/remove/set all update the dictionary).

**For untyped lookups** like `TryResolve(formKey)` (no type parameter):

1. Iterate each group via a new `EnumerateGroups()` method
2. For each group, do `group.RecordCache.TryGetValue(formKey)` - O(1) per group
3. Total: O(g) where g = number of groups (~60), instead of O(n) where n = total records

This is the optimization the existing TODO comments reference.

### Tier 2: Nested Group Lookup via EnumerateGroups (Medium)

**For types that exist in nested structures** (Cells, DialogResponses, etc.):

`EnumerateGroups<TMajor>()` would return an `IEnumerable<IGroupGetter>` or equivalent that knows how to find all groups that could contain `TMajor`. For Cells, this means:
- Worldspace sub-cells (each WorldspaceSubBlock has `ExtendedList<Cell>`)
- Interior cells (the mod-level `Cells` list group)

A new `IRecordLookup<TMajor>` interface (or similar) could wrap this, providing a `TryGetValue(FormKey)` that loops only the relevant nested groups.

### Tier 3: Cached Lookup with Validation (The Big Optimization)

**For deeply nested records** like PlacedObjects:

These live in `Cell.Persistent` / `Cell.Temporary` as `ExtendedList<IPlaced>` - plain lists with no dictionary index. A naive lookup requires iterating every cell's placed object lists.

**Solution: Build-as-you-go cache with ModContext-style validation**

#### Cache Structure

```csharp
// Conceptual - actual implementation may differ
internal class MutableRecordLocationCache
{
    // Maps FormKey -> a "location hint" that knows where the record was last seen
    private readonly Dictionary<FormKey, RecordLocationHint> _locationCache = new();

    // Tracks whether we've done a full enumeration for a given type
    private readonly HashSet<Type> _fullyEnumeratedTypes = new();
}

internal class RecordLocationHint
{
    // The record itself (for quick reference equality check)
    public IMajorRecordGetter CachedRecord { get; }

    // How to re-confirm: e.g., "look in this Cell's Persistent list"
    // Could be a lambda, or structured data
    public Func<IModGetter, IMajorRecordGetter?> ReconfirmFunc { get; }

    // Optional: index hint for the list (short-circuit check)
    public int? ListIndex { get; }
}
```

#### Lookup Flow

```
TryResolve<IPlacedObjectGetter>(formKey):
  1. Check _locationCache for formKey
     - HIT: Call hint.ReconfirmFunc(mod)
       - If record still there -> return it (fast path)
       - If not -> remove from cache, fall through to step 2
     - MISS: Fall through to step 2

  2. Full enumeration (slow path)
     - Walk all cells -> all placed object lists
     - For EACH record found during walk, cache its location hint
     - If target found -> return it
     - If not found -> return false (cache miss confirmed)
```

#### Validation Strategy

The `ReconfirmFunc` encodes "where to look" to re-validate a cached result:

**For a PlacedObject in a Cell's Persistent list:**
```csharp
// Structured location for validation
reconfirm = (mod) =>
{
    // 1. Quick check: is the cell still where we think it is?
    var worldspace = mod.TryGetTopLevelGroup<IWorldspaceGetter>()
        ?.RecordCache.TryGetValue(worldspaceFormKey);
    if (worldspace == null) return null;

    // 2. Navigate to the cell (we cached block/subblock indices)
    var block = worldspace.SubCells.ElementAtOrDefault(blockIndex);
    var subBlock = block?.Items.ElementAtOrDefault(subBlockIndex);
    var cell = subBlock?.Items.FirstOrDefault(c => c.FormKey == cellFormKey);
    if (cell == null) return null;

    // 3. Check the list - try cached index first, then scan
    var list = cell.Persistent;
    if (listIndex < list.Count && list[listIndex].FormKey == formKey)
        return list[listIndex]; // Index still valid!

    // Index shifted, scan the list (still cheaper than full mod scan)
    return list.FirstOrDefault(p => p.FormKey == formKey);
};
```

This validation is O(1) best case (index still valid) or O(cell_size) worst case (scan one cell's list), vs O(total_placed_objects_in_entire_mod) for a fresh lookup.

#### Cache Invalidation Strategy

Since the mod is mutable, cached entries may become stale. The strategy is **optimistic with lazy validation**:

1. **No proactive invalidation** - we don't hook into mod mutations
2. **Validate on access** - each cached lookup runs `ReconfirmFunc`
3. **Rebuild on miss** - if validation fails, do a full scan and rebuild cache entries found along the way
4. **"Confirmed miss" expiry** - if we previously confirmed a FormKey doesn't exist, and someone asks again, we must re-scan (the mod may have been mutated to add it)

The key insight is that **re-validation of a cached hit is cheap** (check one specific location), while **confirming a miss is always expensive** (must scan everything). We can optimize for the common case where records exist and don't move.

---

## Implementation Plan

### Phase 1: Tier 1 - Top-Level Group Direct Lookup

**Goal**: O(1) typed lookups, O(g) untyped lookups for top-level records.

#### Step 1.1: Add `EnumerateGroups()` to IModGetter

Add to `IMajorRecordGetterEnumerable` (or a new interface):

```csharp
/// <summary>
/// Enumerates all top-level groups in the mod
/// </summary>
IEnumerable<IGroupGetter> EnumerateGroups();
```

This needs to be implemented per game mod (SkyrimMod, Fallout4Mod, etc.) - likely via code generation since the mod types are generated.

#### Step 1.2: Optimize typed TryResolve in MutableModLinkCache

For `TryResolve<TMajor>(FormKey formKey, ...)`:

```csharp
// Try top-level group first (O(1))
var group = _sourceMod.TryGetTopLevelGroup<TMajor>();
if (group != null)
{
    return group.RecordCache.TryGetValue(formKey, out var rec)
        ? (majorRec = (TMajor)rec, true)
        : (majorRec = default, false);
}

// Not a top-level type - fall through to enumeration
// (Phase 2/3 will optimize this path)
```

**Note**: `TryGetTopLevelGroup` returns null for nested types (PlacedObject, Cell, etc.) which is exactly the behavior we want - it naturally falls through.

#### Step 1.3: Optimize untyped TryResolve in MutableModLinkCache

For `TryResolve(FormKey formKey, ...)` (no type parameter):

```csharp
// Check each group's dictionary - O(g) where g ≈ 60
foreach (var group in _sourceMod.EnumerateGroups())
{
    if (group.RecordCache.TryGetValue(formKey, out var rec))
    {
        majorRec = rec;
        return true;
    }
}

// If not found in any top-level group, need to check nested records
// (Phase 2/3 will optimize this path)
```

#### Step 1.4: Optimize Type-parameter TryResolve

For `TryResolve(FormKey formKey, Type type, ...)`:

```csharp
var group = _sourceMod.TryGetTopLevelGroup(type);
if (group != null)
{
    return group.RecordCache.TryGetValue(formKey, out var rec)
        ? (majorRec = rec, true)
        : (majorRec = default, false);
}
// Fall through for nested types
```

#### Step 1.5: Handle EditorID lookups

EditorID lookups are trickier since groups don't have EditorID-indexed caches. Options:
- For typed: Scan only the relevant group's records (O(group_size) vs O(total_records))
- For untyped: Scan each group sequentially (same O(n) worst case, but can skip groups based on type)

Even without a dictionary, scanning one group of ~2000 NPCs is far better than scanning ~100k total records.

### Phase 2: Tier 2 - Nested Group Support

**Goal**: Efficient lookup for records in nested groups (Cells, etc.)

#### Step 2.1: Categorize record types

Create a mapping of record types to their lookup strategy:

```csharp
enum RecordLookupStrategy
{
    TopLevelGroup,     // Npcs, Weapons, etc. - use TryGetTopLevelGroup
    NestedInCells,     // Cells themselves (in worldspace sub-cells + mod cell list)
    NestedInPlaced,    // PlacedObject, PlacedNpc, etc. (in Cell.Persistent/Temporary)
    NestedInDialog,    // DialogResponses (in DialogTopics)
    // ... other nested patterns
}
```

#### Step 2.2: Implement specialized lookup paths

For each nested pattern, create a focused search:

**Cells**: Search `mod.Worldspaces` -> SubCells hierarchy + `mod.Cells` list group
**PlacedObjects**: Same as Cells but one level deeper into Persistent/Temporary lists
**DialogResponses**: Search `mod.DialogTopics` -> Responses

Each path searches only the relevant subset of the mod structure.

#### Step 2.3: Wire into MutableModLinkCache

After the `TryGetTopLevelGroup` fast path fails, route to the appropriate nested search based on the requested type (using metadata from the registration system to determine which strategy to use).

### Phase 3: Tier 3 - Build-As-You-Go Cache with Validation

**Goal**: Amortized near-O(1) lookups for nested records.

#### Step 3.1: Design the RecordLocationHint

```csharp
/// <summary>
/// A cached "breadcrumb" that remembers where a record was found,
/// enabling fast re-validation without full mod enumeration.
/// </summary>
internal abstract class RecordLocationHint
{
    public FormKey FormKey { get; }

    /// <summary>
    /// Attempt to re-find the record at its cached location.
    /// Returns null if the record is no longer where it was cached.
    /// </summary>
    public abstract IMajorRecordGetter? TryReconfirm(IModGetter mod);
}

/// For top-level group records - trivial validation
internal class GroupLocationHint : RecordLocationHint
{
    private readonly Type _groupType;

    public override IMajorRecordGetter? TryReconfirm(IModGetter mod)
    {
        var group = mod.TryGetTopLevelGroup(_groupType);
        return group?.RecordCache.TryGetValue(FormKey, out var rec) == true ? rec : null;
    }
}

/// For placed objects in cells - hierarchical validation
internal class CellPlacedLocationHint : RecordLocationHint
{
    public FormKey WorldspaceFormKey { get; }
    public FormKey CellFormKey { get; }
    public int BlockIndex { get; }
    public int SubBlockIndex { get; }
    public int ListIndex { get; }       // Index within Persistent/Temporary
    public bool IsPersistent { get; }   // Persistent vs Temporary

    public override IMajorRecordGetter? TryReconfirm(IModGetter mod)
    {
        // Navigate the hierarchy using cached indices
        // Check index first, then scan cell list if index shifted
        // Return null if cell/worldspace no longer exists
    }
}
```

#### Step 3.2: Add location cache to MutableModLinkCache

```csharp
public sealed class MutableModLinkCache : ILinkCache
{
    private readonly IModGetter _sourceMod;

    // NEW: Location cache for nested records
    private readonly Dictionary<FormKey, RecordLocationHint> _locationHints = new();

    // NEW: Track which types have been fully enumerated
    // (so we know cache misses are definitive)
    private readonly HashSet<Type> _fullyEnumeratedTypes = new();
}
```

#### Step 3.3: Implement cached lookup flow

```csharp
public bool TryResolve<TMajor>(FormKey formKey, out TMajor majorRec, ...)
{
    // 1. Top-level group fast path (Phase 1)
    var group = _sourceMod.TryGetTopLevelGroup<TMajor>();
    if (group != null)
    {
        if (group.RecordCache.TryGetValue(formKey, out var rec))
        {
            majorRec = (TMajor)rec;
            return true;
        }
        majorRec = default;
        return false;
    }

    // 2. Check location cache (Phase 3)
    if (_locationHints.TryGetValue(formKey, out var hint))
    {
        var reconfirmed = hint.TryReconfirm(_sourceMod);
        if (reconfirmed is TMajor found)
        {
            majorRec = found;
            return true;
        }
        // Stale - remove and fall through
        _locationHints.Remove(formKey);
    }

    // 3. Full enumeration with opportunistic caching
    foreach (var item in EnumerateNestedRecords<TMajor>())
    {
        // Cache every record we encounter
        _locationHints[item.FormKey] = BuildLocationHint(item);

        if (item.FormKey == formKey)
        {
            majorRec = (TMajor)item;
            return true;
        }
    }

    majorRec = default;
    return false;
}
```

#### Step 3.4: Context-aware location hints

Leverage the existing `ModContext` parent chain pattern to build location hints. During enumeration, the context already knows the full path (Worldspace -> Block -> SubBlock -> Cell -> record). Extract this into a `RecordLocationHint`.

#### Step 3.5: Handle cache invalidation edge cases

- **Record added after cache miss**: If we cached that a FormKey doesn't exist (by completing a full enumeration), a subsequent lookup must re-enumerate because the mod may have changed. Track this via `_fullyEnumeratedTypes` with some invalidation signal.
- **Record moved between cells**: `TryReconfirm` returns null, triggering re-enumeration
- **Record deleted**: `TryReconfirm` returns null, cache entry removed, re-enumeration confirms miss

### Phase 4: Testing & Benchmarking

#### Step 4.1: Unit Tests

Extend existing test infrastructure in `MutableDirectTests.cs` / `MutableOverlayTests.cs`:

- **Correctness tests**: Ensure all existing tests still pass
- **Mutation tests**: Add record, resolve, remove record, resolve again
- **Cache invalidation tests**:
  - Lookup PlacedObject -> found
  - Move PlacedObject to different cell
  - Lookup again -> still found (at new location)
- **Cache miss tests**:
  - Lookup non-existent FormKey -> miss
  - Add that record
  - Lookup again -> hit
- **Type-specific tests**:
  - Top-level group records (Npcs)
  - Cells (nested in worldspaces)
  - PlacedObjects (deeply nested)
  - Mixed type untyped lookups

#### Step 4.2: Benchmark Suite

Create BenchmarkDotNet benchmarks:

```csharp
[Benchmark] public void MutableCache_TopLevel_TypedLookup()     // Phase 1
[Benchmark] public void MutableCache_TopLevel_UntypedLookup()   // Phase 1
[Benchmark] public void MutableCache_Nested_FirstLookup()       // Phase 2
[Benchmark] public void MutableCache_Nested_RepeatLookup()      // Phase 3
[Benchmark] public void MutableCache_Nested_AfterMutation()     // Phase 3 validation
[Benchmark] public void MutableCache_Miss()                     // Miss path
```

Compare against:
- Current `MutableModLinkCache` (baseline)
- `ImmutableModLinkCache` (theoretical best case)

---

## Record Type Classification

This is critical for implementation - each record type falls into one of these categories:

### Top-Level (Tier 1 - Direct Group Lookup)
~60 types per game. Examples for Skyrim:
- GameSetting, Keyword, Npc, Weapon, Spell, Perk, Quest, Faction...
- Worldspace (the record itself - lookup by FormKey in `mod.Worldspaces`)
- DialogTopic (the record itself)

### Nested in Structure (Tier 2/3 - Requires Walking)
- **Cell**: Lives in `WorldspaceSubBlock.Items` (ExtendedList) AND `mod.Cells` (IListGroup -> CellBlock -> Items)
- **PlacedObject, PlacedNpc, PlacedArrow, PlacedBarrier, etc.** (IPlaced): Lives in `Cell.Persistent`/`Cell.Temporary` (ExtendedList)
- **Landscape**: Lives directly under Cell
- **NavigationMesh**: Lives in `Cell.NavigationMeshes` (ExtendedList) - note: may not be a major record
- **DialogResponses**: Lives under DialogTopic

### Determination Method
`TryGetTopLevelGroup<T>()` already returns null for nested types - this is the natural discriminator. The existing Loqui registration system can also provide this metadata.

---

## Risk Assessment

| Risk | Mitigation |
|------|------------|
| Cache grows unbounded | Cap cache size, use LRU eviction, or clear on explicit signal |
| Thread safety | Document same locking requirement as current MutableModLinkCache |
| Stale cache after mutation | Optimistic validation on every access; no silent staleness |
| Performance regression for small mods | For very small mods, overhead of cache management may exceed benefit. Consider a threshold or opt-in. |
| Complexity of location hints | Start simple (Phase 1 alone is a huge win), add Phases 2/3 incrementally |
| Code generation changes for EnumerateGroups | Leverage existing `TryGetTopLevelGroup` pattern; may not need code gen |

---

## Estimated Impact

| Scenario | Before | After (Phase 1) | After (Phase 3) |
|----------|--------|------------------|------------------|
| Typed lookup (top-level, e.g., NPC) | O(n) | **O(1)** | O(1) |
| Untyped lookup (top-level) | O(n) | **O(g) ≈ O(60)** | O(g) |
| Typed lookup (nested, e.g., PlacedObject) - first | O(n) | O(n) | O(n) + cache build |
| Typed lookup (nested) - repeat | O(n) | O(n) | **O(1) validate** |
| 1000 NPC lookups on 100k record mod | ~50M comparisons | **~1000 dict lookups** | ~1000 dict lookups |
| 1000 PlacedObject lookups on 100k record mod | ~50M comparisons | ~50M comparisons | **~1000 validations** |

Phase 1 alone is the biggest bang-for-buck and should be done first. It's also the lowest risk since it leverages existing infrastructure (`TryGetTopLevelGroup`, `RecordCache`).

---

## Open Questions

1. **EnumerateGroups scope**: Should this include only `IGroupGetter` instances, or also `IListGroupGetter`? For optimization purposes, we mainly need the FormKey-indexed groups.

2. **EditorID secondary index**: Should we add an EditorID-indexed dictionary to groups? Currently only FormKey is indexed. An EditorID index would make EditorID lookups O(1) too, but adds memory and maintenance cost.

3. **Cache lifetime**: Should the location cache be clearable/resettable? A `InvalidateCache()` or `ClearCache()` method could be useful if users know they've made bulk mutations.

4. **Concurrency model**: Current doc says "must be locked alongside mutations." Do we want to make the cache internally thread-safe (e.g., `ConcurrentDictionary`), or keep the external locking requirement?

5. **Meta-interface support**: Types like `IPlaceableObjectGetter` map to multiple concrete types. The optimization needs to handle these correctly - likely by checking all relevant groups.
