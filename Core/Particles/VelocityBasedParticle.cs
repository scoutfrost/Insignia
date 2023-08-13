using System;
using Insignia.Core.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Insignia
{
	public class VelocityBasedParticle : Particle
	{
		private float stretchAmount;
		Vector2 scale;
		public VelocityBasedParticle(float stretchAmount, Color color, Vector2 velocity, Vector2 pos, float rotation, Vector2 scale, int alpha)
		{
			this.stretchAmount = stretchAmount;
			Color = color;
			Velocity = velocity;
			Position = pos;
			Angle = rotation;
			this.scale = scale;
            Alpha = alpha;
            TextureName = "VelocityBasedParticle";
        }
        public override void Update()
        {
			Velocity *= 0.97f;
            scale = Velocity / 5 * stretchAmount;
            scale *= 0.99f;
            Lighting.AddLight(Position, Color.R / 255, Color.G / 255, Color.B / 255);
        }
        public override void CustomDraw(SpriteBatch sb)
        {
            //string texturePath = (this.GetType().Namespace + "." + TextureName).Replace('.', '/');
            //Texture = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.ImmediateLoad).Value;

            float alpha = MathHelper.Clamp((float)Alpha, 0f, 255f);

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
			Main.EntitySpriteDraw(Texture, Position - Main.screenPosition, null, Color * (1 - alpha / 255), Angle, Texture.Size() / 2, scale, SpriteEffects.None);
			sb.End();
        }
    }
}

