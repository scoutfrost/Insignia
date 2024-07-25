using Insignia.Biomes.ColdBiome.Backgrounds;
using Insignia.Biomes.ColdBiome.Tiles;
using Insignia.Core.ModPlayers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace Insignia.Biomes.ColdBiome
{
    public class BiomeTileCount : ModSystem
    {
        public int tileCount;
        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            tileCount = tileCounts[ModContent.TileType<GlacialChunkTile>()];
        }
    }
    internal class ColdBiome : ModBiome
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<ColdBiomeBackground>();
        public override bool IsBiomeActive(Player player)
        {
            bool tilecount = ModContent.GetInstance<BiomeTileCount>().tileCount >= 40;
            player.GetModPlayer<InsigniaPlayer>().inColdBiome = tilecount;
            return tilecount;
        }
    }
}
