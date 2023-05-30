using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Core.Particles;

using Terraria;
using Terraria.ModLoader;

namespace Insignia.Core.Common.Systems
{
    public static class ParticleSystem
    {
        public static List<Particle> particles = new List<Particle>();
        private static readonly int maxParticles = 500;
        public static void GenerateParticle(Particle particle)
        {
            if (particle != null)
                particles.Add(particle);
        }
        public static void UpdateParticles()
        {
            for (int particle = 0; particle < particles.Count; particle++)
            {
                Particle Particle = particles[particle];
                Particle.Update();
                Particle.Position += Particle.Velocity;
                Particle.TimeLeft--;
                Particle.Angle += Particle.AngularVelocity;
                if (particles[particle].TimeLeft <= 0 || particles.Count >= maxParticles || Particle.Kill || Particle.Size <= 0.01f)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }
            }
        }
        public static void Draw()
        {
            foreach (Particle Particle in particles)
            {
                if (Particle == null || Particle.TextureName == null)
                    continue;

                string texturePath = (Particle.GetType().Namespace + "." + Particle.TextureName).Replace('.', '/');
                Particle.Texture = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                if (Particle.Opacity == null)
                    Particle.Opacity = 1;
                float opacity = MathHelper.Clamp((float)Particle.Opacity, 0f, 1f);

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                    RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                Main.EntitySpriteDraw(Particle.Texture, Particle.Position - Main.screenPosition, Particle.Texture.Bounds, Particle.Color * opacity,
                    Particle.Angle, Particle.Texture.Size() / 2, Particle.Size, SpriteEffects.None, default);
                Main.spriteBatch.End();
            }
        }
        public static void AnimateFromVerticalSpritesheet(Particle particle, int numberOfFrames, int animSpeed = 5, int? animTimer = null)
        {
            if (particle != null)
            {
                if (animTimer == null)
                    animTimer = particle.TimeLeft;
                int whichFrame = (int)(animTimer / animSpeed % numberOfFrames);
                particle.Frame = Utils.Frame(particle.Texture, 1, numberOfFrames, 0, whichFrame, 0, 0);
            }
        }
        public static void Unload()
        {
            particles = null;
        }
    }
}
