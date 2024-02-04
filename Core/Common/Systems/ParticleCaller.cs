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