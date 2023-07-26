using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Insignia.Core.Particles
{
    internal class SparkleParticle : Particle
    {
        public SparkleParticle(Color color, float scale, Vector2 position, Vector2 velocity, float opacity, int maxTime = 60)
        {
            Color = color;
            Size = scale;
            Position = position;
            Velocity = velocity;
            TimeLeft = maxTime;
            TextureName = "SparkleParticle";
            Opacity = opacity;
        }
        public override void Update()
        {
            Lighting.AddLight(Position, Color.R / 255, Color.G / 255, Color.B / 255);
            Velocity = Velocity.RotatedByRandom(MathHelper.ToRadians(2)) * 0.99f;
            Size *= 0.97f;
            AngularVelocity = Velocity.ToRotation();
        }
    }
}
