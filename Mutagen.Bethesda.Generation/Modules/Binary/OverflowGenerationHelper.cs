using Loqui.Generation;
using Mutagen.Bethesda.Generation.Modules.Plugin;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog.StructuredStrings;
using Noggog.StructuredStrings.CSharp;

namespace Mutagen.Bethesda.Generation.Modules.Binary;

public class OverflowGenerationHelper
{
    public static void GenerateWrapperOverflowParse(StructuredStringBuilder sb, TypeGeneration typeGen,
        MutagenFieldData data, bool isMajorRecord = false)
    {
        if (data.OverflowRecordType.HasValue
            && data.BinaryOverlayFallback != BinaryGenerationType.Custom)
        {
            var prefix = isMajorRecord ? "_payload.Fields." : "_";
            sb.AppendLine($"{prefix}{typeGen.Name}LengthOverride = lastParsed.{nameof(PreviousParse.LengthOverride)};");
            sb.AppendLine($"if (lastParsed.{nameof(PreviousParse.LengthOverride)}.HasValue)");
            using (sb.CurlyBrace())
            {
                sb.AppendLine($"stream.Position += lastParsed.{nameof(PreviousParse.LengthOverride)}.Value;");
            }
        }
    }

    public static void GenerateWrapperOverflowMember(StructuredStringBuilder sb, TypeGeneration typeGen,
        StructuredStringBuilder? payloadSb = null)
    {
        if (payloadSb != null)
        {
            payloadSb.AppendLine($"public int? {typeGen.Name}LengthOverride;");
        }
        else
        {
            sb.AppendLine($"private int? _{typeGen.Name}LengthOverride;");
        }
    }
}