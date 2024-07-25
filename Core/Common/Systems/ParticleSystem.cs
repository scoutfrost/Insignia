using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Core.Particles;
using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Terraria.ID;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using ReLogic.Threading;

namespace Insignia.Core.Common.Systems
{
    public static class ParticleSystem
    {
        static GraphicsDevice gD = Main.graphics.GraphicsDevice;
        static SpriteBatch sB = Main.spriteBatch;
        static RenderTarget2D renderTarget = new(Main.graphics.GraphicsDevice, gD.PresentationParameters.BackBufferWidth, gD.PresentationParameters.BackBufferHeight);
        static List<Particle> particles = new();
        static List<Particle> metaBalls = new();
        static List<Type> metaballTypes = new();
        static readonly int maxParticles = 1000;
        public static void GenerateParticle(params Particle[] p)
        {
            foreach (Particle particle in p)
            {
                if (particle != null)
                {
                    particles.Add(particle);
                    string texturePath = (particle.GetType().Namespace + "." + particle.TextureName).Replace('.', '/');
                    particle.Texture = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.ImmediateLoad).Value;
                }
            }
        }
        public static void UpdateParticles()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                Particle Particle = particles[i];
                Particle.Update();
                Particle.Position += Particle.Velocity;
                Particle.TimeLeft--;
                Particle.Angle += Particle.AngularVelocity;
                if (particles[i].TimeLeft <= 0 || particles.Count >= maxParticles || Particle.Kill || Particle.Size <= 0.00001f)
                {
                    particles.RemoveAt(i);
                    i--;
                    metaBalls.Remove(Particle);
                }
            }
        }
        public static void Draw()
        {
            foreach (Particle particle in particles)
            {
                if (particle == null || particle.TextureName == null)
                    continue;

                particle.Alpha ??= 0;
                
                float alpha = MathHelper.Clamp((float)particle.Alpha, 0f, 255f);

                if (particle.ShouldCustomDraw == false)
                {
                    if (particle.isMetaBall == true)
                    {
                        if (!metaBalls.Contains(particle))
                            metaBalls.Add(particle);

                        if (!metaballTypes.Contains(particle.GetType())) 
                            metaballTypes.Add(particle.GetType());
                    }
                    else
                    {
                        sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                            RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                        Main.EntitySpriteDraw(particle.Texture, particle.Position - Main.screenPosition, particle.Texture.Bounds, particle.Color * (1 - alpha / 255),
                            particle.Angle, particle.Texture.Size() / 2, particle.Size, SpriteEffects.None, default);

                        sB.End();
                    }
                }
                else
                {
                    particle.CustomDraw(Main.spriteBatch);
                }
            }
        }
        public static void DrawMetaBalls()
        {
            for (int i = 0; i < metaballTypes.Count; i++)
            {
                if (metaBalls.Count > 0)
                {
                    Color outlineColor = Color.White;
                    Main.graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
                    RenderTargetBinding[] previousRTs = gD.GetRenderTargets();

                    gD.SetRenderTarget(renderTarget);
                    gD.Clear(Color.Transparent);

                    sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                        RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                    for (int j = 0; j < metaBalls.Count; j++)
                    {
                        if (metaBalls[j].GetType() == metaballTypes[i])
                        {
                            Particle particle = metaBalls[j];

                            float alpha = MathHelper.Clamp((float)particle.Alpha, 0f, 255f);

                            sB.Draw(particle.Texture, particle.Position - Main.screenPosition, particle.Texture.Bounds, particle.Color * (1 - alpha / 255),
                                particle.Angle, particle.Texture.Size() / 2, particle.Size, SpriteEffects.None, 0);

                            outlineColor = particle.outlineColor;
                        }
                    }
                    sB.End();

                    gD.SetRenderTargets(previousRTs);

                    GameShaders.Misc["OutlineShader"].Shader.Parameters["outlineColor"].SetValue(outlineColor.ToVector4());
                    GameShaders.Misc["OutlineShader"].Shader.Parameters["uImageSize"].SetValue(renderTarget.Size() / 2);

                    sB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                        RasterizerState.CullNone, GameShaders.Misc["OutlineShader"].Shader);

                    sB.Draw(renderTarget, Vector2.Zero, Color.White);

                    sB.End();
                }
            }
        }
        public static void AnimateFromVerticalSpritesheet(Particle particle, int numberOfFrames, int animSpeed = 5, int? animTimer = null)
        {
            if (particle != null)
            {
                animTimer ??= particle.TimeLeft;
                int whichFrame = (int)(animTimer / animSpeed % numberOfFrames);
                particle.Frame = Utils.Frame(particle.Texture, 1, numberOfFrames, 0, whichFrame, 0, 0);
            }
        }
        public static void Unload()
        {
            particles = null;
            renderTarget.Dispose();
        }
    }
}

