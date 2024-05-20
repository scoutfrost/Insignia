using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insignia.Content.Tiles
{
    public class GlacialChunkItem : ModItem
    {
        //      public override string Texture => Helper.Empty;

        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GlacialChunkTile>());
            Item.width = 12; //The hitbox dimensions are intentionally smaller so that it looks nicer when fished up on a bobber
            Item.height = 12;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 3);
        }
    }
}