using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Insignia.Prim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Core.Common.Systems;
using Terraria.DataStructures;

namespace Insignia.Content.Items.Weapons.Sets.Glacial
{
    internal class TestProj : ModProjectile
    {
		public override string Texture => "Insignia/Content/Items/Weapons/Sets/Glacial/GlacialBasher";
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0; 
		}
		public override void SetDefaults()
		{
			Projectile.width = 30;
			Projectile.height = 30; 
			Projectile.friendly = true; 
			Projectile.hostile = false; 
			Projectile.DamageType = DamageClass.Melee; 
			Projectile.light = 0.5f;
			Projectile.timeLeft = 360;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
		}
		int i = 0;
		List<Vector2> keypoints = new();
		Vector2 mouse;
        public override void OnSpawn(IEntitySource source)
        {
			ProjKeyFrameHandler keyFrameHandler = new(KeyFrameInterpolationCurve.Slerp, "Insignia/Content/Items/Weapons/Sets/Glacial/SwingPoints");
			keypoints = keyFrameHandler.GetPoints(40);
			mouse = Main.MouseWorld;
		}
		float rot;
		public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (i < keypoints.Count - 1) {
                i++;
				if (player.direction == 1)
				{
					rot = MathHelper.PiOver4;
					//Main.NewText(player.direction);
				}
				else if (player.direction == -1)
				{
					rot = 0;
				}

				Projectile.rotation = player.Center.DirectionTo(player.Center + keypoints[i]).ToRotation() + rot + player.Center.DirectionTo(mouse).ToRotation();

			}
            else {
				Projectile.Kill();
            }
			Dust d = Dust.NewDustPerfect(Main.LocalPlayer.Center + (keypoints[i].RotatedBy(player.Center.DirectionTo(mouse).ToRotation()) * player.direction), DustID.Adamantite, Vector2.Zero);
			d.noGravity = true;
			Projectile.Center = player.Center + (keypoints[i].RotatedBy(player.Center.DirectionTo(mouse).ToRotation()));
        }
    }
}

