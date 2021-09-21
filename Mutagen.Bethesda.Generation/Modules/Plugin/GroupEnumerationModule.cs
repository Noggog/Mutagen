using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Loqui;
using Loqui.Generation;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Generation.Modules.Plugin
{
    public class GroupEnumerationModule : GenerationModule
    {
        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (!await HasGroupsInTree(obj, false)) return;
            // GenerateClassImplementation(obj, fg);
        }

        public static void GenerateClassImplementation(ObjectGeneration obj, FileGeneration fg, bool onlyGetter = false)
        {
            fg.AppendLine("[DebuggerStepThrough]");
            fg.AppendLine(
                $"IEnumerable<{nameof(IGroupCommonGetter)}> {nameof(IGroupGetterEnumerable)}.EnumerateGroups() => this.EnumerateGroups();");
            fg.AppendLine("[DebuggerStepThrough]");
            fg.AppendLine(
                $"IEnumerable<{nameof(IGroupCommonGetter)}<TMajor>> {nameof(IGroupGetterEnumerable)}.EnumerateGroups<TMajor>(bool throwIfUnknown) => this.EnumerateMajorRecords{obj.GetGenericTypes(MaskType.Normal, "TMajor")}(throwIfUnknown: throwIfUnknown);");
            fg.AppendLine("[DebuggerStepThrough]");
            fg.AppendLine(
                $"IEnumerable<{nameof(IGroupCommonGetter)}> {nameof(IGroupGetterEnumerable)}.EnumerateGroups(Type type, bool throwIfUnknown) => this.EnumerateMajorRecords(type: type, throwIfUnknown: throwIfUnknown);");
            if (!onlyGetter)
            {
                fg.AppendLine("[DebuggerStepThrough]");
                fg.AppendLine(
                    $"IEnumerable<IGroupCommon<{nameof(IMajorRecordCommon)}>> {nameof(IGroupEnumerable)}.EnumerateGroups<TMajor>(bool throwIfUnknown) => this.EnumerateGroups<TMajor>();");
            }
        }

        public override async Task GenerateInCommonMixin(ObjectGeneration obj, FileGeneration fg)
        {
            if (!await HasGroupsInTree(obj, includeBaseClass: false)) return;
            var needsCatch = obj.GetObjectType() == ObjectType.Mod;
            string catchLine = needsCatch ? ".Catch(e => throw RecordException.Enrich(e, obj.ModKey))" : string.Empty;
            string enderSemi = needsCatch ? string.Empty : ";";
            fg.AppendLine("[DebuggerStepThrough]");
            using (var args = new FunctionWrapper(fg,
                $"public static IEnumerable<{nameof(IGroupCommonGetter)}> EnumerateGroups{obj.GetGenericTypes(MaskType.Normal)}"))
            {
                args.Wheres.AddRange(obj.GenerateWhereClauses(LoquiInterfaceType.IGetter, obj.Generics));
                args.Add($"this {obj.Interface(getter: true, internalInterface: true)} obj");
            }

            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return {obj.CommonClassInstance("obj", LoquiInterfaceType.IGetter, CommonGenerics.Class)}.EnumerateGroups",
                    suffixLine: catchLine))
                {
                    args.AddPassArg("obj");
                }
            }

            fg.AppendLine();
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg, MaskTypeSet maskTypes)
        {
            bool getter = maskTypes.Applicable(LoquiInterfaceType.IGetter, CommonGenerics.Class);
            bool setter = maskTypes.Applicable(LoquiInterfaceType.ISetter, CommonGenerics.Class);
            if (!getter && !setter) return;
            var accessor = new Accessor("obj");
            if (!await HasGroupsInTree(obj, includeBaseClass: false)) return;
            var overrideStr =
                await obj.FunctionOverride(async c => await HasGroups(c, includeBaseClass: false, includeSelf: true));

            if (getter)
            {
                using (var args = new FunctionWrapper(fg,
                    $"public{overrideStr}IEnumerable<{nameof(IGroupCommonGetter)}> EnumerateGroups"))
                {
                    args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
                }

                using (new BraceWrapper(fg))
                {
                    var fgCount = fg.Count;
                    foreach (var baseClass in obj.BaseClassTrail())
                    {
                        if (await HasGroups(baseClass, includeBaseClass: true, includeSelf: true))
                        {
                            fg.AppendLine("foreach (var item in base.EnumerateGroups(obj))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("yield return item;");
                            }

                            break;
                        }
                    }

                    foreach (var field in obj.IterateFields())
                    {
                        switch (field)
                        {
                            case LoquiType _:
                            case ContainerType _:
                            case DictType _:
                                break;
                            default:
                                continue;
                        }

                        var fieldFg = new FileGeneration();
                        if (field is LoquiType loqui)
                        {
                            var isGroup = loqui.TargetObjectGeneration?.GetObjectType() == ObjectType.Group;
                            if (isGroup
                                || await HasGroups(loqui, includeBaseClass: true))
                            {
                                var subFg = new FileGeneration();
                                var fieldAccessor = loqui.Nullable ? $"{loqui.Name}item" : $"{accessor}.{loqui.Name}";
                                await LoquiTypeHandler(subFg, fieldAccessor, loqui, generic: null, checkType: false);
                                if (subFg.Count == 0) continue;
                                if (loqui.Singleton
                                    || !loqui.Nullable)
                                {
                                    fieldFg.AppendLines(subFg);
                                }
                                else
                                {
                                    fieldFg.AppendLine($"if ({accessor}.{loqui.Name} is {{}} {loqui.Name}item)");
                                    using (new BraceWrapper(fieldFg))
                                    {
                                        fieldFg.AppendLines(subFg);
                                    }
                                }
                            }
                        }
                        else if (field is ContainerType cont)
                        {
                            if (!(cont.SubTypeGeneration is LoquiType contLoqui)) continue;
                            var isGroup = contLoqui.TargetObjectGeneration?.GetObjectType() == ObjectType.Group;
                            if (isGroup
                                || await HasGroups(contLoqui, includeBaseClass: true))
                            {
                                fieldFg.AppendLine(
                                    $"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".EmptyIfNull()" : null)})");
                                using (new BraceWrapper(fieldFg))
                                {
                                    await LoquiTypeHandler(fieldFg, $"subItem", contLoqui, generic: null,
                                        checkType: false);
                                }
                            }
                        }
                        else if (field is DictType dict)
                        {
                            if (dict.Mode != DictMode.KeyedValue) continue;
                            if (!(dict.ValueTypeGen is LoquiType dictLoqui)) continue;
                            var isGroup = dictLoqui.TargetObjectGeneration?.GetObjectType() == ObjectType.Group;
                            if (isGroup
                                || await HasGroups(dictLoqui, includeBaseClass: true))
                            {
                                fieldFg.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items)");
                                using (new BraceWrapper(fieldFg))
                                {
                                    await LoquiTypeHandler(fieldFg, $"subItem", dictLoqui, generic: null,
                                        checkType: false);
                                }
                            }
                        }

                        if (fieldFg.Count > 0)
                        {
                            if (field.Nullable)
                            {
                                fg.AppendLine(
                                    $"if ({field.NullableAccessor(getter: true, Accessor.FromType(field, accessor.ToString()))})");
                            }

                            using (new BraceWrapper(fg, doIt: field.Nullable))
                            {
                                fg.AppendLines(fieldFg);
                            }
                        }
                    }

                    if (fgCount == fg.Count)
                    {
                        fg.AppendLine("yield break;");
                    }
                }

                fg.AppendLine();
            }

            return;

            // Generate base overrides  
            foreach (var baseClass in obj.BaseClassTrail())
            {
                if (await HasGroups(baseClass, includeBaseClass: true, includeSelf: true))
                {
                    using (var args = new FunctionWrapper(fg,
                        $"public override IEnumerable<{nameof(IMajorRecordCommon)}{(getter ? "Getter" : null)}> EnumerateMajorRecords"))
                    {
                        args.Add($"{baseClass.Interface(getter: getter)} obj");
                    }

                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            "EnumerateMajorRecords"))
                        {
                            args.Add($"({obj.Interface(getter: getter)})obj");
                        }
                    }

                    fg.AppendLine();
                }
            }

            using (var args = new FunctionWrapper(fg,
                $"public{overrideStr}IEnumerable<{nameof(IMajorRecordCommonGetter)}> EnumeratePotentiallyTypedMajorRecords"))
            {
                args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
                args.Add($"Type? type");
                args.Add($"bool throwIfUnknown");
            }

            using (new BraceWrapper(fg))
            {
                fg.AppendLine("if (type == null) return EnumerateMajorRecords(obj);");
                fg.AppendLine("return EnumerateMajorRecords(obj, type, throwIfUnknown);");
            }

            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public{overrideStr}IEnumerable<{nameof(IMajorRecordCommonGetter)}> EnumerateMajorRecords"))
            {
                args.Add($"{obj.Interface(getter: getter, internalInterface: true)} obj");
                args.Add($"Type type");
                args.Add($"bool throwIfUnknown");
            }

            using (new BraceWrapper(fg))
            {
                if (setter)
                {
                    fg.AppendLine(
                        $"foreach (var item in {obj.CommonClass(LoquiInterfaceType.IGetter, CommonGenerics.Class)}.Instance.EnumerateMajorRecords(obj, type, throwIfUnknown))");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("yield return item;");
                    }
                }
                else
                {
                    var fgCount = fg.Count;
                    foreach (var baseClass in obj.BaseClassTrail())
                    {
                        if (await HasGroups(baseClass, includeBaseClass: true, includeSelf: true))
                        {
                            fg.AppendLine("foreach (var item in base.EnumerateMajorRecords<TMajor>(obj))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("yield return item;");
                            }

                            break;
                        }
                    }

                    fg.AppendLine("switch (type.Name)");
                    using (new BraceWrapper(fg))
                    {
                        var gameCategory = obj.GetObjectData().GameCategory;
                        fg.AppendLine($"case \"{nameof(IMajorRecordCommon)}\":");
                        fg.AppendLine($"case \"{nameof(IMajorRecord)}\":");
                        fg.AppendLine($"case \"{nameof(MajorRecord)}\":");
                        if (gameCategory != null)
                        {
                            fg.AppendLine($"case \"I{gameCategory}MajorRecord\":");
                            fg.AppendLine($"case \"{gameCategory}MajorRecord\":");
                        }

                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine(
                                $"if (!{obj.RegistrationName}.SetterType.IsAssignableFrom(obj.GetType())) yield break;");
                            fg.AppendLine("foreach (var item in this.EnumerateMajorRecords(obj))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("yield return item;");
                            }

                            fg.AppendLine("yield break;");
                        }

                        fg.AppendLine($"case \"{nameof(IMajorRecordGetter)}\":");
                        fg.AppendLine($"case \"{nameof(IMajorRecordCommonGetter)}\":");
                        if (gameCategory != null)
                        {
                            fg.AppendLine($"case \"I{gameCategory}MajorRecordGetter\":");
                        }

                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine("foreach (var item in this.EnumerateMajorRecords(obj))");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("yield return item;");
                            }

                            fg.AppendLine("yield break;");
                        }

                        Dictionary<object, FileGeneration> generationDict = new Dictionary<object, FileGeneration>();
                        foreach (var field in obj.IterateFields())
                        {
                            FileGeneration fieldGen;
                            if (field is LoquiType loqui)
                            {
                                if (loqui.TargetObjectGeneration.IsListGroup()) continue;
                                var isMajorRecord = loqui.TargetObjectGeneration != null &&
                                                    await loqui.TargetObjectGeneration.IsMajorRecord();
                                if (!isMajorRecord
                                    && !await HasGroups(loqui, includeBaseClass: true))
                                {
                                    continue;
                                }

                                if (loqui.TargetObjectGeneration.GetObjectType() == ObjectType.Group)
                                {
                                    fieldGen = generationDict.GetOrAdd(loqui.GetGroupTarget());
                                }
                                else
                                {
                                    fieldGen = generationDict.GetOrAdd(((object)loqui?.TargetObjectGeneration) ??
                                                                       loqui);
                                }
                            }
                            else if (field is ContainerType cont)
                            {
                                if (!(cont.SubTypeGeneration is LoquiType contLoqui)) continue;
                                if (contLoqui.RefType == LoquiType.LoquiRefType.Generic)
                                {
                                    fieldGen = generationDict.GetOrAdd("default:");
                                }
                                else
                                {
                                    fieldGen = generationDict.GetOrAdd(((object)contLoqui?.TargetObjectGeneration) ??
                                                                       contLoqui);
                                }
                            }
                            else if (field is DictType dict)
                            {
                                if (dict.Mode != DictMode.KeyedValue) continue;
                                if (!(dict.ValueTypeGen is LoquiType dictLoqui)) continue;
                                if (dictLoqui.RefType == LoquiType.LoquiRefType.Generic)
                                {
                                    fieldGen = generationDict.GetOrAdd("default:");
                                }
                                else
                                {
                                    fieldGen = generationDict.GetOrAdd(((object)dictLoqui?.TargetObjectGeneration) ??
                                                                       dictLoqui);
                                }
                            }
                            else
                            {
                                continue;
                            }

                            await ApplyIterationLines(field, fieldGen, accessor, getter);
                        }

                        bool doAdditionlDeepLogic = obj.Name != "ListGroup";

                        if (doAdditionlDeepLogic)
                        {
                            // var deepRecordMapping = await FindDeepRecords(obj);
                            // foreach (var deepRec in deepRecordMapping)
                            // {
                            //     FileGeneration deepFg = generationDict.GetOrAdd(deepRec.Key);
                            //     foreach (var field in deepRec.Value)
                            //     {
                            //         await ApplyIterationLines(field, deepFg, accessor, getter, targetObj: deepRec.Key);
                            //     }
                            // }

                            HashSet<string> blackList = new HashSet<string>();
                            foreach (var kv in generationDict)
                            {
                                switch (kv.Key)
                                {
                                    case LoquiType loqui:
                                        if (loqui.RefType == LoquiType.LoquiRefType.Generic)
                                        {
                                            // Handled in default case  
                                            continue;
                                        }
                                        else
                                        {
                                            fg.AppendLine($"case \"{loqui.Interface(getter: true)}\":");
                                            fg.AppendLine($"case \"{loqui.Interface(getter: false)}\":");
                                            if (loqui.HasInternalGetInterface)
                                            {
                                                fg.AppendLine(
                                                    $"case \"{loqui.Interface(getter: true, internalInterface: true)}\":");
                                            }

                                            if (loqui.HasInternalSetInterface)
                                            {
                                                fg.AppendLine(
                                                    $"case \"{loqui.Interface(getter: false, internalInterface: true)}\":");
                                            }

                                            if (loqui.RefType == LoquiType.LoquiRefType.Interface)
                                            {
                                                blackList.Add(loqui.SetterInterface);
                                            }
                                        }

                                        break;
                                    case ObjectGeneration targetObj:
                                        fg.AppendLine($"case \"{targetObj.ObjectName}\":");
                                        fg.AppendLine($"case \"{targetObj.Interface(getter: true)}\":");
                                        fg.AppendLine($"case \"{targetObj.Interface(getter: false)}\":");
                                        if (targetObj.HasInternalGetInterface)
                                        {
                                            fg.AppendLine(
                                                $"case \"{targetObj.Interface(getter: true, internalInterface: true)}\":");
                                        }

                                        if (targetObj.HasInternalSetInterface)
                                        {
                                            fg.AppendLine(
                                                $"case \"{targetObj.Interface(getter: false, internalInterface: true)}\":");
                                        }

                                        break;
                                    case string str:
                                        if (str != "default:")
                                        {
                                            throw new NotImplementedException();
                                        }

                                        continue;
                                    default:
                                        throw new NotImplementedException();
                                }

                                using (new DepthWrapper(fg))
                                {
                                    fg.AppendLines(kv.Value);
                                    fg.AppendLine("yield break;");
                                }
                            }

                            // Generate for major record marker interfaces 
                            if (LinkInterfaceModule.ObjectMappings.TryGetValue(obj.ProtoGen.Protocol, out var interfs))
                            {
                                foreach (var interf in interfs)
                                {
                                    if (blackList.Contains(interf.Key)) continue;
                                    FileGeneration subFg = new FileGeneration();
                                    HashSet<ObjectGeneration> passedObjects = new HashSet<ObjectGeneration>();
                                    HashSet<TypeGeneration> deepObjects = new HashSet<TypeGeneration>();
                                    foreach (var subObj in interf.Value)
                                    {
                                        var grup = obj.Fields
                                            .WhereCastable<TypeGeneration, GroupType>()
                                            .Where(g => g.GetGroupTarget() == subObj)
                                            .FirstOrDefault();

                                        if (grup != null)
                                        {
                                            subFg.AppendLine(
                                                $"foreach (var item in EnumerateMajorRecords({accessor}, typeof({grup.GetGroupTarget().Interface(getter: true)}), throwIfUnknown: throwIfUnknown))");
                                            using (new BraceWrapper(subFg))
                                            {
                                                subFg.AppendLine("yield return item;");
                                            }

                                            passedObjects.Add(grup.GetGroupTarget());
                                        }
                                        // else if (deepRecordMapping.TryGetValue(subObj, out var deepRec))
                                        // {
                                        //     foreach (var field in deepRec)
                                        //     {
                                        //         deepObjects.Add(field);
                                        //     }
                                        // }
                                    }

                                    foreach (var deepObj in deepObjects)
                                    {
                                        await ApplyIterationLines(deepObj, subFg, accessor, getter,
                                            blackList: passedObjects);
                                    }

                                    if (!subFg.Empty)
                                    {
                                        fg.AppendLine($"case \"{interf.Key}\":");
                                        using (new BraceWrapper(fg))
                                        {
                                            fg.AppendLine(
                                                $"if (!{obj.RegistrationName}.SetterType.IsAssignableFrom(obj.GetType())) yield break;");
                                            fg.AppendLines(subFg);
                                            fg.AppendLine("yield break;");
                                        }

                                        fg.AppendLine($"case \"{interf.Key}Getter\":");
                                        using (new BraceWrapper(fg))
                                        {
                                            fg.AppendLines(subFg);
                                            fg.AppendLine("yield break;");
                                        }
                                    }
                                }
                            }
                        }

                        fg.AppendLine("default:");
                        using (new DepthWrapper(fg))
                        {
                            if (generationDict.TryGetValue("default:", out var gen))
                            {
                                fg.AppendLines(gen);
                                fg.AppendLine("yield break;");
                            }
                            else
                            {
                                fg.AppendLine("if (throwIfUnknown)");
                                using (new BraceWrapper(fg))
                                {
                                    fg.AppendLine(
                                        "throw new ArgumentException($\"Unknown major record type: {type}\");");
                                }

                                fg.AppendLine($"else");
                                using (new BraceWrapper(fg))
                                {
                                    fg.AppendLine("yield break;");
                                }
                            }
                        }
                    }
                }
            }

            fg.AppendLine();

            // Generate base overrides  
            foreach (var baseClass in obj.BaseClassTrail())
            {
                if (await HasGroups(baseClass, includeBaseClass: true, includeSelf: true))
                {
                    using (var args = new FunctionWrapper(fg,
                        $"public override IEnumerable<TMajor> EnumerateMajorRecords<TMajor>"))
                    {
                        args.Add($"{baseClass.Interface(getter: getter)} obj");
                        args.Wheres.Add($"where TMajor : {nameof(IMajorRecordCommon)}{(getter ? "Getter" : null)}");
                    }

                    using (new BraceWrapper(fg))
                    {
                        using (var args = new ArgsWrapper(fg,
                            "EnumerateMajorRecords<TMajor>"))
                        {
                            args.Add($"({obj.Interface(getter: getter)})obj");
                        }
                    }

                    fg.AppendLine();
                }
            }
        }

        async Task ApplyIterationLines(
            TypeGeneration field,
            FileGeneration fieldGen,
            Accessor accessor,
            bool getter,
            ObjectGeneration targetObj = null,
            HashSet<ObjectGeneration> blackList = null)
        {
            if (field is GroupType group)
            {
                if (blackList?.Contains(group.GetGroupTarget()) ?? false) return;
                fieldGen.AppendLine(
                    $"foreach (var item in obj.{field.Name}.EnumerateMajorRecords(type, throwIfUnknown: throwIfUnknown))");
                using (new BraceWrapper(fieldGen))
                {
                    fieldGen.AppendLine("yield return item;");
                }
            }
            else if (field is LoquiType loqui)
            {
                if (blackList?.Contains(loqui.TargetObjectGeneration) ?? false) return;
                var fieldAccessor =
                    loqui.Nullable ? $"{targetObj?.ObjectName}{loqui.Name}item" : $"{accessor}.{loqui.Name}";
                if (loqui.TargetObjectGeneration.GetObjectType() == ObjectType.Group)
                {
                    // List groups 
                    fieldGen.AppendLine(
                        $"foreach (var item in obj.{field.Name}.EnumerateMajorRecords(type, throwIfUnknown: throwIfUnknown))");
                    using (new BraceWrapper(fieldGen))
                    {
                        fieldGen.AppendLine("yield return item;");
                    }

                    return;
                }

                var subFg = new FileGeneration();
                await LoquiTypeHandler(subFg, fieldAccessor, loqui, generic: "TMajor", checkType: false,
                    targetObj: targetObj);
                if (subFg.Count == 0) return;
                if (loqui.Singleton
                    || !loqui.Nullable)
                {
                    fieldGen.AppendLines(subFg);
                }
                else
                {
                    using (new BraceWrapper(fieldGen))
                    {
                        fieldGen.AppendLine($"if ({accessor}.{loqui.Name} is {{}} {fieldAccessor})");
                        using (new BraceWrapper(fieldGen))
                        {
                            fieldGen.AppendLines(subFg);
                        }
                    }
                }
            }
            else if (field is ContainerType cont)
            {
                if (!(cont.SubTypeGeneration is LoquiType contLoqui)) return;
                if (contLoqui.RefType == LoquiType.LoquiRefType.Generic)
                {
                    fieldGen.AppendLine($"foreach (var item in obj.{field.Name})");
                    using (new BraceWrapper(fieldGen))
                    {
                        if (await contLoqui.TargetObjectGeneration.IsMajorRecord())
                        {
                            fieldGen.AppendLine($"if (type.IsAssignableFrom(typeof({contLoqui.GenericDef.Name})))");
                            using (new BraceWrapper(fieldGen))
                            {
                                fieldGen.AppendLine($"yield return ({nameof(IMajorRecordCommonGetter)})item;");
                            }
                        }

                        fieldGen.AppendLine(
                            $"foreach (var subItem in item.EnumerateMajorRecords(type, throwIfUnknown: throwIfUnknown))");
                        using (new BraceWrapper(fieldGen))
                        {
                            fieldGen.AppendLine($"yield return subItem;");
                        }
                    }
                }
                else
                {
                    var isMajorRecord = contLoqui.TargetObjectGeneration != null &&
                                        await contLoqui.TargetObjectGeneration.IsMajorRecord();
                    if (isMajorRecord
                        || await HasGroups(contLoqui, includeBaseClass: true))
                    {
                        fieldGen.AppendLine(
                            $"foreach (var subItem in {accessor}.{field.Name}{(field.Nullable ? ".EmptyIfNull()" : null)})");
                        using (new BraceWrapper(fieldGen))
                        {
                            await LoquiTypeHandler(fieldGen, $"subItem", contLoqui, generic: "TMajor", checkType: true);
                        }
                    }
                }
            }
            else if (field is DictType dict)
            {
                if (dict.Mode != DictMode.KeyedValue) return;
                if (!(dict.ValueTypeGen is LoquiType dictLoqui)) return;
                if (dictLoqui.RefType == LoquiType.LoquiRefType.Generic)
                {
                    fieldGen.AppendLine(
                        $"var assignable = type.IsAssignableFrom(typeof({dictLoqui.GenericDef.Name}));");
                    fieldGen.AppendLine($"foreach (var item in obj.{field.Name}.Items)");
                    using (new BraceWrapper(fieldGen))
                    {
                        fieldGen.AppendLine($"if (assignable)");
                        using (new BraceWrapper(fieldGen))
                        {
                            fieldGen.AppendLine($"yield return item;");
                        }

                        fieldGen.AppendLine(
                            $"foreach (var subItem in item.EnumerateMajorRecords(type, throwIfUnknown: false))");
                        using (new BraceWrapper(fieldGen))
                        {
                            fieldGen.AppendLine($"yield return subItem;");
                        }
                    }
                }
                else
                {
                    var isMajorRecord = dictLoqui.TargetObjectGeneration != null &&
                                        await dictLoqui.TargetObjectGeneration.IsMajorRecord();
                    if (isMajorRecord
                        || await HasGroups(dictLoqui, includeBaseClass: true))
                    {
                        fieldGen.AppendLine($"foreach (var subItem in {accessor}.{field.Name}.Items)");
                        using (new BraceWrapper(fieldGen))
                        {
                            await LoquiTypeHandler(fieldGen, $"subItem", dictLoqui, generic: "TMajor",
                                checkType: false);
                        }
                    }
                }
            }
        }

        public static async Task<bool> HasGroupsInTree(ObjectGeneration obj, bool includeBaseClass,
            GenericSpecification? specifications = null)
        {
            if (await HasGroups(obj, includeBaseClass: includeBaseClass, includeSelf: false,
                specifications: specifications)) return true;
            // If no, check subclasses  
            foreach (var inheritingObject in await obj.InheritingObjects())
            {
                if (await HasGroupsInTree(inheritingObject, includeBaseClass: false, specifications: specifications))
                    return true;
            }

            return false;
        }

        public static async Task<bool> HasGroups(LoquiType loqui, bool includeBaseClass,
            GenericSpecification? specifications = null, bool includeSelf = true)
        {
            if (loqui.TargetObjectGeneration != null)
            {
                if (includeSelf && loqui.TargetObjectGeneration.GetObjectType() == ObjectType.Group) return true;
                return await HasGroupsInTree(loqui.TargetObjectGeneration, includeBaseClass,
                    loqui.GenericSpecification);
            }
            else if (specifications != null)
            {
                foreach (var target in specifications.Specifications.Values)
                {
                    if (!ObjectNamedKey.TryFactory(target, out var key)) continue;
                    var specObj = loqui.ObjectGen.ProtoGen.Gen.ObjectGenerationsByObjectNameKey[key];
                    if (specObj.GetObjectType() == ObjectType.Group) return true;
                    return await HasGroupsInTree(specObj, includeBaseClass);
                }
            }

            return false;
        }

        public static async Task<bool> HasGroups(ObjectGeneration obj, bool includeBaseClass, bool includeSelf,
            GenericSpecification? specifications = null)
        {
            foreach (var field in obj.IterateFields(includeBaseClass: includeBaseClass))
            {
                if (field is LoquiType loqui)
                {
                    if (includeSelf
                        && loqui.TargetObjectGeneration != null
                        && loqui.TargetObjectGeneration.GetObjectType() == ObjectType.Group)
                    {
                        return true;
                    }

                    if (await HasGroups(loqui, includeBaseClass, specifications)) return true;
                }
                else if (field is ContainerType cont)
                {
                    if (cont.SubTypeGeneration is LoquiType contLoqui)
                    {
                        if (await HasGroups(contLoqui, includeBaseClass, specifications)) return true;
                    }
                }
                else if (field is DictType dict)
                {
                    if (dict.ValueTypeGen is LoquiType valLoqui)
                    {
                        if (await HasGroups(valLoqui, includeBaseClass, specifications)) return true;
                    }

                    if (dict.KeyTypeGen is LoquiType keyLoqui)
                    {
                        if (await HasGroups(keyLoqui, includeBaseClass, specifications)) return true;
                    }
                }
            }

            return false;
        }

        private async Task LoquiTypeHandler(
            FileGeneration fg,
            Accessor loquiAccessor,
            LoquiType loquiType,
            string? generic,
            bool checkType,
            ObjectGeneration? targetObj = null)
        {
            // ToDo  
            // Quick hack.  Real solution should use reflection to investigate the interface  
            if (loquiType.RefType == LoquiType.LoquiRefType.Interface)
            {
                if (checkType)
                {
                    fg.AppendLine($"if (type.IsAssignableFrom({loquiAccessor}.GetType()))");
                }

                using (new BraceWrapper(fg, doIt: checkType))
                {
                    fg.AppendLine($"yield return {loquiAccessor};");
                }

                return;
            }

            if (loquiType.TargetObjectGeneration != null
                && await loquiType.TargetObjectGeneration.IsMajorRecord()
                && (targetObj == null || targetObj == loquiType.TargetObjectGeneration))
            {
                if (checkType)
                {
                    fg.AppendLine($"if (type.IsAssignableFrom({loquiAccessor}.GetType()))");
                }

                using (new BraceWrapper(fg, doIt: checkType))
                {
                    fg.AppendLine($"yield return {loquiAccessor};");
                }
            }

            if (!await HasGroups(loquiType, includeBaseClass: true))
            {
                return;
            }

            fg.AppendLine($"yield return {loquiAccessor};");
        }
    }
}