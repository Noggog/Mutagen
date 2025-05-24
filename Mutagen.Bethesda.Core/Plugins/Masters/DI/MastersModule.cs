using StrongInject;

namespace Mutagen.Bethesda.Plugins.Masters.DI;

[Register<TransitiveMasterContentCalculator, ITransitiveMasterContentCalculator>]
[Register<MasterReferenceReaderFactory, IMasterReferenceReaderFactory>]
[Register<MasterFlagsLookupCompiler, IMasterFlagsLookupCompiler>]
[Register<KeyedMasterStyleReader, IKeyedMasterStyleReader>]
internal class MastersModule
{
    
}