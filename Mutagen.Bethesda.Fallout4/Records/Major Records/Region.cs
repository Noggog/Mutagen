using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using System.Buffers.Binary;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins.Internals;

namespace Mutagen.Bethesda.Fallout4;

public partial class Region
{
    [Flags]
    public enum MajorFlag
    {
        BorderRegion = 0x40
    }
}

partial class RegionBinaryCreateTranslation
{
    public static partial ParseResult FillBinaryRegionAreaLogicCustom(MutagenFrame frame, IRegionInternal item, PreviousParse lastParsed)
    {
        var rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out _);
        while (rdat.Equals(RecordTypes.RDAT))
        {
            ParseRegionData(frame, item);
            if (frame.Complete) break;
            rdat = HeaderTranslation.GetNextSubrecordType(frame.Reader, out _);
        }

        return null;
    }

    public static RecordTriggerSpecs GetTypes(RegionData.RegionDataType type) => type switch
    {
        RegionData.RegionDataType.Object => RegionObjects_Registration.TriggerSpecs,
        RegionData.RegionDataType.Weather => RegionWeather_Registration.TriggerSpecs,
        RegionData.RegionDataType.Map => RegionMap_Registration.TriggerSpecs,
        RegionData.RegionDataType.Grass => RegionGrasses_Registration.TriggerSpecs,
        RegionData.RegionDataType.Sound => RegionSounds_Registration.TriggerSpecs,
        RegionData.RegionDataType.Land => RegionLand_Registration.TriggerSpecs,
        _ => throw new ArgumentException($"Unexpected type {type}", nameof(type))
    };

    static void ParseRegionData(MutagenFrame frame, IRegionInternal item)
    {
        var rdatFrame = frame.Reader.GetSubrecord();
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);

        frame = frame.SpawnAll();
        
        switch (dataType)
        {
            case RegionData.RegionDataType.Object:
                item.Objects = RegionObjects.CreateFromBinary(frame);
                break;
            case RegionData.RegionDataType.Map:
                item.Map = RegionMap.CreateFromBinary(frame);
                break;
            case RegionData.RegionDataType.Grass:
                item.Grasses = RegionGrasses.CreateFromBinary(frame);
                break;
            case RegionData.RegionDataType.Sound:
                item.Sounds = RegionSounds.CreateFromBinary(frame);
                break;
            case RegionData.RegionDataType.Weather:
                item.Weather = RegionWeather.CreateFromBinary(frame);
                break;
            case RegionData.RegionDataType.Land:
                item.Land = RegionLand.CreateFromBinary(frame);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

partial class RegionBinaryWriteTranslation
{
    public static partial void WriteBinaryRegionAreaLogicCustom(MutagenWriter writer, IRegionGetter item)
    {
        item.Objects?.WriteToBinary(writer);
        item.Weather?.WriteToBinary(writer);
        item.Map?.WriteToBinary(writer);
        item.Land?.WriteToBinary(writer);
        item.Grasses?.WriteToBinary(writer);
        item.Sounds?.WriteToBinary(writer);
    }
}

partial class RegionBinaryOverlay
{
    internal partial class RegionRecordDataPayload
    {
        public ReadOnlyMemorySlice<byte>? ObjectsSpan;
        public ReadOnlyMemorySlice<byte>? WeatherSpan;
        public ReadOnlyMemorySlice<byte>? MapSpan;
        public ReadOnlyMemorySlice<byte>? GrassesSpan;
        public ReadOnlyMemorySlice<byte>? SoundsSpan;
        public ReadOnlyMemorySlice<byte>? LandSpan;
    }

    public IRegionObjectsGetter? Objects { get { return Payload.ObjectsSpan.HasValue ? RegionObjectsBinaryOverlay.RegionObjectsFactory(new OverlayStream(Payload.ObjectsSpan.Value, _package), _package) : default; } }

    public IRegionWeatherGetter? Weather { get { return Payload.WeatherSpan.HasValue ? RegionWeatherBinaryOverlay.RegionWeatherFactory(new OverlayStream(Payload.WeatherSpan.Value, _package), _package) : default; } }

    public IRegionMapGetter? Map { get { return Payload.MapSpan.HasValue ? RegionMapBinaryOverlay.RegionMapFactory(new OverlayStream(Payload.MapSpan.Value, _package), _package) : default; } }

    public IRegionGrassesGetter? Grasses { get { return Payload.GrassesSpan.HasValue ? RegionGrassesBinaryOverlay.RegionGrassesFactory(new OverlayStream(Payload.GrassesSpan.Value, _package), _package) : default; } }

    public IRegionSoundsGetter? Sounds { get { return Payload.SoundsSpan.HasValue ? RegionSoundsBinaryOverlay.RegionSoundsFactory(new OverlayStream(Payload.SoundsSpan.Value, _package), _package) : default; } }

    public IRegionLandGetter? Land { get { return Payload.LandSpan.HasValue ? RegionLandBinaryOverlay.RegionLandFactory(new OverlayStream(Payload.LandSpan.Value, _package), _package) : default; } }

    public partial ParseResult RegionAreaLogicCustomParse(
        OverlayStream stream,
        int offset,
        PreviousParse lastParsed)
    {
        var rdat = stream.GetSubrecordHeader();
        while (rdat.RecordType.Equals(RecordTypes.RDAT))
        {
            ParseRegionData(stream, offset);
            if (stream.Complete) break;
            rdat = stream.GetSubrecordHeader();
        }

        return null;
    }

    private void ParseRegionData(OverlayStream stream, int offset)
    {
        int loc = stream.Position - offset;
        var rdatFrame = stream.ReadSubrecord();
        RegionData.RegionDataType dataType = (RegionData.RegionDataType)BinaryPrimitives.ReadUInt32LittleEndian(rdatFrame.Content);

        switch (dataType)
        {
            case RegionData.RegionDataType.Object:
                _payload.Fields.ObjectsSpan = _recordData.Slice(loc);
                break;
            case RegionData.RegionDataType.Map:
                _payload.Fields.MapSpan = _recordData.Slice(loc);
                break;
            case RegionData.RegionDataType.Grass:
                _payload.Fields.GrassesSpan = _recordData.Slice(loc);
                break;
            case RegionData.RegionDataType.Land:
                _payload.Fields.LandSpan = _recordData.Slice(loc);
                break;
            case RegionData.RegionDataType.Sound:
                _payload.Fields.SoundsSpan = _recordData.Slice(loc);
                break;
            case RegionData.RegionDataType.Weather:
                _payload.Fields.WeatherSpan = _recordData.Slice(loc);
                break;
            default:
                throw new NotImplementedException();
        }

        var types = RegionBinaryCreateTranslation.GetTypes(dataType);
        while (stream.TryGetSubrecord(types.AllRecordTypes, out var rec)
               && rec.RecordType != RecordTypes.RDAT)
        {
            stream.Position += rec.TotalLength;
        }
    }
}