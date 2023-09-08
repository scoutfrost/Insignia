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
        public static Vector2 Slerp(Vector2 start, Vector2 end, float t)
        {
            return new Vector2((float)Math.Cos(t), (float)Math.Sin(t));
        }
    }
}
