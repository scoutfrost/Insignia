using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
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
            BleedBuildupMultiplier = 1;
            BleedDamageMultiplier = 1;
        }
    }
    public class BleedReset : ModSystem
    {
        public override void PostUpdateEverything()
        {
            Main.LocalPlayer.GetModPlayer<InsigniaPlayer>().BleedProc = false;
        }
    }
}
