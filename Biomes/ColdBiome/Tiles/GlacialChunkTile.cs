using Insignia.Core.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insignia.Biomes.ColdBiome.Tiles 
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
    public class GlacialChunkTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;

            DustType = DustID.Ice;
            HitSound = SoundSystem.GlacialChunkSound;

            AddMapEntry(new Color(20, 80, 200));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        //# TO DO: FIX UP THE KILL SOUND (I THINK I GOTTA MAKE AN EntitySource_OnBreak

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (!fail)
            {
                SoundEngine.PlaySound(SoundSystem.GlacialChunkKillSound);
            }
        }
    }
}