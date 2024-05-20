using Insignia.Content.Items.Materials;
using Insignia.Core.ModPlayers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insignia.Content.Items.Accessories
{
    internal class HornedGlove : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<InsigniaPlayer>().BleedDamageMultiplier += 0.1f;
            player.GetModPlayer<InsigniaPlayer>().BleedBuildupMultiplier -= 0.05f;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            Helpers.GeneralHelper.AddExpandableTooltip(ref tooltips, Mod, Color.Blue, Helpers.GeneralHelper.BleedDescription, Color.LightBlue);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<Horn>(), 1);
            recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.AddIngredient(ItemID.Leather, 12);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
