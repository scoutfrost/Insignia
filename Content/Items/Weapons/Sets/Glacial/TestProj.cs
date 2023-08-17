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
			Projectile.timeLeft = 100;
		}
		int i = 0;
		List<Vector2> keypoints = new();
        public override void OnSpawn(IEntitySource source)
        {
			ProjKeyFrameHandler keyFrameHandler = new(KeyFrameInterpolationCurve.Lerp, "Insignia/Content/Items/Weapons/Sets/Glacial/SwingPoints");
			keypoints = keyFrameHandler.GetPoints();

			Main.NewText(keypoints.Count + "real");
		}
        public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			//Main.NewText(keypoints.Count);
			while (i < keypoints.Count * 2 - 1)
			{
				i++;
				if (i % 2 == 0)
				{
					Projectile.Center = player.Center + (keypoints[i / 2]);
				}
			}
		}
    }
}
