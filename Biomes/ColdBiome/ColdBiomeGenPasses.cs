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
using Insignia.Core.Common.Systems;
using Insignia.Biomes.ColdBiome.Tiles;
using System.Linq;
using UtfUnknown.Core.Models.SingleByte.Hungarian;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Insignia.Biomes
{
	internal class ArcticBiomeGeneration : ModSystem
	{
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int genIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Gems In Ice Biome"));
			if (genIndex != -1)
			{
				tasks.Insert(genIndex + 1, new PassLegacy("Arctic Biome Gen", ArcticBiomeGen));
			}
		}

		public enum RoomShape
		{
			wideCavern,
			waterRoom,
			cavern
		}
		List<Point> rooms = new();
		private List<Vector2> keypoints;


		public static bool JustPressed(Keys key)
		{
			return Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
		}
		public override void PostUpdateWorld()
		{
			if (JustPressed(Keys.D1))
			{
				//TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
				TestMethod2((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
			}
		}
		private void TestMethod2(int x, int y)
		{
			/*for (int i = 0; i < 50; i++)
			{
				for (int j = 0; j < 50; j++)
				{
					WorldGen.KillWall((int)(Main.MouseWorld.X / 16) + i, (int)(Main.MouseWorld.Y / 16) + j);
				}
			}

			List<int> a;
			a = [25, 25, 25, 25];*/
			/*for (int i = x; i < x + 10; i++)
            {
                for (int j = y; j < x + 10; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (IsTileSingle(x,y))
						{
                            WorldGen.KillTile(x, y);
                        }
                    }
                }
            }*/
			//Main.NewText
			ArcticBiomeGen(default, default);
		}

        static bool IsTileSingle(int x, int y, int maxFacesExposed = 4)
		{
			if (Main.tile[x, y].TileType == ModContent.TileType<GlacialChunkTile>())
			{
				int count = 0;
				for (int k = 0; k < 4; k++)
				{
					if (WorldGen.SolidTile(Main.tile[new Point(x, y) + Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * k).ToPoint()]) &&
						Main.tile[x, y].TileType == ModContent.TileType<GlacialChunkTile>())
					{
						count++;
					}
				}
				if (count <= 4 - maxFacesExposed)
					return true;
			}
			return false;
		}
		int EvaluateChance(List<int> probabilities)
		{
			int rand = WorldGen.genRand.Next(100);
			probabilities.Sort();
			probabilities.Insert(0, 0);
			int sum = probabilities[0];
			for (int i = 1; i < probabilities.Count; i++)
			{
				sum += probabilities[i];
				if (rand <= sum && rand >= probabilities[i])
				{
					return i;
				}
			}
			return 1;
		}
		public void ArcticBiomeGen(GenerationProgress progress, GameConfiguration config)
        {
            #region finding startpos
            bool hasFoundStartPos = false;
			Point iceBiomeTile = new();
			Point startingPos = new();
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int y = 0; y < Main.maxTilesY; y++)
				{
					if (y < Main.worldSurface)
					{
						if ((Main.tile[i, y].TileType == TileID.SnowBlock || Main.tile[i, y].TileType == TileID.IceBlock) && !hasFoundStartPos)
						{
							iceBiomeTile = new Point(i, y);
							startingPos = new(i + WorldGen.genRand.Next(200, 340), y + WorldGen.genRand.Next(120, 250));
							hasFoundStartPos = true;
							WorldUtils.Gen(startingPos, new Shapes.Circle(225, 120), Actions.Chain(new Actions.SetTile((ushort)ModContent.TileType<GlacialChunkTile>()), new Actions.ClearWall()));

							WorldUtils.Gen(startingPos, new Shapes.Circle(235, 130), Actions.Chain(new Modifiers.IsSolid(), new Modifiers.Dither(0.5f),
								new Actions.SetTile((ushort)ModContent.TileType<GlacialChunkTile>())));
						}
					}
				}
			}
            #endregion
            #region room gen

            int successRate = 25; // starting success rate
			List<Point> points = [];//holds a tile coord
			List<int> chancePerTile = []; // the success rate of the tiles in the below, above, to the right, and to the left of it, IN THAT ORDER
			Point[] directions = [new(0, 1), new(0, -1), new(1, 0), new(-1, 0)]; //down, up, right, left

			for (int i = 0; i < 10; i++)
			{
				Point coords = startingPos + new Point(WorldGen.genRand.Next(-200, 200), WorldGen.genRand.Next(-50, 50));

				WorldGen.KillTile(startingPos.X, startingPos.Y);

				points.Add(coords);
				for (int j = 0; j < 4; j++)
				{
					chancePerTile.Add(successRate);
				}
			}
			int c = 0;
			while (c < 750)
			{
				for (int i = 0; i < points.Count; i++)
				{
					List<int> chances = [chancePerTile[i], chancePerTile[i + 1], chancePerTile[i + 2], chancePerTile[i + 3]];
					int chance = EvaluateChance(chances) - 1;
					Point tile = points[i] + directions[chance];
					int circlesize = WorldGen.genRand.NextBool(2) ? 2 : 1;
					if (WorldGen.SolidTile(tile))
                    {
                        WorldUtils.Gen(tile, new Shapes.Circle(circlesize), Actions.Chain(new Modifiers.Blotches(2, 0.5f), new Actions.ClearTile(true)));
                        if (tile.Y > startingPos.Y) {
                            WorldUtils.Gen(tile, new Shapes.Circle(circlesize), Actions.Chain(new Modifiers.Blotches(2, 0.5f), new Actions.SetLiquid()));
						}
						/*for (int k = 0; k < chances.Count - 1; k++) couldnt get the varying chances to work :/
                        {
                            if (chances[k] == chance)
                            {
                                chancePerTile[i * 4 + k] += 3;
                            }
                            else
                            {
                                chancePerTile[i * 4 + k] -= 1;
                            }
                        }*/
					}
					points[i] = tile + (directions[chance].ToVector2() * circlesize).ToPoint();
				}
				c++;
			}
			int miscRoomCount = 5;
			for (int i = iceBiomeTile.X; i < (iceBiomeTile.X < Main.maxTilesX / 2 ? Main.maxTilesX / 2 : Main.maxTilesX); i++)
			{
				for (int j = iceBiomeTile.Y; j < Main.maxTilesY - 200; j++)
                {
                    if (IsTileSingle(i, j, 3))
                    {
                        if (WorldGen.genRand.NextBool(1, 2))
                        {
                            WorldGen.KillTile(i, j);
                        }
                    }
                    if (IsTileSingle(i, j, 4))
                    {
                        WorldGen.KillTile(i, j);
                    }
                }
            }
            List<Point> miscRooms = [];

            for (int i = 0; i < 100; i++)
            {
                Dictionary<ushort, int> data = [];
                Point potentialPoint = startingPos + new Point(WorldGen.genRand.Next(-200, 200), WorldGen.genRand.Next(-100, 100));
				Point dimensions = new(WorldGen.genRand.Next(5, 25), WorldGen.genRand.Next(5, 20));

                WorldUtils.Gen(potentialPoint, new Shapes.Rectangle(dimensions.X, dimensions.Y), new Actions.TileScanner((ushort)ModContent.TileType<GlacialChunkTile>()).Output(data));
                if (data[(ushort)ModContent.TileType<GlacialChunkTile>()] == (dimensions.X * dimensions.Y) && data[(ushort)ModContent.TileType<GlacialChunkTile>()] != 0 && miscRooms.Count < miscRoomCount)
                {
                    WorldUtils.Gen(potentialPoint, new Shapes.Circle((int)(dimensions.X * 1.5f), (int)(dimensions.Y * 1.5f)), Actions.Chain(new Modifiers.Blotches(), new Actions.SetTile((ushort)ModContent.TileType<PlantMatterTile>())));
                    WorldUtils.Gen(potentialPoint, new Shapes.Circle(dimensions.X, dimensions.Y), Actions.Chain(new Modifiers.Blotches(), new Actions.ClearTile(true)));
                    miscRooms.Add(potentialPoint);
                }
            }
            points.AddRange(miscRooms);
            for (int i = 0; i < points.Count - 1; i++)
			{
				Vector2 next = points[i + 1].ToVector2();
				Vector2 point = points[i].ToVector2();
				WorldGen.digTunnel(point.X, point.Y, next.DirectionTo(point).X, next.DirectionTo(point).Y, 100, 1, false);
			}
				//WorldGen.TileRunner()

				/*Dictionary<Point, int> keyPoints = new();
				int deleteTileStretchAmount = 5;

				// viewer discretion advised
				int rand = WorldGen.genRand.Next(3, 5);
				for (int i = 0; i < rand; i++)
				{
					int randCoord;
					int randCoord2;
					int roomSpaceMultiplier = rand == 3 ? 160 : 120;
					int randRoom = WorldGen.genRand.Next(0, 3);
					Point startSmallerPoints;
					Point start;
					Point spaceBetweenRooms = new(i * roomSpaceMultiplier, WorldGen.genRand.Next(-10, 10) * rand);

					switch (randRoom)
					{
						case (int)RoomShape.wideCavern:
							Main.NewText("wide");
							for (int j = 0; j < 40; j++)
							{
								randCoord = WorldGen.genRand.Next(0, 45) / (int)MathHelper.Clamp(rand / 4, 1, 10);
								randCoord2 = WorldGen.genRand.Next(0, 25) / (int)MathHelper.Clamp(rand / 4, 1, 10);

								start = startingPos + new Point(randCoord, randCoord2) + spaceBetweenRooms;

								if (Main.tile[start].TileType == TileID.SnowBlock || Main.tile[start].TileType == TileID.IceBlock)
								{
									startSmallerPoints = startingPos + new Point(WorldGen.genRand.Next(-20, 55), WorldGen.genRand.Next(1, 20)) + spaceBetweenRooms;
									if (!keyPoints.ContainsKey(start) && !keyPoints.ContainsKey(startSmallerPoints) && startSmallerPoints != start)
									{
										keyPoints.Add(start, 5 + WorldGen.genRand.Next(0, 5));
										keyPoints.Add(startSmallerPoints, 3);
									}
								}
							}
							break;
						default: //cavern
							Main.NewText("cavern");
							{
								for (int j = 0; j < 35; j++)
								{
									randCoord = WorldGen.genRand.Next(0, 40);
									randCoord2 = WorldGen.genRand.Next(0, 40);

									start = startingPos + new Point(randCoord, randCoord2) + spaceBetweenRooms;

									if (Main.tile[start].TileType == TileID.SnowBlock || Main.tile[start].TileType == TileID.IceBlock)
									{
										startSmallerPoints = startingPos + new Point(randCoord, WorldGen.genRand.Next(-10, 45)) + spaceBetweenRooms;
										if (!keyPoints.ContainsKey(start) && !keyPoints.ContainsKey(startSmallerPoints) && startSmallerPoints != start)
										{
											keyPoints.Add(start, 7);
											keyPoints.Add(startSmallerPoints, 3);
										}
									}
								}
								break;
							}

					}
				}
				Point[] tempArrayKeys = new Point[keyPoints.Count];
				int[] tempArrayValues = new int[keyPoints.Count];

				keyPoints.Keys.CopyTo(tempArrayKeys, 0);
				keyPoints.Values.CopyTo(tempArrayValues, 0);
				// clearing area around initial points

				for (int x = 0; x < Main.maxTilesX; x++) // so true
				{
					for (int y = 0; y < Main.maxTilesY; y++)
					{
						if (Main.tile[x, y].TileType == TileID.SnowBlock || Main.tile[x, y].TileType == TileID.IceBlock)
						{
							for (int k = 0; k < keyPoints.Count; k++)
							{
								//WorldGen.PlaceTile(tempArray[k].X, tempArray[k].Y, TileID.Adamantite, false, true);
								if ((int)Vector2.Distance(new Vector2(x, y), tempArrayKeys[k].ToVector2()) > tempArrayValues[k] && (int)Vector2.Distance(new Vector2(x, y), tempArrayKeys[k].ToVector2()) < tempArrayValues[k] + 10)
								{
									WorldGen.PlaceTile(x, y, ModContent.TileType<GlacialChunkTile>(), true, true);
								}
								if ((int)Vector2.Distance(new Vector2(x, y), tempArrayKeys[k].ToVector2()) == tempArrayValues[k])
								{
									WorldGen.TileRunner(x, y, 35, 1, ModContent.TileType<GlacialChunkTile>(), true);
								}
							}
						}
					}
				}
				for (int i = 0; i < keyPoints.Count; i++)
				{
					WorldUtils.Gen(tempArrayKeys[i], new Shapes.Circle(tempArrayValues[i] + deleteTileStretchAmount, tempArrayValues[i]), Actions.Chain(new Modifiers.Blotches(), new Actions.ClearTile(true)));
				}
				/*for (int x = 0; x < Main.maxTilesX; x++) // so true
				{
					for (int y = 0; y < Main.maxTilesY; y++)
					{
						if (Main.tile[x, y].TileType == TileID.SnowBlock || Main.tile[x, y].TileType == ModContent.TileType<GlacialChunkTile>() || Main.tile[x, y].TileType == TileID.IceBlock)
						{
							for (int k = 0; k < keyPoints.Count; k++)
							{
								if ((int)Vector2.Distance(new Vector2(x, y), tempArrayKeys[k].ToVector2()) < tempArrayValues[k])
								{
									WorldGen.KillTile(x, y);
								}
							}
						}
					}
				}*/


				/*if (Main.tile[x, y].TileType == TileID.SnowBlock || Main.tile[x, y].TileType == ModContent.TileType<GlacialChunkTile>() || Main.tile[x, y].TileType == TileID.IceBlock)
				{
					if (!foundFirstIce)
					{
						firstIce = new(x, y);
						foundFirstIce = true;
					}
					for (int k = 0; k < keyPoints.Count; k++)
					{
						//bool shouldPlaceTile = true;
						//WorldGen.PlaceTile(tempArray[k].X, tempArray[k].Y, TileID.Adamantite, false, true);
						if ((int)Vector2.Distance(new Vector2(x, y), tempArray[k].ToVector2()) == maxDist + 5)
						{
							WorldGen.TileRunner(x, y, 15, 1, ModContent.TileType<GlacialChunkTile>(), true);
						}
					}
					for (int k = 0; k < keyPoints.Count; k++)
					{
						if ((int)Vector2.Distance(new Vector2(x, y), tempArray[k].ToVector2()) < maxDist)
						{
							WorldGen.KillTile(x, y);
						}
					}
				}*/

			


			//int rand = WorldGen.genRand.Next(0, 3);
			//Point spaceBetweenRooms = new(i * WorldGen.genRand.Next(45, 55), WorldGen.genRand.Next(-10, 10));

			//}
			/*for (int i = 0; i < WorldGen.genRand.Next(3, 7); i++)
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
						int cavernX = WorldGen.genRand.Next(15, 50);
						WorldUtils.Gen(startingPos + spaceBetweenRooms - sizeDiffPoint, new Shapes.Circle(cavernX + sizeDiff, (cavernX + sizeDiff) / 2), Actions.Chain(new GenAction[]
						{
							new Modifiers.Dither(0.4f),
							new Actions.ClearTile()
						}));
						WorldUtils.Gen(startingPos + spaceBetweenRooms, new Shapes.Circle(cavernX - sizeDiff, (cavernX - sizeDiff) / 2), new Actions.Clear());
						break;
				}//TODO: make a method that i can call for every case taking some params like a random int for room size and a shape type
			}*/
			#endregion



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
	}
}

