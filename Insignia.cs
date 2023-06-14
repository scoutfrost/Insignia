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

namespace Insignia
{
	public class Insignia : Mod
	{
        public override void Unload()
        {
            ParticleSystem.Unload();
        }
    }
}