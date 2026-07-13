namespace Mutagen.Bethesda.Starfield;

public partial class Ammunition
{
    [Flags]
    public enum MajorFlag : uint
    {
        NonPlayable = 0x0000_0004,
        GroundPiece = 0x0000_0010,
        HiddenFromLocalMap = 0x0000_0200,
        UsedAsPlatform = 0x0000_0800,
        HasCurrents = 0x0008_0000,
        NavmeshFilter = 0x0400_0000,
        NavmeshBoundingBox = 0x0800_0000,
        NavmeshOnlyCut = 0x1000_0000,
        NavmeshIgnoreErosion = 0x2000_0000,
        NavmeshGround = 0x4000_0000,
        MustBeUnique = 0x8000_0000,
    }

    [Flags]
    public enum Flag
    {
        IgnoresNormalWeaponResistance = 0x01,
        NonPlayable = 0x02,
        HasCountBased3d = 0x04
    }
}