using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Shouldly;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class EnumerateGroupsTests
{
    [Theory, MutagenModAutoData]
    public void ReturnsNonEmpty(SkyrimMod mod)
    {
        var groups = mod.EnumerateGroups().ToList();
        groups.ShouldNotBeEmpty();
    }

    [Theory, MutagenModAutoData]
    public void AllItemsAreIGroupGetter(SkyrimMod mod)
    {
        foreach (var group in mod.EnumerateGroups())
        {
            group.ShouldBeAssignableTo<IGroupGetter>();
        }
    }

    [Theory, MutagenModAutoData]
    public void DoesNotContainListGroups(SkyrimMod mod)
    {
        foreach (var group in mod.EnumerateGroups())
        {
            group.ShouldNotBeAssignableTo<IListGroupGetter>();
        }
    }

    [Theory, MutagenModAutoData]
    public void ContainsNpcsGroup(SkyrimMod mod)
    {
        mod.EnumerateGroups().ShouldContain(mod.Npcs);
    }

    [Theory, MutagenModAutoData]
    public void ContainsWeaponsGroup(SkyrimMod mod)
    {
        mod.EnumerateGroups().ShouldContain(mod.Weapons);
    }

    [Theory, MutagenModAutoData]
    public void ContainsSpellsGroup(SkyrimMod mod)
    {
        mod.EnumerateGroups().ShouldContain(mod.Spells);
    }

    [Theory, MutagenModAutoData]
    public void ContainsWorldspacesGroup(SkyrimMod mod)
    {
        mod.EnumerateGroups().ShouldContain(mod.Worldspaces);
    }

    [Theory, MutagenModAutoData]
    public void AllGroupsHaveDistinctContainedRecordType(SkyrimMod mod)
    {
        var groups = mod.EnumerateGroups().ToList();
        var types = groups.Select(g => g.ContainedRecordType).ToList();
        types.Count.ShouldBe(types.Distinct().Count());
    }

    [Theory, MutagenModAutoData]
    public void YieldsConsistentCountAcrossMultipleCalls(SkyrimMod mod)
    {
        var count1 = mod.EnumerateGroups().Count();
        var count2 = mod.EnumerateGroups().Count();
        count1.ShouldBe(count2);
    }

    [Theory, MutagenModAutoData]
    public void EmptyGroupsAreYielded(SkyrimMod mod)
    {
        // Fresh mod has no records, but all groups should be yielded
        var groups = mod.EnumerateGroups().ToList();
        groups.Count.ShouldBeGreaterThan(50);
        groups.ShouldAllBe(g => g.Count == 0);
    }

    [Theory, MutagenModAutoData]
    public void AllGroupsHaveNonNullContainedRecordType(SkyrimMod mod)
    {
        foreach (var group in mod.EnumerateGroups())
        {
            group.ContainedRecordType.ShouldNotBeNull();
        }
    }

    [Theory, MutagenModAutoData]
    public void GroupWithRecordHasPopulatedRecordCache(SkyrimMod mod)
    {
        var npc = mod.Npcs.AddNew();

        var npcsGroup = mod.EnumerateGroups()
            .Single(g => ReferenceEquals(g, mod.Npcs));

        npcsGroup.RecordCache.Count.ShouldBe(1);
        npcsGroup.RecordCache.TryGetValue(npc.FormKey, out var found).ShouldBeTrue();
        found.ShouldBe(npc);
    }

    [Theory, MutagenModAutoData]
    public void CanFindRecordByFormKeyAcrossGroups(SkyrimMod mod)
    {
        var npc = mod.Npcs.AddNew();
        var weapon = mod.Weapons.AddNew();

        IMajorRecordGetter? foundNpc = null;
        IMajorRecordGetter? foundWeapon = null;

        foreach (var group in mod.EnumerateGroups())
        {
            if (group.RecordCache.TryGetValue(npc.FormKey, out var rec))
                foundNpc = rec;
            if (group.RecordCache.TryGetValue(weapon.FormKey, out var rec2))
                foundWeapon = rec2;
        }

        foundNpc.ShouldNotBeNull();
        foundNpc.ShouldBe(npc);
        foundWeapon.ShouldNotBeNull();
        foundWeapon.ShouldBe(weapon);
    }

    [Theory, MutagenModAutoData]
    public void NonExistentFormKeyNotFoundAcrossGroups(SkyrimMod mod)
    {
        var bogusFormKey = new FormKey(mod.ModKey, 0xDEAD);

        foreach (var group in mod.EnumerateGroups())
        {
            group.RecordCache.TryGetValue(bogusFormKey, out _).ShouldBeFalse();
        }
    }

    [Theory, MutagenModAutoData]
    public void WorksThroughIModGetterInterface(SkyrimMod mod)
    {
        mod.Npcs.AddNew();

        IModGetter modGetter = mod;
        var groups = modGetter.EnumerateGroups().ToList();

        groups.ShouldNotBeEmpty();
        groups.Any(g => g.RecordCache.Count > 0).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void AddedRecordIsReflectedInEnumeratedGroup(SkyrimMod mod)
    {
        // Enumerate before adding
        var groupsBefore = mod.EnumerateGroups()
            .Single(g => ReferenceEquals(g, mod.Npcs));
        groupsBefore.RecordCache.Count.ShouldBe(0);

        // Add a record
        var npc = mod.Npcs.AddNew();

        // Enumerate again - the same group object should now reflect the new record
        var groupsAfter = mod.EnumerateGroups()
            .Single(g => ReferenceEquals(g, mod.Npcs));
        groupsAfter.RecordCache.Count.ShouldBe(1);
        groupsAfter.RecordCache.TryGetValue(npc.FormKey, out _).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void RemovedRecordIsReflectedInEnumeratedGroup(SkyrimMod mod)
    {
        var npc = mod.Npcs.AddNew();

        // Verify it's there
        mod.EnumerateGroups()
            .Single(g => ReferenceEquals(g, mod.Npcs))
            .RecordCache.Count.ShouldBe(1);

        // Remove it
        mod.Npcs.Remove(npc.FormKey);

        // Verify it's gone
        mod.EnumerateGroups()
            .Single(g => ReferenceEquals(g, mod.Npcs))
            .RecordCache.Count.ShouldBe(0);
    }

    [Theory, MutagenModAutoData]
    public void MultipleRecordsInSameGroupAllAccessible(SkyrimMod mod)
    {
        var npc1 = mod.Npcs.AddNew();
        var npc2 = mod.Npcs.AddNew();
        var npc3 = mod.Npcs.AddNew();

        var npcsGroup = mod.EnumerateGroups()
            .Single(g => ReferenceEquals(g, mod.Npcs));

        npcsGroup.RecordCache.Count.ShouldBe(3);
        npcsGroup.RecordCache.TryGetValue(npc1.FormKey, out _).ShouldBeTrue();
        npcsGroup.RecordCache.TryGetValue(npc2.FormKey, out _).ShouldBeTrue();
        npcsGroup.RecordCache.TryGetValue(npc3.FormKey, out _).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void RecordsInDifferentGroupsDoNotCrossContaminate(SkyrimMod mod)
    {
        var npc = mod.Npcs.AddNew();
        var weapon = mod.Weapons.AddNew();

        var npcsGroup = mod.EnumerateGroups()
            .Single(g => ReferenceEquals(g, mod.Npcs));
        var weaponsGroup = mod.EnumerateGroups()
            .Single(g => ReferenceEquals(g, mod.Weapons));

        // NPC should not be in weapons group
        weaponsGroup.RecordCache.TryGetValue(npc.FormKey, out _).ShouldBeFalse();
        // Weapon should not be in NPCs group
        npcsGroup.RecordCache.TryGetValue(weapon.FormKey, out _).ShouldBeFalse();
    }

    [Theory, MutagenModAutoData]
    public void CountMatchesSumOfAllGroupRecordCounts(SkyrimMod mod)
    {
        mod.Npcs.AddNew();
        mod.Npcs.AddNew();
        mod.Weapons.AddNew();

        var totalRecords = mod.EnumerateGroups()
            .Sum(g => g.RecordCache.Count);

        totalRecords.ShouldBe(3);
    }

    [Theory, MutagenModAutoData]
    public void GroupCountDoesNotChangeAfterAddingRecords(SkyrimMod mod)
    {
        var countBefore = mod.EnumerateGroups().Count();

        mod.Npcs.AddNew();
        mod.Weapons.AddNew();
        mod.Spells.AddNew();

        var countAfter = mod.EnumerateGroups().Count();
        countAfter.ShouldBe(countBefore);
    }

    [Theory, MutagenModAutoData]
    public void EnumerationOrderIsConsistent(SkyrimMod mod)
    {
        var types1 = mod.EnumerateGroups().Select(g => g.ContainedRecordType).ToList();
        var types2 = mod.EnumerateGroups().Select(g => g.ContainedRecordType).ToList();

        types1.ShouldBe(types2);
    }
}

public class EnumerateGroupsOverlayTests
{
    [Fact]
    public void WorksOnBinaryOverlay()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        mod.Npcs.AddNew(mod.GetNextFormKey());

        using var stream = new MemoryTributary();
        mod.WriteToBinary(stream);
        stream.Position = 0;

        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);
        var groups = overlay.EnumerateGroups().ToList();

        groups.ShouldNotBeEmpty();
        groups.ShouldAllBe(g => g is IGroupGetter);
    }

    [Fact]
    public void OverlayGroupContainsSerializedRecords()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npcFormKey = mod.GetNextFormKey();
        var npc = mod.Npcs.AddNew(npcFormKey);

        using var stream = new MemoryTributary();
        mod.WriteToBinary(stream);
        stream.Position = 0;

        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);

        IMajorRecordGetter? found = null;
        foreach (var group in overlay.EnumerateGroups())
        {
            if (group.RecordCache.TryGetValue(npcFormKey, out var rec))
            {
                found = rec;
                break;
            }
        }

        found.ShouldNotBeNull();
        found!.FormKey.ShouldBe(npcFormKey);
    }

    [Fact]
    public void OverlayDoesNotContainListGroups()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        using var stream = new MemoryTributary();
        mod.WriteToBinary(stream);
        stream.Position = 0;

        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);

        foreach (var group in overlay.EnumerateGroups())
        {
            group.ShouldNotBeAssignableTo<IListGroupGetter>();
        }
    }

    [Fact]
    public void OverlayGroupCountMatchesDirectModGroupCount()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);

        using var stream = new MemoryTributary();
        mod.WriteToBinary(stream);
        stream.Position = 0;

        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);

        var directCount = mod.EnumerateGroups().Count();
        var overlayCount = overlay.EnumerateGroups().Count();

        overlayCount.ShouldBe(directCount);
    }

    [Fact]
    public void OverlayMultipleRecordsAcrossGroupsAllFound()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimSE);
        var npcFormKey = mod.GetNextFormKey();
        var weaponFormKey = mod.GetNextFormKey();
        mod.Npcs.AddNew(npcFormKey);
        mod.Weapons.AddNew(weaponFormKey);

        using var stream = new MemoryTributary();
        mod.WriteToBinary(stream);
        stream.Position = 0;

        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);

        var foundFormKeys = new HashSet<FormKey>();
        foreach (var group in overlay.EnumerateGroups())
        {
            foreach (var formKey in group.FormKeys)
            {
                foundFormKeys.Add(formKey);
            }
        }

        foundFormKeys.ShouldContain(npcFormKey);
        foundFormKeys.ShouldContain(weaponFormKey);
    }
}
