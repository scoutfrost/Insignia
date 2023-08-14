using System;
using Insignia.Core.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Insignia.Core.Particles
{
	internal class VelocityBasedParticle : Particle
	{
		private float stretchAmount;
		Vector2 scale;
        float scaleFalloff;
        public override bool ShouldCustomDraw => true;
        public VelocityBasedParticle(float stretchAmount, Color color, Vector2 velocity, Vector2 pos, Vector2 scale, int alpha, float scaleFalloff, int timeLeft = 60)
		{
			this.stretchAmount = stretchAmount;
			Color = color;
			Velocity = velocity;
			Position = pos;
			this.scale = scale;
            Alpha = alpha;
            TimeLeft = timeLeft;
            Angle = Velocity.ToRotation();
            this.scaleFalloff = scaleFalloff;

            Size = 1; // setting size even though its not in use so particle doesnt despawn
            TextureName = "VelocityBasedParticle";
        }
        public override void Update()
        {
            MathHelper.Clamp(scaleFalloff, 0, 1);
            Velocity *= scaleFalloff;
            scale = new Vector2(Velocity.Length() / 10 * stretchAmount, scale.Y);
            scale.Y *= 0.99f;
            Lighting.AddLight(Position, Color.R / 255, Color.G / 255, Color.B / 255);
        }
        public override void CustomDraw(SpriteBatch sb)
        {
            float alpha = MathHelper.Clamp((float)Alpha, 0f, 255f);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			Main.EntitySpriteDraw(Texture, Position - Main.screenPosition, null, Color * (1 - alpha / 255), Angle, Texture.Size() / 2, scale, SpriteEffects.None);
			sb.End();
        }
    }
}



