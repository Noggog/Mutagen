using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

public interface ITransitiveMasterLocator
{
    IReadOnlyCollection<ModKey> GetAllMastersUnordered(
        IReadOnlyCollection<ModKey> starterMasters);
    IReadOnlyCollection<ModKey> GetAllMastersUnordered(
        ModKey self,
        IEnumerable<ModKey> starterMasters);
    IReadOnlyCollection<ModKey> GetAllMastersUnordered(
        ModKey self,
        IEnumerable<ModKey> starterMasters,
        IReadOnlyCache<IModListingGetter<IModGetter>, ModKey>? alreadyLocatedMods);
}

public class TransitiveMasterLocator : ITransitiveMasterLocator
{
    private readonly IFileSystem _fileSystem;
    private readonly IDataDirectoryProvider _dataDirectoryProvider;
    private readonly IGameReleaseContext _gameReleaseContext;
    private readonly ITransitiveMasterCalculator _transitiveMasterCalculator;

    public TransitiveMasterLocator(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectoryProvider,
        IGameReleaseContext gameReleaseContext,
        ITransitiveMasterCalculator transitiveMasterCalculator)
    {
        _fileSystem = fileSystem;
        _dataDirectoryProvider = dataDirectoryProvider;
        _gameReleaseContext = gameReleaseContext;
        _transitiveMasterCalculator = transitiveMasterCalculator;
    }
    
    public IReadOnlyCollection<ModKey> GetAllMastersUnordered(
        IReadOnlyCollection<ModKey> starterMasters)
    {
        return starterMasters
            .SelectMany(x => GetAllMastersUnordered(x, starterMasters, alreadyLocatedMods: null))
            .ToHashSet();
    }
    
    public IReadOnlyCollection<ModKey> GetAllMastersUnordered(
        ModKey self,
        IEnumerable<ModKey> starterMasters)
    {
        return GetAllMastersUnordered(self, starterMasters, alreadyLocatedMods: null);
    }
    
    public IReadOnlyCollection<ModKey> GetAllMastersUnordered(
        ModKey self,
        IEnumerable<ModKey> starterMasters,
        IReadOnlyCache<IModListingGetter<IModGetter>, ModKey>? alreadyLocatedMods)
    {
        return _transitiveMasterCalculator.GetAllMastersUnordered(
            self: self,
            starterMasters: starterMasters,
            masterFetcher: master =>
            {
                if (alreadyLocatedMods != null 
                    && alreadyLocatedMods.TryGetValue(master, out var locatedMod)
                    && locatedMod.Mod != null)
                {
                    return locatedMod.Mod.MasterReferences
                        .Select(x => x.Master)
                        .ToArray();
                }
                var modPath = new ModPath(Path.Combine(_dataDirectoryProvider.Path, master.FileName));
                var header = ModHeaderFrame.FromPath(modPath, _gameReleaseContext.Release, fileSystem: _fileSystem);
                return header.Masters(master).Select(x => x.Master).ToArray();
            });
    }
}