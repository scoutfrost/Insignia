using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace Insignia.Helpers
{
    internal static class EasingFunctions
    {
        public static Vector2 RotateVector(Vector2 start, float angle, Vector2 center, float radius = default)
        {
            return radius == default ? center.DirectionTo(start.RotatedBy(angle, center)) : center.DirectionTo(start.RotatedBy(angle, center)) * radius;
            //alt implementations that may or may not work
            //return Utils.AngleLerp(start.ToRotation(), end.ToRotation(), t).ToRotationVector2();
            // return new((float)Math.Cos(t), (float)Math.Sin(t)); //* radius;
        }

        public static Vector2 Bezier(Vector2[] controlPoints, float t)
        {
            //my own implementation of a bezier that takes any number of points
            Vector2[] temp = controlPoints;
            while (temp.Length > 1)
            {
                temp = BezierCalculation(temp, t);
            }
            return temp[0];
        }

        private static Vector2[] BezierCalculation(Vector2[] c, float t)
        {
            Vector2[] temp = new Vector2[c.Length - 1];
            for (int i = 0; i < c.Length - 1; i++)
            {
                temp[i] = Vector2.Lerp(c[i], c[i + 1], t);
            }
            return temp;
        }

        //below functions are taken from https://easings.net/
        public static float EaseInOutQuad(float x)
        {
            return x < 0.5 ? 2 * x * x : 1 - (float)Math.Pow(-2 * x + 2, 2) / 2;
        }

        public static float EaseInOutBack(float x)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return x < 0.5f ? (float)(Math.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2 : (float)(Math.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
        }
    }
}