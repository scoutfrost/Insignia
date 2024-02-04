using Insignia.Core.Common.Systems;
using Insignia.Core.Particles;
using Insignia.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insignia.Content.Items.Weapons.Ranged
{
    public class Cryobite : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1f;
            Item.value = Item.buyPrice(0, 0, 80, 0);

            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
            Item.shootSpeed = 40f;
            Item.shoot = ModContent.ProjectileType<CyroProj>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));

            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<CyroProj>();
        }
        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundSystem.CyroBang with { Volume = 0.5f, PitchVariance = 0.5f }, player.Center);

            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 13f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
            Gore.NewGore(source, player.Center + muzzleOffset * 1, new Vector2(player.direction * -1, -0.5f) * 2, Mod.Find<ModGore>("CryobiteChasingGore").Type, 1f);

            //    Projectile.NewProjectile(player.GetSource_ItemUse(Item), position + muzzleOffset, Vector2.Zero, ModContent.ProjectileType<CryobiteChasing>(), 0, 0, player.whoAmI);

            return true;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextFloat() >= 0.25f;
        }

        public override Vector2? HoldoutOffset()
        {
            var offset = new Vector2(-3, 0);
            return offset;
        }
    }

    /* public class CryobiteChasing : ModProjectile
     {
         private Vector2 flashoffset = Vector2.Zero;

         private Player Owner => Main.player[Projectile.owner];

         private bool FullyUsed = false;

         public override void SetDefaults()
         {
             Projectile.width = 2;
             Projectile.damage = 0;
             Projectile.height = 2;
             Projectile.DamageType = DamageClass.Ranged;
             Projectile.timeLeft = 4;
             Projectile.friendly = false;
             Projectile.aiStyle = 0;
             Projectile.scale = 1f;
             Projectile.alpha = 255;
             Projectile.netImportant = true;
             Projectile.netUpdate = true;
         }

         public override void AI()
         {
             Player player = Main.player[Projectile.owner];

             Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.8f);
             Projectile.rotation = Projectile.ai[0];
             if (!FullyUsed)
             {
                 FullyUsed = true;
                 flashoffset = Projectile.Center - Owner.Center;
             }
             //     Projectile.rotation = player.DirectionTo(Main.MouseWorld).ToRotation;
             Projectile.rotation = player.DirectionTo(Main.MouseWorld).ToRotation();

             Projectile.Center = Owner.Center + flashoffset;
         }

         public override bool PreDraw(ref Color lightColor)
         {
             Texture2D mainTex = TextureAssets.Projectile[Projectile.type].Value;
             Main.spriteBatch.Draw(mainTex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(8, mainTex.Height / 2), Projectile.scale, SpriteEffects.None, 0f);

             Texture2D glowTex = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
             Color glowColor = Color.Orange;
             glowColor.A = 0;
             Main.spriteBatch.Draw(glowTex, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(8, glowTex.Height / 2), Projectile.scale, SpriteEffects.None, 0f);
             return false;
         }
     }*/

    public class CyroProj : ModProjectile
    {
        public override string Texture
          => Helper.Empty;

        private const int timeLeftMax = 18;
        private Vector2 origin;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(12);

            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = timeLeftMax;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            if (Projectile.timeLeft == timeLeftMax)
            {
                origin = Projectile.Center;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 0)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                VelocityBasedParticle velParticle = new(3, Color.LightBlue, (Projectile.velocity / 4).RotatedBy(MathHelper.ToRadians(-10)).RotatedBy(MathHelper.ToRadians(10 * i)), Projectile.Center, Vector2.One, 50, 0.3f);
                SparkleParticle particle = new(Color.LightBlue, 1, Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(25)) * i / 10, 100);
                ParticleSystem.GenerateParticle(particle, velParticle);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float Descaling = (float)
            Projectile.timeLeft / timeLeftMax;
            float ScaleY = 18;
            Color color = Color.LightBlue;

            for (int i = 0; i < 3; i++)
            {
                float shotLength = origin.Distance(Projectile.Center);

                Texture2D texture = (i > 1) ?
                Mod.Assets.Request<Texture2D>("Assets/Effects/Trail_1").Value : Mod.Assets.Request<Texture2D>("Assets/Effects/GlowTrail").Value;
                Vector2 scale = new Vector2(shotLength, MathHelper.Lerp(ScaleY, 5, 1f - Descaling)) / texture.Size();

                //descales by colour
                color = (color with { A = 0 }) * Descaling;
                Main.EntitySpriteDraw(texture, origin - Main.screenPosition, null, color, Projectile.velocity.ToRotation(), new Vector2(0, texture.Height / 2), scale, SpriteEffects.None, 0);

                Texture2D Trailing = Mod.Assets.Request<Texture2D>("Assets/Effects/GradientCirc").Value;
                Vector2 Endscale = origin + (Vector2.UnitX * shotLength).RotatedBy(Projectile.velocity.ToRotation());


                Main.EntitySpriteDraw(Trailing, Endscale - Main.screenPosition, null, color, 0, Trailing.Size() / 2, (0.13f - (i * 0.1f)) * Descaling, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}