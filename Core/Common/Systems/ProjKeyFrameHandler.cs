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
        public delegate Vector2 CustomFunction(Vector2 start, Vector2 end, float t);

        CustomFunction Customfunc;

        KeyFrameInterpolationCurve keyFrameInterpolationCurve;
        static string texturePath;
        int pointCount;
        Texture2D tex;

        /// <param name="keyFrameInterpolationCurve">The method of interpolating new points between the provided points.</param>
        /// <param name="texPath">The path to the texture with the keypoints on it.</param>
        /// <param name="totalPointCount">The total amount of points you want to be returned.</param>
        /// <param name="CustomInterpolationFunc">Only set this to something if you have chosen KeyFrameInterpolationCurve.Custom.</param>
        public ProjKeyFrameHandler(KeyFrameInterpolationCurve keyFrameInterpolationCurve, string texPath, int totalPointCount = 30, CustomFunction CustomInterpolationFunc = null)
        {
            this.keyFrameInterpolationCurve = keyFrameInterpolationCurve;
            texturePath = texPath;
            pointCount = totalPointCount;
            tex = (Texture2D)ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Customfunc = CustomInterpolationFunc;
        }

        /// <param name="radius">The radius of the interpolated circle. Only set this if youre using KeyFrameInterpolationCurve.Slerp.</param>
        /// <returns>The interpolated points.</returns>
        public List<Vector2> GetPoints(float radius = default)
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
            {
                //TODO: implement rest of curves and stuff
                default://case KeyFrameInterpolationCurve.Lerp:
                    if (coordsForColorData.Count != 1)
                    {
                        for (int i = 0; i < coordsForColorData.Count - 1; i++)
                        {
                            // Main.NewText("wow");
                            for (float k = 0; k <= pointCount / coordsForColorData.Count; k++)
                            {
                                returnPoints.Add(Vector2.Lerp(coordsForColorData[i], coordsForColorData[i + 1], k / (pointCount / coordsForColorData.Count)));
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
                            for (float k = 0; k <= pointCount / coordsForColorData.Count; k++)
                            {
                                Vector2 center = new(coordsForColorData[i].X, (coordsForColorData[i + 1].Y - coordsForColorData[i].Y) / 2);
                                float maxRotation = (coordsForColorData[i + 1] - center).ToRotation() - (coordsForColorData[i] - center).ToRotation();
                                returnPoints.Add(EasingFunctions.Slerp(coordsForColorData[i], coordsForColorData[i + 1], k / (pointCount / coordsForColorData.Count) * maxRotation, radius));
                            }
                        }
                        
                        for (int i = 0; i < coordsForColorData.Count - 1; i++)
                        {
                            Main.NewText(coordsForColorData[i]);
                            Main.NewText(coordsForColorData[i + 1]);
                        }    
                        return returnPoints;
                    }
                    return null;
                case KeyFrameInterpolationCurve.Custom:
                    if (coordsForColorData.Count != 1)
                    {
                        for (int i = 0; i < coordsForColorData.Count - 1; i++)
                        {
                            // Main.NewText("wow");
                            for (float k = 0; k < pointCount / coordsForColorData.Count; k++)
                            {
                                returnPoints.Add(Customfunc(coordsForColorData[i], coordsForColorData[i + 1], k / pointCount / coordsForColorData.Count));
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
