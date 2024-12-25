using Insignia.Core.Common.Systems;
using Insignia.Core.ModPlayers;
using Insignia.Core.Particles;
using Microsoft.Xna.Framework;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Insignia.Content.Items.Accessories
{
    internal class BloodSmokeAccessory : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Green;
            Item.accessory = true; 
        }
        public override void Load()
        {
            base.Load();
        }
        bool hasSubscribed = false;
        Player player;
        NPC npc;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!hasSubscribed)
            {
                player.GetModPlayer<AccessoryPlayer>().OnHitNPCEvent += BloodSmokeAccessory_OnHitNPCEvent;
                hasSubscribed = true;
            }
            this.player = player;
            //Main.NewText(player.GetModPlayer<InsigniaPlayer>().BleedProc);
            if (p == null || p.kill)
                return;

            if (time > 60)
            {
                p.kill = true;
                npc = null;
                time = 0;
            }
            else
            {
                time++;
                for (int i = 0; i < p.Points.Length; i++)
                {
                    p.Points[i] = Helpers.EasingFunctions.Bezier([npc.Center, player.Center], (float)i / 10);
                }
                p.Draw();
            }
        }
        int time = 0;
        PrimTrail p;
        private void BloodSmokeAccessory_OnHitNPCEvent(NPC npc, NPC.HitInfo arg2, int arg3, Player arg4)
        {
            if (player.GetModPlayer<InsigniaPlayer>().BleedProc)
            {
                this.npc = npc;
                List<Vector2> primPoints = [];
                for (int i = 0; i < 10; i++)
                {
                    primPoints.Add(Helpers.EasingFunctions.Bezier([npc.Center, player.Center], (float) i / 10));
                }
                p = PrimHandler.CreateTrail<PrimTrail>(false, default);

                p.Points = primPoints.ToArray();
                p.Color = Color.DarkRed;
                p.Width = 10;
                p.Initialize();

                player.statLife += 40;
            }
        }
    }
}
