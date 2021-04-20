/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Autogenerated by Loqui.  Do not manually change.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*/
#region Usings
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Mutagen.Bethesda.Records.Binary.Overlay;
using Mutagen.Bethesda.Records.Binary.Streams;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
#endregion

#nullable enable
namespace Mutagen.Bethesda.Oblivion
{
    #region Class
    public partial class KeyData :
        IEquatable<IKeyDataGetter>,
        IKeyData,
        ILoquiObjectSetter<KeyData>
    {
        #region Ctor
        public KeyData()
        {
            CustomCtor();
        }
        partial void CustomCtor();
        #endregion

        #region Value
        public UInt32 Value { get; set; } = default;
        #endregion
        #region Weight
        public Single Weight { get; set; } = default;
        #endregion

        #region To String

        public void ToString(
            FileGeneration fg,
            string? name = null)
        {
            KeyDataMixIn.ToString(
                item: this,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IKeyDataGetter rhs) return false;
            return ((KeyDataCommon)((IKeyDataGetter)this).CommonInstance()!).Equals(this, rhs, crystal: null);
        }

        public bool Equals(IKeyDataGetter? obj)
        {
            return ((KeyDataCommon)((IKeyDataGetter)this).CommonInstance()!).Equals(this, obj, crystal: null);
        }

        public override int GetHashCode() => ((KeyDataCommon)((IKeyDataGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

        #region Mask
        public class Mask<TItem> :
            IEquatable<Mask<TItem>>,
            IMask<TItem>
        {
            #region Ctors
            public Mask(TItem initialValue)
            {
                this.Value = initialValue;
                this.Weight = initialValue;
            }

            public Mask(
                TItem Value,
                TItem Weight)
            {
                this.Value = Value;
                this.Weight = Weight;
            }

            #pragma warning disable CS8618
            protected Mask()
            {
            }
            #pragma warning restore CS8618

            #endregion

            #region Members
            public TItem Value;
            public TItem Weight;
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
                if (!object.Equals(this.Value, rhs.Value)) return false;
                if (!object.Equals(this.Weight, rhs.Weight)) return false;
                return true;
            }
            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(this.Value);
                hash.Add(this.Weight);
                return hash.ToHashCode();
            }

            #endregion

            #region All
            public bool All(Func<TItem, bool> eval)
            {
                if (!eval(this.Value)) return false;
                if (!eval(this.Weight)) return false;
                return true;
            }
            #endregion

            #region Any
            public bool Any(Func<TItem, bool> eval)
            {
                if (eval(this.Value)) return true;
                if (eval(this.Weight)) return true;
                return false;
            }
            #endregion

            #region Translate
            public Mask<R> Translate<R>(Func<TItem, R> eval)
            {
                var ret = new KeyData.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
                obj.Value = eval(this.Value);
                obj.Weight = eval(this.Weight);
            }
            #endregion

            #region To String
            public override string ToString()
            {
                return ToString(printMask: null);
            }

            public string ToString(KeyData.Mask<bool>? printMask = null)
            {
                var fg = new FileGeneration();
                ToString(fg, printMask);
                return fg.ToString();
            }

            public void ToString(FileGeneration fg, KeyData.Mask<bool>? printMask = null)
            {
                fg.AppendLine($"{nameof(KeyData.Mask<TItem>)} =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                    if (printMask?.Value ?? true)
                    {
                        fg.AppendItem(Value, "Value");
                    }
                    if (printMask?.Weight ?? true)
                    {
                        fg.AppendItem(Weight, "Weight");
                    }
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
            public Exception? Value;
            public Exception? Weight;
            #endregion

            #region IErrorMask
            public object? GetNthMask(int index)
            {
                KeyData_FieldIndex enu = (KeyData_FieldIndex)index;
                switch (enu)
                {
                    case KeyData_FieldIndex.Value:
                        return Value;
                    case KeyData_FieldIndex.Weight:
                        return Weight;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthException(int index, Exception ex)
            {
                KeyData_FieldIndex enu = (KeyData_FieldIndex)index;
                switch (enu)
                {
                    case KeyData_FieldIndex.Value:
                        this.Value = ex;
                        break;
                    case KeyData_FieldIndex.Weight:
                        this.Weight = ex;
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthMask(int index, object obj)
            {
                KeyData_FieldIndex enu = (KeyData_FieldIndex)index;
                switch (enu)
                {
                    case KeyData_FieldIndex.Value:
                        this.Value = (Exception?)obj;
                        break;
                    case KeyData_FieldIndex.Weight:
                        this.Weight = (Exception?)obj;
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public bool IsInError()
            {
                if (Overall != null) return true;
                if (Value != null) return true;
                if (Weight != null) return true;
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

            public void ToString(FileGeneration fg, string? name = null)
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
            protected void ToString_FillInternal(FileGeneration fg)
            {
                fg.AppendItem(Value, "Value");
                fg.AppendItem(Weight, "Weight");
            }
            #endregion

            #region Combine
            public ErrorMask Combine(ErrorMask? rhs)
            {
                if (rhs == null) return this;
                var ret = new ErrorMask();
                ret.Value = this.Value.Combine(rhs.Value);
                ret.Weight = this.Weight.Combine(rhs.Weight);
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
            public readonly bool DefaultOn;
            public bool OnOverall;
            public bool Value;
            public bool Weight;
            #endregion

            #region Ctors
            public TranslationMask(
                bool defaultOn,
                bool onOverall = true)
            {
                this.DefaultOn = defaultOn;
                this.OnOverall = onOverall;
                this.Value = defaultOn;
                this.Weight = defaultOn;
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

            protected void GetCrystal(List<(bool On, TranslationCrystal? SubCrystal)> ret)
            {
                ret.Add((Value, null));
                ret.Add((Weight, null));
            }

            public static implicit operator TranslationMask(bool defaultOn)
            {
                return new TranslationMask(defaultOn: defaultOn, onOverall: defaultOn);
            }

        }
        #endregion

        #region Mutagen
        public static readonly RecordType GrupRecordType = KeyData_Registration.TriggeringRecordType;
        #endregion

        #region Binary Translation
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected object BinaryWriteTranslator => KeyDataBinaryWriteTranslation.Instance;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IBinaryItem.BinaryWriteTranslator => this.BinaryWriteTranslator;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            RecordTypeConverter? recordTypeConverter = null)
        {
            ((KeyDataBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                recordTypeConverter: recordTypeConverter);
        }
        #region Binary Create
        public static KeyData CreateFromBinary(
            MutagenFrame frame,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new KeyData();
            ((KeyDataSetterCommon)((IKeyDataGetter)ret).CommonSetterInstance()!).CopyInFromBinary(
                item: ret,
                frame: frame,
                recordTypeConverter: recordTypeConverter);
            return ret;
        }

        #endregion

        public static bool TryCreateFromBinary(
            MutagenFrame frame,
            out KeyData item,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var startPos = frame.Position;
            item = CreateFromBinary(frame, recordTypeConverter);
            return startPos != frame.Position;
        }
        #endregion

        void IPrintable.ToString(FileGeneration fg, string? name) => this.ToString(fg, name);

        void IClearable.Clear()
        {
            ((KeyDataSetterCommon)((IKeyDataGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static KeyData GetNew()
        {
            return new KeyData();
        }

    }
    #endregion

    #region Interface
    public partial interface IKeyData :
        IKeyDataGetter,
        ILoquiObjectSetter<IKeyData>,
        IWeightValue
    {
        new UInt32 Value { get; set; }
        new Single Weight { get; set; }
    }

    public partial interface IKeyDataGetter :
        ILoquiObject,
        IBinaryItem,
        ILoquiObject<IKeyDataGetter>,
        IWeightValueGetter
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object? CommonSetterInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonSetterTranslationInstance();
        static ILoquiRegistration Registration => KeyData_Registration.Instance;
        UInt32 Value { get; }
        Single Weight { get; }

    }

    #endregion

    #region Common MixIn
    public static partial class KeyDataMixIn
    {
        public static void Clear(this IKeyData item)
        {
            ((KeyDataSetterCommon)((IKeyDataGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static KeyData.Mask<bool> GetEqualsMask(
            this IKeyDataGetter item,
            IKeyDataGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((KeyDataCommon)((IKeyDataGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string ToString(
            this IKeyDataGetter item,
            string? name = null,
            KeyData.Mask<bool>? printMask = null)
        {
            return ((KeyDataCommon)((IKeyDataGetter)item).CommonInstance()!).ToString(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void ToString(
            this IKeyDataGetter item,
            FileGeneration fg,
            string? name = null,
            KeyData.Mask<bool>? printMask = null)
        {
            ((KeyDataCommon)((IKeyDataGetter)item).CommonInstance()!).ToString(
                item: item,
                fg: fg,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IKeyDataGetter item,
            IKeyDataGetter rhs,
            KeyData.TranslationMask? equalsMask = null)
        {
            return ((KeyDataCommon)((IKeyDataGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                crystal: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IKeyData lhs,
            IKeyDataGetter rhs)
        {
            ((KeyDataSetterTranslationCommon)((IKeyDataGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: default,
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IKeyData lhs,
            IKeyDataGetter rhs,
            KeyData.TranslationMask? copyMask = null)
        {
            ((KeyDataSetterTranslationCommon)((IKeyDataGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IKeyData lhs,
            IKeyDataGetter rhs,
            out KeyData.ErrorMask errorMask,
            KeyData.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((KeyDataSetterTranslationCommon)((IKeyDataGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = KeyData.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IKeyData lhs,
            IKeyDataGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((KeyDataSetterTranslationCommon)((IKeyDataGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static KeyData DeepCopy(
            this IKeyDataGetter item,
            KeyData.TranslationMask? copyMask = null)
        {
            return ((KeyDataSetterTranslationCommon)((IKeyDataGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static KeyData DeepCopy(
            this IKeyDataGetter item,
            out KeyData.ErrorMask errorMask,
            KeyData.TranslationMask? copyMask = null)
        {
            return ((KeyDataSetterTranslationCommon)((IKeyDataGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static KeyData DeepCopy(
            this IKeyDataGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((KeyDataSetterTranslationCommon)((IKeyDataGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

        #region Binary Translation
        public static void CopyInFromBinary(
            this IKeyData item,
            MutagenFrame frame,
            RecordTypeConverter? recordTypeConverter = null)
        {
            ((KeyDataSetterCommon)((IKeyDataGetter)item).CommonSetterInstance()!).CopyInFromBinary(
                item: item,
                frame: frame,
                recordTypeConverter: recordTypeConverter);
        }

        #endregion

    }
    #endregion

}

namespace Mutagen.Bethesda.Oblivion.Internals
{
    #region Field Index
    public enum KeyData_FieldIndex
    {
        Value = 0,
        Weight = 1,
    }
    #endregion

    #region Registration
    public partial class KeyData_Registration : ILoquiRegistration
    {
        public static readonly KeyData_Registration Instance = new KeyData_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Oblivion.ProtocolKey;

        public static readonly ObjectKey ObjectKey = new ObjectKey(
            protocolKey: ProtocolDefinition_Oblivion.ProtocolKey,
            msgID: 193,
            version: 0);

        public const string GUID = "01f6df94-7cf4-41f7-9a02-1996484507a7";

        public const ushort AdditionalFieldCount = 2;

        public const ushort FieldCount = 2;

        public static readonly Type MaskType = typeof(KeyData.Mask<>);

        public static readonly Type ErrorMaskType = typeof(KeyData.ErrorMask);

        public static readonly Type ClassType = typeof(KeyData);

        public static readonly Type GetterType = typeof(IKeyDataGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IKeyData);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Oblivion.KeyData";

        public const string Name = "KeyData";

        public const string Namespace = "Mutagen.Bethesda.Oblivion";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

        public static readonly RecordType TriggeringRecordType = RecordTypes.DATA;
        public static readonly Type BinaryWriteTranslation = typeof(KeyDataBinaryWriteTranslation);
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
        ushort? ILoquiRegistration.GetNameIndex(StringCaseAgnostic name) => throw new NotImplementedException();
        bool ILoquiRegistration.GetNthIsEnumerable(ushort index) => throw new NotImplementedException();
        bool ILoquiRegistration.GetNthIsLoqui(ushort index) => throw new NotImplementedException();
        bool ILoquiRegistration.GetNthIsSingleton(ushort index) => throw new NotImplementedException();
        string ILoquiRegistration.GetNthName(ushort index) => throw new NotImplementedException();
        bool ILoquiRegistration.IsNthDerivative(ushort index) => throw new NotImplementedException();
        bool ILoquiRegistration.IsProtected(ushort index) => throw new NotImplementedException();
        Type ILoquiRegistration.GetNthType(ushort index) => throw new NotImplementedException();
        #endregion

    }
    #endregion

    #region Common
    public partial class KeyDataSetterCommon
    {
        public static readonly KeyDataSetterCommon Instance = new KeyDataSetterCommon();

        partial void ClearPartial();
        
        public void Clear(IKeyData item)
        {
            ClearPartial();
            item.Value = default;
            item.Weight = default;
        }
        
        #region Mutagen
        public void RemapLinks(IKeyData obj, IReadOnlyDictionary<FormKey, FormKey> mapping)
        {
        }
        
        #endregion
        
        #region Binary Translation
        public virtual void CopyInFromBinary(
            IKeyData item,
            MutagenFrame frame,
            RecordTypeConverter? recordTypeConverter = null)
        {
            frame = frame.SpawnWithFinalPosition(HeaderTranslation.ParseSubrecord(
                frame.Reader,
                recordTypeConverter.ConvertToCustom(RecordTypes.DATA)));
            UtilityTranslation.SubrecordParse(
                record: item,
                frame: frame,
                recordTypeConverter: recordTypeConverter,
                fillStructs: KeyDataBinaryCreateTranslation.FillBinaryStructs);
        }
        
        #endregion
        
    }
    public partial class KeyDataCommon
    {
        public static readonly KeyDataCommon Instance = new KeyDataCommon();

        public KeyData.Mask<bool> GetEqualsMask(
            IKeyDataGetter item,
            IKeyDataGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new KeyData.Mask<bool>(false);
            ((KeyDataCommon)((IKeyDataGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IKeyDataGetter item,
            IKeyDataGetter rhs,
            KeyData.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            if (rhs == null) return;
            ret.Value = item.Value == rhs.Value;
            ret.Weight = item.Weight.EqualsWithin(rhs.Weight);
        }
        
        public string ToString(
            IKeyDataGetter item,
            string? name = null,
            KeyData.Mask<bool>? printMask = null)
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
            IKeyDataGetter item,
            FileGeneration fg,
            string? name = null,
            KeyData.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                fg.AppendLine($"KeyData =>");
            }
            else
            {
                fg.AppendLine($"{name} (KeyData) =>");
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
            IKeyDataGetter item,
            FileGeneration fg,
            KeyData.Mask<bool>? printMask = null)
        {
            if (printMask?.Value ?? true)
            {
                fg.AppendItem(item.Value, "Value");
            }
            if (printMask?.Weight ?? true)
            {
                fg.AppendItem(item.Weight, "Weight");
            }
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IKeyDataGetter? lhs,
            IKeyDataGetter? rhs,
            TranslationCrystal? crystal)
        {
            if (lhs == null && rhs == null) return false;
            if (lhs == null || rhs == null) return false;
            if ((crystal?.GetShouldTranslate((int)KeyData_FieldIndex.Value) ?? true))
            {
                if (lhs.Value != rhs.Value) return false;
            }
            if ((crystal?.GetShouldTranslate((int)KeyData_FieldIndex.Weight) ?? true))
            {
                if (!lhs.Weight.EqualsWithin(rhs.Weight)) return false;
            }
            return true;
        }
        
        public virtual int GetHashCode(IKeyDataGetter item)
        {
            var hash = new HashCode();
            hash.Add(item.Value);
            hash.Add(item.Weight);
            return hash.ToHashCode();
        }
        
        #endregion
        
        
        public object GetNew()
        {
            return KeyData.GetNew();
        }
        
        #region Mutagen
        public IEnumerable<FormLinkInformation> GetContainedFormLinks(IKeyDataGetter obj)
        {
            yield break;
        }
        
        #endregion
        
    }
    public partial class KeyDataSetterTranslationCommon
    {
        public static readonly KeyDataSetterTranslationCommon Instance = new KeyDataSetterTranslationCommon();

        #region DeepCopyIn
        public void DeepCopyIn(
            IKeyData item,
            IKeyDataGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy)
        {
            if ((copyMask?.GetShouldTranslate((int)KeyData_FieldIndex.Value) ?? true))
            {
                item.Value = rhs.Value;
            }
            if ((copyMask?.GetShouldTranslate((int)KeyData_FieldIndex.Weight) ?? true))
            {
                item.Weight = rhs.Weight;
            }
        }
        
        #endregion
        
        public KeyData DeepCopy(
            IKeyDataGetter item,
            KeyData.TranslationMask? copyMask = null)
        {
            KeyData ret = (KeyData)((KeyDataCommon)((IKeyDataGetter)item).CommonInstance()!).GetNew();
            ((KeyDataSetterTranslationCommon)((IKeyDataGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public KeyData DeepCopy(
            IKeyDataGetter item,
            out KeyData.ErrorMask errorMask,
            KeyData.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            KeyData ret = (KeyData)((KeyDataCommon)((IKeyDataGetter)item).CommonInstance()!).GetNew();
            ((KeyDataSetterTranslationCommon)((IKeyDataGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = KeyData.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public KeyData DeepCopy(
            IKeyDataGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            KeyData ret = (KeyData)((KeyDataCommon)((IKeyDataGetter)item).CommonInstance()!).GetNew();
            ((KeyDataSetterTranslationCommon)((IKeyDataGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: true);
            return ret;
        }
        
    }
    #endregion

}

namespace Mutagen.Bethesda.Oblivion
{
    public partial class KeyData
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => KeyData_Registration.Instance;
        public static KeyData_Registration Registration => KeyData_Registration.Instance;
        [DebuggerStepThrough]
        protected object CommonInstance() => KeyDataCommon.Instance;
        [DebuggerStepThrough]
        protected object CommonSetterInstance()
        {
            return KeyDataSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected object CommonSetterTranslationInstance() => KeyDataSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IKeyDataGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object IKeyDataGetter.CommonSetterInstance() => this.CommonSetterInstance();
        [DebuggerStepThrough]
        object IKeyDataGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

    }
}

#region Modules
#region Binary Translation
namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class KeyDataBinaryWriteTranslation : IBinaryWriteTranslator
    {
        public readonly static KeyDataBinaryWriteTranslation Instance = new KeyDataBinaryWriteTranslation();

        public static void WriteEmbedded(
            IKeyDataGetter item,
            MutagenWriter writer)
        {
            writer.Write(item.Value);
            Mutagen.Bethesda.Binary.FloatBinaryTranslation.Instance.Write(
                writer: writer,
                item: item.Weight);
        }

        public void Write(
            MutagenWriter writer,
            IKeyDataGetter item,
            RecordTypeConverter? recordTypeConverter = null)
        {
            using (HeaderExport.Header(
                writer: writer,
                record: recordTypeConverter.ConvertToCustom(RecordTypes.DATA),
                type: Mutagen.Bethesda.Binary.ObjectType.Subrecord))
            {
                WriteEmbedded(
                    item: item,
                    writer: writer);
            }
        }

        public void Write(
            MutagenWriter writer,
            object item,
            RecordTypeConverter? recordTypeConverter = null)
        {
            Write(
                item: (IKeyDataGetter)item,
                writer: writer,
                recordTypeConverter: recordTypeConverter);
        }

    }

    public partial class KeyDataBinaryCreateTranslation
    {
        public readonly static KeyDataBinaryCreateTranslation Instance = new KeyDataBinaryCreateTranslation();

        public static void FillBinaryStructs(
            IKeyData item,
            MutagenFrame frame)
        {
            item.Value = frame.ReadUInt32();
            item.Weight = Mutagen.Bethesda.Binary.FloatBinaryTranslation.Instance.Parse(frame: frame);
        }

    }

}
namespace Mutagen.Bethesda.Oblivion
{
    #region Binary Write Mixins
    public static class KeyDataBinaryTranslationMixIn
    {
        public static void WriteToBinary(
            this IKeyDataGetter item,
            MutagenWriter writer,
            RecordTypeConverter? recordTypeConverter = null)
        {
            ((KeyDataBinaryWriteTranslation)item.BinaryWriteTranslator).Write(
                item: item,
                writer: writer,
                recordTypeConverter: recordTypeConverter);
        }

    }
    #endregion


}
namespace Mutagen.Bethesda.Oblivion.Internals
{
    public partial class KeyDataBinaryOverlay :
        BinaryOverlay,
        IKeyDataGetter
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => KeyData_Registration.Instance;
        public static KeyData_Registration Registration => KeyData_Registration.Instance;
        [DebuggerStepThrough]
        protected object CommonInstance() => KeyDataCommon.Instance;
        [DebuggerStepThrough]
        protected object CommonSetterTranslationInstance() => KeyDataSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IKeyDataGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object? IKeyDataGetter.CommonSetterInstance() => null;
        [DebuggerStepThrough]
        object IKeyDataGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

        void IPrintable.ToString(FileGeneration fg, string? name) => this.ToString(fg, name);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected object BinaryWriteTranslator => KeyDataBinaryWriteTranslation.Instance;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IBinaryItem.BinaryWriteTranslator => this.BinaryWriteTranslator;
        void IBinaryItem.WriteToBinary(
            MutagenWriter writer,
            RecordTypeConverter? recordTypeConverter = null)
        {
            ((KeyDataBinaryWriteTranslation)this.BinaryWriteTranslator).Write(
                item: this,
                writer: writer,
                recordTypeConverter: recordTypeConverter);
        }

        public UInt32 Value => BinaryPrimitives.ReadUInt32LittleEndian(_data.Slice(0x0, 0x4));
        public Single Weight => _data.Slice(0x4, 0x4).Float();
        partial void CustomFactoryEnd(
            OverlayStream stream,
            int finalPos,
            int offset);

        partial void CustomCtor();
        protected KeyDataBinaryOverlay(
            ReadOnlyMemorySlice<byte> bytes,
            BinaryOverlayFactoryPackage package)
            : base(
                bytes: bytes,
                package: package)
        {
            this.CustomCtor();
        }

        public static KeyDataBinaryOverlay KeyDataFactory(
            OverlayStream stream,
            BinaryOverlayFactoryPackage package,
            RecordTypeConverter? recordTypeConverter = null)
        {
            var ret = new KeyDataBinaryOverlay(
                bytes: HeaderTranslation.ExtractSubrecordMemory(stream.RemainingMemory, package.MetaData.Constants),
                package: package);
            var finalPos = checked((int)(stream.Position + stream.GetSubrecord().TotalLength));
            int offset = stream.Position + package.MetaData.Constants.SubConstants.TypeAndLengthLength;
            stream.Position += 0x8 + package.MetaData.Constants.SubConstants.HeaderLength;
            ret.CustomFactoryEnd(
                stream: stream,
                finalPos: stream.Length,
                offset: offset);
            return ret;
        }

        public static KeyDataBinaryOverlay KeyDataFactory(
            ReadOnlyMemorySlice<byte> slice,
            BinaryOverlayFactoryPackage package,
            RecordTypeConverter? recordTypeConverter = null)
        {
            return KeyDataFactory(
                stream: new OverlayStream(slice, package),
                package: package,
                recordTypeConverter: recordTypeConverter);
        }

        #region To String

        public void ToString(
            FileGeneration fg,
            string? name = null)
        {
            KeyDataMixIn.ToString(
                item: this,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IKeyDataGetter rhs) return false;
            return ((KeyDataCommon)((IKeyDataGetter)this).CommonInstance()!).Equals(this, rhs, crystal: null);
        }

        public bool Equals(IKeyDataGetter? obj)
        {
            return ((KeyDataCommon)((IKeyDataGetter)this).CommonInstance()!).Equals(this, obj, crystal: null);
        }

        public override int GetHashCode() => ((KeyDataCommon)((IKeyDataGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

    }

}
#endregion

#endregion

