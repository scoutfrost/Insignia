using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Insignia.Biomes.ColdBiome.Tiles;
using Insignia.Core.Common.Systems;
using Insignia.Core.Particles;
using Insignia.Helpers;
using Insignia.Prim;
using Microsoft.Build.Construction;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Insignia.Content.Items.Weapons.Ranged
{
    public class IceGrenadeLauncher : ModItem
    {
      public override
            bool CanUseItem(Player 
 player){
             return player.ownedProjectileCounts[Item.shoot] <
                1;
 }
        /// <summary>
        /// Sets the defaults.
        /// </summary>
        public override 
void 
            SetDefaults           ()
 { Item.damage = 25;   Item.DamageType = DamageClass.Ranged; Item.width


                //32
               // / \
               // 8   4
//               / \ / \
     //         4 2  2  2
    ///        / \
    ///        2  2 
                = 32;
            Item.height = 32;
           Item.useStyle = ItemUseStyleID.Shoot;
        //sets the items rare
            Item.rare = ItemRarityID.Green;
              Item.autoReuse = false;
           Item.useAmmo = ItemID.Grenade;
         Item.noMelee = !false; /// not false
            //;
                             Item.shootSpeed = 0f;/// the speed of the shoot
            Item.noUseGraphic = true;
             Item.consumable = false;


            Item.potion = !true;
           Item.useTime = 20;
                                                                                               Item.useAnimation
                = 20;
       Item.stack= 1;
            double ninety = 90;
                Item.damage 
=
                (int)ninety;
                            ;
            Item.shoot = 1              != 1 !=((1                            )!= 
                2 != (1!= 1 )) ?ModContent.ProjectileType<
                       IceGrenadeLauncherProj

 // this is the >(): 
                >():
                (int)Shoot((byte)(float)19);
Item.channel =
true;
}                                                                                                                                                                                                           public override bool Shoot(Player 
             player
,
            EntitySource_ItemUse_WithAmmo
            source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
{
            Projectile.NewProjectile(source, position
, Vector2.Zero, Item.shoot, damage, knockback); return false; }
        public
            float Shoot
            (int integer)
        {
            Item.shoot = 84; return 72;}
public override bool 
CanConsumeAmmo(Item ammo,
Player player)  {  return true
;}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.IllegalGunParts, 1);
            recipe.AddIngredient(ModContent.ItemType<GlacialChunkItem>(), 60);
            recipe.AddIngredient(ModContent.ItemType<CrystalTileItem>(), 60);
            recipe.Register();
        }
    }
    public class IceGrenadeLauncherProj : ModProjectile
    {
        public override string Texture => "Insignia/Content/Items/Weapons/Ranged/IceGrenadeLauncher";
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = -1;
        }
        Player Player => Main.player[Projectile.owner];
        int shotTimer = 0;
        int recoilAnimTimer = 0;
        bool hasShot = false;
        int holdDistance = 20;
        float lerpSpeed = 0.5f;
        int shotInterval = 40;
        int recoilTime = 5;
        int recoilAmount = 3;
        public override bool? CanDamage()
        {
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            shotInterval = (int)(shotInterval * Player.GetAttackSpeed(DamageClass.Ranged));
            Player.RotatedRelativePoint(Player.MountedCenter, false, true);
        }
        public override void AI()
        {
            Player.heldProj = Projectile.whoAmI;

            if (!Player.channel)
                Projectile.Kill();

            Player.ChangeDir(Math.Sign(Main.MouseWorld.X - Player.Center.X));

            Projectile.Center = Vector2.Lerp(Projectile.Center, Player.Center + Player.Center.DirectionTo(Main.MouseWorld) * holdDistance, lerpSpeed);
            Projectile.rotation = Player.Center.DirectionTo(Projectile.Center).ToRotation();
            Projectile.rotation += Player.direction == -1 ? MathHelper.Pi : 0;

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2 * Player.direction);

            if (hasShot)
            {
                Lighting.AddLight(Projectile.Center, Color.LightBlue.ToVector3() / 2);
                recoilAnimTimer++;
                if (recoilAnimTimer > recoilTime)
                {
                    recoilAnimTimer = 0;
                    hasShot = false;
                }
                Projectile.Center -= Projectile.Center.DirectionTo(Main.MouseWorld) * recoilAnimTimer * recoilAmount;
                Projectile.rotation -= MathHelper.ToRadians(recoilAnimTimer) * recoilAmount * Player.direction;
            }

            if (shotTimer++ >= shotInterval)
            {
                Shoot(30);
                hasShot = true;
                shotTimer = 0;
            }
            else
                return;
            
            if (!Player.PickAmmo(Player.HeldItem, out int type, out float speed, out int damage, out float knockBack, out int ammoItemID, false))
            {
                Projectile.Kill();
                Projectile.active = false;
            }

        }
        private void Shoot(int projSpeed)
        {
            GenericGlowParticle muzzleFlash = new(Projectile.Center + Projectile.Center.DirectionTo(Main.MouseWorld) * 30, Vector2.Zero, Color.LightBlue * 0.9f, 0.6f, 2);
            ParticleSystem.GenerateParticle(muzzleFlash);
            for (int i = 0; i < 10; i++)
            {
                GenericGlowParticle p = new(Projectile.Center, Projectile.Center.DirectionTo(Main.MouseWorld) * Main.rand.NextFloat() * 10, Color.AliceBlue, 0.2f, 30, 1, 0.9f);
                ParticleSystem.GenerateParticle(p);
            }
            Projectile.NewProjectile(new EntitySource_Parent(Projectile), Projectile.Center, Projectile.Center.DirectionTo(Main.MouseWorld) * projSpeed, ModContent.ProjectileType<IceGrenadeProjectile>(), Projectile.damage, Projectile.knockBack);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, lightColor, Vector2.One, Player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
    } 
    public class GrenadeAmmoGlobal : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.type == ItemID.Grenade;
        }
        public override void SetDefaults(Item entity)
        {
            entity.ammo = entity.type;
        }
    }
    public class IceGrenadeProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.damage = 1;
        }
        NPC stickedNPC;
        int explosionTimer = 0;
        Vector2 dist = Vector2.Zero;
        public override bool? CanDamage()
        {
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (explosionTimer > 280 && explosionTimer < 285)
            {
                return Color.Blue;
            }
            return null;
        }
        public override void AI()
        {
            Projectile.velocity *= .98f;
            Projectile.velocity.Y += 1;
            MathHelper.Clamp(Projectile.velocity.Y, float.MinValue, 16);
            if (stickedNPC != null && stickedNPC.active)
            {
                Projectile.Center = stickedNPC.Center + dist;
                Projectile.velocity.Y -= 1;
                Projectile.rotation *= 0.6f;
            }
            else
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.Hitbox.Intersects(Projectile.Hitbox) && npc.active && !npc.friendly)
                    {
                        stickedNPC = Main.npc[i];
                        dist = Projectile.Center - stickedNPC.Center;
                        break;
                    }
                }
            }
            Projectile.rotation += Projectile.velocity.X / (Projectile.width / 2);

            explosionTimer++;
            if (explosionTimer >= 300)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IceGrenadeLauncherExplosion>(), 25, 5);
                Projectile.Kill();
            }
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.Hitbox.Intersects(Projectile.Hitbox))
                {
                    if (proj.type == ModContent.ProjectileType<IceGrenadeLauncherExplosion>())
                    {
                        explosionTimer = 280;
                        break;
                    }
                    else if (proj.velocity.LengthSquared() > 100 && proj.DamageType == DamageClass.Ranged)
                    {
                        for (int j = 0; j < 3; j++)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, proj.velocity * 2, ModContent.ProjectileType<Shrapnel>(), 25, 5);
                        explosionTimer = 300;
                        break;
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Math.Abs(Projectile.velocity.Y) < 1)
            {
                Projectile.velocity.Y = 0;
                return false;
            }
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X * 0.5f;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
            }
            return false;
        }
    }
    public class IceGrenadeLauncherExplosion : ModProjectile
    {
        public override string Texture => Helpers.GeneralHelper.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.damage = 1;
            Projectile.timeLeft = timeLeft;
            Projectile.tileCollide = false;
        }
        int timeLeft = 15;
        CirclePrim prim;
        float startingRadiusTimer = 0;
        static Texture2D primTexture;
        static Texture2D particleTexture;
        List<GenericGlowParticle> particles = new();
        public override void Load()
        {
            primTexture = Mod.Assets.Request<Texture2D>("Assets/Effects/GlowTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            particleTexture = Mod.Assets.Request<Texture2D>("Assets/Effects/bloom", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override void OnSpawn(IEntitySource source)
        {
            prim = (CirclePrim)PrimHandler.CreateTrail<CirclePrim>(false, default);
            prim.Texture = primTexture;
            prim.SetData(Color.LightSkyBlue, Projectile.Center, 1, 10, 20, false, 0, 20);
            prim.Initialize();

            for (int i = 0; i < 50; i++)
            {
                //GenericGlowParticle p = new(Projectile.Center, Main.rand.NextVector2CircularEdge(3, 3), Color.WhiteSmoke, 0.1f, 60);
                GenericGlowParticle ringParticle = new(Projectile.Center, Vector2.Zero, Color.WhiteSmoke * 0.5f, 0.2f, 60, 1, 0.95f, 0.99f);
                ParticleSystem.GenerateParticle(ringParticle);
                ringParticle.Texture = particleTexture;
                ringParticle.BlendState = BlendState.Additive;
                particles.Add(ringParticle);
            }

            for (int i = 0; i < Main.rand.Next(3, 5); i++)
            {
                Projectile.NewProjectile(source, Projectile.Center, new Vector2(1, -0.5f).RotatedByRandom(MathHelper.Pi) * 50, ModContent.ProjectileType<Shrapnel>(), 10, 10);
            }
        }
        public override bool PreDraw(ref Color lightColor) 
        {
            startingRadiusTimer += MathHelper.Pi / timeLeft;

            prim.radius += MathF.Sin(startingRadiusTimer) * 10;
            prim.Width = MathF.Sin(startingRadiusTimer) * 20;
            prim.rotation = prim.radius / 10;

            prim.texCoordTopOffset = new Vector2(0, 0.2f);
            prim.texCoordBottomOffset = new Vector2(0, -0.2f);
            prim.Draw();
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Position = Projectile.Center + (Vector2.UnitX.RotatedBy(MathHelper.TwoPi / particles.Count * i) * prim.radius * Main.rand.NextFloat(1, 1.1f)).RotatedBy(MathF.Sin(startingRadiusTimer));
                //particles[i].Velocity *= Projectile.Center.DirectionTo(particles[i].Position) * 1.1f;
            }
            
            return true;
        }
    }
    public class Shrapnel : ModProjectile
    {
        public override string Texture => "Insignia/Content/Items/Weapons/Ranged/IceShrapnelProjectile";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
            Projectile.damage = 12;
            Projectile.friendly = true;
        }
        PrimTrail prim;
        public override void OnSpawn(IEntitySource source)
        {
            prim = PrimHandler.CreateTrail<PrimTrail>(false, default);
            prim.Points = Projectile.oldPos;
            prim.Color = Color.LightBlue * 0.4f;
            prim.Width = 10;
            prim.WidthFallOff = true;
            prim.ColorChangeDelegate = new((float progress, Color color) => 
            { 
                return Color.Lerp(color, Color.AliceBlue * 0.3f, progress) * progress * progress;
            });
            prim.Texture = Mod.Assets.Request<Texture2D>("Assets/Effects/GlowTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            prim.Initialize();
        }
        public override void AI()
        {
            prim.Points = Projectile.oldPos;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity.Y += 2;
            MathHelper.Clamp(Projectile.velocity.Y, 0, 16);
            Projectile.velocity *= 0.9f;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.Center, 7, 7, DustID.Adamantite, 0, 0, 0, Color.Aqua);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            prim.Draw();
            return true;
        }
    }
}