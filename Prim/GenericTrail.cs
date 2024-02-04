using Insignia.Core.Common.Systems;
using Microsoft.Xna.Framework;

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