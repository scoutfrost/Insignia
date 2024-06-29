using Insignia.Core.ModPlayers;
using Insignia.Core.Particles;
using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //this.player = player;
            if (!hasSubscribed)
            {
                player.GetModPlayer<AccessoryPlayer>().OnHitNPCEvent += BloodSmokeAccessory_OnHitNPCEvent;
                hasSubscribed = true;
            }
        }

        private void BloodSmokeAccessory_OnHitNPCEvent(NPC arg1, NPC.HitInfo arg2, int arg3, Player arg4)
        {
            throw new System.NotImplementedException();
        }
    }
}
