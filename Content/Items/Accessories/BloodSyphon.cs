using Insignia.Core.ModPlayers;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using System.Net.Sockets;
using Terraria.ID;

namespace Insignia.Content.Items.Accessories
{
    internal class BloodSyphon : ModItem
    {
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<InsigniaPlayer>().BleedDamageMultiplier -= 0.05f;
            player.GetModPlayer<InsigniaPlayer>().BleedBuildupMultiplier += 0.1f;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Helpers.GeneralHelper.AddExpandableTooltip(ref tooltips, Mod, Color.Blue, Helpers.GeneralHelper.BleedDescription, Color.LightBlue);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.SilkRope, 30);
            recipe.AddIngredient(ItemID.IronBar, 7);
            recipe.AddIngredient(ItemID.Bottle, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();  
        }
    }
}
