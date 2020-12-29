using Loqui;
using Loqui.Generation;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Generation
{
    public class GameCategoryExtensionsModule : GenerationModule
    {
        public override async Task FinalizeGeneration(ProtocolGeneration proto)
        {
            await base.FinalizeGeneration(proto);
            if (proto.Protocol.Namespace != "Bethesda") return;

            FileGeneration fg = new FileGeneration();
            fg.AppendLine("using System;");
            fg.AppendLine();

            using (new NamespaceWrapper(fg, "Mutagen.Bethesda"))
            {
                using (var cl = new ClassWrapper(fg, "GameCategoryHelper"))
                {
                    cl.Partial = true;
                    cl.Static = true;
                }
                using (new BraceWrapper(fg))
                {
                    using (var args = new FunctionWrapper(fg,
                        $"public static {nameof(GameCategory)} FromModType<TMod>"))
                    {
                        args.Wheres.Add($"where TMod : {nameof(IModGetter)}");
                    }
                    using (new BraceWrapper(fg))
                    {
                        fg.AppendLine("switch (typeof(TMod).Name)");
                        using (new BraceWrapper(fg))
                        {
                            foreach (var cat in EnumExt.GetValues<GameCategory>())
                            {
                                fg.AppendLine($"case \"I{cat}Mod\":");
                                fg.AppendLine($"case \"I{cat}GetterMod\":");
                                using (new DepthWrapper(fg))
                                {
                                    fg.AppendLine($"return {nameof(GameCategory)}.{cat};");
                                }
                            }

                            fg.AppendLine("default:");
                            using (new BraceWrapper(fg))
                            {
                                fg.AppendLine("throw new ArgumentException($\"Unknown game type for: {typeof(TMod).Name}\");");
                            }
                        }
                    }
                }
            }

            var path = Path.Combine(proto.DefFileLocation.FullName, "../Extensions", $"GameCategoryHelper{Loqui.Generation.Constants.AutogeneratedMarkerString}.cs");
            fg.Generate(path);
            proto.GeneratedFiles.Add(path, ProjItemType.Compile);
        }
    }
}