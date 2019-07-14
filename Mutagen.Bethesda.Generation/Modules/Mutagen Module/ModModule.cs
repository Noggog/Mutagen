﻿using Loqui;
using Loqui.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class ModModule : GenerationModule
    {
        public override async Task<IEnumerable<string>> RequiredUsingStatements(ObjectGeneration obj)
        {
            if (obj.GetObjectData().ObjectType != ObjectType.Mod) return EnumerableExt<string>.Empty;
            return new string[]
            {
                "DynamicData",
                "CSharpExt.Rx"
            };
        }
        
        public override async Task GenerateInClass(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectData().ObjectType != ObjectType.Mod) return;
            if (!obj.Node.TryGetAttribute<GameMode>(Mutagen.Bethesda.Generation.Constants.GAME_MODE, out var gameMode))
            {
                throw new ArgumentException("Mod object must specify game mode enum.");
            }
            obj.CustomData[Mutagen.Bethesda.Generation.Constants.GAME_MODE] = gameMode;
            fg.AppendLine($"public {nameof(GameMode)} GameMode => {nameof(GameMode)}.{gameMode};");
            fg.AppendLine($"IReadOnlyCache<T, {nameof(FormKey)}> {nameof(IModGetter)}.{nameof(IModGetter.GetGroupGetter)}<T>() => this.GetGroupGetter<T>();");
            fg.AppendLine($"ISourceCache<T, {nameof(FormKey)}> {nameof(IMod)}.{nameof(IMod.GetGroup)}<T>() => this.GetGroup<T>();");
            fg.AppendLine($"private ISourceCache<IMajorRecord, FormKey> _majorRecords = new SourceCache<IMajorRecord, FormKey>(m => m.FormKey);");
            fg.AppendLine($"public IObservableCache<IMajorRecord, FormKey> MajorRecords => _majorRecords;");
            fg.AppendLine($"public IMajorRecord this[FormKey id]");
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("get => MajorRecords.Lookup(id).Value;");
                fg.AppendLine("set => SetMajorRecord(id, value);");
            }

            using (var args = new FunctionWrapper(fg,
                "protected void SetMajorRecord"))
            {
                args.Add("FormKey id");
                args.Add("IMajorRecord record");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("switch (record)");
                using (new BraceWrapper(fg))
                {
                    foreach (var field in obj.IterateFields())
                    {
                        if (!(field is LoquiType loqui)) continue;
                        if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                        if (!loqui.TryGetSpecificationAsObject("T", out var subObj))
                        {
                            throw new ArgumentException();
                        }
                        if (!await subObj.IsMajorRecord()) continue;
                        fg.AppendLine($"case {subObj.Name} {field.Name.ToLower()}:");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine($"{loqui.ProtectedName}.Items.Set({field.Name.ToLower()});");
                            fg.AppendLine($"break;");
                        }
                    }
                    fg.AppendLine("default:");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine($"throw new ArgumentException($\"Unknown settable MajorRecord type: {{record?.GetType()}}\");");
                    }
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                "public void AddRecords"))
            {
                args.Add($"{obj.Name} rhsMod");
                args.Add($"GroupMask mask = null");
            }
            using (new BraceWrapper(fg))
            {
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                    fg.AppendLine($"if (mask?.{field.Name} ?? true)");
                    using (new BraceWrapper(fg))
                    {
                        if (loqui.TargetObjectGeneration.Name == "Group")
                        {
                            fg.AppendLine($"this.{field.Name}.Items.Set(rhsMod.{field.Name}.Items.Items);");
                        }
                        else
                        {
                            fg.AppendLine($"if (rhsMod.{field.Name}.Items.Count > 0)");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("throw new NotImplementedException(\"Cell additions need implementing\");");
                            }
                        }
                    }
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                $"public Dictionary<FormKey, {nameof(IMajorRecordCommon)}> CopyInDuplicate"))
            {
                args.Add($"{obj.Name} rhs");
                args.Add($"GroupMask mask = null");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine($"var duppedRecords = new List<({nameof(IMajorRecordCommon)} Record, FormKey OriginalFormKey)>();");
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                    fg.AppendLine($"if (mask?.{field.Name} ?? true)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"this.{field.Name}.Items.{(loqui.TargetObjectGeneration.Name == "Group" ? "Set" : "AddRange")}(");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine($"rhs.{field.Name}.Items.Items");
                            using (new DepthWrapper(fg))
                            {
                                fg.AppendLine($".Select(i => i.Duplicate(this.GetNextFormKey, duppedRecords))");
                                fg.AppendLine($".Cast<{loqui.GetGroupTarget().Name}>());");
                            }
                        }
                    }
                }
                fg.AppendLine($"Dictionary<FormKey, {nameof(IMajorRecordCommon)}> router = new Dictionary<FormKey, {nameof(IMajorRecordCommon)}>();");
                fg.AppendLine($"router.Set(duppedRecords.Select(dup => new KeyValuePair<FormKey, {nameof(IMajorRecordCommon)}>(dup.OriginalFormKey, dup.Record)));");
                fg.AppendLine("foreach (var rec in router.Values)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"foreach (var link in rec.Links)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"if (link.FormKey.ModKey == rhs.ModKey");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine($"&& router.TryGetValue(link.FormKey, out var duppedRecord))");
                        }
                        using (new BraceWrapper(fg))
                        {
                            fg.AppendLine($"link.FormKey = duppedRecord.FormKey;");
                        }
                    }
                }
                fg.AppendLine("foreach (var rec in router.Values)");
                using (new BraceWrapper(fg))
                {
                    fg.AppendLine($"foreach (var link in rec.Links)");
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine($"link.Link(modList: null, sourceMod: this);");
                    }
                }
                fg.AppendLine($"return router;");
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                "public void SyncRecordCount"))
            {
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("this.ModHeader.Stats.NumRecords = GetRecordCount();");
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                "public int GetRecordCount"))
            {
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("int count = this.MajorRecords.Count;");
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                    fg.AppendLine($"count += {field.Name}.Items.Count > 0 ? 1 : 0;");
                }
                fg.AppendLine("GetCustomRecordCount((customCount) => count += customCount);");
                fg.AppendLine("return count;");
            }
            fg.AppendLine();

            fg.AppendLine("partial void GetCustomRecordCount(Action<int> setter);");
            fg.AppendLine();

            await base.GenerateInClass(obj, fg);
        }

        public override async Task GenerateInCtor(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            await obj.FieldCtorsGeneratedSignal;
            using (var args = new ArgsWrapper(fg,
                $"Observable.Merge")
            {
                SemiColon = false
            })
            {
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                    if (!loqui.TryGetSpecificationAsObject("T", out var subObj))
                    {
                        throw new ArgumentException();
                    }
                    if (await subObj.IsMajorRecord())
                    {
                        args.Add($"{field.ProtectedName}.Items.Connect().Transform<IMajorRecord, {subObj.Name}, FormKey>((i) => i)");
                    }
                }
            }
            using (new DepthWrapper(fg))
            {
                fg.AppendLine(".PopulateInto(_majorRecords);");
            }
            await base.GenerateInCtor(obj, fg);
        }

        public override async Task GenerateInVoid(ObjectGeneration obj, FileGeneration fg)
        {
            if (obj.GetObjectType() != ObjectType.Mod) return;
            fg.AppendLine("public class GroupMask");
            using (new BraceWrapper(fg))
            {
                foreach (var field in obj.IterateFields())
                {
                    if (!(field is LoquiType loqui)) continue;
                    if (loqui.TargetObjectGeneration == null) continue;
                    if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                    fg.AppendLine($"public bool {loqui.Name};");
                }

                fg.AppendLine("public GroupMask()");
                using (new BraceWrapper(fg))
                {
                }

                fg.AppendLine("public GroupMask(bool defaultValue)");
                using (new BraceWrapper(fg))
                {
                    foreach (var field in obj.IterateFields())
                    {
                        if (!(field is LoquiType loqui)) continue;
                        if (loqui.TargetObjectGeneration == null) continue;
                        if (loqui.TargetObjectGeneration.GetObjectType() != ObjectType.Group) continue;
                        fg.AppendLine($"{loqui.Name} = defaultValue;");
                    }
                }
            }
        }

        public override async Task GenerateInCommonMixin(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInCommonMixin(obj, fg);

            if (obj.GetObjectType() != ObjectType.Mod) return;
            using (var args = new FunctionWrapper(fg,
                "public static IReadOnlyCache<T, FormKey> GetGroupGetter<T>",
                wheres: $"where T : {nameof(IMajorRecordInternalGetter)}"))
            {
                args.Add($"this {obj.Interface(getter: true)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return (IReadOnlyCache<T, FormKey>){obj.CommonClassInstance("obj")}.GetGroup<T>"))
                {
                    args.AddPassArg("obj");
                }
            }
            fg.AppendLine();

            using (var args = new FunctionWrapper(fg,
                "public static ISourceCache<T, FormKey> GetGroup<T>",
                wheres: $"where T : {nameof(IMajorRecordInternal)}"))
            {
                args.Add($"this {obj.Interface(getter: false)} obj");
            }
            using (new BraceWrapper(fg))
            {
                using (var args = new ArgsWrapper(fg,
                    $"return (ISourceCache<T, FormKey>){obj.CommonClassInstance("obj")}.GetGroup<T>"))
                {
                    args.AddPassArg("obj");
                }
            }
        }

        public override async Task GenerateInCommon(ObjectGeneration obj, FileGeneration fg)
        {
            await base.GenerateInCommon(obj, fg);

            if (obj.GetObjectType() != ObjectType.Mod) return;
            using (var args = new FunctionWrapper(fg,
                "public object GetGroup<T>",
                wheres: $"where T : {nameof(IMajorRecordInternalGetter)}"))
            {
                args.Add($"{obj.Interface(getter: true)} obj");
            }
            using (new BraceWrapper(fg))
            {
                fg.AppendLine("switch (typeof(T).Name)");
                using (new BraceWrapper(fg))
                {
                    foreach (var field in obj.IterateFields())
                    {
                        if (!(field is LoquiType loqui)) continue;
                        if (loqui.TargetObjectGeneration?.GetObjectData().ObjectType != ObjectType.Group) continue;
                        if (!loqui.TryGetSpecificationAsObject("T", out var subObj))
                        {
                            throw new ArgumentException();
                        }
                        fg.AppendLine($"case \"{subObj.Name}\":");
                        fg.AppendLine($"case \"{subObj.Interface(getter: true)}\":");
                        fg.AppendLine($"case \"{subObj.Interface(getter: false)}\":");
                        using (new DepthWrapper(fg))
                        {
                            fg.AppendLine($"return obj.{field.Name}.Items;");
                        }
                    }
                    fg.AppendLine("default:");
                    using (new DepthWrapper(fg))
                    {
                        fg.AppendLine("throw new ArgumentException($\"Unknown group type: {typeof(T)}\");");
                    }
                }
            }
            fg.AppendLine();
        }
    }
}
