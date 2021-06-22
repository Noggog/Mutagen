/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Autogenerated by Loqui.  Do not manually change.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*/
#region Usings
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Pex;
using Mutagen.Bethesda.Pex.Internals;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
#endregion

#nullable enable
namespace Mutagen.Bethesda.Pex
{
    #region Class
    public partial class PexObjectState :
        IEquatable<IPexObjectStateGetter>,
        ILoquiObjectSetter<PexObjectState>,
        IPexObjectState
    {
        #region Ctor
        public PexObjectState()
        {
            CustomCtor();
        }
        partial void CustomCtor();
        #endregion

        #region Name
        public String? Name { get; set; }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String? IPexObjectStateGetter.Name => this.Name;
        #endregion
        #region Functions
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ExtendedList<PexObjectNamedFunction> _Functions = new ExtendedList<PexObjectNamedFunction>();
        public ExtendedList<PexObjectNamedFunction> Functions
        {
            get => this._Functions;
            init => this._Functions = value;
        }
        #region Interface Members
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<IPexObjectNamedFunctionGetter> IPexObjectStateGetter.Functions => _Functions;
        #endregion

        #endregion

        #region To String

        public void ToString(
            FileGeneration fg,
            string? name = null)
        {
            PexObjectStateMixIn.ToString(
                item: this,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IPexObjectStateGetter rhs) return false;
            return ((PexObjectStateCommon)((IPexObjectStateGetter)this).CommonInstance()!).Equals(this, rhs, crystal: null);
        }

        public bool Equals(IPexObjectStateGetter? obj)
        {
            return ((PexObjectStateCommon)((IPexObjectStateGetter)this).CommonInstance()!).Equals(this, obj, crystal: null);
        }

        public override int GetHashCode() => ((PexObjectStateCommon)((IPexObjectStateGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

        #region Mask
        public class Mask<TItem> :
            IEquatable<Mask<TItem>>,
            IMask<TItem>
        {
            #region Ctors
            public Mask(TItem initialValue)
            {
                this.Name = initialValue;
                this.Functions = new MaskItem<TItem, IEnumerable<MaskItemIndexed<TItem, PexObjectNamedFunction.Mask<TItem>?>>?>(initialValue, Enumerable.Empty<MaskItemIndexed<TItem, PexObjectNamedFunction.Mask<TItem>?>>());
            }

            public Mask(
                TItem Name,
                TItem Functions)
            {
                this.Name = Name;
                this.Functions = new MaskItem<TItem, IEnumerable<MaskItemIndexed<TItem, PexObjectNamedFunction.Mask<TItem>?>>?>(Functions, Enumerable.Empty<MaskItemIndexed<TItem, PexObjectNamedFunction.Mask<TItem>?>>());
            }

            #pragma warning disable CS8618
            protected Mask()
            {
            }
            #pragma warning restore CS8618

            #endregion

            #region Members
            public TItem Name;
            public MaskItem<TItem, IEnumerable<MaskItemIndexed<TItem, PexObjectNamedFunction.Mask<TItem>?>>?>? Functions;
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
                if (!object.Equals(this.Name, rhs.Name)) return false;
                if (!object.Equals(this.Functions, rhs.Functions)) return false;
                return true;
            }
            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(this.Name);
                hash.Add(this.Functions);
                return hash.ToHashCode();
            }

            #endregion

            #region All
            public bool All(Func<TItem, bool> eval)
            {
                if (!eval(this.Name)) return false;
                if (this.Functions != null)
                {
                    if (!eval(this.Functions.Overall)) return false;
                    if (this.Functions.Specific != null)
                    {
                        foreach (var item in this.Functions.Specific)
                        {
                            if (!eval(item.Overall)) return false;
                            if (item.Specific != null && !item.Specific.All(eval)) return false;
                        }
                    }
                }
                return true;
            }
            #endregion

            #region Any
            public bool Any(Func<TItem, bool> eval)
            {
                if (eval(this.Name)) return true;
                if (this.Functions != null)
                {
                    if (eval(this.Functions.Overall)) return true;
                    if (this.Functions.Specific != null)
                    {
                        foreach (var item in this.Functions.Specific)
                        {
                            if (!eval(item.Overall)) return false;
                            if (item.Specific != null && !item.Specific.All(eval)) return false;
                        }
                    }
                }
                return false;
            }
            #endregion

            #region Translate
            public Mask<R> Translate<R>(Func<TItem, R> eval)
            {
                var ret = new PexObjectState.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
                obj.Name = eval(this.Name);
                if (Functions != null)
                {
                    obj.Functions = new MaskItem<R, IEnumerable<MaskItemIndexed<R, PexObjectNamedFunction.Mask<R>?>>?>(eval(this.Functions.Overall), Enumerable.Empty<MaskItemIndexed<R, PexObjectNamedFunction.Mask<R>?>>());
                    if (Functions.Specific != null)
                    {
                        var l = new List<MaskItemIndexed<R, PexObjectNamedFunction.Mask<R>?>>();
                        obj.Functions.Specific = l;
                        foreach (var item in Functions.Specific.WithIndex())
                        {
                            MaskItemIndexed<R, PexObjectNamedFunction.Mask<R>?>? mask = item.Item == null ? null : new MaskItemIndexed<R, PexObjectNamedFunction.Mask<R>?>(item.Item.Index, eval(item.Item.Overall), item.Item.Specific?.Translate(eval));
                            if (mask == null) continue;
                            l.Add(mask);
                        }
                    }
                }
            }
            #endregion

            #region To String
            public override string ToString()
            {
                return ToString(printMask: null);
            }

            public string ToString(PexObjectState.Mask<bool>? printMask = null)
            {
                var fg = new FileGeneration();
                ToString(fg, printMask);
                return fg.ToString();
            }

            public void ToString(FileGeneration fg, PexObjectState.Mask<bool>? printMask = null)
            {
                fg.AppendLine($"{nameof(PexObjectState.Mask<TItem>)} =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                    if (printMask?.Name ?? true)
                    {
                        fg.AppendItem(Name, "Name");
                    }
                    if ((printMask?.Functions?.Overall ?? true)
                        && Functions.TryGet(out var FunctionsItem))
                    {
                        fg.AppendLine("Functions =>");
                        fg.AppendLine("[");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendItem(FunctionsItem.Overall);
                            if (FunctionsItem.Specific != null)
                            {
                                foreach (var subItem in FunctionsItem.Specific)
                                {
                                    fg.AppendLine("[");
                                    using (new DepthWrapper(fg))
                                    {
                                        subItem?.ToString(fg);
                                    }
                                    fg.AppendLine("]");
                                }
                            }
                        }
                        fg.AppendLine("]");
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
            public Exception? Name;
            public MaskItem<Exception?, IEnumerable<MaskItem<Exception?, PexObjectNamedFunction.ErrorMask?>>?>? Functions;
            #endregion

            #region IErrorMask
            public object? GetNthMask(int index)
            {
                PexObjectState_FieldIndex enu = (PexObjectState_FieldIndex)index;
                switch (enu)
                {
                    case PexObjectState_FieldIndex.Name:
                        return Name;
                    case PexObjectState_FieldIndex.Functions:
                        return Functions;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthException(int index, Exception ex)
            {
                PexObjectState_FieldIndex enu = (PexObjectState_FieldIndex)index;
                switch (enu)
                {
                    case PexObjectState_FieldIndex.Name:
                        this.Name = ex;
                        break;
                    case PexObjectState_FieldIndex.Functions:
                        this.Functions = new MaskItem<Exception?, IEnumerable<MaskItem<Exception?, PexObjectNamedFunction.ErrorMask?>>?>(ex, null);
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthMask(int index, object obj)
            {
                PexObjectState_FieldIndex enu = (PexObjectState_FieldIndex)index;
                switch (enu)
                {
                    case PexObjectState_FieldIndex.Name:
                        this.Name = (Exception?)obj;
                        break;
                    case PexObjectState_FieldIndex.Functions:
                        this.Functions = (MaskItem<Exception?, IEnumerable<MaskItem<Exception?, PexObjectNamedFunction.ErrorMask?>>?>)obj;
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public bool IsInError()
            {
                if (Overall != null) return true;
                if (Name != null) return true;
                if (Functions != null) return true;
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
                fg.AppendItem(Name, "Name");
                if (Functions.TryGet(out var FunctionsItem))
                {
                    fg.AppendLine("Functions =>");
                    fg.AppendLine("[");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendItem(FunctionsItem.Overall);
                        if (FunctionsItem.Specific != null)
                        {
                            foreach (var subItem in FunctionsItem.Specific)
                            {
                                fg.AppendLine("[");
                                using (new DepthWrapper(fg))
                                {
                                    subItem?.ToString(fg);
                                }
                                fg.AppendLine("]");
                            }
                        }
                    }
                    fg.AppendLine("]");
                }
            }
            #endregion

            #region Combine
            public ErrorMask Combine(ErrorMask? rhs)
            {
                if (rhs == null) return this;
                var ret = new ErrorMask();
                ret.Name = this.Name.Combine(rhs.Name);
                ret.Functions = new MaskItem<Exception?, IEnumerable<MaskItem<Exception?, PexObjectNamedFunction.ErrorMask?>>?>(ExceptionExt.Combine(this.Functions?.Overall, rhs.Functions?.Overall), ExceptionExt.Combine(this.Functions?.Specific, rhs.Functions?.Specific));
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
            public bool Name;
            public PexObjectNamedFunction.TranslationMask? Functions;
            #endregion

            #region Ctors
            public TranslationMask(
                bool defaultOn,
                bool onOverall = true)
            {
                this.DefaultOn = defaultOn;
                this.OnOverall = onOverall;
                this.Name = defaultOn;
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
                ret.Add((Name, null));
                ret.Add((Functions == null ? DefaultOn : !Functions.GetCrystal().CopyNothing, Functions?.GetCrystal()));
            }

            public static implicit operator TranslationMask(bool defaultOn)
            {
                return new TranslationMask(defaultOn: defaultOn, onOverall: defaultOn);
            }

        }
        #endregion

        void IPrintable.ToString(FileGeneration fg, string? name) => this.ToString(fg, name);

        void IClearable.Clear()
        {
            ((PexObjectStateSetterCommon)((IPexObjectStateGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static PexObjectState GetNew()
        {
            return new PexObjectState();
        }

    }
    #endregion

    #region Interface
    public partial interface IPexObjectState :
        ILoquiObjectSetter<IPexObjectState>,
        IPexObjectStateGetter
    {
        new String? Name { get; set; }
        new ExtendedList<PexObjectNamedFunction> Functions { get; }
    }

    public partial interface IPexObjectStateGetter :
        ILoquiObject,
        ILoquiObject<IPexObjectStateGetter>
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object? CommonSetterInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonSetterTranslationInstance();
        static ILoquiRegistration Registration => PexObjectState_Registration.Instance;
        String? Name { get; }
        IReadOnlyList<IPexObjectNamedFunctionGetter> Functions { get; }

    }

    #endregion

    #region Common MixIn
    public static partial class PexObjectStateMixIn
    {
        public static void Clear(this IPexObjectState item)
        {
            ((PexObjectStateSetterCommon)((IPexObjectStateGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static PexObjectState.Mask<bool> GetEqualsMask(
            this IPexObjectStateGetter item,
            IPexObjectStateGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((PexObjectStateCommon)((IPexObjectStateGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string ToString(
            this IPexObjectStateGetter item,
            string? name = null,
            PexObjectState.Mask<bool>? printMask = null)
        {
            return ((PexObjectStateCommon)((IPexObjectStateGetter)item).CommonInstance()!).ToString(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void ToString(
            this IPexObjectStateGetter item,
            FileGeneration fg,
            string? name = null,
            PexObjectState.Mask<bool>? printMask = null)
        {
            ((PexObjectStateCommon)((IPexObjectStateGetter)item).CommonInstance()!).ToString(
                item: item,
                fg: fg,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IPexObjectStateGetter item,
            IPexObjectStateGetter rhs,
            PexObjectState.TranslationMask? equalsMask = null)
        {
            return ((PexObjectStateCommon)((IPexObjectStateGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                crystal: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IPexObjectState lhs,
            IPexObjectStateGetter rhs)
        {
            ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: default,
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IPexObjectState lhs,
            IPexObjectStateGetter rhs,
            PexObjectState.TranslationMask? copyMask = null)
        {
            ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IPexObjectState lhs,
            IPexObjectStateGetter rhs,
            out PexObjectState.ErrorMask errorMask,
            PexObjectState.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = PexObjectState.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IPexObjectState lhs,
            IPexObjectStateGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static PexObjectState DeepCopy(
            this IPexObjectStateGetter item,
            PexObjectState.TranslationMask? copyMask = null)
        {
            return ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static PexObjectState DeepCopy(
            this IPexObjectStateGetter item,
            out PexObjectState.ErrorMask errorMask,
            PexObjectState.TranslationMask? copyMask = null)
        {
            return ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static PexObjectState DeepCopy(
            this IPexObjectStateGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: errorMask);
        }

    }
    #endregion

}

namespace Mutagen.Bethesda.Pex.Internals
{
    #region Field Index
    public enum PexObjectState_FieldIndex
    {
        Name = 0,
        Functions = 1,
    }
    #endregion

    #region Registration
    public partial class PexObjectState_Registration : ILoquiRegistration
    {
        public static readonly PexObjectState_Registration Instance = new PexObjectState_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Pex.ProtocolKey;

        public static readonly ObjectKey ObjectKey = new ObjectKey(
            protocolKey: ProtocolDefinition_Pex.ProtocolKey,
            msgID: 12,
            version: 0);

        public const string GUID = "8840a495-0e1d-41d4-9397-a81a14b0b040";

        public const ushort AdditionalFieldCount = 2;

        public const ushort FieldCount = 2;

        public static readonly Type MaskType = typeof(PexObjectState.Mask<>);

        public static readonly Type ErrorMaskType = typeof(PexObjectState.ErrorMask);

        public static readonly Type ClassType = typeof(PexObjectState);

        public static readonly Type GetterType = typeof(IPexObjectStateGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IPexObjectState);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Pex.PexObjectState";

        public const string Name = "PexObjectState";

        public const string Namespace = "Mutagen.Bethesda.Pex";

        public const byte GenericCount = 0;

        public static readonly Type? GenericRegistrationType = null;

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
    public partial class PexObjectStateSetterCommon
    {
        public static readonly PexObjectStateSetterCommon Instance = new PexObjectStateSetterCommon();

        partial void ClearPartial();
        
        public void Clear(IPexObjectState item)
        {
            ClearPartial();
            item.Name = default;
            item.Functions.Clear();
        }
        
    }
    public partial class PexObjectStateCommon
    {
        public static readonly PexObjectStateCommon Instance = new PexObjectStateCommon();

        public PexObjectState.Mask<bool> GetEqualsMask(
            IPexObjectStateGetter item,
            IPexObjectStateGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new PexObjectState.Mask<bool>(false);
            ((PexObjectStateCommon)((IPexObjectStateGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IPexObjectStateGetter item,
            IPexObjectStateGetter rhs,
            PexObjectState.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            if (rhs == null) return;
            ret.Name = string.Equals(item.Name, rhs.Name);
            ret.Functions = item.Functions.CollectionEqualsHelper(
                rhs.Functions,
                (loqLhs, loqRhs) => loqLhs.GetEqualsMask(loqRhs, include),
                include);
        }
        
        public string ToString(
            IPexObjectStateGetter item,
            string? name = null,
            PexObjectState.Mask<bool>? printMask = null)
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
            IPexObjectStateGetter item,
            FileGeneration fg,
            string? name = null,
            PexObjectState.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                fg.AppendLine($"PexObjectState =>");
            }
            else
            {
                fg.AppendLine($"{name} (PexObjectState) =>");
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
            IPexObjectStateGetter item,
            FileGeneration fg,
            PexObjectState.Mask<bool>? printMask = null)
        {
            if ((printMask?.Name ?? true)
                && item.Name.TryGet(out var NameItem))
            {
                fg.AppendItem(NameItem, "Name");
            }
            if (printMask?.Functions?.Overall ?? true)
            {
                fg.AppendLine("Functions =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                    foreach (var subItem in item.Functions)
                    {
                        fg.AppendLine("[");
                        using (new DepthWrapper(fg))
                        {
                            subItem?.ToString(fg, "Item");
                        }
                        fg.AppendLine("]");
                    }
                }
                fg.AppendLine("]");
            }
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IPexObjectStateGetter? lhs,
            IPexObjectStateGetter? rhs,
            TranslationCrystal? crystal)
        {
            if (!EqualsMaskHelper.RefEquality(lhs, rhs, out var isEqual)) return isEqual;
            if ((crystal?.GetShouldTranslate((int)PexObjectState_FieldIndex.Name) ?? true))
            {
                if (!string.Equals(lhs.Name, rhs.Name)) return false;
            }
            if ((crystal?.GetShouldTranslate((int)PexObjectState_FieldIndex.Functions) ?? true))
            {
                if (!lhs.Functions.SequenceEqualNullable(rhs.Functions)) return false;
            }
            return true;
        }
        
        public virtual int GetHashCode(IPexObjectStateGetter item)
        {
            var hash = new HashCode();
            if (item.Name.TryGet(out var Nameitem))
            {
                hash.Add(Nameitem);
            }
            hash.Add(item.Functions);
            return hash.ToHashCode();
        }
        
        #endregion
        
        
        public object GetNew()
        {
            return PexObjectState.GetNew();
        }
        
    }
    public partial class PexObjectStateSetterTranslationCommon
    {
        public static readonly PexObjectStateSetterTranslationCommon Instance = new PexObjectStateSetterTranslationCommon();

        #region DeepCopyIn
        public void DeepCopyIn(
            IPexObjectState item,
            IPexObjectStateGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy)
        {
            if ((copyMask?.GetShouldTranslate((int)PexObjectState_FieldIndex.Name) ?? true))
            {
                item.Name = rhs.Name;
            }
            if ((copyMask?.GetShouldTranslate((int)PexObjectState_FieldIndex.Functions) ?? true))
            {
                errorMask?.PushIndex((int)PexObjectState_FieldIndex.Functions);
                try
                {
                    item.Functions.SetTo(
                        rhs.Functions
                        .Select(r =>
                        {
                            return r.DeepCopy(
                                errorMask: errorMask,
                                default(TranslationCrystal));
                        }));
                }
                catch (Exception ex)
                when (errorMask != null)
                {
                    errorMask.ReportException(ex);
                }
                finally
                {
                    errorMask?.PopIndex();
                }
            }
        }
        
        #endregion
        
        public PexObjectState DeepCopy(
            IPexObjectStateGetter item,
            PexObjectState.TranslationMask? copyMask = null)
        {
            PexObjectState ret = (PexObjectState)((PexObjectStateCommon)((IPexObjectStateGetter)item).CommonInstance()!).GetNew();
            ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public PexObjectState DeepCopy(
            IPexObjectStateGetter item,
            out PexObjectState.ErrorMask errorMask,
            PexObjectState.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            PexObjectState ret = (PexObjectState)((PexObjectStateCommon)((IPexObjectStateGetter)item).CommonInstance()!).GetNew();
            ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = PexObjectState.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public PexObjectState DeepCopy(
            IPexObjectStateGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            PexObjectState ret = (PexObjectState)((PexObjectStateCommon)((IPexObjectStateGetter)item).CommonInstance()!).GetNew();
            ((PexObjectStateSetterTranslationCommon)((IPexObjectStateGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
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

namespace Mutagen.Bethesda.Pex
{
    public partial class PexObjectState
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => PexObjectState_Registration.Instance;
        public static PexObjectState_Registration Registration => PexObjectState_Registration.Instance;
        [DebuggerStepThrough]
        protected object CommonInstance() => PexObjectStateCommon.Instance;
        [DebuggerStepThrough]
        protected object CommonSetterInstance()
        {
            return PexObjectStateSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected object CommonSetterTranslationInstance() => PexObjectStateSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IPexObjectStateGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object IPexObjectStateGetter.CommonSetterInstance() => this.CommonSetterInstance();
        [DebuggerStepThrough]
        object IPexObjectStateGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

    }
}

