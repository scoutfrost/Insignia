using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using System.Collections.Generic;
using Terraria.WorldBuilding;
using Terraria.IO;
using System;

namespace Insignia.Biomes
{
	internal class WorldGenTutorialWorld : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int iceBiomeGenIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Floating Islands"));
			if (iceBiomeGenIndex != -1)
				tasks.Insert(iceBiomeGenIndex - 1, new PassLegacy("Arctic Biome Gen", ArcticBiomeGen));
		}
	
		public void ArcticBiomeGen(GenerationProgress progress, GameConfiguration config)
        {
			
		}
        public static bool JustPressed(Keys key)
		{
			return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
		}
		public override void PostUpdateWorld()
		{
			if (JustPressed(Keys.D1))
				TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
		}

		private void TestMethod(int x, int y1)
		{
			Dust.QuickBox(new Vector2(x, y1) * 16, new Vector2(x + 1, y1 + 1) * 16, 2, Color.YellowGreen, null);
			/*bool right = WorldGen.genRand.NextBool();
			int startX = right ? (Main.maxTilesX / 2) + WorldGen.genRand.Next(100, 150) : (Main.maxTilesX / 2) - WorldGen.genRand.Next(100, 150);
			int endX = right ? startX + WorldGen.genRand.Next(150, 225) : startX - WorldGen.genRand.Next(150, 225);
			Main.NewText(startX + " start");
			Main.NewText(endX + " end");
			Main.NewText((int)Main.player[0].Center.X / 16 + " player cetenrt er ");
			Main.NewText(right);*/
		}
	}
}