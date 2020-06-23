using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Microsoft.VisualBasic.CompilerServices;

namespace Mutagen.Bethesda.Generation
{
    public class BooleanBinaryTranslationGeneration : PrimitiveBinaryTranslationGeneration<bool>
    {
        public BooleanBinaryTranslationGeneration() 
            : base(expectedLen: 1)
        {
        }

        public override string GenerateForTypicalWrapper(
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor dataAccessor,
            Accessor packageAccessor)
        {
            BoolType b = typeGen as BoolType;
            if (b.BoolAsMarker == null)
            {
                return $"{dataAccessor}[0] == 1";
            }
            else
            {
                return "true";
            }
        }

        public override async Task GenerateCopyIn(
            FileGeneration fg,
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor frameAccessor,
            Accessor itemAccessor, 
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor)
        {
            BoolType b = typeGen as BoolType;
            if (b.BoolAsMarker == null)
            {
                await base.GenerateCopyIn(fg, objGen, typeGen, frameAccessor, itemAccessor, errorMaskAccessor, translationMaskAccessor);
            }
            else
            {
                fg.AppendLine($"{itemAccessor} = true;");
            }
        }

        public override void GenerateWrite(
            FileGeneration fg, 
            ObjectGeneration objGen, 
            TypeGeneration typeGen, 
            Accessor writerAccessor,
            Accessor itemAccessor, 
            Accessor errorMaskAccessor,
            Accessor translationMaskAccessor, 
            Accessor converterAccessor)
        {
            BoolType b = typeGen as BoolType;
            if (b.BoolAsMarker == null)
            {
                base.GenerateWrite(
                    fg,
                    objGen,
                    typeGen,
                    writerAccessor,
                    itemAccessor,
                    errorMaskAccessor,
                    translationMaskAccessor,
                    converterAccessor);
            }
            else
            {
                var data = typeGen.GetFieldData();
                using (var args = new ArgsWrapper(fg,
                    $"{this.Namespace}BooleanBinaryTranslation.Instance.WriteAsMarker"))
                {
                    args.Add($"writer: {writerAccessor}");
                    args.Add($"item: {ItemWriteAccess(typeGen, itemAccessor)}");
                    if (data.RecordType.HasValue
                        && data.HandleTrigger)
                    {
                        args.Add($"header: recordTypeConverter.ConvertToCustom({objGen.RecordTypeHeaderName(data.RecordType.Value)})");
                    }
                }
            }
        }
    }
}
