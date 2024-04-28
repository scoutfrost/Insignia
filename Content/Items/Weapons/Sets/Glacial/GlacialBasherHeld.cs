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
using Terraria.DataStructures;
using Insignia.Core.Particles;
using Insignia.Core.Common.Systems;

namespace Insignia.Content.Items.Weapons.Sets.Glacial
{
    internal class GlacialBasherHeld : ModProjectile
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
		}
        public override void OnSpawn(IEntitySource source)
		{
			GenericGlowParticle particle = new(Projectile.Center, Main.rand.NextVector2Unit() * 1.2f, Color.MistyRose, 0.4f, 120);
			ParticleSystem.GenerateParticle(particle);
		}
        public override void AI()
        {
			Player player = Main.player[Projectile.owner];
			Projectile.velocity = player.DirectionTo(Main.MouseWorld) * 10;
		}
        public override bool PreDraw(ref Color lightColor)
		{ 
			GenericPrimTrail primtrail = new(new(200,200,200,1), Projectile.oldPos, 10);
			primtrail.Draw();
			return true;
		}
    }
}
