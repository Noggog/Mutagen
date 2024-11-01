using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Analysis.DI;

public class MultiModFileAnalyzer
{
    public IReadOnlyCollection<TMod> Split<TMod>(TMod inputMod, int masterLimit, Func<int, ModKey> modKeyGenerator)
        where TMod : IMod
    {
        var collector = FillCollector(inputMod, masterLimit);
        if (collector.AllMasters.Count <= masterLimit) return [inputMod];
        
        int modCountIndex = 0;
        var nextOutgoingMod = ModInstantiator.Activator(modKeyGenerator(modCountIndex++), inputMod.GameRelease);

        while (collector.HasMore)
        {
            var maxSize = collector.ByMasterCount.Keys.Last();
        }
    }
    
    internal struct RecordReference(IMajorRecordGetter Record, HashSet<ModKey> Masters);

    internal RecordMasterCountCollector FillCollector(IModGetter modGetter, int masterLimit)
    {
        var ret = new RecordMasterCountCollector();
        foreach (var majRec in modGetter.EnumerateMajorRecords())
        {
            var mastersForRec = new HashSet<ModKey>();
            
            ret.AllMasters.Add(majRec.FormKey.ModKey);
            mastersForRec.Add(majRec.FormKey.ModKey);

            foreach (var link in majRec.EnumerateFormLinks())
            {
                ret.AllMasters.Add(link.FormKey.ModKey);
                mastersForRec.Add(link.FormKey.ModKey);
            }

            if (mastersForRec.Count > masterLimit)
            {
                throw new TooManyMastersException(modGetter.ModKey, mastersForRec.ToArray());
            }
                
            ret.ToSizeMapping[majRec.FormKey] = mastersForRec.Count;
            ret.ByMasterCount.GetOrAdd(mastersForRec.Count).Add(majRec.FormKey, new RecordReference(majRec, mastersForRec));
        }

        return ret;
    }

    internal class RecordMasterCountCollector
    {
        public readonly SortedDictionary<int, Dictionary<FormKey, RecordReference>> ByMasterCount = new();
        public readonly Dictionary<FormKey, int> ToSizeMapping = new();
        public readonly HashSet<ModKey> AllMasters = new();
        public bool HasMore => ToSizeMapping.Count > 0;
    }
}