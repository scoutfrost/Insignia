using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Insignia.Core.Common.Systems;
using Insignia.Core.Particles;
using Terraria.Graphics.Shaders;
using Terraria.Graphics.Effects;
using ReLogic.Content;

namespace Insignia
{
	public class Insignia : Mod
    {
        /*public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Asset<Effect> outlineShader = Assets.Request<Effect>("Effects/OutlineShader", ReLogic.Content.AssetRequestMode.ImmediateLoad);

                GameShaders.Misc.Add("OutlineShader", new(outlineShader, "Edge"));
            }
        }*/
    }
}
