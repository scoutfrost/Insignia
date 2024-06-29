using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Insignia.Biomes.ColdBiome
{
    public class ExampleBiomeTileCount : ModSystem
    {
        public int exampleBlockCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            exampleBlockCount = tileCounts[ModContent.TileType<ExampleBlock>()];
        }
    }
    internal class ColdBiome : ModBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            bool b1 = ModContent.GetInstance<ExampleBiomeTileCount>().exampleBlockCount >= 40;

            // Second, we will limit this biome to the inner horizontal third of the map as our second custom condition
            bool b2 = Math.Abs(player.position.ToTileCoordinates().X - Main.maxTilesX / 2) < Main.maxTilesX / 6;

            // Finally, we will limit the height at which this biome can be active to above ground (ie sky and surface). Most (if not all) surface biomes will use this condition.
            bool b3 = player.ZoneSkyHeight || player.ZoneOverworldHeight;
            return b1 && b2 && b3;
        }
    }
}
