using Insignia.Core.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Insignia.Biomes.ColdBiome.Tiles
{
    public class BigGlacialChunkMultitile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileCut[Type] = false;
            Main.tileLavaDeath[Type] = true;
            DustType = DustID.Ice;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 10;
            TileObjectData.newTile.Width = 8;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(80, 150, 220), name);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundSystem.GlacialChunkKillSound with { Volume = 1.5f, Pitch = -0.2f });
            }
        }
    }
}