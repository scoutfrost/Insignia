using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Insignia.Helpers
{
    internal static class EasingFunctions
    {
        public static Vector2 Slerp(Vector2 start, Vector2 end, float t, Vector2 center, float radius = default)
        {
            return radius == default ? (center).DirectionTo(start.RotatedBy(t, center)) : (center).DirectionTo(start.RotatedBy(t, center)) * radius;
            //alt implementations
            //return Utils.AngleLerp(start.ToRotation(), end.ToRotation(), t).ToRotationVector2(); 
            // return new((float)Math.Cos(t), (float)Math.Sin(t)); //* radius; 
        }
        public static Vector2[] Bezier(Vector2[] controlPoints, float t)
        {
            //Vector2 q1 = Vector2.Lerp(a, b, ammount);
            //Vector2 q2 = Vector2.Lerp(b, c, ammount);
            //return Vector2.Lerp(q1, q2, ammount);
            Vector2[] temp = new Vector2[controlPoints.Length - 1];
            int sum = 0;

            for (int j = 0; j < temp.Length; j++) {
                sum += j;
            }
            
            for (int i = 0; i < sum; i++)
            {
                temp = BezierCalculation(temp, t);
            }
            return temp;
            
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
    }
}
