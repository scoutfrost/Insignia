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
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.UI;

namespace Insignia.Core.Common.Systems
{
    internal class ParticleCaller : ModSystem
    {
        public override void PostDrawTiles()
        {
            if (!Main.dedServ)
            {
                ParticleSystem.UpdateParticles();
                ParticleSystem.Draw();
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            ParticleSystem.DrawMetaBalls();
        }
    }
}
