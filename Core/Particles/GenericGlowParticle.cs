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
        float scaleMult;
        float velocityMult;
        public GenericGlowParticle(Vector2 position, Vector2 velocity, Color color, float scale, int maxTime = 60, int lightIntensity = 1, float scaleMultiplier = 0.97f, float velocityMultiplier = 0.99f, string altTextureName = default)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Size = scale;
            TimeLeft = maxTime;
            MathHelper.Clamp(lightIntensity, 1, 255);
            lightIntensity = 256 - lightIntensity;
            this.lightIntensity = lightIntensity;
            scaleMult = scaleMultiplier;
            velocityMult = velocityMultiplier;

            if (altTextureName == null)
                TextureName = "GenericGlowParticle";
            else 
                TextureName = altTextureName;
            //isMetaBall = true;
            //outlineColor = Color.White;
        }
        public override void Update()
        {
            Lighting.AddLight(Position, Color.R / lightIntensity * Size, Color.G / lightIntensity * Size, Color.B / lightIntensity * Size);
            Velocity = Velocity.RotatedByRandom(MathHelper.ToRadians(5)) * velocityMult;
            Size *= scaleMult;
        }
    }
}