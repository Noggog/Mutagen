using Mutagen.Bethesda.Plugins.Masters.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IEnvironmentLoadOrderListingsProvider : IListingsProvider
{
}

public class EnvironmentLoadOrderListingsProvider : IEnvironmentLoadOrderListingsProvider
{
    private readonly ILoadOrderListingsProvider _loadOrderListingsProvider;
    private readonly ITransitiveMasterLocator _transitiveMasterLocator;
    
    public EnvironmentLoadOrderListingsProvider(
        ILoadOrderListingsProvider loadOrderListingsProvider,
        ITransitiveMasterLocator transitiveMasterLocator)
    {
        _loadOrderListingsProvider = loadOrderListingsProvider;
        _transitiveMasterLocator = transitiveMasterLocator;
    }

    public IEnumerable<ILoadOrderListingGetter> Get()
    {
        return _transitiveMasterLocator.GetAllMasters(
                _loadOrderListingsProvider
                    .Get()
                    .Where(x => x.Enabled)
                    .Select(x => x.ModKey).ToArray())
            .Select(x => new LoadOrderListing(x, enabled: true));
    }
}