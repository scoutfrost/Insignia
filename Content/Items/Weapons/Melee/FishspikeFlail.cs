using Insignia.Core.Common.Systems;
using Insignia.Core.Particles;
using Insignia.Helpers;
using Insignia.Prim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insignia.Content.Items.Weapons.Melee
{
    public class FishspikeFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
           

            Item.ResearchUnlockCount = 1;

            ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
        }

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 42;
            Item.useAnimation = 42;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2.5f;
            Item.rare = ItemRarityID.Green;
            Item.autoReuse = true;
            Item.shootSpeed = 30f;
            Item.shoot = ModContent.ProjectileType<FishspikeFlailProj>();
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(gold: 3, silver: 3);
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

       
    }
    public class FishspikeFlailProj : ModProjectile
    {
        private const string ChainTexturePath = "Insignia/Content/Items/Weapons/Melee/FishspikeFlailProj_Chain"; // The folder path to the flail chain sprite
        private const string ChainTextureExtraPath = "Insignia/Content/Items/Weapons/Melee/FishspikeFlailProj_Chain";  // This texture and related code is optional and used for a unique effe

        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // This ensures that the projectile is synced when other players join the world.
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 0;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;

            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true; // Used for hit cooldown changes in the ai hook
            Projectile.localNPCHitCooldown = 10; // This facilitates custom hit cooldown logic

            Projectile.aiStyle = ProjAIStyleID.Flail;

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        private const int balls = 2;
        private Vector2 origin;
       
          
        public override void AI()
        {
           
        
            if (Main.myPlayer == Projectile.owner && Projectile.ai[0] == 2f && Projectile.ai[1] == 0f)
            {

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<BouncingFishSpike>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                Projectile.ai[1]++;

                for (int i = 0; i < 5; i++)
                {
                    VelocityBasedParticle velParticle = new(5, Color.LightBlue, (Projectile.velocity / 4).RotatedBy(MathHelper.ToRadians(-10)).RotatedBy(MathHelper.ToRadians(10 * i)), Projectile.Center, Vector2.One, 50, 0.8f);
                    SparkleParticle particle = new(Color.LightBlue, 1, Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(25)) * i / 10, 100);
                    ParticleSystem.GenerateParticle(particle, velParticle);
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X * 0.4f, Projectile.velocity.Y * 0.4f);
            Main.dust[dust].scale = 1f; 
            Main.dust[dust].noGravity = true;

        }

      
        private float time;
        private int i;
        private Player Player => Main.player[Projectile.owner];
        private List<Vector2> points;

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

            // This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
            playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
            Asset<Texture2D> chainTextureExtra = ModContent.Request<Texture2D>(ChainTextureExtraPath); // This texture and related code is optional and used for a unique effect

            Rectangle? chainSourceRectangle = null;
            // Drippler Crippler customizes sourceRectangle to cycle through sprite frames: sourceRectangle = asset.Frame(1, 6);
            float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap.

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Size() / 2f : chainTexture.Size() / 2f;
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
            if (chainSegmentLength == 0)
                chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
            float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (chainLengthRemainingToDraw > 0f)
            {
                // This code gets the lighting at the current tile coordinates
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                // Flaming Mace and Drippler Crippler use code here to draw custom sprite frames with custom lighting.
                // Cycling through frames: sourceRectangle = asset.Frame(1, 6, 0, chainCount % 6);
                // This example shows how Flaming Mace works. It checks chainCount and changes chainTexture and draw color at different values

                Asset<Texture2D> chainTextureToDraw = chainTexture;
                if (chainCount >= 4)
                {
                    // Use normal chainTexture and lighting, no changes
                }
                else if (chainCount >= 3)
                {
                    // Near to the ball, we draw a custom chain texture and slightly make it glow if unlit.
                    chainTextureToDraw = chainTextureExtra;
                    byte minValue = 140;
                    if (chainDrawColor.R < minValue)
                        chainDrawColor.R = minValue;

                    if (chainDrawColor.G < minValue)
                        chainDrawColor.G = minValue;

                    if (chainDrawColor.B < minValue)
                        chainDrawColor.B = minValue;
                }
                else
                    // Close to the ball, we draw a custom chain texture and draw it at full brightness glow.
                    chainTextureToDraw = chainTextureExtra;

                // Here, we draw the chain texture at the coordinates
                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                // chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
                chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }
           
            return true;
        }
    }

    public class BouncingFishSpike : ModProjectile
    {
        public Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.netImportant = true; // This ensures that the projectile is synced when other players join the world.
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.CloneDefaults(ProjectileID.Shuriken);

            Projectile.penetrate = -1;

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--; //Make sure it doesnt penetrate anymore
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                Projectile.velocity *= 0.6f;

                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
            }
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnKill(int timeLeft)
        {
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, Mod.Find<ModGore>("Fishspikegore1").Type, 1f);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, Mod.Find<ModGore>("Fishspikegore").Type, 1f);
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, Mod.Find<ModGore>("Fishspikegore2").Type, 1f);




        }
        public override void AI()
        {
            Projectile.rotation += 0.4f * Projectile.direction;

            Projectile.velocity.Y += 0.2f;
            Projectile.velocity.X *= 0.95f;

            Projectile.aiStyle = 0;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = ModContent.Request<Texture2D>("Insignia/Assets/Effects/GradientCirc").Value;
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
                var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + offset;
                float sizec = Projectile.scale * (Projectile.oldPos.Length - k) / (Projectile.oldPos.Length * 1.2f);
                Color color = new Color(50, 255, 236) * (1f - Projectile.alpha) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, frame, color, Projectile.oldRot[k], frame.Size() / 2f, sizec, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.ZoomMatrix);

            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 directionTo = target.DirectionTo(Owner.Center);
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.position);
            Dust.NewDustPerfect(target.Center + directionTo * 10, DustID.Blood, directionTo.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f) + 3.14f) * -Main.rand.NextFloat(0.5f, 5f), 0, default, 1.3f);

            Projectile.penetrate--; //Make sure it doesnt penetrate anymore
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                Projectile.velocity *= 0.7f;
            }
        }

    }
}