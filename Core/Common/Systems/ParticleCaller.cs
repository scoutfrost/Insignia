using Insignia.Core.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Insignia.Core.Common.Systems
{
    internal class ParticleCaller : ModSystem
    {
        public override void PostDrawTiles()
        {
            if (!Main.dedServ)
            {
                ParticleSystem.Draw();
                ParticleSystem.UpdateParticles();
            }
        }
    }
}
