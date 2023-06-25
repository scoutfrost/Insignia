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
        public override bool ShouldBasicDraw => true;
        public GenericPrimTrail(Color color, Vector2[] points, float width)
        {
            Color = color;
            Points = points;
            Width = width;
        }
    }
}
