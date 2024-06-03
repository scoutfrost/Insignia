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
        public float BleedDamageMultiplier = 1;
        public float BleedBuildupMultiplier = 1;
        public bool BleedProc;
        public override void ResetEffects()
        {
            BleedProc = false;
            BleedBuildupMultiplier = 1;
            BleedDamageMultiplier = 1;
        }
    }
}
