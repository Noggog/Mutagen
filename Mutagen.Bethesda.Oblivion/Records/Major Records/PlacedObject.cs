using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;

namespace Mutagen.Bethesda.Oblivion;

public partial class PlacedObject
{
    [Flags]
    public enum ActionFlag
    {
        UseDefault = 0x001,
        Activate = 0x002,
        Open = 0x004,
        OpenByDefault = 0x008
    }
}

partial class PlacedObjectBinaryCreateTranslation
{
    public static partial void FillBinaryOpenByDefaultCustom(MutagenFrame frame, IPlacedObjectInternal item, PreviousParse lastParsed)
    {
        item.OpenByDefault = true;
        frame.Position += frame.MetaData.Constants.SubConstants.HeaderLength;
    }
}

partial class PlacedObjectBinaryWriteTranslation
{
    public static partial void WriteBinaryOpenByDefaultCustom(MutagenWriter writer, IPlacedObjectGetter item)
    {
        if (item.OpenByDefault)
        {
            using (HeaderExport.Subrecord(writer, RecordTypes.ONAM))
            {
            }
        }
    }
}

partial class PlacedObjectBinaryOverlay
{
    internal partial class PlacedObjectRecordDataPayload
    {
        public int? OpenByDefaultLocation;
    }

    public partial bool GetOpenByDefaultCustom() => Payload.OpenByDefaultLocation.HasValue;

    partial void OpenByDefaultCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _payload.Fields.OpenByDefaultLocation = (ushort)(stream.Position - offset);
    }
}