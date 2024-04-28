using Microsoft.Xna.Framework;
using System;

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