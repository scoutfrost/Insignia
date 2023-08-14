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
        int lightIntensity;
        public GenericGlowParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime = 60, int lightIntensity = 1)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Size = scale;
            TimeLeft = maxTime;
            MathHelper.Clamp(lightIntensity, 1, 255);
            lightIntensity = 256 - lightIntensity;
            this.lightIntensity = lightIntensity;
            TextureName = "GenericGlowParticle";
        }
        public override void Update()
        {
            Lighting.AddLight(Position, Color.R / 255 / lightIntensity, Color.G / 255 / lightIntensity, Color.B / 255 / lightIntensity);
            Velocity = Velocity.RotatedByRandom(MathHelper.ToRadians(5)) * 0.99f;
            Size *= 0.97f;
        }
    }
}