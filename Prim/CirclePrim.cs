using Insignia.Core.Common.Systems;
using Insignia.Core.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Utilities.Terraria.Utilities;

namespace Insignia.Prim
{
    internal class CirclePrim : PrimTrail
    {
        public Vector2 center;
        public float radius;
        public float particleCount;
        public float rotation;
        public void SetData(Color color, Vector2 center, float radius, float width, int pointCount, bool pixelated = false, float rotation = 0, float particleCount = 0)
        {
            this.center = center;
            this.radius = radius;
            Color = color;
            Points = new Vector2[pointCount];
            Width = width;
            Pixelated = pixelated;
            this.rotation = rotation;
            this.particleCount = particleCount;
            for (int i = 0; i < particleCount; i++)
            {
                GenericGlowParticle p = new((Vector2.UnitX.RotatedBy(MathHelper.TwoPi / (Points.Length - 1.1f) * i) * radius).RotatedBy(rotation), Vector2.Zero, Color.AliceBlue, 1);
                ParticleSystem.GenerateParticle(p);
            }
        }
        protected override void Update()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] = center + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / (Points.Length - 1.1f) * i) * radius).RotatedBy(rotation);
            }
        }
    }
}
