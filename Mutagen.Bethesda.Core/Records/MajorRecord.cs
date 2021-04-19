using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mutagen.Bethesda
{
    public partial interface IMajorRecord : IMajorRecordCommon
    {
        new FormKey FormKey { get; }
        
    }

    public partial interface IMajorRecordGetter : IMajorRecordCommonGetter
    {
    }

    [DebuggerDisplay("{GetType().Name} {this.EditorID?.ToString()} {this.FormKey.ToString()}")]
    public partial class MajorRecord : IFormLinkContainer
    {
        #region EditorID
        public virtual String? EditorID { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String? IMajorRecordGetter.EditorID => this.EditorID;
        #endregion

        /// <summary>
        /// A convenience property to print "EditorID - FormKey"
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string TitleString => $"{this.EditorID} - {this.FormKey}";

        public bool IsCompressed
        {
            get => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Constants.Constants.CompressedFlag);
            set => this.MajorRecordFlagsRaw = EnumExt.SetFlag(this.MajorRecordFlagsRaw, Constants.Constants.CompressedFlag, value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IMajorRecordCommonGetter.IsCompressed => this.IsCompressed;

        public bool IsDeleted
        {
            get => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Constants.Constants.DeletedFlag);
            set => this.MajorRecordFlagsRaw = EnumExt.SetFlag(this.MajorRecordFlagsRaw, Constants.Constants.DeletedFlag, value);
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        bool IMajorRecordCommonGetter.IsDeleted => this.IsDeleted;

        protected abstract ushort? FormVersionAbstract { get; }
        ushort? IMajorRecordCommonGetter.FormVersion => FormVersionAbstract;
        ushort? IFormVersionGetter.FormVersion => FormVersionAbstract;

        public virtual bool Disable()
        {
            if (this.IsDeleted) return false;
            MajorRecordFlagsRaw = EnumExt.SetFlag(MajorRecordFlagsRaw, (int)Constants.Constants.InitiallyDisabled, true);
            return true;
        }

        #region Comparers
        public static IEqualityComparer<IMajorRecordCommonGetter> FormKeyEqualityComparer => _formKeyEqualityComparer;

        static readonly MajorRecordFormKeyComparer _formKeyEqualityComparer = new MajorRecordFormKeyComparer();

        class MajorRecordFormKeyComparer : IEqualityComparer<IMajorRecordCommonGetter>
        {
            public bool Equals(IMajorRecordCommonGetter? x, IMajorRecordCommonGetter? y)
            {
                return x?.FormKey == y?.FormKey;
            }

            public int GetHashCode(IMajorRecordCommonGetter obj)
            {
                return obj.FormKey.GetHashCode();
            }
        }
        #endregion

        public bool Equals(IFormLinkGetter? other)
        {
            if (other == null) return false;
            return other.Equals(this);
        }
    }
}

namespace Mutagen.Bethesda.Internals
{
    [DebuggerDisplay("{GetType().Name} {this.EditorID?.ToString()} {this.FormKey.ToString()}")]
    public abstract partial class MajorRecordBinaryOverlay : IMajorRecordCommonGetter
    {
        public bool IsCompressed => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Constants.Constants.CompressedFlag);
        public bool IsDeleted => EnumExt.HasFlag(this.MajorRecordFlagsRaw, Constants.Constants.DeletedFlag);

        protected abstract ushort? FormVersionAbstract { get; }
        ushort? IMajorRecordCommonGetter.FormVersion => FormVersionAbstract;
        ushort? IFormVersionGetter.FormVersion => FormVersionAbstract;

        public bool Equals(IFormLinkGetter? other)
        {
            if (other == null) return false;
            return other.Equals(this);
        }
    }
}
