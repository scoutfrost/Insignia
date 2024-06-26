﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NATUPNPLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Insignia.Helpers
{
    internal static class ProjectileDrawHelper
    {
        public static void QuickDrawProjectile(Projectile Projectile, float? offsetX, float? offsetY, string texPath, Color drawColor, float scale)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1 || Projectile.direction == -1)
                spriteEffects = SpriteEffects.FlipVertically;

            Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(texPath);

            int frameHeight = tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, tex.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
            if (offsetX != null)
                origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);
            if (offsetY != null)
                origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);

            Main.EntitySpriteDraw(tex,
                 Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, scale, spriteEffects, 0);
        }
        public static void QuickDrawProjectile(Projectile Projectile, float? offsetX, float? offsetY, string texPath, Color drawColor, Vector2 scale, SpriteEffects sE = default)
        {
            Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(texPath);

            int frameHeight = tex.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new(0, startY, tex.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() / 2f;
            if (offsetX != null)
                origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX);
            if (offsetY != null)
                origin.Y = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Height - offsetY : offsetY);

            Main.EntitySpriteDraw(tex,
                 Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, scale, sE, 0);
        }
    }
}