using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class AIPackage
    {
        [Flags]
        public enum Flag
        {
            OffersServices = 0x0000001,
            MustReachLocation = 0x0000002,
            MustComplete = 0x0000004,
            LockDoorsAtPackageStart = 0x0000008,
            LockDoorsAtPackageEnd = 0x0000010,
            LockDoorsAtLocation = 0x0000020,
            UnlockDoorsAtPackageStart = 0x0000040,
            UnlockDoorsAtPackageEnd = 0x0000080,
            UnlockDoorsAtLocation = 0x0000100,
            ContinueIfPCNear = 0x0000200,
            OncePerDay = 0x0000400,
            Unused = 0x0000800,
            SkipFalloutBehavior = 0x0001000,
            AlwaysRun = 0x0002000,
            AlwaysSneak = 0x0020000,
            AllowSwimming = 0x0040000,
            AllowFalls = 0x0080000,
            ArmorUnequpped = 0x0100000,
            WeaponsUnequipped = 0x0200000,
            DefensiveCombat = 0x0400000,
            UseHorse = 0x0800000,
            NoIdleAnims = 0x1000000
        }

        public enum Types
        {
            Find = 0,
            Follow = 1,
            Escort = 2,
            Eat = 3,
            Sleep = 4,
            Wander = 5,
            Travel = 6,
            Accompany = 7,
            UseItemAt = 8,
            Ambush = 9,
            FleeNotCombat = 10,
            CastMagic = 11,
        }
    }

    namespace Internals
    {
        public partial class AIPackageDataBinaryCreateTranslation
        {
            static partial void FillBinaryFlagsCustom(MutagenFrame frame, IAIPackageData item)
            {
                if (frame.Remaining == 8)
                {
                    var span = frame.Reader.ReadSpan(8);
                    item.Flags = EnumExt<AIPackage.Flag>.Convert(BinaryPrimitives.ReadUInt32LittleEndian(span));
                    item.Type = EnumExt<AIPackage.Types>.Convert(BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(4)));
                }
                else if (frame.Remaining == 4)
                {
                    var span = frame.Reader.ReadSpan(4);
                    item.Flags = EnumExt<AIPackage.Flag>.Convert(BinaryPrimitives.ReadUInt16LittleEndian(span));
                    item.Type = EnumExt<AIPackage.Types>.Convert(span[2]);
                }
                else
                {
                    throw new ArgumentException($"Odd length for general AI field: {frame.Remaining}");
                }
            }
        }

        public partial class AIPackageDataBinaryWriteTranslation
        {
            static partial void WriteBinaryFlagsCustom(MutagenWriter writer, IAIPackageDataGetter item)
            {
                Mutagen.Bethesda.Binary.EnumBinaryTranslation<AIPackage.Flag>.Instance.Write(
                    writer,
                    item.Flags,
                    length: 4);
                Mutagen.Bethesda.Binary.EnumBinaryTranslation<AIPackage.Types>.Instance.Write(
                    writer,
                    item.Type,
                    length: 4);
            }
        }

        public partial class AIPackageDataBinaryOverlay
        {
            public AIPackage.Flag GetFlagsCustom(int location)
            {
                if (_data.Length > 4)
                {
                    return EnumExt<AIPackage.Flag>.Convert(BinaryPrimitives.ReadUInt32LittleEndian(_data));
                }
                else
                {
                    return EnumExt<AIPackage.Flag>.Convert(BinaryPrimitives.ReadUInt16LittleEndian(_data));
                }
            }

            public AIPackage.Types GetTypeCustom(int location)
            {
                if (_data.Length > 4)
                {
                    return EnumExt<AIPackage.Types>.Convert(BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(4)));
                }
                else
                {
                    return EnumExt<AIPackage.Types>.Convert(_data[2]);
                }
            }
        }
    }
}
