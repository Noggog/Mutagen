/*
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
 * Autogenerated by Loqui.  Do not manually change.
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * 
*/
#region Usings
using Loqui;
using Loqui.Internal;
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
    public partial class DebugPropertyGroup :
        IDebugPropertyGroup,
        IEquatable<IDebugPropertyGroupGetter>,
        ILoquiObjectSetter<DebugPropertyGroup>
    {
        #region Ctor
        public DebugPropertyGroup()
        {
            CustomCtor();
        }
        partial void CustomCtor();
        #endregion

        #region ObjectName
        public String ObjectName { get; set; } = string.Empty;
        #endregion
        #region GroupName
        public String GroupName { get; set; } = string.Empty;
        #endregion
        #region PropertyNames
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ExtendedList<String> _PropertyNames = new ExtendedList<String>();
        public ExtendedList<String> PropertyNames
        {
            get => this._PropertyNames;
            init => this._PropertyNames = value;
        }
        #region Interface Members
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IReadOnlyList<String> IDebugPropertyGroupGetter.PropertyNames => _PropertyNames;
        #endregion

        #endregion

        #region To String

        public void ToString(
            FileGeneration fg,
            string? name = null)
        {
            DebugPropertyGroupMixIn.ToString(
                item: this,
                name: name);
        }

        #endregion

        #region Equals and Hash
        public override bool Equals(object? obj)
        {
            if (obj is not IDebugPropertyGroupGetter rhs) return false;
            return ((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)this).CommonInstance()!).Equals(this, rhs, crystal: null);
        }

        public bool Equals(IDebugPropertyGroupGetter? obj)
        {
            return ((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)this).CommonInstance()!).Equals(this, obj, crystal: null);
        }

        public override int GetHashCode() => ((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)this).CommonInstance()!).GetHashCode(this);

        #endregion

        #region Mask
        public class Mask<TItem> :
            IEquatable<Mask<TItem>>,
            IMask<TItem>
        {
            #region Ctors
            public Mask(TItem initialValue)
            {
                this.ObjectName = initialValue;
                this.GroupName = initialValue;
                this.PropertyNames = new MaskItem<TItem, IEnumerable<(int Index, TItem Value)>?>(initialValue, Enumerable.Empty<(int Index, TItem Value)>());
            }

            public Mask(
                TItem ObjectName,
                TItem GroupName,
                TItem PropertyNames)
            {
                this.ObjectName = ObjectName;
                this.GroupName = GroupName;
                this.PropertyNames = new MaskItem<TItem, IEnumerable<(int Index, TItem Value)>?>(PropertyNames, Enumerable.Empty<(int Index, TItem Value)>());
            }

            #pragma warning disable CS8618
            protected Mask()
            {
            }
            #pragma warning restore CS8618

            #endregion

            #region Members
            public TItem ObjectName;
            public TItem GroupName;
            public MaskItem<TItem, IEnumerable<(int Index, TItem Value)>?>? PropertyNames;
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
                if (!object.Equals(this.ObjectName, rhs.ObjectName)) return false;
                if (!object.Equals(this.GroupName, rhs.GroupName)) return false;
                if (!object.Equals(this.PropertyNames, rhs.PropertyNames)) return false;
                return true;
            }
            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(this.ObjectName);
                hash.Add(this.GroupName);
                hash.Add(this.PropertyNames);
                return hash.ToHashCode();
            }

            #endregion

            #region All
            public bool All(Func<TItem, bool> eval)
            {
                if (!eval(this.ObjectName)) return false;
                if (!eval(this.GroupName)) return false;
                if (this.PropertyNames != null)
                {
                    if (!eval(this.PropertyNames.Overall)) return false;
                    if (this.PropertyNames.Specific != null)
                    {
                        foreach (var item in this.PropertyNames.Specific)
                        {
                            if (!eval(item.Value)) return false;
                        }
                    }
                }
                return true;
            }
            #endregion

            #region Any
            public bool Any(Func<TItem, bool> eval)
            {
                if (eval(this.ObjectName)) return true;
                if (eval(this.GroupName)) return true;
                if (this.PropertyNames != null)
                {
                    if (eval(this.PropertyNames.Overall)) return true;
                    if (this.PropertyNames.Specific != null)
                    {
                        foreach (var item in this.PropertyNames.Specific)
                        {
                            if (!eval(item.Value)) return false;
                        }
                    }
                }
                return false;
            }
            #endregion

            #region Translate
            public Mask<R> Translate<R>(Func<TItem, R> eval)
            {
                var ret = new DebugPropertyGroup.Mask<R>();
                this.Translate_InternalFill(ret, eval);
                return ret;
            }

            protected void Translate_InternalFill<R>(Mask<R> obj, Func<TItem, R> eval)
            {
                obj.ObjectName = eval(this.ObjectName);
                obj.GroupName = eval(this.GroupName);
                if (PropertyNames != null)
                {
                    obj.PropertyNames = new MaskItem<R, IEnumerable<(int Index, R Value)>?>(eval(this.PropertyNames.Overall), Enumerable.Empty<(int Index, R Value)>());
                    if (PropertyNames.Specific != null)
                    {
                        var l = new List<(int Index, R Item)>();
                        obj.PropertyNames.Specific = l;
                        foreach (var item in PropertyNames.Specific.WithIndex())
                        {
                            R mask = eval(item.Item.Value);
                            l.Add((item.Index, mask));
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

            public string ToString(DebugPropertyGroup.Mask<bool>? printMask = null)
            {
                var fg = new FileGeneration();
                ToString(fg, printMask);
                return fg.ToString();
            }

            public void ToString(FileGeneration fg, DebugPropertyGroup.Mask<bool>? printMask = null)
            {
                fg.AppendLine($"{nameof(DebugPropertyGroup.Mask<TItem>)} =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                    if (printMask?.ObjectName ?? true)
                    {
                        fg.AppendItem(ObjectName, "ObjectName");
                    }
                    if (printMask?.GroupName ?? true)
                    {
                        fg.AppendItem(GroupName, "GroupName");
                    }
                    if ((printMask?.PropertyNames?.Overall ?? true)
                        && PropertyNames.TryGet(out var PropertyNamesItem))
                    {
                        fg.AppendLine("PropertyNames =>");
                        fg.AppendLine("[");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendItem(PropertyNamesItem.Overall);
                            if (PropertyNamesItem.Specific != null)
                            {
                                foreach (var subItem in PropertyNamesItem.Specific)
                                {
                                    fg.AppendLine("[");
                                    using (new DepthWrapper(fg))
                                    {
                                        fg.AppendItem(subItem);
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
            public Exception? ObjectName;
            public Exception? GroupName;
            public MaskItem<Exception?, IEnumerable<(int Index, Exception Value)>?>? PropertyNames;
            #endregion

            #region IErrorMask
            public object? GetNthMask(int index)
            {
                DebugPropertyGroup_FieldIndex enu = (DebugPropertyGroup_FieldIndex)index;
                switch (enu)
                {
                    case DebugPropertyGroup_FieldIndex.ObjectName:
                        return ObjectName;
                    case DebugPropertyGroup_FieldIndex.GroupName:
                        return GroupName;
                    case DebugPropertyGroup_FieldIndex.PropertyNames:
                        return PropertyNames;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthException(int index, Exception ex)
            {
                DebugPropertyGroup_FieldIndex enu = (DebugPropertyGroup_FieldIndex)index;
                switch (enu)
                {
                    case DebugPropertyGroup_FieldIndex.ObjectName:
                        this.ObjectName = ex;
                        break;
                    case DebugPropertyGroup_FieldIndex.GroupName:
                        this.GroupName = ex;
                        break;
                    case DebugPropertyGroup_FieldIndex.PropertyNames:
                        this.PropertyNames = new MaskItem<Exception?, IEnumerable<(int Index, Exception Value)>?>(ex, null);
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public void SetNthMask(int index, object obj)
            {
                DebugPropertyGroup_FieldIndex enu = (DebugPropertyGroup_FieldIndex)index;
                switch (enu)
                {
                    case DebugPropertyGroup_FieldIndex.ObjectName:
                        this.ObjectName = (Exception?)obj;
                        break;
                    case DebugPropertyGroup_FieldIndex.GroupName:
                        this.GroupName = (Exception?)obj;
                        break;
                    case DebugPropertyGroup_FieldIndex.PropertyNames:
                        this.PropertyNames = (MaskItem<Exception?, IEnumerable<(int Index, Exception Value)>?>)obj;
                        break;
                    default:
                        throw new ArgumentException($"Index is out of range: {index}");
                }
            }

            public bool IsInError()
            {
                if (Overall != null) return true;
                if (ObjectName != null) return true;
                if (GroupName != null) return true;
                if (PropertyNames != null) return true;
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
                fg.AppendItem(ObjectName, "ObjectName");
                fg.AppendItem(GroupName, "GroupName");
                if (PropertyNames.TryGet(out var PropertyNamesItem))
                {
                    fg.AppendLine("PropertyNames =>");
                    fg.AppendLine("[");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendItem(PropertyNamesItem.Overall);
                        if (PropertyNamesItem.Specific != null)
                        {
                            foreach (var subItem in PropertyNamesItem.Specific)
                            {
                                fg.AppendLine("[");
                                using (new DepthWrapper(fg))
                                {
                                    fg.AppendItem(subItem);
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
                ret.ObjectName = this.ObjectName.Combine(rhs.ObjectName);
                ret.GroupName = this.GroupName.Combine(rhs.GroupName);
                ret.PropertyNames = new MaskItem<Exception?, IEnumerable<(int Index, Exception Value)>?>(ExceptionExt.Combine(this.PropertyNames?.Overall, rhs.PropertyNames?.Overall), ExceptionExt.Combine(this.PropertyNames?.Specific, rhs.PropertyNames?.Specific));
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
            public bool ObjectName;
            public bool GroupName;
            public bool PropertyNames;
            #endregion

            #region Ctors
            public TranslationMask(
                bool defaultOn,
                bool onOverall = true)
            {
                this.DefaultOn = defaultOn;
                this.OnOverall = onOverall;
                this.ObjectName = defaultOn;
                this.GroupName = defaultOn;
                this.PropertyNames = defaultOn;
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
                ret.Add((ObjectName, null));
                ret.Add((GroupName, null));
                ret.Add((PropertyNames, null));
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
            ((DebugPropertyGroupSetterCommon)((IDebugPropertyGroupGetter)this).CommonSetterInstance()!).Clear(this);
        }

        internal static DebugPropertyGroup GetNew()
        {
            return new DebugPropertyGroup();
        }

    }
    #endregion

    #region Interface
    public partial interface IDebugPropertyGroup :
        IDebugPropertyGroupGetter,
        ILoquiObjectSetter<IDebugPropertyGroup>
    {
        new String ObjectName { get; set; }
        new String GroupName { get; set; }
        new ExtendedList<String> PropertyNames { get; }
    }

    public partial interface IDebugPropertyGroupGetter :
        ILoquiObject,
        ILoquiObject<IDebugPropertyGroupGetter>
    {
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object? CommonSetterInstance();
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        object CommonSetterTranslationInstance();
        static ILoquiRegistration Registration => DebugPropertyGroup_Registration.Instance;
        String ObjectName { get; }
        String GroupName { get; }
        IReadOnlyList<String> PropertyNames { get; }

    }

    #endregion

    #region Common MixIn
    public static partial class DebugPropertyGroupMixIn
    {
        public static void Clear(this IDebugPropertyGroup item)
        {
            ((DebugPropertyGroupSetterCommon)((IDebugPropertyGroupGetter)item).CommonSetterInstance()!).Clear(item: item);
        }

        public static DebugPropertyGroup.Mask<bool> GetEqualsMask(
            this IDebugPropertyGroupGetter item,
            IDebugPropertyGroupGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            return ((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)item).CommonInstance()!).GetEqualsMask(
                item: item,
                rhs: rhs,
                include: include);
        }

        public static string ToString(
            this IDebugPropertyGroupGetter item,
            string? name = null,
            DebugPropertyGroup.Mask<bool>? printMask = null)
        {
            return ((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)item).CommonInstance()!).ToString(
                item: item,
                name: name,
                printMask: printMask);
        }

        public static void ToString(
            this IDebugPropertyGroupGetter item,
            FileGeneration fg,
            string? name = null,
            DebugPropertyGroup.Mask<bool>? printMask = null)
        {
            ((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)item).CommonInstance()!).ToString(
                item: item,
                fg: fg,
                name: name,
                printMask: printMask);
        }

        public static bool Equals(
            this IDebugPropertyGroupGetter item,
            IDebugPropertyGroupGetter rhs,
            DebugPropertyGroup.TranslationMask? equalsMask = null)
        {
            return ((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)item).CommonInstance()!).Equals(
                lhs: item,
                rhs: rhs,
                crystal: equalsMask?.GetCrystal());
        }

        public static void DeepCopyIn(
            this IDebugPropertyGroup lhs,
            IDebugPropertyGroupGetter rhs)
        {
            ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: default,
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IDebugPropertyGroup lhs,
            IDebugPropertyGroupGetter rhs,
            DebugPropertyGroup.TranslationMask? copyMask = null)
        {
            ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: default,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
        }

        public static void DeepCopyIn(
            this IDebugPropertyGroup lhs,
            IDebugPropertyGroupGetter rhs,
            out DebugPropertyGroup.ErrorMask errorMask,
            DebugPropertyGroup.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: false);
            errorMask = DebugPropertyGroup.ErrorMask.Factory(errorMaskBuilder);
        }

        public static void DeepCopyIn(
            this IDebugPropertyGroup lhs,
            IDebugPropertyGroupGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask)
        {
            ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)lhs).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: lhs,
                rhs: rhs,
                errorMask: errorMask,
                copyMask: copyMask,
                deepCopy: false);
        }

        public static DebugPropertyGroup DeepCopy(
            this IDebugPropertyGroupGetter item,
            DebugPropertyGroup.TranslationMask? copyMask = null)
        {
            return ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask);
        }

        public static DebugPropertyGroup DeepCopy(
            this IDebugPropertyGroupGetter item,
            out DebugPropertyGroup.ErrorMask errorMask,
            DebugPropertyGroup.TranslationMask? copyMask = null)
        {
            return ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
                item: item,
                copyMask: copyMask,
                errorMask: out errorMask);
        }

        public static DebugPropertyGroup DeepCopy(
            this IDebugPropertyGroupGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            return ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)item).CommonSetterTranslationInstance()!).DeepCopy(
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
    public enum DebugPropertyGroup_FieldIndex
    {
        ObjectName = 0,
        GroupName = 1,
        PropertyNames = 2,
    }
    #endregion

    #region Registration
    public partial class DebugPropertyGroup_Registration : ILoquiRegistration
    {
        public static readonly DebugPropertyGroup_Registration Instance = new DebugPropertyGroup_Registration();

        public static ProtocolKey ProtocolKey => ProtocolDefinition_Pex.ProtocolKey;

        public static readonly ObjectKey ObjectKey = new ObjectKey(
            protocolKey: ProtocolDefinition_Pex.ProtocolKey,
            msgID: 3,
            version: 0);

        public const string GUID = "43092218-3eb0-48e6-9470-0dc6fbf25245";

        public const ushort AdditionalFieldCount = 3;

        public const ushort FieldCount = 3;

        public static readonly Type MaskType = typeof(DebugPropertyGroup.Mask<>);

        public static readonly Type ErrorMaskType = typeof(DebugPropertyGroup.ErrorMask);

        public static readonly Type ClassType = typeof(DebugPropertyGroup);

        public static readonly Type GetterType = typeof(IDebugPropertyGroupGetter);

        public static readonly Type? InternalGetterType = null;

        public static readonly Type SetterType = typeof(IDebugPropertyGroup);

        public static readonly Type? InternalSetterType = null;

        public const string FullName = "Mutagen.Bethesda.Pex.DebugPropertyGroup";

        public const string Name = "DebugPropertyGroup";

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
    public partial class DebugPropertyGroupSetterCommon
    {
        public static readonly DebugPropertyGroupSetterCommon Instance = new DebugPropertyGroupSetterCommon();

        partial void ClearPartial();
        
        public void Clear(IDebugPropertyGroup item)
        {
            ClearPartial();
            item.ObjectName = string.Empty;
            item.GroupName = string.Empty;
            item.PropertyNames.Clear();
        }
        
    }
    public partial class DebugPropertyGroupCommon
    {
        public static readonly DebugPropertyGroupCommon Instance = new DebugPropertyGroupCommon();

        public DebugPropertyGroup.Mask<bool> GetEqualsMask(
            IDebugPropertyGroupGetter item,
            IDebugPropertyGroupGetter rhs,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            var ret = new DebugPropertyGroup.Mask<bool>(false);
            ((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)item).CommonInstance()!).FillEqualsMask(
                item: item,
                rhs: rhs,
                ret: ret,
                include: include);
            return ret;
        }
        
        public void FillEqualsMask(
            IDebugPropertyGroupGetter item,
            IDebugPropertyGroupGetter rhs,
            DebugPropertyGroup.Mask<bool> ret,
            EqualsMaskHelper.Include include = EqualsMaskHelper.Include.All)
        {
            if (rhs == null) return;
            ret.ObjectName = string.Equals(item.ObjectName, rhs.ObjectName);
            ret.GroupName = string.Equals(item.GroupName, rhs.GroupName);
            ret.PropertyNames = item.PropertyNames.CollectionEqualsHelper(
                rhs.PropertyNames,
                (l, r) => string.Equals(l, r),
                include);
        }
        
        public string ToString(
            IDebugPropertyGroupGetter item,
            string? name = null,
            DebugPropertyGroup.Mask<bool>? printMask = null)
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
            IDebugPropertyGroupGetter item,
            FileGeneration fg,
            string? name = null,
            DebugPropertyGroup.Mask<bool>? printMask = null)
        {
            if (name == null)
            {
                fg.AppendLine($"DebugPropertyGroup =>");
            }
            else
            {
                fg.AppendLine($"{name} (DebugPropertyGroup) =>");
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
            IDebugPropertyGroupGetter item,
            FileGeneration fg,
            DebugPropertyGroup.Mask<bool>? printMask = null)
        {
            if (printMask?.ObjectName ?? true)
            {
                fg.AppendItem(item.ObjectName, "ObjectName");
            }
            if (printMask?.GroupName ?? true)
            {
                fg.AppendItem(item.GroupName, "GroupName");
            }
            if (printMask?.PropertyNames?.Overall ?? true)
            {
                fg.AppendLine("PropertyNames =>");
                fg.AppendLine("[");
                using (new DepthWrapper(fg))
                {
                    foreach (var subItem in item.PropertyNames)
                    {
                        fg.AppendLine("[");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendItem(subItem);
                        }
                        fg.AppendLine("]");
                    }
                }
                fg.AppendLine("]");
            }
        }
        
        #region Equals and Hash
        public virtual bool Equals(
            IDebugPropertyGroupGetter? lhs,
            IDebugPropertyGroupGetter? rhs,
            TranslationCrystal? crystal)
        {
            if (!EqualsMaskHelper.RefEquality(lhs, rhs, out var isEqual)) return isEqual;
            if ((crystal?.GetShouldTranslate((int)DebugPropertyGroup_FieldIndex.ObjectName) ?? true))
            {
                if (!string.Equals(lhs.ObjectName, rhs.ObjectName)) return false;
            }
            if ((crystal?.GetShouldTranslate((int)DebugPropertyGroup_FieldIndex.GroupName) ?? true))
            {
                if (!string.Equals(lhs.GroupName, rhs.GroupName)) return false;
            }
            if ((crystal?.GetShouldTranslate((int)DebugPropertyGroup_FieldIndex.PropertyNames) ?? true))
            {
                if (!lhs.PropertyNames.SequenceEqualNullable(rhs.PropertyNames)) return false;
            }
            return true;
        }
        
        public virtual int GetHashCode(IDebugPropertyGroupGetter item)
        {
            var hash = new HashCode();
            hash.Add(item.ObjectName);
            hash.Add(item.GroupName);
            hash.Add(item.PropertyNames);
            return hash.ToHashCode();
        }
        
        #endregion
        
        
        public object GetNew()
        {
            return DebugPropertyGroup.GetNew();
        }
        
    }
    public partial class DebugPropertyGroupSetterTranslationCommon
    {
        public static readonly DebugPropertyGroupSetterTranslationCommon Instance = new DebugPropertyGroupSetterTranslationCommon();

        #region DeepCopyIn
        public void DeepCopyIn(
            IDebugPropertyGroup item,
            IDebugPropertyGroupGetter rhs,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask,
            bool deepCopy)
        {
            if ((copyMask?.GetShouldTranslate((int)DebugPropertyGroup_FieldIndex.ObjectName) ?? true))
            {
                item.ObjectName = rhs.ObjectName;
            }
            if ((copyMask?.GetShouldTranslate((int)DebugPropertyGroup_FieldIndex.GroupName) ?? true))
            {
                item.GroupName = rhs.GroupName;
            }
            if ((copyMask?.GetShouldTranslate((int)DebugPropertyGroup_FieldIndex.PropertyNames) ?? true))
            {
                errorMask?.PushIndex((int)DebugPropertyGroup_FieldIndex.PropertyNames);
                try
                {
                    item.PropertyNames.SetTo(rhs.PropertyNames);
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
        
        public DebugPropertyGroup DeepCopy(
            IDebugPropertyGroupGetter item,
            DebugPropertyGroup.TranslationMask? copyMask = null)
        {
            DebugPropertyGroup ret = (DebugPropertyGroup)((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)item).CommonInstance()!).GetNew();
            ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                item: ret,
                rhs: item,
                errorMask: null,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            return ret;
        }
        
        public DebugPropertyGroup DeepCopy(
            IDebugPropertyGroupGetter item,
            out DebugPropertyGroup.ErrorMask errorMask,
            DebugPropertyGroup.TranslationMask? copyMask = null)
        {
            var errorMaskBuilder = new ErrorMaskBuilder();
            DebugPropertyGroup ret = (DebugPropertyGroup)((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)item).CommonInstance()!).GetNew();
            ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
                ret,
                item,
                errorMask: errorMaskBuilder,
                copyMask: copyMask?.GetCrystal(),
                deepCopy: true);
            errorMask = DebugPropertyGroup.ErrorMask.Factory(errorMaskBuilder);
            return ret;
        }
        
        public DebugPropertyGroup DeepCopy(
            IDebugPropertyGroupGetter item,
            ErrorMaskBuilder? errorMask,
            TranslationCrystal? copyMask = null)
        {
            DebugPropertyGroup ret = (DebugPropertyGroup)((DebugPropertyGroupCommon)((IDebugPropertyGroupGetter)item).CommonInstance()!).GetNew();
            ((DebugPropertyGroupSetterTranslationCommon)((IDebugPropertyGroupGetter)ret).CommonSetterTranslationInstance()!).DeepCopyIn(
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
    public partial class DebugPropertyGroup
    {
        #region Common Routing
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ILoquiRegistration ILoquiObject.Registration => DebugPropertyGroup_Registration.Instance;
        public static DebugPropertyGroup_Registration Registration => DebugPropertyGroup_Registration.Instance;
        [DebuggerStepThrough]
        protected object CommonInstance() => DebugPropertyGroupCommon.Instance;
        [DebuggerStepThrough]
        protected object CommonSetterInstance()
        {
            return DebugPropertyGroupSetterCommon.Instance;
        }
        [DebuggerStepThrough]
        protected object CommonSetterTranslationInstance() => DebugPropertyGroupSetterTranslationCommon.Instance;
        [DebuggerStepThrough]
        object IDebugPropertyGroupGetter.CommonInstance() => this.CommonInstance();
        [DebuggerStepThrough]
        object IDebugPropertyGroupGetter.CommonSetterInstance() => this.CommonSetterInstance();
        [DebuggerStepThrough]
        object IDebugPropertyGroupGetter.CommonSetterTranslationInstance() => this.CommonSetterTranslationInstance();

        #endregion

    }
}

