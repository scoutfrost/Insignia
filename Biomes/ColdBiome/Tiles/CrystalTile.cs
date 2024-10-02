using Insignia.Core.Common.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Insignia.Biomes.ColdBiome.Tiles
{
    internal class CrystalTileItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CrystalTile>());
            Item.width = 12; //The hitbox dimensions are intentionally smaller so that it looks nicer when fished up on a bobber
            Item.height = 12;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(0, 3);
        }
    }
    public class CrystalTile : CompositeTile
    {
        public override int HorizontalSheetCount { get; } = 5;

        public override int VerticalSheetCount { get; } = 3;

        public override void SetStaticDefaults()
        {
            Main.tileMergeDirt[Type] = false;
            Main.tileSolid[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileFrameImportant[Type] = true;
               
            TileID.Sets.Conversion.Stone[Type] = true;

            AddMapEntry(new Color(56, 56, 65));

            MineResist = 1f;
            HitSound = SoundID.Tink;
            DustType = DustID.RedMoss;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

    }
}
