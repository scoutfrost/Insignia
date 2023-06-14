using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insignia.Core.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Insignia.Prim
{
    internal class GenericPrimTrail : PrimTrail
    {
        public GenericPrimTrail(Color color, List<Vector2> points, PrimitiveType type, int opacity)
        {
            Color = color;
            Points = points;
            Type = type;
            Opacity = opacity;
        }
    }
}
