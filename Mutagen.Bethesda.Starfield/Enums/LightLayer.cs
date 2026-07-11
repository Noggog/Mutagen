namespace Mutagen.Bethesda.Starfield;

[Flags]
public enum LightLayer
{
    DefaultLightLayer = 0x1,
    HelmetLight = 0x2,
    ShipInterior = 0x4,
    ShipExterior = 0x8,
}
