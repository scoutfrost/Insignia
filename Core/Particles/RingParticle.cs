using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace Insignia.Core.Particles
{
    internal class RingParticle : Particle
    {
        public RingParticle(float scale, Vector2 pos, Color color, float angularVel, int maxTime = 60)
        {
            Velocity = Vector2.Zero;
            Size = scale;
            Position = pos;
            Color = color;
            TimeLeft = maxTime;
            AngularVelocity = angularVel;
        }
        public override void Update()
        {
            Size *= (float)Math.Sin(TimeLeft) / 100;
        }
    }
}
