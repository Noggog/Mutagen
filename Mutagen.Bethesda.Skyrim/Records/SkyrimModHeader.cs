using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Core;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System;
using System.Diagnostics;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SkyrimModHeader
    {
        [Flags]
        public enum HeaderFlag
        {
            Master = 0x0000_0001,
            Localized = 0x0000_0080,
            LightMaster = 0x0000_0200,
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        int IModHeaderCommon.RawFlags
        {
            get => (int)this.Flags;
            set => this.Flags = (HeaderFlag)value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        uint IModHeaderCommon.NumRecords
        {
            get => this.Stats.NumRecords;
            set => this.Stats.NumRecords = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        uint IModHeaderCommon.NextFormID
        {
            get => this.Stats.NextFormID;
            set => this.Stats.NextFormID = value;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        uint IModHeaderCommon.MinimumCustomFormID => SkyrimMod.DefaultInitialNextFormID;

        IExtendedList<MasterReference> IModHeaderCommon.MasterReferences => this.MasterReferences;
    }

    public partial interface ISkyrimModHeader : IModHeaderCommon
    {
    }

    namespace Internals
    {
        public partial class SkyrimModHeaderBinaryCreateTranslation
        {
            static partial void FillBinaryMasterReferencesCustom(MutagenFrame frame, ISkyrimModHeader item)
            {
                item.MasterReferences.SetTo(
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<MasterReference>.Instance.Parse(
                        frame: frame.SpawnAll(),
                        triggeringRecord: RecordTypes.MAST,
                        transl: MasterReference.TryCreateFromBinary));
                frame.MetaData.MasterReferences.SetTo(item.MasterReferences);
            }
        }

        public partial class SkyrimModHeaderBinaryWriteTranslation
        {
            static partial void WriteBinaryMasterReferencesCustom(MutagenWriter writer, ISkyrimModHeaderGetter item)
            {
                Mutagen.Bethesda.Binary.ListBinaryTranslation<IMasterReferenceGetter>.Instance.Write(
                    writer: writer,
                    items: item.MasterReferences,
                    transl: (MutagenWriter subWriter, IMasterReferenceGetter subItem, RecordTypeConverter? conv) =>
                    {
                        var Item = subItem;
                        ((MasterReferenceBinaryWriteTranslation)((IBinaryItem)Item).BinaryWriteTranslator).Write(
                            item: Item,
                            writer: subWriter,
                            recordTypeConverter: conv);
                    });
            }
        }
    }
}
