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
        float scaleMult;
        public SparkleParticle(Color color, float scale, Vector2 position, Vector2 velocity, int alpha, int maxTime = 60, float scaleMultiplier = 0.97f)
        {
            Color = color;
            Size = scale;
            Position = position;
            Velocity = velocity;
            TimeLeft = maxTime;
            Alpha = alpha;
            scaleMult = scaleMultiplier;
            TextureName = "SparkleParticle";
        }
        public override void Update()
        {
            Lighting.AddLight(Position, Color.R / 255, Color.G / 255, Color.B / 255);
            Velocity = Velocity.RotatedByRandom(MathHelper.ToRadians(2)) * 0.99f;
            Size *= scaleMult;
            AngularVelocity = Velocity.ToRotation();
        }
    }
}
