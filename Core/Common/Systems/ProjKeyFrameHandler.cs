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
        public List<Vector2> coordsForColorData;
        public Vector2 rotationCenter;
        /// <param name="keyFrameInterpolationCurve">The method of interpolating new points between the provided points.</param>
        /// <param name="texPath">The path to the texture with the keypoints on it.</param>
        /// <param name="speed">The speed of the projectile. This could also be thought of as the total returned amount of points, but its little over.</param>
        /// <param name="CustomInterpolationFunc">Only set this to something if you have chosen KeyFrameInterpolationCurve.Custom.</param>
        public ProjKeyFrameHandler(KeyFrameInterpolationCurve keyFrameInterpolationCurve, string texPath, int speed = 30, CustomFunction CustomInterpolationFunc = null)
        {
            this.keyFrameInterpolationCurve = keyFrameInterpolationCurve;
            texturePath = texPath;
            pointCount = speed;
            tex = (Texture2D)ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Customfunc = CustomInterpolationFunc;
        }

        /// <param name="radius">The radius of the interpolated circle. Only set this if youre using KeyFrameInterpolationCurve.Slerp.</param>
        /// <returns>The interpolated points.</returns>
        public List<Vector2> GetPoints(float radius = default, bool flipVertically = false, Color centerColor = default)
        {
            List<Vector2> returnPoints = new();
            int height = tex.Height;
            int width = tex.Width;

            Color[] colorData = new Color[tex.Width * tex.Height];
            tex.GetData(colorData);
            coordsForColorData = new();
            rotationCenter = default;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color currPixel = colorData[y * width + x];

                    if (currPixel.A != 0 && currPixel != centerColor)
                    {
                        coordsForColorData.Add(new Vector2(x, y));
                    }
                    else if (currPixel == centerColor)
                    {
                        rotationCenter = new Vector2(x, y);
                    }
                }
            }
            if (flipVertically)
            {
                for (int i = 0; i < coordsForColorData.Count; i++)
                {
                    coordsForColorData[i] = new Vector2(coordsForColorData[i].X, coordsForColorData[i].Y * -1);
                }
                rotationCenter = new Vector2(rotationCenter.X, rotationCenter.Y * -1);
            }
            switch (keyFrameInterpolationCurve)
            {
                //might implement lagrange polynomial later but nrn
                case KeyFrameInterpolationCurve.Bezier:
                    if (coordsForColorData.Count != 1)
                    {
                        for (float k = 0; k <= pointCount; k++)
                        {
                            returnPoints.Add(EasingFunctions.Bezier(coordsForColorData.ToArray(), k / pointCount));
                        }
                        return returnPoints;
                    }
                    return null;
                case KeyFrameInterpolationCurve.Lerp:
                    if (coordsForColorData.Count != 1)
                    {
                        for (int i = 0; i < coordsForColorData.Count - 1; i++)
                        {
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
                            Vector2 center = centerColor == default ? new(coordsForColorData[0].X, (coordsForColorData[^1].Y + coordsForColorData[0].Y) / 2) : rotationCenter; //if no given center: average of the y value, but not the x - makes it more intuitive to use
                            float maxRotation = center.AngleTo(coordsForColorData[i + 1]) - center.AngleTo(coordsForColorData[i]);
                            for (float k = 0; k <= pointCount / coordsForColorData.Count; k++)
                            {
                                returnPoints.Add(EasingFunctions.RotateVector(coordsForColorData[i], k / (pointCount / coordsForColorData.Count) * maxRotation, center, radius));
                            }
                        }
                        return returnPoints;
                    }
                    return null;
                case KeyFrameInterpolationCurve.Custom:
                    if (coordsForColorData.Count != 1)
                    {
                        for (int i = 0; i < coordsForColorData.Count - 1; i++)
                        {
                            for (float k = 0; k < pointCount / coordsForColorData.Count; k++)
                            {
                                returnPoints.Add(Customfunc(coordsForColorData[i], coordsForColorData[i + 1], k / pointCount / coordsForColorData.Count));
                            }
                        }
                        return returnPoints;
                    }
                    return null;
                case KeyFrameInterpolationCurve.CompleteCustom:
                    if (coordsForColorData.Count != 1)
                        Customfunc(default, default, default);
                    return null;
                default:
                    return null;
            }
        }
        public delegate Vector2 DesiredChange(Vector2 point, int i);
        /// <summary>
        /// Change all points in a certain way, implemented by desiredchange.
        /// </summary>
        /// <param name="points">The returned keypoints from GetPoints().</param>
        /// <param name="desiredChange">How you would like to change the points.</param>
        public void ChangePoints(ref List<Vector2> points, DesiredChange desiredChange)
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i] = desiredChange(points[i], i);
            }
        }
        /// <summary>
        /// Calculates the correct position of the projectile based on the parameters. To use, set the projectile's center to what this function returns.
        /// </summary>
        /// <param name="projectile">The projectile.</param>
        /// <param name="vectorToMouse">The player's center + the normalized vector from the player to the mouse. Make sure to set this in ai. Don't use Main.MouseWorld, create a seperate variable in onspawn and set that to Main.Mouseworld instead.</param>
        /// <param name="owner">The projectile's owner.</param>
        /// <param name="points">The returned keypoints from GetPoints().</param>
        /// <param name="i">Acts in the method as a timer. Set this to zero if the player is facing right, keypoints.Count - 1 if facing left. Set this in OnSpawn.</param>
        /// <param name="projOffset">An offset for the position of the projectile.</param>
        /// <param name="upswing">Use in conjunction with a modplayer variable. Whether the projectile swings up or not.</param>
        /// <param name="rotOffset">An offset for the rotation of the projectile, measured in radians</param>
        /// <returns>The projectile's center with the right movement applied from the points.</returns>
        public Vector2 CalculateSwordSwingPointsAndApplyRotation(Projectile projectile, Vector2 vectorToMouse, Player owner, List<Vector2> points, ref int i, Vector2 projOffset = default, bool upswing = false, float rotOffset = 0)
        {
            if (projOffset == default)
            {
                projOffset = Vector2.Zero;
            }

            if (i < points.Count - 1)
            {
                i++;
                projectile.rotation = owner.Center.DirectionTo(owner.Center + points[i]).ToRotation() + owner.Center.DirectionTo(vectorToMouse).ToRotation() + rotOffset + MathHelper.PiOver4;
            }
            else
            {
                projectile.Kill();
            }

            return owner.Center + points[i].RotatedBy(owner.Center.DirectionTo(vectorToMouse).ToRotation()) + projOffset;
        }
        public static List<Vector2> CalculateVelocitiesFromPoints(List<Vector2> points)
        {
            List<Vector2> velocities = new();
            for (int i = 1; i < points.Count; i++)
            {
                velocities.Add(points[i] - points[i - 1]);
            }
            return velocities;
        }
        /// <summary>
        /// Sets common variables that most held projectiles have. Call this in AI.
        /// </summary>
        /// <param name="projectile">The projectile.</param>
        /// <param name="owner">The projectile's owner.</param>
        /// <param name="mouse">Make sure to only capture Main.Mouseworld as a variable when the projectile spawns in OnSpawn, dont use Main.Mouseworld in the AI hook.</param>
        public void SetAiDefaults(Projectile projectile, Player owner, Vector2 mouse)
        {
            //cool that i dont have to make these ref 
            projectile.direction = owner.direction;
            projectile.spriteDirection = projectile.direction;
            owner.direction = Math.Sign(owner.DirectionTo(mouse).X);
            //owner.heldProj = projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
        }
        public List<float> CalculateRatios()
        {
            List<float> returnRatios = new();
            for (int i = 0; i < coordsForColorData.Count - 1; i++)
            {
                float current = rotationCenter.AngleTo(coordsForColorData[i]);
                float next = rotationCenter.AngleTo(coordsForColorData[i + 1]);
                returnRatios.Add(next - current);
            }
            for (int i = 0; i < returnRatios.Count; i++)
            {
                float maxRotation = rotationCenter.AngleTo(coordsForColorData[^1]) - rotationCenter.AngleTo(coordsForColorData[0]);
                returnRatios[i] = returnRatios[i] / maxRotation;
            }
            return returnRatios;
        }
    }
    public enum KeyFrameInterpolationCurve
    {
        Lerp,
        Slerp,
        Bezier,
        Custom,
        CompleteCustom
    }
}