﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Notifying;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class DialogItem
    {
        public enum Flag
        {
            Goodbye = 0x001,
            Random = 0x002,
            SayOnce = 0x004,
            RunImmediately = 0x008,
            InfoRefusal = 0x010,
            RandomEnd = 0x020,
            RunForRumors = 0x040
        }

        partial void CustomCtor()
        {
            this.CompiledScript_Property.Subscribe(
                (change) =>
                {
                    this.MetadataSummary.CompiledSizeInternal = change.New.Length;
                },
                cmds: NotifyingSubscribeParameters.NoFire);
        }

        private readonly static RecordTypeConverter conditionConverter = new RecordTypeConverter(
            new KeyValuePair<RecordType, RecordType>(
                DialogItem_Registration.CTDA_HEADER,
                new RecordType("CTDT")));

        private readonly static RecordTypeConverter metaConverter = new RecordTypeConverter(
            new KeyValuePair<RecordType, RecordType>(
                new RecordType("SCHR"),
                new RecordType("SCHD")));

        static partial void FillBinary_Conditions_Custom(MutagenFrame frame, DialogItem item, int fieldIndex, Func<DialogItem_ErrorMask> errorMask)
        {
            item.Conditions.SetIfSucceededOrDefault(Mutagen.Bethesda.Binary.ListBinaryTranslation<DialogCondition, MaskItem<Exception, DialogCondition_ErrorMask>>.Instance.ParseRepeatedItem(
               frame: new MutagenFrame(frame.Reader),
               triggeringRecord: DialogItem_Registration.CTDA_HEADER,
               fieldIndex: (int)DialogItem_FieldIndex.Conditions,
               lengthLength: Mutagen.Bethesda.Constants.SUBRECORD_LENGTHLENGTH,
               errorMask: errorMask,
               transl: (MutagenFrame r, bool listDoMasks, out MaskItem<Exception, DialogCondition_ErrorMask> listSubMask) =>
               {
                   return LoquiBinaryTranslation<DialogCondition, DialogCondition_ErrorMask>.Instance.Parse(
                       frame: r.Spawn(snapToFinalPosition: false),
                       doMasks: listDoMasks,
                       errorMask: out listSubMask);
               }));
        }

        static partial void WriteBinary_Conditions_Custom(MutagenWriter writer, DialogItem item, int fieldIndex, Func<DialogItem_ErrorMask> errorMask)
        {
            Mutagen.Bethesda.Binary.ListBinaryTranslation<DialogCondition, MaskItem<Exception, DialogCondition_ErrorMask>>.Instance.Write(
                writer: writer,
                item: item.Conditions,
                fieldIndex: (int)DialogItem_FieldIndex.Conditions,
                errorMask: errorMask,
                transl: (MutagenWriter subWriter, DialogCondition subItem, bool listDoMasks, out MaskItem<Exception, DialogCondition_ErrorMask> listSubMask) =>
                {
                    LoquiBinaryTranslation<DialogCondition, DialogCondition_ErrorMask>.Instance.Write(
                        writer: subWriter,
                        item: subItem,
                        doMasks: listDoMasks,
                        errorMask: out listSubMask);
                }
                );
        }

        static partial void FillBinary_ConditionsOld_Custom(MutagenFrame frame, DialogItem item, Func<DialogItem_ErrorMask> errorMask)
        {
            item.Conditions.SetIfSucceededOrDefault(Mutagen.Bethesda.Binary.ListBinaryTranslation<DialogCondition, MaskItem<Exception, DialogCondition_ErrorMask>>.Instance.ParseRepeatedItem(
               frame: frame,
               triggeringRecord: new RecordType("CTDT"),
               fieldIndex: (int)DialogItem_FieldIndex.Conditions,
               lengthLength: Mutagen.Bethesda.Constants.SUBRECORD_LENGTHLENGTH,
               errorMask: errorMask,
               transl: (MutagenFrame r, bool listDoMasks, out MaskItem<Exception, DialogCondition_ErrorMask> listSubMask) =>
               {
                   byte[] bytes = r.ReadBytes((int)r.Remaining);
                   bytes[4] = 0x18;
                   byte[] newBytes = new byte[bytes.Length + 4];
                   Array.Copy(bytes, newBytes, bytes.Length);
                   return LoquiBinaryTranslation<DialogCondition, DialogCondition_ErrorMask>.Instance.Parse(
                       frame: new MutagenFrame(new BinaryMemoryReadStream(newBytes)),
                       doMasks: listDoMasks,
                       errorMask: out listSubMask,
                       recordTypeConverter: conditionConverter);
               }));
        }

        static partial void WriteBinary_ConditionsOld_Custom(MutagenWriter writer, DialogItem item, Func<DialogItem_ErrorMask> errorMask)
        {
        }

        static partial void FillBinary_MetadataSummaryOld_Custom(MutagenFrame frame, DialogItem item, Func<DialogItem_ErrorMask> errorMask)
        {
            var tmpMetadataSummary = ScriptMetaSummary.Create_Binary(
                frame: frame,
                doMasks: errorMask != null,
                recordTypeConverter: metaConverter);
            item.MetadataSummary.CopyFieldsFrom(
                rhs: tmpMetadataSummary.Object,
                def: null,
                cmds: null,
                copyMask: null,
                doMasks: errorMask != null,
                errorMask: out ScriptMetaSummary_ErrorMask MetadataSummaryerrorMask);
            var combinedMetadataSummary = ScriptMetaSummary_ErrorMask.Combine(tmpMetadataSummary.ErrorMask, MetadataSummaryerrorMask);
            ErrorMask.HandleErrorMask(
                creator: errorMask,
                index: (int)DialogItem_FieldIndex.MetadataSummary,
                errMaskObj: combinedMetadataSummary == null ? null : new MaskItem<Exception, ScriptMetaSummary_ErrorMask>(null, combinedMetadataSummary));
        }

        static partial void WriteBinary_MetadataSummaryOld_Custom(MutagenWriter writer, DialogItem item, Func<DialogItem_ErrorMask> errorMask)
        {
        }
    }
}