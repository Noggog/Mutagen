using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Skyrim;

public partial class Armor
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0004,
        Shield = 0x0000_0040
    }
    
    IFormLinkNullableGetter<IObjectEffectGetter> IEnchantableGetter.ObjectEffect => this.ObjectEffect;
}

partial class ArmorBinaryCreateTranslation
{
    public static partial void FillBinaryBodyTemplateCustom(MutagenFrame frame, IArmorInternal item, PreviousParse lastParsed)
    {
        item.BodyTemplate = BodyTemplateBinaryCreateTranslation.Parse(frame);
    }
}

partial class ArmorBinaryWriteTranslation
{
    public static partial void WriteBinaryBodyTemplateCustom(MutagenWriter writer, IArmorGetter item)
    {
        if (item.BodyTemplate is { } templ)
        {
            BodyTemplateBinaryWriteTranslation.Write(writer, templ);
        }
    }
}

partial class ArmorBinaryOverlay
{
    internal partial class ArmorRecordDataPayload
    {
        public int? BodyTemplateLocation;
    }

    public partial IBodyTemplateGetter? GetBodyTemplateCustom()
    {
        return Payload.BodyTemplateLocation.HasValue ? BodyTemplateBinaryOverlay.CustomFactory(new OverlayStream(_recordData.Slice(Payload.BodyTemplateLocation!.Value), _package), _package) : default;
    }
    public bool BodyTemplate_IsSet => Payload.BodyTemplateLocation.HasValue;

    partial void BodyTemplateCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _payload.Fields.BodyTemplateLocation = (stream.Position - offset);
    }
}