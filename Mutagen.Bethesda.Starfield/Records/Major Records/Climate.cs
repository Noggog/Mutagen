using Mutagen.Bethesda.Plugins.Binary.Streams;

namespace Mutagen.Bethesda.Starfield;

partial class ClimateBinaryCreateTranslation
{
    private static bool GetTime(byte b, out TimeOnly date)
    {
        if (b > 144)
        {
            throw new ArgumentException("Cannot have a time value greater than 144");
            date = default(TimeOnly);
            return false;
        }
        date = new TimeOnly().AddMinutes(b * 10);
        return true;
    }

    public static TimeOnly GetTime(byte b)
    {
        if (b > 144)
        {
            throw new ArgumentException("Cannot have a time value greater than 144");
        }
        return new TimeOnly().AddMinutes(b * 10);
    }

    public static partial void FillBinarySunriseBeginCustom(MutagenFrame frame, IClimateInternal item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunriseBegin = date;
        }
    }

    public static partial void FillBinarySunriseEndCustom(MutagenFrame frame, IClimateInternal item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunriseEnd = date;
        }
    }

    public static partial void FillBinarySunsetBeginCustom(MutagenFrame frame, IClimateInternal item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunsetBegin = date;
        }
    }

    public static partial void FillBinarySunsetEndCustom(MutagenFrame frame, IClimateInternal item)
    {
        if (GetTime(frame.Reader.ReadUInt8(), out var date))
        {
            item.SunsetEnd = date;
        }
    }
}

partial class ClimateBinaryWriteTranslation
{
    private static byte GetByte(TimeOnly d)
    {
        var mins = d.Hour * 60 + d.Minute;
        return (byte)(mins / 10);
    }

    public static partial void WriteBinarySunriseBeginCustom(MutagenWriter writer, IClimateGetter item)
    {
        writer.Write(GetByte(item.SunriseBegin));
    }

    public static partial void WriteBinarySunriseEndCustom(MutagenWriter writer, IClimateGetter item)
    {
        writer.Write(GetByte(item.SunriseEnd));
    }

    public static partial void WriteBinarySunsetBeginCustom(MutagenWriter writer, IClimateGetter item)
    {
        writer.Write(GetByte(item.SunsetBegin));
    }

    public static partial void WriteBinarySunsetEndCustom(MutagenWriter writer, IClimateGetter item)
    {
        writer.Write(GetByte(item.SunsetEnd));
    }
}

partial class ClimateBinaryOverlay
{
    public partial TimeOnly GetSunriseBeginCustom() => Payload.TNAMLocation.HasValue ? ClimateBinaryCreateTranslation.GetTime(_recordData.Span[Payload.TNAMLocation.Value.Min + 0]) : default;
    public partial TimeOnly GetSunriseEndCustom() => Payload.TNAMLocation.HasValue ? ClimateBinaryCreateTranslation.GetTime(_recordData.Span[Payload.TNAMLocation!.Value.Min + 1]) : default;
    public partial TimeOnly GetSunsetBeginCustom() => Payload.TNAMLocation.HasValue ? ClimateBinaryCreateTranslation.GetTime(_recordData.Span[Payload.TNAMLocation!.Value.Min + 2]) : default;
    public partial TimeOnly GetSunsetEndCustom() => Payload.TNAMLocation.HasValue ? ClimateBinaryCreateTranslation.GetTime(_recordData.Span[Payload.TNAMLocation!.Value.Min + 3]) : default;

    public byte PhaseLength
    {
        get
        {
            if (!Payload.TNAMLocation.HasValue) return default;
            return (byte)(_recordData[Payload.TNAMLocation.Value.Min + 5] % 64);
        }
    }
}