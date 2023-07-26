using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Graphics.Renderers;
using Insignia.Core.Particles;

namespace Insignia.Core.Particles
{
    public class GenericGlowParticle : Particle
    {
        public GenericGlowParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Size = scale;
            TimeLeft = maxTime;
            TextureName = "GenericGlowParticle";
        }
        public override void Update()
        {
            Lighting.AddLight(Position, Color.R / 255, Color.G / 255, Color.B / 255);
            Velocity = Velocity.RotatedByRandom(MathHelper.ToRadians(5)) * 0.99f;
            Size *= 0.97f;
        }
    }
}