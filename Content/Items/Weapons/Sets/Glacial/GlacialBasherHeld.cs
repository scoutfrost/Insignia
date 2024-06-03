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
using System.Runtime.CompilerServices;

namespace Insignia.Content.Items.Weapons.Sets.Glacial
{
    internal class GlacialBasherHeld : ModProjectile
    {
		public override string Texture => "Insignia/Content/Items/Weapons/Sets/Glacial/GlacialBasher";
		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
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
			Projectile.timeLeft = 3600;
			Projectile.tileCollide = false;
		}
		GenericPrimTrail primtrail;
        public override void OnSpawn(IEntitySource source)
        {
            primtrail = new(new(200, 200, 200, 1), Projectile.oldPos, 10);
        }
        public override void AI()
        {
			Projectile.Center = Main.MouseWorld;
			Dust d = Dust.NewDustPerfect(Main.MouseWorld, DustID.Adamantite, Vector2.Zero);
			d.noGravity = true;
			Dust d1 = Dust.NewDustPerfect(Main.MouseWorld + (Vector2.UnitX * 10).RotatedBy(MathHelper.PiOver2), DustID.KryptonMoss, Vector2.Zero);
			d1.noGravity = true;
			Dust d2 = Dust.NewDustPerfect(Main.MouseWorld + (Vector2.UnitX * 10).RotatedBy(-MathHelper.PiOver2), DustID.AncientLight, Vector2.Zero);
			d2.noGravity = true;


			//Main.NewText(ProjectileID.Sets.TrailCacheLength[Projectile.type]);
		}
        public override bool PreDraw(ref Color lightColor)
        {
            primtrail.Draw();
			return true;
		}
    }
}
