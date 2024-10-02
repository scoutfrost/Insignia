using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Insignia.Core.ModPlayers
{
    internal class InsigniaPlayer : ModPlayer
    {
        public bool inColdBiome;
        public float BleedDamageMultiplier = 1;
        public float BleedBuildupMultiplier = 1;
        public bool BleedProc;
        public int immuneTimeAdd;
        public override void ResetEffects()
        {
            if (immuneTimeAdd > 0)
            {
                Player.immuneNoBlink = true;
                Player.immune = true;
                immuneTimeAdd--;
            }
            BleedProc = false;
            BleedBuildupMultiplier = 1;
            BleedDamageMultiplier = 1;
        }
    }
}
