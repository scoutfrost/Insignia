﻿using Insignia.Core.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Insignia.Content.Tiles
{
    public class ArcticTreeMultiTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileCut[Type] = false;
            Main.tileLavaDeath[Type] = true;
            HitSound = SoundSystem.TreeHurt;
            DustType = DustID.Ice;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 12;
            TileObjectData.newTile.Width = 10;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16 };
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
            Tile tile = Framing.GetTileSafely(i, j);

            if (Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundSystem.TreeKill);
            }
        }
    }
}