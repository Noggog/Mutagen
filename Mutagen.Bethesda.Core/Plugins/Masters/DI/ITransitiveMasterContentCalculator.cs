namespace Mutagen.Bethesda.Plugins.Masters.DI;

public interface ITransitiveMasterContentCalculator
{
    IReadOnlyCollection<ModKey> GetAllMastersUnordered(
        ModKey self,
        IEnumerable<ModKey> starterMasters,
        Func<ModKey, IEnumerable<ModKey>> masterFetcher);
}

public class TransitiveMasterContentCalculator : ITransitiveMasterContentCalculator
{
    public IReadOnlyCollection<ModKey> GetAllMastersUnordered(
        ModKey self,
        IEnumerable<ModKey> starterMasters,
        Func<ModKey, IEnumerable<ModKey>> masterFetcher)
    {
        var masters = new HashSet<ModKey>();
        var remainingMasters = new Queue<ModKey>(starterMasters);
          
        while (remainingMasters.Count > 0)
        {
            var master = remainingMasters.Dequeue();
            masters.Add(master);

            IEnumerable<ModKey> mastersOfMod = masterFetcher(master);
            
            foreach (var parent in mastersOfMod)
            {
                var masterKey = parent;

                if (masterKey != self && masters.Add(parent))
                {
                    remainingMasters.Enqueue(parent);
                }
            }
        }

        return masters;
    }
}