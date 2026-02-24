using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim.Internals;

namespace Mutagen.Bethesda.Skyrim;

public partial class GameSettingBool
{
    public override GameSettingType SettingType => GameSettingType.Bool;
}

partial class GameSettingBoolBinaryCreateTranslation
{
    public static partial void FillBinaryDataCustom(MutagenFrame frame, IGameSettingBoolInternal item, PreviousParse lastParsed)
    {
        var subFrame = frame.ReadSubrecord();
        item.Data = (bool)(BinaryPrimitives.ReadUInt32LittleEndian(subFrame.Content) != 0);
    }
}

partial class GameSettingBoolBinaryWriteTranslation
{
    public static partial void WriteBinaryDataCustom(MutagenWriter writer, IGameSettingBoolGetter item)
    {
        if (item.Data is not { } data) return;
        using (HeaderExport.Subrecord(writer, RecordTypes.DATA))
        {
            writer.Write(data ? 1 : 0);
        }
    }
}

partial class GameSettingBoolBinaryOverlay
{
    internal partial class GameSettingBoolRecordDataPayload
    {
        public int? DataLocation;
    }

    bool GetDataIsSetCustom() => Payload.DataLocation.HasValue;
    public partial bool? GetDataCustom()
    {
        return Payload.DataLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(HeaderTranslation.ExtractSubrecordMemory(_recordData, Payload.DataLocation.Value, _package.MetaData.Constants)) != 0 : default;
    }
    partial void DataCustomParse(OverlayStream stream, int finalPos, int offset)
    {
        _payload.Fields.DataLocation = (ushort)(stream.Position - offset);
    }
}