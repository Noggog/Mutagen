﻿using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;

namespace Mutagen.Bethesda.Binary
{
    public class FormKeyBinaryTranslation
    {
        public readonly static FormKeyBinaryTranslation Instance = new FormKeyBinaryTranslation();

        public FormKey Parse(
            ReadOnlySpan<byte> span,
            MasterReferenceReader masterReferences)
        {
            var id = BinaryPrimitives.ReadUInt32LittleEndian(span);
            var modID = span[3];
            if (modID < masterReferences.Masters.Count)
            {
                return new FormKey(
                    masterReferences.Masters[modID].Master,
                    id);
            }
            return new FormKey(
                masterReferences.CurrentMod,
                id);
        }

        public bool Parse(
            MutagenFrame frame,
            out FormKey item,
            MasterReferenceReader masterReferences)
        {
            item = Parse(
                frame.ReadSpan(4),
                masterReferences);
            return true;
        }

        public FormKey Parse(
            MutagenFrame frame,
            MasterReferenceReader masterReferences)
        {
            return Parse(
                frame.ReadSpan(4),
                masterReferences);
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            MasterReferenceReader masterReferences,
            bool nullable = false)
        {
            UInt32BinaryTranslation.Instance.Write(
                writer: writer,
                item: masterReferences.GetFormID(item).Raw);
        }

        public void Write(
            MutagenWriter writer,
            FormKey item,
            MasterReferenceReader masterReferences,
            RecordType header,
            bool nullable = false)
        {
            using (HeaderExport.ExportHeader(writer, header, ObjectType.Subrecord))
            {
                this.Write(
                    writer,
                    item,
                    masterReferences);
            }
        }
    }
}