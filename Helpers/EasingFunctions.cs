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
        public static Vector2 Slerp(Vector2 start, Vector2 end, float t, float radius = default)
        {
            return radius == default ? (end - start).DirectionTo(start.RotatedBy(t, end - start)) : (end - start).DirectionTo(start.RotatedBy(t, end - start)) * radius;
            //return Utils.AngleLerp(start.ToRotation(), end.ToRotation(), t).ToRotationVector2(); alt implementations
            // return new((float)Math.Cos(t), (float)Math.Sin(t)); //* radius; 
        }
    }
}
