using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using static Terraria.ModLoader.Core.TmodFile;

namespace Insignia.Core.Common.Systems
{
    internal class ShaderLoader : ModSystem
    {
        public override void Load()
        {
            List<FileEntry> shaders = [];
            MethodInfo info = typeof(Mod).GetProperty("File", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true);
            TmodFile file = (TmodFile)info.Invoke(Mod, null);

            foreach (FileEntry f in file)
            {
                if (f.Name.EndsWith(".xnb") && f.Name.StartsWith("Effects/"))
                    shaders.Add(f);
            }
            foreach (FileEntry f in shaders)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    string shaderName = f.Name.Replace(".xnb", "").Replace("Effects/", "");
                    Asset<Effect> shader = Mod.Assets.Request<Effect>("Effects/" + shaderName, AssetRequestMode.ImmediateLoad);
                    GameShaders.Misc.Add(shaderName, new(shader, shaderName + "Pass"));
                }
            }
        }
    }
}