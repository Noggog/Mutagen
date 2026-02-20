using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking.Helpers;
using Xunit;

#nullable disable
#pragma warning disable CS0618 // Type or member is obsolete

namespace Mutagen.Bethesda.UnitTests.Plugins.Cache.Linking;

/// <summary>
/// Tests specifically targeting the mutable cache optimization paths:
/// - Typed FormKey group fast-path (O(1) via TryGetTopLevelGroup)
/// - Untyped FormKey group enumeration (O(g) via EnumerateGroups)
/// - EditorID scoped group lookups
/// - SimpleContext wrapping
/// - Mutation visibility (add/remove records)
/// - Load order integration
/// </summary>
public class MutableCacheOptimizationTests : IClassFixture<LinkingTestInit>, IClassFixture<LoquiUse>
{
    public MutableCacheOptimizationTests(LinkingTestInit testInit, LoquiUse loqui)
    {
    }

    #region Typed FormKey Lookups (Group Fast Path)

    [Fact]
    public void TypedFormKey_TopLevel_Hit()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.True(cache.TryResolve<INpcGetter>(npc.FormKey, out var found));
        Assert.Same(npc, found);
    }

    [Fact]
    public void TypedFormKey_TopLevel_Miss()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        mod.Npcs.AddNew(); // Add an NPC so the group exists
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        var badFormKey = new FormKey(TestConstants.PluginModKey, 0xFFFFFF);
        Assert.False(cache.TryResolve<INpcGetter>(badFormKey, out _));
    }

    [Fact]
    public void TypedFormKey_WrongType_Miss()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        // Looking for the NPC's FormKey but as an Ammunition type should miss
        Assert.False(cache.TryResolve<IAmmunitionGetter>(npc.FormKey, out _));
    }

    #endregion

    #region Untyped FormKey Lookups (EnumerateGroups Path)

    [Fact]
    public void UntypedFormKey_TopLevel_Hit()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.True(cache.TryResolve(npc.FormKey, out IMajorRecordGetter found));
        Assert.Same(npc, found);
    }

    [Fact]
    public void UntypedFormKey_TopLevel_Miss()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        var badFormKey = new FormKey(TestConstants.PluginModKey, 0xFFFFFF);
        Assert.False(cache.TryResolve(badFormKey, out IMajorRecordGetter _));
    }

    #endregion

    #region Type Parameter Lookups

    [Fact]
    public void TypeParam_FormKey_Hit()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.True(cache.TryResolve(npc.FormKey, typeof(INpcGetter), out var found));
        Assert.Same(npc, found);
    }

    [Fact]
    public void TypeParam_FormKey_Miss()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        var badFormKey = new FormKey(TestConstants.PluginModKey, 0xFFFFFF);
        Assert.False(cache.TryResolve(badFormKey, typeof(INpcGetter), out _));
    }

    #endregion

    #region EditorID Lookups

    [Fact]
    public void EditorID_Typed_Hit()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        npc.EditorID = "TestNpc";
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.True(cache.TryResolve<INpcGetter>("TestNpc", out var found));
        Assert.Same(npc, found);
    }

    [Fact]
    public void EditorID_Typed_Miss()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        npc.EditorID = "TestNpc";
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.False(cache.TryResolve<INpcGetter>("NonExistentEditorId", out _));
    }

    [Fact]
    public void EditorID_Untyped_Hit()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        npc.EditorID = "TestNpc";
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.True(cache.TryResolve("TestNpc", typeof(IMajorRecordGetter), out var found));
        Assert.Same(npc, found);
    }

    #endregion

    #region Mutation Visibility

    [Fact]
    public void Mutation_AddRecord_ThenResolve()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        // Initially empty
        var formKey = new FormKey(TestConstants.PluginModKey, 0x800);
        Assert.False(cache.TryResolve<INpcGetter>(formKey, out _));

        // Add NPC with specific FormKey
        var npc = mod.Npcs.AddNew();

        // Mutable cache should see it immediately
        Assert.True(cache.TryResolve<INpcGetter>(npc.FormKey, out var found));
        Assert.Same(npc, found);
    }

    [Fact]
    public void Mutation_RemoveRecord_ThenResolve()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var fk = npc.FormKey;
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        // Should find it
        Assert.True(cache.TryResolve<INpcGetter>(fk, out _));

        // Remove it
        mod.Npcs.Remove(fk);

        // Should no longer find it
        Assert.False(cache.TryResolve<INpcGetter>(fk, out _));
    }

    #endregion

    #region SimpleContext Wrapping

    [Fact]
    public void SimpleContext_Typed_ReturnsProperModContext()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.True(cache.TryResolveSimpleContext<INpcGetter>(npc.FormKey, out var context));
        Assert.Equal(TestConstants.PluginModKey, context.ModKey);
        Assert.Same(npc, context.Record);
    }

    [Fact]
    public void SimpleContext_Untyped_ReturnsProperModContext()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.True(cache.TryResolveSimpleContext(npc.FormKey, typeof(INpcGetter), out var context));
        Assert.Equal(TestConstants.PluginModKey, context.ModKey);
        Assert.Same(npc, context.Record);
    }

    [Fact]
    public void SimpleContext_Typed_Miss()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        var badFormKey = new FormKey(TestConstants.PluginModKey, 0xFFFFFF);
        Assert.False(cache.TryResolveSimpleContext<INpcGetter>(badFormKey, out _));
    }

    #endregion

    #region Load Order Integration

    [Fact]
    public void LoadOrder_MutableMod_Resolves()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.True(cache.TryResolve<INpcGetter>(npc.FormKey, out var found));
        Assert.Same(npc, found);
    }

    [Fact]
    public void LoadOrder_MutableMod_Miss()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        mod.Npcs.AddNew();
        var cache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        var badFormKey = new FormKey(TestConstants.PluginModKey, 0xFFFFFF);
        Assert.False(cache.TryResolve<INpcGetter>(badFormKey, out _));
    }

    [Fact]
    public void LoadOrder_MultipleMutableMods_WinnerOverride()
    {
        var mod1 = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc1 = mod1.Npcs.AddNew();

        var mod2 = new SkyrimMod(TestConstants.PluginModKey2, SkyrimRelease.SkyrimLE);
        var npc2 = new Npc(npc1.FormKey, SkyrimRelease.SkyrimLE);
        mod2.Npcs.RecordCache.Set(npc2);

        var cache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(mod1, mod2);

        // Winner (last in load order) should be mod2's version
        Assert.True(cache.TryResolve<INpcGetter>(npc1.FormKey, out var winner));
        Assert.Same(npc2, winner);
    }

    [Fact]
    public void LoadOrder_MutableMod_MutationVisible()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var cache = new MutableLoadOrderLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        // Initially empty
        Assert.False(cache.TryResolve<INpcGetter>(new FormKey(TestConstants.PluginModKey, 0x800), out _));

        // Add record
        var npc = mod.Npcs.AddNew();

        // Should be visible through load order cache
        Assert.True(cache.TryResolve<INpcGetter>(npc.FormKey, out var found));
        Assert.Same(npc, found);
    }

    #endregion

    #region Multiple Record Types

    [Fact]
    public void MultipleGroups_TypedLookup_FindsCorrectGroup()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var ammo = mod.Ammunitions.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        // Each typed lookup should find only in its own group
        Assert.True(cache.TryResolve<INpcGetter>(npc.FormKey, out var foundNpc));
        Assert.Same(npc, foundNpc);

        Assert.True(cache.TryResolve<IAmmunitionGetter>(ammo.FormKey, out var foundAmmo));
        Assert.Same(ammo, foundAmmo);

        // Cross-type should miss
        Assert.False(cache.TryResolve<IAmmunitionGetter>(npc.FormKey, out _));
        Assert.False(cache.TryResolve<INpcGetter>(ammo.FormKey, out _));
    }

    [Fact]
    public void UntypedLookup_FindsAcrossGroups()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var ammo = mod.Ammunitions.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        // Untyped should find either
        Assert.True(cache.TryResolve(npc.FormKey, out IMajorRecordGetter foundNpc));
        Assert.Same(npc, foundNpc);

        Assert.True(cache.TryResolve(ammo.FormKey, out IMajorRecordGetter foundAmmo));
        Assert.Same(ammo, foundAmmo);
    }

    #endregion

    #region Full Context Methods (Generic Class)

    [Fact]
    public void FullContext_Typed_Hit()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        Assert.True(cache.TryResolveContext<INpc, INpcGetter>(npc.FormKey, out var context));
        Assert.Equal(TestConstants.PluginModKey, context.ModKey);
        Assert.Same(npc, context.Record);
    }

    [Fact]
    public void FullContext_Typed_Miss()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        mod.Npcs.AddNew();
        var cache = new MutableModLinkCache<ISkyrimMod, ISkyrimModGetter>(mod);

        var badFormKey = new FormKey(TestConstants.PluginModKey, 0xFFFFFF);
        Assert.False(cache.TryResolveContext<INpc, INpcGetter>(badFormKey, out _));
    }

    #endregion

    #region Untyped Cache

    [Fact]
    public void UntypedCache_TypedFormKey_Hit()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToUntypedMutableLinkCache();

        Assert.True(cache.TryResolve<INpcGetter>(npc.FormKey, out var found));
        Assert.Same(npc, found);
    }

    [Fact]
    public void UntypedCache_TypedFormKey_Miss()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        mod.Npcs.AddNew();
        var cache = mod.ToUntypedMutableLinkCache();

        var badFormKey = new FormKey(TestConstants.PluginModKey, 0xFFFFFF);
        Assert.False(cache.TryResolve<INpcGetter>(badFormKey, out _));
    }

    [Fact]
    public void UntypedCache_SimpleContext_Hit()
    {
        var mod = new SkyrimMod(TestConstants.PluginModKey, SkyrimRelease.SkyrimLE);
        var npc = mod.Npcs.AddNew();
        var cache = mod.ToUntypedMutableLinkCache();

        Assert.True(cache.TryResolveSimpleContext<INpcGetter>(npc.FormKey, out var context));
        Assert.Equal(TestConstants.PluginModKey, context.ModKey);
        Assert.Same(npc, context.Record);
    }

    #endregion
}
