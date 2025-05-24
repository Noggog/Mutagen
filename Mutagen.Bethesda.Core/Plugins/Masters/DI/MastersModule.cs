using StrongInject;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

[Register<TransitiveMasterCalculator, ITransitiveMasterCalculator>]
[Register<MasterReferenceReaderFactory, IMasterReferenceReaderFactory>]
[Register<MasterFlagsLookupCompiler, IMasterFlagsLookupCompiler>]
[Register<KeyedMasterStyleReader, IKeyedMasterStyleReader>]
internal class MastersModule
{
    
}