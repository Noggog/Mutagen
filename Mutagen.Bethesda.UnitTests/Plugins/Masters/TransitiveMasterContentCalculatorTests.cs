using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Masters.DI;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;

namespace Mutagen.Bethesda.UnitTests.Plugins.Masters;

public class TransitiveMasterContentCalculatorTests
{
    [Theory, MutagenAutoData]
    public void Typical(
        ModKey self,
        ModKey master,
        ModKey transitive,
        ModKey transitive2,
        TransitiveMasterContentCalculator sut)
    {
        sut.GetAllMastersUnordered(
            self: self,
            starterMasters: new[] { master },
            masterFetcher: key =>
            {
                if (key == master)
                {
                    return [transitive];
                }
                if (key == transitive)
                {
                    return [transitive2];
                }
                if (key == transitive2)
                {
                    return [];
                }
                throw new Exception();
            })
            .ShouldEqual(master, transitive, transitive2);
    }
    
    [Theory, MutagenAutoData]
    public void ExcludesSelf(
        ModKey self,
        ModKey master,
        ModKey transitive,
        ModKey transitive2,
        TransitiveMasterContentCalculator sut)
    {
        sut.GetAllMastersUnordered(
            self: self,
            starterMasters: new[] { master },
            masterFetcher: key =>
            {
                if (key == master)
                {
                    return [transitive, ];
                }
                if (key == transitive)
                {
                    return [transitive2];
                }
                if (key == transitive2)
                {
                    return [self];
                }
                throw new Exception();
            })
            .ShouldEqual(master, transitive, transitive2);
    }
}