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
	internal class ArcticBiomeGeneration : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
			if (genIndex != -1)
			{
				tasks.Insert(genIndex - 1, new PassLegacy("Arctic Biome Gen", ArcticBiomeGen));
			}
		}

		public enum RoomShape
		{
			halfCircle,
			tunnelRoom,
			cavern
		}
		List<Point> rooms = new();
		public void ArcticBiomeGen(GenerationProgress progress, GameConfiguration config)
		{
            #region finding startpos
            bool hasFoundStartPos = false;
            Point startingPos = new(0, 0);
			int attempts = 0;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
				if (hasFoundStartPos)
				{
					break;
				}
                for (int y = 0; y < Main.maxTilesY; y++)
                {
					if (Main.tile[i, y].TileType == TileID.SnowBlock || Main.tile[i, y].TileType == TileID.IceBlock)
					{
                        Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
                        WorldUtils.Gen(new Point(i, y), new Shapes.Rectangle(40, 40), new Actions.TileScanner(TileID.SnowBlock, TileID.IceBlock).Output(dictionary));
                        int blockCount = dictionary[TileID.SnowBlock] + dictionary[TileID.IceBlock];

                        if (blockCount >= 40 * 40 / 1.5f || attempts >= 50)
						{
							if (y < Main.worldSurface + WorldGen.genRand.Next(-40, 40))
							{
								startingPos = new(i + WorldGen.genRand.Next(70, 140), y);
								hasFoundStartPos = true;
								break;
							}
						}
						else
						{
							attempts++;
						}
					}
                } 
            }
            #endregion
            #region room gen
            for (int i = 0; i < WorldGen.genRand.Next(3, 7); i++)
			{
				int rand = WorldGen.genRand.Next(0, 3);
                Point spaceBetweenRooms = new(i * WorldGen.genRand.Next(45, 55), WorldGen.genRand.Next(-10, 10));

                int sizeDiff = WorldGen.genRand.Next(5, 10);
				Point sizeDiffPoint = new Point(sizeDiff, -sizeDiff);
                switch (rand)
				{
                    case (int)RoomShape.halfCircle:
						int domeSize = WorldGen.genRand.Next(10, 45);
                        WorldUtils.Gen(startingPos + spaceBetweenRooms - sizeDiffPoint, new Shapes.HalfCircle(domeSize + sizeDiff), Actions.Chain(new GenAction[]
						{
                            new Modifiers.Dither(0.4f),
                            new Actions.ClearTile()
						}));
                        WorldUtils.Gen(startingPos + spaceBetweenRooms, new Shapes.HalfCircle(domeSize - sizeDiff), new Actions.Clear());
                        break;
					case (int)RoomShape.tunnelRoom:
						Vector2 size = new(WorldGen.genRand.Next(15, 25), WorldGen.genRand.Next(20, 40));
                        WorldUtils.Gen(startingPos + spaceBetweenRooms - new Point(0, (int)size.Y / 2), new Shapes.Rectangle((int)size.X + sizeDiff, (int)size.Y + sizeDiff), Actions.Chain(new GenAction[]
						{
							new Modifiers.Dither(0.4f),
							new Actions.ClearTile()
						}));
						WorldUtils.Gen(startingPos + spaceBetweenRooms - new Point(0, (int)size.Y / 2) - sizeDiffPoint, new Shapes.Rectangle((int)size.X - sizeDiff, (int)size.Y - sizeDiff), new Actions.Clear());
						break;
					case (int)RoomShape.cavern:
						int cavernX = WorldGen.genRand.Next(15,50);
                        WorldUtils.Gen(startingPos + spaceBetweenRooms - sizeDiffPoint, new Shapes.Circle(cavernX + sizeDiff, (cavernX + sizeDiff) / 2), Actions.Chain(new GenAction[]
                        {
                            new Modifiers.Dither(0.4f),
                            new Actions.ClearTile()
                        }));
                        WorldUtils.Gen(startingPos + spaceBetweenRooms, new Shapes.Circle(cavernX - sizeDiff, (cavernX - sizeDiff) / 2), new Actions.Clear());
                        break;
				}//TODO: make a method that i can call for every case taking some params like a random int for room size and a shape type
			}
            #endregion
            //WorldUtils.Gen(startingPos, new Shapes.HalfCircle(WorldGen.genRand.Next(10, 45)), new Actions.Clear());
            //WorldGen.digTunnel(startingPos.X, startingPos.Y, WorldGen.genRand.Next(1, 5), 10, WorldGen.genRand.Next(5, 10), 5);
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
			
            
        }
	}

    /*public class Elipse : GenShape
    {
    ignore this stuff
		Point focalPoint1;
		Point focalPoint2;
		int radius;
		int f1Length;
		int f2Length;
		public Elipse(Point f1, Point f2, int radius)
		{
			focalPoint1 = f1;
			focalPoint2 = f2;
			this.radius = radius;
			f1Length = radius / 2;
			f2Length = radius / 2;
		}
        public override bool Perform(Point origin, GenAction action)
        {
			bool isDone = false;
			bool takeAbs = false;
			while (!isDone)
			{
				f1Length++;
				f2Length--;
				if (Math.Abs(f2Length) >= radius - Math.Abs((focalPoint1 - focalPoint2).ToVector2().Length()))
				{
					takeAbs = true;
				}
                if (takeAbs == true)
                {
                    f2Length = Math.Abs(f2Length);
                }
				if (f1Length >= radius - Math.Abs((focalPoint1 - focalPoint2).ToVector2().Length()))
				{
                }
            }
        }
    }*/
}
