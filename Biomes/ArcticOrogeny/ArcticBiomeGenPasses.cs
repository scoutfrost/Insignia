using Insignia.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

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
            wideCavern,
            waterRoom,
            cavern
        }

        private List<Point> rooms = new();
        private List<Vector2> keypoints;

        public void ArcticBiomeGen(GenerationProgress progress, GameConfiguration config)
        {
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
            {
                //TestMethod((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
                TestMethod2((int)Main.MouseWorld.X / 16, (int)Main.MouseWorld.Y / 16);
            }
        }

        private void TestMethod2(int x, int y)
        {
            //ProjKeyFrameHandler keyFrameHandler = new(KeyFrameInterpolationCurve.Lerp, "Insignia/Content/Items/Weapons/Sets/Glacial/SwingPoints");
            //keypoints = keyFrameHandler.GetPoints();

            //Main.NewText(keypoints.Count + "real");
        }

        private void TestMethod(int x1, int y1)
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
                        Dictionary<ushort, int> dictionary = new();
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

            #endregion finding startpos

            #region room gen

            Dictionary<Point, int> keyPoints = new();
            int deleteTileStretchAmount = 5;
            // I DONT CARE I COUILD OPTIMIZE THIS I DONT CARE THERES BETTER WAYS OF DOING THIS IM SICK AND TIRED OF WORLGHEJ SHUT UP SUHT UP SHUT UP SHUIT OP SUHT UP SHUT UP SHUIT IP SHUIT UP FWEB FGERG ERVGHE QBRG EWRBGWBRGVWRE BGUREWGBWRTHGVWHRTBG HRTB GB RTG
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
                WorldUtils.Gen(tempArrayKeys[i], new Shapes.Circle(tempArrayValues[i] + deleteTileStretchAmount, tempArrayValues[i]), new Actions.ClearTile(true));
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
        }

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

        #endregion room gen

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