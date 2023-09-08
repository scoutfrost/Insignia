using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Helpers;

namespace Insignia.Core.Common.Systems
{
    public class ProjKeyFrameHandler
    {
        KeyFrameInterpolationCurve keyFrameInterpolationCurve;
        static string texturePath;
        int pointCount;
        Texture2D tex;
        public ProjKeyFrameHandler(KeyFrameInterpolationCurve keyFrameInterpolationCurve, string texPath, int totalPointCount = 30)
        {
            this.keyFrameInterpolationCurve = keyFrameInterpolationCurve;
            texturePath = texPath;
            pointCount = totalPointCount;
            tex = (Texture2D)ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
        }
        public List<Vector2> GetPoints()
        {
            List<Vector2> returnPoints = new();
            int height = tex.Height;
            int width = tex.Width;

            Color[] colorData = new Color[tex.Width * tex.Height];
            tex.GetData(colorData);
            List<Vector2> coordsForColorData = new();

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    Color currPixel = colorData[row * width + column];

                    if (currPixel.A != 0)
                    {
                        coordsForColorData.Add(new Vector2(column, row));
                    }
                }
            }
            
            switch (keyFrameInterpolationCurve)
            {//TODO: convert outputs to velocities and make a bool to decide whether you want velocities or positions 
                //also implement rest of curves and stuff
                default://case KeyFrameInterpolationCurve.Lerp:
                    if (coordsForColorData.Count != 1)
                    {
                        for (int i = 0; i < coordsForColorData.Count - 1; i++)
                        {
                            // Main.NewText("wow");
                            for (float k = 0; k < pointCount / coordsForColorData.Count; k++)
                            {
                                returnPoints.Add(Vector2.Lerp(coordsForColorData[i], coordsForColorData[i + 1], k / pointCount / coordsForColorData.Count));
                            }
                        }
                        return returnPoints;
                    }
                    return null;
                case KeyFrameInterpolationCurve.Slerp:
                    if (coordsForColorData.Count != 1)
                    {
                        for (int i = 0; i < coordsForColorData.Count - 1; i++)
                        {
                            for (int k = 0; k < pointCount / coordsForColorData.Count - 1; k++)
                            {
                                returnPoints.Add(EasingFunctions.Slerp(coordsForColorData[i], coordsForColorData[i + 1], k / pointCount / coordsForColorData.Count));
                            }
                        }
                        return returnPoints;
                    }
                    return null;
            }
        }
    }
    //long ahh name
    public enum KeyFrameInterpolationCurve
    {
        Lerp,
        Slerp,
        Bezier,
        Custom
    }
}
