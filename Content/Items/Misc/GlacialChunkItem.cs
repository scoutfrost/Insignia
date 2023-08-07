using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;
using Insignia.Helpers;

namespace Insignia.Content.Items.Misc
{
 
    public class GlacialChunkItem: ModItem
    {

        public override string Texture => Helper.Empty;

        public override void SetStaticDefaults()
        {
         

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
         Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.GlacialChunkTile>());
            Item.width = 12; //The hitbox dimensions are intentionally smaller so that it looks nicer when fished up on a bobber
            Item.height = 12;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 3);
        }

        
    }
}
