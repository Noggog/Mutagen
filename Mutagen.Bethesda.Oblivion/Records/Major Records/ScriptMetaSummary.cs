using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Oblivion.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class ScriptMetaSummary
    {
        internal int CompiledSizeInternal
        {
            set => this.CompiledSize = value;
        }
    }

    namespace Internals
    {
        public partial class ScriptMetaSummaryBinaryCreateTranslation
        {
            static partial void FillBinary_CompiledSize_Custom(MutagenFrame frame, ScriptMetaSummary item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                frame.Position += 4;
            }
        }

        public partial class ScriptMetaSummaryBinaryWriteTranslation
        {
            static partial void WriteBinary_CompiledSize_Custom(MutagenWriter writer, IScriptMetaSummaryGetter item, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                Int32BinaryTranslation.Instance.Write(
                    writer,
                    item.CompiledSize);
            }
        }
    }
}
