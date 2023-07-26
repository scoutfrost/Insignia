using Insignia.Content.Items.Weapons.Sets.Torgustus;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Insignia.Core.Particles;
using Insignia.Core.Common.Systems;
using System;
using System.Collections.Generic;
using Insignia.Helpers;
using ReLogic.Content;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Prim;

namespace Insignia.Content.Items.Weapons.Sets.Torgustus
{
    public class TorgustusGreatbow : ModItem
    {
        public override void SetStaticDefaults() => CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        public override bool AltFunctionUse(Player player) => !player.HasBuff(ModContent.BuffType<PoweredTorgustusBowCooldown>());
        public override void SetDefaults()
        {
            Item.damage = 27;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = 750000;
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<TorgustusBowProj>();
            Item.crit = 4;
            Item.shootSpeed = 0;
            Item.useAmmo = AmmoID.Arrow;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useAnimation = 2;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useAnimation = 10;
                Item.useTime = 10;

                for (int i = 0; i < 5; i++)
                {
                    GenericGlowParticle particle = new(new Vector2(player.Center.X + Main.rand.Next(-30, 30), player.Center.Y), new Vector2(0, -Main.rand.NextFloat(0.7f, 1.2f)), Color.LightYellow, 0.5f, 120);
                    SparkleParticle sparkle = new(Color.LightYellow, 1, new Vector2(player.Center.X + Main.rand.Next(-30, 30), player.Center.Y), new Vector2(0, -Main.rand.NextFloat(0.7f, 1.2f)), 120);

                    ParticleSystem.GenerateParticle(sparkle);
                    ParticleSystem.GenerateParticle(particle);
                }
                player.GetModPlayer<TorgustusBowPlayer>().hasRightClicked = true;
                player.AddBuff(ModContent.BuffType<PoweredTorgustusBowCooldown>(), 1200, true, false);
            }
            return player.ownedProjectileCounts[Mod.Find<ModProjectile>("TorgustusBowProj").Type] < 1;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.myPlayer == player.whoAmI && player.altFunctionUse != 2) {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<TorgustusBowProj>(), damage, knockback, player.whoAmI);
            }
            return false;
        }
    }
    public class TorgustusBowProj : ModProjectile
    {
        public override string Texture => "Insignia/Content/Items/Weapons/Sets/Torgustus/TorgustusBowProj";
        public override void SetStaticDefaults() => Main.projFrames[Projectile.type] = 3;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        bool rightClicked;
        int strongArrowIndex;
        Player player => Main.player[Projectile.owner];
        Projectile strongArrow;
        ref float strongArrowTimer => ref Projectile.ai[0];
        public override void OnSpawn(IEntitySource source)
        {
            rightClicked = player.GetModPlayer<TorgustusBowPlayer>().hasRightClicked;
            player.GetModPlayer<TorgustusBowPlayer>().poweredShotCount++;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            int frameDelay = (int)(rightClicked ? 30 * player.GetAttackSpeed(DamageClass.Ranged) : 8 * player.GetAttackSpeed(DamageClass.Ranged));
            strongArrowTimer++;
            player.heldProj = Projectile.whoAmI;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.direction = player.direction;
                Projectile.spriteDirection = Projectile.direction;
                player.direction = Math.Sign(player.DirectionTo(Main.MouseWorld).X);
                player.heldProj = Projectile.whoAmI;
                player.itemTime = 2;
                player.itemAnimation = 2;
                Projectile.rotation = player.Center.DirectionTo(Main.MouseWorld).ToRotation();
                Projectile.Center = player.Center + player.Center.DirectionTo(Main.MouseWorld).ToRotation().ToRotationVector2() * 10;
                Projectile.netUpdate = true;
                if (!player.channel) {
                    Projectile.Kill();
                }
                Projectile.frameCounter++;

                if (Projectile.frameCounter % frameDelay == 0) {
                    Projectile.frame++;
                }
                if (Projectile.frame == 3 && !rightClicked) {
                    Shoot();
                }
                if (Projectile.frame >= 3)
                {
                    if (rightClicked)
                    {
                        Projectile.frame = 2;
                        Projectile.frameCounter = 0;
                    }
                    else
                    {
                        Projectile.frame = 0;
                        Projectile.frameCounter = 0;
                    }
                }
                if (rightClicked && Projectile.frame == 0 && Main.myPlayer == Projectile.owner)
                {
                    if (strongArrowTimer == 1) {
                        strongArrowIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TorgustusArrow>(), 0, 1, Projectile.owner, 0, 0);
                        player.GetModPlayer<TorgustusBowPlayer>().arrowType = ModContent.ProjectileType<TorgustusArrow>();
                    }
                    strongArrow = Main.projectile[strongArrowIndex];
                }
                if (rightClicked)
                {
                    if (strongArrow != null)
                    {
                        strongArrow.rotation = Projectile.rotation;
                        strongArrow.Center = Projectile.Center + new Vector2(10 * player.direction ,0);
                    }
                }

                if (rightClicked && player.channel && strongArrowTimer == frameDelay * 3) {
                    for (int i = 0; i < 40; i++)
                    {
                        GenericGlowParticle particle = new(player.Center, Main.rand.NextVector2Unit() * 1.2f, Color.MistyRose, 0.4f, 120);
                        ParticleSystem.GenerateParticle(particle);
                    }
                }

                if (!player.channel && rightClicked)
                {
                    if (strongArrowTimer >= frameDelay * 3) {
                        strongArrow.velocity = Projectile.DirectionTo(Main.MouseWorld) * 25;
                        strongArrow.timeLeft = 300;
                    }
                    else {
                        strongArrow.velocity = Projectile.DirectionTo(Main.MouseWorld) * 3 * (strongArrowTimer / frameDelay * 3);
                        strongArrow.timeLeft = 120;
                    }
                    strongArrow.damage = (int)strongArrow.velocity.LengthSquared() / 7;
                    strongArrow.penetrate = (int)strongArrow.velocity.Length() / 7;
                }


                if (Projectile.frame == 0) {
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
                }
                if (Projectile.frame == 1) {
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);
                }
                if (Projectile.frame == 2) {
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color color = rightClicked ? Color.FloralWhite : lightColor;
            if (rightClicked) {
                ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, new Color(255, 80, 40, 0), 1.2f);
            }
            ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, color, 1);
            return false;
        }
        void Shoot()
        {
            if (!player.PickAmmo(player.HeldItem, out int type, out float speed, out int damage, out float knockBack, out int ammoItemID, false))
            {
                Projectile.Kill();
                Projectile.active = false;
            }

            SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
            if (!rightClicked) {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.DirectionTo(Main.MouseWorld) * 12, type, damage, 1, Projectile.owner, 0, 0);
            }

            player.GetModPlayer<TorgustusBowPlayer>().arrowType = type;
        }
    }
    internal class TorgustusArrow : ModProjectile
    {
        public override string Texture => "Insignia/Content/Items/Weapons/Sets/Torgustus/TorgustusArrow";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }
        Vector2 mouse;

        Player player => Main.player[Projectile.owner];
        public override void OnSpawn(IEntitySource source)
        {
            mouse = Main.MouseWorld;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void AI()
        {
            if (Projectile.ai[1] == 1) { // aka checking if its been through a portal
                Projectile.velocity *= 1f;
            }
            if (Projectile.velocity.Length() < 25) {
                Projectile.velocity *= 0.97f;
            }
        }
        private static Asset<Texture2D> telegraphTexture;
        public override void Load()
        {
            telegraphTexture = ModContent.Request<Texture2D>("Insignia/Content/Items/Weapons/Sets/Torgustus/TorgustusBowTelegraph");
        }
        public override void Unload() 
        { 
            telegraphTexture = null; 
        }

        int primTimer = 0;
        float lengthOfHitlineIntersect;
        float collisionPoint = 0;
        bool collidingWithPortal;
        ref float ScaleTimer => ref Projectile.ai[0];
        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, Color.LightYellow, 1);

            List<Projectile> portals = player.GetModPlayer<TorgustusPortalPlayer>().portalsActive;
            if (Projectile.ai[1] == 1)
            {
                primTimer++;

                if (primTimer > 20)
                {
                    GenericPrimTrail primtrail = new(new(220, 110, 30, 150), Projectile.oldPos, 5);
                    primtrail.Draw();
                }

                if (Projectile.velocity.Length() > 25)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 offset = i % 2 == 0 ? new Vector2(i * 3, 20) : new Vector2(i * 3, -20);
                        Vector2 dustVel = (Projectile.Center + offset).DirectionTo(Projectile.Center + new Vector2(40 + Projectile.velocity.Length(), 0));

                        Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.RainbowTorch, dustVel * Projectile.velocity.Length() / 4, 0, Color.White, 2);
                        dust.noGravity = true;
                    }
                }
                lengthOfHitlineIntersect = 0;
            }

            ScaleTimer += 0.1f;
            ScaleTimer = Math.Clamp(ScaleTimer, 0, 1.5f);
            Vector2 scale = collidingWithPortal ? new(0.5f + ScaleTimer, 1) : new(-0.5f - ScaleTimer, -1);

            float hitLength = telegraphTexture.Width();
            Vector2 hitLine = Projectile.Center + Projectile.rotation.ToRotationVector2() * hitLength;
            //Dust.NewDustPerfect(hitLine, DustID.Adamantite, Vector2.Zero, 0, Color.AliceBlue, 1);

            Vector2 drawPos = collidingWithPortal ? new(85, 34) : new(-39, -30);
            Vector2 drawingVectors = -Main.screenPosition + drawPos.RotatedBy(Projectile.rotation) + new Vector2(0, player.gfxOffY);

            var portal = portals.Where(proj => proj.active && Collision.CheckAABBvLineCollision(proj.Center, proj.Hitbox.Size(), Projectile.Center, hitLine, 8, ref collisionPoint));
            foreach (var proj in portal)
            {
                float yPosOfIntersection = player.direction == 1 ? proj.Hitbox.TopLeft().Y : proj.Hitbox.TopRight().Y;
                Vector2 cornerOfPortal = player.direction == 1 ? proj.Hitbox.TopLeft() : proj.Hitbox.TopRight();

                Vector2 vector2HitLineInHitbox = hitLine.RotatedBy(player.direction == 1 ? -Projectile.rotation : 0, player.Center);
                vector2HitLineInHitbox.Y = yPosOfIntersection;

                lengthOfHitlineIntersect = (vector2HitLineInHitbox - cornerOfPortal).Length();

                collidingWithPortal = true;
            }

            if (portal.Count() <= 0)
            {
                lengthOfHitlineIntersect = 0;
                collidingWithPortal = false;
            }

            if (lengthOfHitlineIntersect > hitLength) {
                lengthOfHitlineIntersect = hitLength;
            }
            var exitPortal = portals.Where(proj => proj.active && !Collision.CheckAABBvLineCollision(proj.Center, proj.Hitbox.Size(), Projectile.Center, hitLine, 8 * Projectile.scale, ref collisionPoint));
            if (collidingWithPortal && exitPortal.Any())
            {
                foreach (var proj in exitPortal)
                {
                    //drawing exit telegraph
                    Main.EntitySpriteDraw(telegraphTexture.Value, proj.Center + drawingVectors + new Vector2(23 * player.direction, 0),
                        new Rectangle(0, 0, (int)lengthOfHitlineIntersect, telegraphTexture.Height()),
                        Color.LightGoldenrodYellow * 0.7f, Projectile.rotation + MathHelper.Pi, telegraphTexture.Size(), collidingWithPortal ? -scale : scale, SpriteEffects.FlipHorizontally, default);
                }
            }
            else {
                lengthOfHitlineIntersect = 0;
            }
            //drawing telegraph on way in portal 
            Main.EntitySpriteDraw(telegraphTexture.Value, Projectile.Center + drawingVectors,
                new Rectangle(0, 0, telegraphTexture.Width() - (int)lengthOfHitlineIntersect, telegraphTexture.Height()),
                Color.LightGoldenrodYellow * 0.7f, Projectile.rotation, telegraphTexture.Size(), scale, SpriteEffects.None, default);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            if (Projectile.velocity.Length() > 5)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 offset = i % 2 == 0 ? new Vector2(i * 3, 20) : new Vector2(i * 3, -20);
                    Vector2 dustVel = (Projectile.Center + offset).DirectionTo(Projectile.Center + new Vector2(40 * player.direction + Projectile.velocity.Length(), 0));
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.RainbowTorch, dustVel * Projectile.velocity.Length() / 4, 0, Color.White, 2);
                    dust.noGravity = true;
                }
            }
            SparkleParticle particle = new(Color.FloralWhite, 1, Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi), 100);
            ParticleSystem.GenerateParticle(particle);
        }
    }
    public class PoweredTorgustusBowCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
    }
    internal class TorgustusBowPlayer : ModPlayer
    {
        public int poweredShotCount;
        public int arrowType;
        public bool hasRightClicked;
        public override void ResetEffects()
        {
            if (poweredShotCount >= 5) {
                hasRightClicked = false;
                poweredShotCount = 0;
            }
        }
        public override void Unload()
        {
            poweredShotCount = 0;
            hasRightClicked = false;
        }
    }
}