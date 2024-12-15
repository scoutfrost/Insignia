using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Insignia.Core.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Insignia.Prim
{
    internal class GenericPrimTrail : PrimTrail
    {
        public void SetData(Color color, Vector2[] points, float width, bool widthFallOff = true, Effect shader = default, bool pixelated = false)
        {
            Color = color;
            Points = points;
            Width = width;
            WidthFallOff = widthFallOff;
            Shader = shader;
            Pixelated = pixelated;
        }
    }
}