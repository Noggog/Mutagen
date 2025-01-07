using Mutagen.Bethesda.Plugins.Masters.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI;

/// <summary>
/// Provides the listings.  This typically includes implicit, listed, Creation Club, and transitive masters.
/// </summary>
public interface ILoadOrderListingsProvider : IListingsProvider
{
}

public sealed class LoadOrderListingsProvider : ILoadOrderListingsProvider
{
    private readonly IOrderListings _orderListings;
    private readonly IImplicitListingsProvider _implicitListingsProvider;
    private readonly IPluginListingsProvider _pluginListingsProvider;
    private readonly ICreationClubListingsProvider _cccListingsProvider;
    private readonly ITransitiveMasterLocator _transitiveMasterLocator;

    public LoadOrderListingsProvider(
        IOrderListings orderListings,
        IImplicitListingsProvider implicitListingsProvider,
        IPluginListingsProvider pluginListingsProvider,
        ICreationClubListingsProvider cccListingsProvider,
        ITransitiveMasterLocator transitiveMasterLocator)
    {
        _orderListings = orderListings;
        _implicitListingsProvider = implicitListingsProvider;
        _pluginListingsProvider = pluginListingsProvider;
        _cccListingsProvider = cccListingsProvider;
        _transitiveMasterLocator = transitiveMasterLocator;
    }
        
    /// <inheritdoc />
    public IEnumerable<ILoadOrderListingGetter> Get()
    {
        var implicitListings = _implicitListingsProvider.Get().ToArray();

        var listedPlugins = _pluginListingsProvider.Get().Except(implicitListings)
            .OnlyEnabled()
            .ToDictionary(x => x.ModKey, x => x);

        var transitivePluginsModKeys = _transitiveMasterLocator.GetAllMasters(listedPlugins.Keys);

        var transitivePlugins = transitivePluginsModKeys
            .Select(x => new LoadOrderListing(x, enabled: true))
            .Cast<ILoadOrderListingGetter>()
            .ToArray();
        
        return _orderListings.Order(
            implicitListings: implicitListings,
            pluginsListings: transitivePlugins,
            creationClubListings: _cccListingsProvider.Get(throwIfMissing: false),
            selector: x => x.ModKey);
    }
}

public sealed class LoadOrderListingsInjection : ILoadOrderListingsProvider
{
    private readonly ILoadOrderListingGetter[] _listings;
        
    public LoadOrderListingsInjection(IEnumerable<ILoadOrderListingGetter> listings)
    {
        _listings = listings.ToArray();
    }
        
    public LoadOrderListingsInjection(params ILoadOrderListingGetter[] listings)
    {
        _listings = listings;
    }
        
    public LoadOrderListingsInjection(params ModKey[] keys)
    {
        _listings = keys
            .Select<ModKey, ILoadOrderListingGetter>(x => new LoadOrderListing(x, true))
            .ToArray();
    }
        
    public IEnumerable<ILoadOrderListingGetter> Get() => _listings;
}