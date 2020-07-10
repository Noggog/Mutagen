/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Autogenerated by Loqui.  Do not manually change.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*/
#region Usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Loqui;
using Loqui.Internal;
using Noggog;
using Mutagen.Bethesda.Skyrim.Internals;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Binary;
using System.Buffers.Binary;
using Mutagen.Bethesda.Internals;
#endregion

#nullable enable
namespace Mutagen.Bethesda.Skyrim
{
    #region Class
    public abstract partial class ATopicReference :
        IATopicReference,
        ILoquiObjectSetter<ATopicReference>,
        IEquatable<ATopicReference>,
        IEqualsMask
    {
        #region Ctor
        public ATopicReference()
        {
            CustomCtor();
        }
        partial void CustomCtor();
        #endregion


        #region To String

        public virtual void ToString(
            FileGeneration fg,
            string? name = null)
        {
            ATopicReferenceMixIn.ToString(
                item: this,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (!(obj is IATopicReferenceGetter rhs)) return false;
            return ((ATopicReferenceCommon)((IATopicReferenceGetter)this).CommonInstance()!).Equals(this, rhs);
        }

        public bool Equals(ATopicReference? obj)
        {
            return ((ATopicReferenceCommon)((IATopicReferenceGetter)this).CommonInstance()!).Equals(this, obj);
        }

        public override int GetHashCode() => ((ATopicReferenceCommon)((IATopicReferenceGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

        #region Mask
        public class Mask<TItem> :
            IMask<TItem>,
            IEquatable<Mask<TItem>>
        {
            #region Ctors
            public Mask(TItem initialValue)
            {
            }


            #pragma warning disable CS8618
            protected Mask()
            {
            }
            #pragma warning restore CS8618

            #endregion

            #region Equals
            public override bool Equals(object? obj)
            {
                if (!(obj is Mask<TItem> rhs)) return false;
                return Equals(rhs);
            }

            public bool Equals(Mask<TItem>? rhs)
            {
                if (rhs == null) return false;
                return true;
            }
            public override int GetHashCode()
            {
                var hash = new HashCode();
                return hash.ToHashCode();
            }

            #endregion

            #region All
            public virtual bool All(Func<TItem, bool> eval)
            {
                return true;
            }
            #endregion

            #region Any
            public virtual bool Any(Func<TItem, bool> eval)
            {
                return false;
            }
            #endregion

            #region Translate
            public Mask<R> Translate<R>(Func<TItem, R> eval)
            {
                var ret = new ATopicReference.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
            }
            #endregion

            #region To String
            public override string ToString()
            {
                return ToString(printMask: null);
            }

            public string ToString(ATopicReference.Mask<bool>? printMask = null)
            {
                var fg = new FileGeneration();
                ToString(fg, printMask);
                return fg.ToString();
            }

            public void ToString(FileGeneration fg, ATopicReference.Mask<bool>? printMask = null)
            {
                fg.AppendLine($"{nameof(ATopicReference.Mask<TItem>)} =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                }
                fg.AppendLine("]");
            }
            #endregion

        }

        public class ErrorMask :
            IErrorMask,
            IErrorMask<ErrorMask>
        {
            #region Members
            public Exception? Overall { get; set; }
            private List<string>? _warnings;
            public List<string> Warnings
            {
                get
                {
                    if (_warnings == null)
                    {
                        _warnings = new List<string>();
                    }
                    return _warnings;
                }
            }
            #endregion

            #region IErrorMask
            public virtual object? GetNthMask(int index)
            {
                ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
                switch (enu)
                {
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public virtual void SetNthException(int index, Exception ex)
            {
                ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
                switch (enu)
                {
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public virtual void SetNthMask(int index, object obj)
            {
                ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
                switch (enu)
                {
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public virtual bool IsInError()
            {
                if (Overall != null) return true;
                return false;
            }
            #endregion

            #region To String
            public override string ToString()
            {
                var fg = new FileGeneration();
                ToString(fg, null);
                return fg.ToString();
            }

            public virtual void ToString(FileGeneration fg, string? name = null)
            {
                fg.AppendLine($"{(name ?? "ErrorMask")} =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                    if (this.Overall != null)
                    {
                        fg.AppendLine("Overall =>");
                        fg.AppendLine("[");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine($"{this.Overall}");
                        }
                        fg.AppendLine("]");
                    }
                    ToString_FillInternal(fg);
                }
                fg.AppendLine("]");
            }
            protected virtual void ToString_FillInternal(FileGeneration fg)
            {
            }
            #endregion

            #region Combine
            public ErrorMask Combine(ErrorMask? rhs)
            {
                if (rhs == null) return this;
                var ret = new ErrorMask();
                return ret;
            }
            public static ErrorMask? Combine(ErrorMask? lhs, ErrorMask? rhs)
            {
                if (lhs != null && rhs != null) return lhs.Combine(rhs);
                return lhs ?? rhs;
            }
            #endregion

            #region Factory
            public static ErrorMask Factory(ErrorMaskBuilder errorMask)
            {
                return new ErrorMask();
            }
            #endregion

        }
        public class TranslationMask : ITranslationMask
        {
            #region Members
            private TranslationCrystal? _crystal;
            #endregion

            #region Ctors
            public TranslationMask(bool defaultOn)
            {
            }

            #endregion

            public TranslationCrystal GetCrystal()
            {
                if (_crystal != null) return _crystal;
                var ret = new List<(bool On, TranslationCrystal? SubCrystal)>();
                GetCrystal(ret);
                _crystal = new TranslationCrystal(ret.ToArray());
                return _crystal;
            }

            protected virtual void GetCrystal(List<(bool On, TranslationCrystal? SubCrystal)> ret)
            {
            }
        }
        #endregion

        #region Mutagen
        public static readonly RecordType GrupRecordType = ATopicReference_Registration.TriggeringRecordType;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual IEnumerable<FormKey> LinkFormKeys => ATopicReferenceCommon.Instance.GetLinkFormKeys(this);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEnumerable<FormKey> ILinkedFormKeyContainer.LinkFormKeys => ATopicReferenceCommon.Instance.GetLinkFormKeys(this);
        protected virtual void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => ATopicReferenceCommon.Instance.RemapLinks(this, mapping);
        void ILinkedFormKeyContainer.RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => ATopicReferenceCommon.Instance.RemapLinks(this, mapping);
        #endregion

        #region Binary Translation
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual object BinaryWriteTranslator => ATopicReferenceBinaryWriteTranslation.Instance;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IBinaryItem.BinaryWriteTranslator => this.BinaryWriteTranslator;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            RecordTypeConverter? recordTypeConverter = null)
        {
            ((ATopicReferenceBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                recordTypeConverter: recordTypeConverter);
        }
        #endregion

        void IPrintable.ToString(FileGeneration fg, string? name) => this.ToString(fg, name);
        IMask<bool> ILoquiObjectGetter.GetHasBeenSetIMask() => this.GetHasBeenSetMask();
        IMask<bool> IEqualsMask.GetEqualsIMask(object rhs, EqualsMaskHelper.Include include) => this.GetEqualsMask((IATopicReferenceGetter)rhs, include);

        void IClearable.Clear()
        {
            ((ATopicReferenceSetterCommon)((IATopicReferenceGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static ATopicReference GetNew()
        {
            throw new ArgumentException("New called on an abstract class.");
        }

    }
    #endregion

    #region Interface
    public partial interface IATopicReference :
        IATopicReferenceGetter,
        ILoquiObjectSetter<IATopicReference>
    {
    }

    public partial interface IATopicReferenceGetter :
        ILoquiObject,
        ILoquiObject<IATopicReferenceGetter>,
        ILinkedFormKeyContainer,
        IBinaryItem
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object? CommonSetterInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonSetterTranslationInstance();
        static ILoquiRegistration Registration => ATopicReference_Registration.Instance;

    }

    #endregion

    #region Common MixIn
    public static partial class ATopicReferenceMixIn
    {
        public static void Clear(this IATopicReference item)
        {
            ((ATopicReferenceSetterCommon)((IATopicReferenceGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static ATopicReference.Mask<bool> GetEqualsMask(
            this IATopicReferenceGetter item,
            IATopicReferenceGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string ToString(
            this IATopicReferenceGetter item,
            string? name = null,
            ATopicReference.Mask<bool>? printMask = null)
        {
            return ((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).ToString(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void ToString(
            this IATopicReferenceGetter item,
            FileGeneration fg,
            string? name = null,
            ATopicReference.Mask<bool>? printMask = null)
        {
            ((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).ToString(
                item: item,
                fg: fg,
                name: name,
                printMask: printMask);
        }

        public static bool HasBeenSet(
            this IATopicReferenceGetter item,
            ATopicReference.Mask<bool?> checkMask)
        {
            return ((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).HasBeenSet(
                item: item,
                checkMask: checkMask);
        }

        public static ATopicReference.Mask<bool> GetHasBeenSetMask(this IATopicReferenceGetter item)
        {
            var ret = new ATopicReference.Mask<bool>(false);
            ((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).FillHasBeenSetMask(
                item: item,
                mask: ret);
            return ret;
        }

        public static bool Equals(
            this IATopicReferenceGetter item,
            IATopicReferenceGetter rhs)
        {
            return ((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs);
        }

        public static void DeepCopyIn(
            this IATopicReference lhs,
            IATopicReferenceGetter rhs)
        {
            ((ATopicReferenceSetterTranslationCommon)((IATopicReferenceGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: default);
        }

        public static void DeepCopyIn(
            this IATopicReference lhs,
            IATopicReferenceGetter rhs,
            ATopicReference.TranslationMask? copyMask = null)
        {
            ((ATopicReferenceSetterTranslationCommon)((IATopicReferenceGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: copyMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IATopicReference lhs,
            IATopicReferenceGetter rhs,
            out ATopicReference.ErrorMask errorMask,
            ATopicReference.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((ATopicReferenceSetterTranslationCommon)((IATopicReferenceGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal());
            errorMask = ATopicReference.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IATopicReference lhs,
            IATopicReferenceGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((ATopicReferenceSetterTranslationCommon)((IATopicReferenceGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask);
        }

        public static ATopicReference DeepCopy(
            this IATopicReferenceGetter item,
            ATopicReference.TranslationMask? copyMask = null)
        {
            return ((ATopicReferenceSetterTranslationCommon)((IATopicReferenceGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static ATopicReference DeepCopy(
            this IATopicReferenceGetter item,
            out ATopicReference.ErrorMask errorMask,
            ATopicReference.TranslationMask? copyMask = null)
        {
            return ((ATopicReferenceSetterTranslationCommon)((IATopicReferenceGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static ATopicReference DeepCopy(
            this IATopicReferenceGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((ATopicReferenceSetterTranslationCommon)((IATopicReferenceGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

        #region Binary Translation
        [DebuggerStepThrough]
        public static void CopyInFromBinary(
            this IATopicReference item,
            MutagenFrame frame)
        {
            CopyInFromBinary(
                item: item,
                frame: frame,
                recordTypeConverter: null);
        }

        public static void CopyInFromBinary(
            this IATopicReference item,
            MutagenFrame frame,
            RecordTypeConverter? recordTypeConverter = null)
        {
            ((ATopicReferenceSetterCommon)((IATopicReferenceGetter)item).CommonSetterInstance()!).CopyInFromBinary(
                item: item,
                frame: frame,
                recordTypeConverter: recordTypeConverter);
        }

        #endregion

    }
    #endregion

}

namespace Mutagen.Bethesda.Skyrim.Internals
{
    #region Field Index
    public enum ATopicReference_FieldIndex
    {
    }
    #endregion

    #region Registration
    public partial class ATopicReference_Registration : ILoquiRegistration
    {
        public static readonly ATopicReference_Registration Instance = new ATopicReference_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Skyrim.ProtocolKey;

        public static readonly ObjectKey ObjectKey = new ObjectKey(
            protocolKey: ProtocolDefinition_Skyrim.ProtocolKey,
            msgID: 311,
            version: 0);

        public const string GUID = "5bb62f45-9a55-4c05-a849-d896ec097522";

        public const ushort AdditionalFieldCount = 0;

        public const ushort FieldCount = 0;

        public static readonly Type MaskType = typeof(ATopicReference.Mask<>);

        public static readonly Type ErrorMaskType = typeof(ATopicReference.ErrorMask);

        public static readonly Type ClassType = typeof(ATopicReference);

        public static readonly Type GetterType = typeof(IATopicReferenceGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IATopicReference);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Skyrim.ATopicReference";

        public const string Name = "ATopicReference";

        public const string Namespace = "Mutagen.Bethesda.Skyrim";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

        public static ushort? GetNameIndex(StringCaseAgnostic str)
        {
            switch (str.Upper)
            {
                default:
                    return null;
            }
        }

        public static bool GetNthIsEnumerable(ushort index)
        {
            ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
            switch (enu)
            {
                default:
                    throw new ArgumentException($"Index is out of range: {index}");
            }
        }

        public static bool GetNthIsLoqui(ushort index)
        {
            ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
            switch (enu)
            {
                default:
                    throw new ArgumentException($"Index is out of range: {index}");
            }
        }

        public static bool GetNthIsSingleton(ushort index)
        {
            ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
            switch (enu)
            {
                default:
                    throw new ArgumentException($"Index is out of range: {index}");
            }
        }

        public static string GetNthName(ushort index)
        {
            ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
            switch (enu)
            {
                default:
                    throw new ArgumentException($"Index is out of range: {index}");
            }
        }

        public static bool IsNthDerivative(ushort index)
        {
            ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
            switch (enu)
            {
                default:
                    throw new ArgumentException($"Index is out of range: {index}");
            }
        }

        public static bool IsProtected(ushort index)
        {
            ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
            switch (enu)
            {
                default:
                    throw new ArgumentException($"Index is out of range: {index}");
            }
        }

        public static Type GetNthType(ushort index)
        {
            ATopicReference_FieldIndex enu = (ATopicReference_FieldIndex)index;
            switch (enu)
            {
                default:
                    throw new ArgumentException($"Index is out of range: {index}");
            }
        }

        public static readonly RecordType TriggeringRecordType = RecordTypes.PDTO;
        public static readonly Type BinaryWriteTranslation = typeof(ATopicReferenceBinaryWriteTranslation);
        #region Interface
        ProtocolKey ILoquiRegistration.ProtocolKey => ProtocolKey;
        ObjectKey ILoquiRegistration.ObjectKey => ObjectKey;
        string ILoquiRegistration.GUID => GUID;
        ushort ILoquiRegistration.FieldCount => FieldCount;
        ushort ILoquiRegistration.AdditionalFieldCount => AdditionalFieldCount;
        Type ILoquiRegistration.MaskType => MaskType;
        Type ILoquiRegistration.ErrorMaskType => ErrorMaskType;
        Type ILoquiRegistration.ClassType => ClassType;
        Type ILoquiRegistration.SetterType => SetterType;
        Type? ILoquiRegistration.InternalSetterType => InternalSetterType;
        Type ILoquiRegistration.GetterType => GetterType;
        Type? ILoquiRegistration.InternalGetterType => InternalGetterType;
        string ILoquiRegistration.FullName => FullName;
        string ILoquiRegistration.Name => Name;
        string ILoquiRegistration.Namespace => Namespace;
        byte ILoquiRegistration.GenericCount => GenericCount;
        Type? ILoquiRegistration.GenericRegistrationType => GenericRegistrationType;
        ushort? ILoquiRegistration.GetNameIndex(StringCaseAgnostic name) => GetNameIndex(name);
        bool ILoquiRegistration.GetNthIsEnumerable(ushort index) => GetNthIsEnumerable(index);
        bool ILoquiRegistration.GetNthIsLoqui(ushort index) => GetNthIsLoqui(index);
        bool ILoquiRegistration.GetNthIsSingleton(ushort index) => GetNthIsSingleton(index);
        string ILoquiRegistration.GetNthName(ushort index) => GetNthName(index);
        bool ILoquiRegistration.IsNthDerivative(ushort index) => IsNthDerivative(index);
        bool ILoquiRegistration.IsProtected(ushort index) => IsProtected(index);
        Type ILoquiRegistration.GetNthType(ushort index) => GetNthType(index);
        #endregion

    }
    #endregion

    #region Common
    public partial class ATopicReferenceSetterCommon
    {
        public static readonly ATopicReferenceSetterCommon Instance = new ATopicReferenceSetterCommon();

        partial void ClearPartial();
        
        public virtual void Clear(IATopicReference item)
        {
            ClearPartial();
        }
        
        #region Binary Translation
        public virtual void CopyInFromBinary(
            IATopicReference item,
            MutagenFrame frame,
            RecordTypeConverter? recordTypeConverter = null)
        {
        }
        
        #endregion
        
    }
    public partial class ATopicReferenceCommon
    {
        public static readonly ATopicReferenceCommon Instance = new ATopicReferenceCommon();

        public ATopicReference.Mask<bool> GetEqualsMask(
            IATopicReferenceGetter item,
            IATopicReferenceGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new ATopicReference.Mask<bool>(false);
            ((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IATopicReferenceGetter item,
            IATopicReferenceGetter rhs,
            ATopicReference.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            if (rhs == null) return;
        }
        
        public string ToString(
            IATopicReferenceGetter item,
            string? name = null,
            ATopicReference.Mask<bool>? printMask = null)
        {
            var fg = new FileGeneration();
            ToString(
                item: item,
                fg: fg,
                name: name,
                printMask: printMask);
            return fg.ToString();
        }
        
        public void ToString(
            IATopicReferenceGetter item,
            FileGeneration fg,
            string? name = null,
            ATopicReference.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                fg.AppendLine($"ATopicReference =>");
            }
            else
            {
                fg.AppendLine($"{name} (ATopicReference) =>");
            }
            fg.AppendLine("[");
            using (new DepthWrapper(fg))
            {
                ToStringFields(
                    item: item,
                    fg: fg,
                    printMask: printMask);
            }
            fg.AppendLine("]");
        }
        
        protected static void ToStringFields(
            IATopicReferenceGetter item,
            FileGeneration fg,
            ATopicReference.Mask<bool>? printMask = null)
        {
        }
        
        public bool HasBeenSet(
            IATopicReferenceGetter item,
            ATopicReference.Mask<bool?> checkMask)
        {
            return true;
        }
        
        public void FillHasBeenSetMask(
            IATopicReferenceGetter item,
            ATopicReference.Mask<bool> mask)
        {
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IATopicReferenceGetter? lhs,
            IATopicReferenceGetter? rhs)
        {
            if (lhs == null && rhs == null) return false;
            if (lhs == null || rhs == null) return false;
            return true;
        }
        
        public virtual int GetHashCode(IATopicReferenceGetter item)
        {
            var hash = new HashCode();
            return hash.ToHashCode();
        }
        
        #endregion
        
        
        public virtual object GetNew()
        {
            return ATopicReference.GetNew();
        }
        
        #region Mutagen
        public IEnumerable<FormKey> GetLinkFormKeys(IATopicReferenceGetter obj)
        {
            yield break;
        }
        
        public void RemapLinks(IATopicReferenceGetter obj, IReadOnlyDictionary<FormKey, FormKey> mapping) => throw new NotImplementedException();
        #endregion
        
    }
    public partial class ATopicReferenceSetterTranslationCommon
    {
        public static readonly ATopicReferenceSetterTranslationCommon Instance = new ATopicReferenceSetterTranslationCommon();

        #region Deep Copy Fields From
        public virtual void DeepCopyIn(
            IATopicReference item,
            IATopicReferenceGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
        }
        
        #endregion
        
        public ATopicReference DeepCopy(
            IATopicReferenceGetter item,
            ATopicReference.TranslationMask? copyMask = null)
        {
            ATopicReference ret = (ATopicReference)((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).GetNew();
            ret.DeepCopyIn(
                item,
                copyMask: copyMask);
            return ret;
        }
        
        public ATopicReference DeepCopy(
            IATopicReferenceGetter item,
            out ATopicReference.ErrorMask errorMask,
            ATopicReference.TranslationMask? copyMask = null)
        {
            ATopicReference ret = (ATopicReference)((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).GetNew();
            ret.DeepCopyIn(
                item,
                errorMask: out errorMask,
                copyMask: copyMask);
            return ret;
        }
        
        public ATopicReference DeepCopy(
            IATopicReferenceGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            ATopicReference ret = (ATopicReference)((ATopicReferenceCommon)((IATopicReferenceGetter)item).CommonInstance()!).GetNew();
            ret.DeepCopyIn(
                item,
                errorMask: errorMask,
                copyMask: copyMask);
            return ret;
        }
        
    }
    #endregion

}

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ATopicReference
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => ATopicReference_Registration.Instance;
        public static ATopicReference_Registration Registration => ATopicReference_Registration.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonInstance() => ATopicReferenceCommon.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonSetterInstance()
        {
            return ATopicReferenceSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected virtual object CommonSetterTranslationInstance() => ATopicReferenceSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IATopicReferenceGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object IATopicReferenceGetter.CommonSetterInstance() => this.CommonSetterInstance();
        [DebuggerStepThrough]
        object IATopicReferenceGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

    }
}

#region Modules
#region Binary Translation
namespace Mutagen.Bethesda.Skyrim.Internals
{
    public partial class ATopicReferenceBinaryWriteTranslation : IBinaryWriteTranslator
    {
        public readonly static ATopicReferenceBinaryWriteTranslation Instance = new ATopicReferenceBinaryWriteTranslation();

        public virtual void Write(
            MutagenWriter writer,
            IATopicReferenceGetter item,
            RecordTypeConverter? recordTypeConverter = null)
        {
            using (HeaderExport.Header(
                writer: writer,
                record: recordTypeConverter.ConvertToCustom(RecordTypes.PDTO),
                type: Mutagen.Bethesda.Binary.ObjectType.Subrecord))
            {
            }
        }

        public virtual void Write(
            MutagenWriter writer,
            object item,
            RecordTypeConverter? recordTypeConverter = null)
        {
            Write(
                item: (IATopicReferenceGetter)item,
                writer: writer,
                recordTypeConverter: recordTypeConverter);
        }

    }

    public partial class ATopicReferenceBinaryCreateTranslation
    {
        public readonly static ATopicReferenceBinaryCreateTranslation Instance = new ATopicReferenceBinaryCreateTranslation();

    }

}
namespace Mutagen.Bethesda.Skyrim
{
    #region Binary Write Mixins
    public static class ATopicReferenceBinaryTranslationMixIn
    {
        public static void WriteToBinary(
            this IATopicReferenceGetter item,
            MutagenWriter writer)
        {
            ((ATopicReferenceBinaryWriteTranslation)item.BinaryWriteTranslator).Write(
                item: item,
                writer: writer,
                recordTypeConverter: null);
        }

    }
    #endregion


}
namespace Mutagen.Bethesda.Skyrim.Internals
{
    public partial class ATopicReferenceBinaryOverlay :
        BinaryOverlay,
        IATopicReferenceGetter
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => ATopicReference_Registration.Instance;
        public static ATopicReference_Registration Registration => ATopicReference_Registration.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonInstance() => ATopicReferenceCommon.Instance;
        [DebuggerStepThrough]
        protected virtual object CommonSetterTranslationInstance() => ATopicReferenceSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IATopicReferenceGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object? IATopicReferenceGetter.CommonSetterInstance() => null;
        [DebuggerStepThrough]
        object IATopicReferenceGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

        void IPrintable.ToString(FileGeneration fg, string? name) => this.ToString(fg, name);
        IMask<bool> ILoquiObjectGetter.GetHasBeenSetIMask() => this.GetHasBeenSetMask();
        IMask<bool> IEqualsMask.GetEqualsIMask(object rhs, EqualsMaskHelper.Include include) => this.GetEqualsMask((IATopicReferenceGetter)rhs, include);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual IEnumerable<FormKey> LinkFormKeys => ATopicReferenceCommon.Instance.GetLinkFormKeys(this);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IEnumerable<FormKey> ILinkedFormKeyContainer.LinkFormKeys => ATopicReferenceCommon.Instance.GetLinkFormKeys(this);
        protected virtual void RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => ATopicReferenceCommon.Instance.RemapLinks(this, mapping);
        void ILinkedFormKeyContainer.RemapLinks(IReadOnlyDictionary<FormKey, FormKey> mapping) => ATopicReferenceCommon.Instance.RemapLinks(this, mapping);
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected virtual object BinaryWriteTranslator => ATopicReferenceBinaryWriteTranslation.Instance;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IBinaryItem.BinaryWriteTranslator => this.BinaryWriteTranslator;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            RecordTypeConverter? recordTypeConverter = null)
        {
            ((ATopicReferenceBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                recordTypeConverter: recordTypeConverter);
        }

        partial void CustomFactoryEnd(
            OverlayStream stream,
            int finalPos,
            int offset);

        partial void CustomCtor();
        protected ATopicReferenceBinaryOverlay(
            ReadOnlyMemorySlice<byte> bytes,
            BinaryOverlayFactoryPackage package)
            : base(
                bytes: bytes,
                package: package)
        {
            this.CustomCtor();
        }


        #region To String

        public virtual void ToString(
            FileGeneration fg,
            string? name = null)
        {
            ATopicReferenceMixIn.ToString(
                item: this,
                name: name);
        }

        #endregion

    }

}
#endregion

#endregion

