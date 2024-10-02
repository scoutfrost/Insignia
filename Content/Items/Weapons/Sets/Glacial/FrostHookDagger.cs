using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Core.Common.Systems;
using Insignia.Helpers;
using System.Transactions;
using Insignia.Core.ModPlayers;
using Terraria.Graphics.Shaders;

namespace Insignia.Content.Items.Weapons.Sets.Glacial
{
    public class FrostHookDagger : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Melee;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 5;
            Item.useAnimation = 5 ;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;

            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<FrostDaggerProjectile>();
        }
        public override bool AltFunctionUse(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<FrostHookProjectile>()] == 0;
        }
        int count = 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<FrostHookProjectile>(), 2, knockback);
                return false;
            }
            count++;

            if (player.GetModPlayer<FrostHookDaggerPlayer>().hasEnemyHooked || player.GetModPlayer<FrostHookDaggerPlayer>().hasBigEnemyHooked)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage * 3, knockback * 5, -1, count % 2 == 0 ? 0 : 1);
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, -1, count % 2 == 0 ? 0 : 1);
            }
            
            return false;
        }
    }

    public class FrostDaggerProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = timeleft;
            Projectile.knockBack = 10;
            Projectile.penetrate = -1;
        }

        private ProjKeyFrameHandler handler;
        private List<Vector2> points = [];
        private Player Player => Main.player[Projectile.owner];
        private Vector2 mouse;
        private int i;
        private float angle;
        private int startingAngle = 30;
        private bool upSwing = false;
        private PrimTrail prim;
        private List<Vector2> primPoints = [];
        private List<float> rotationRatios = [];
        private static readonly int timeleft = 600;
        private bool flipVertically = false;
        private Texture2D primTexture;
        private bool hasEnemyHooked = false;
        public override void OnSpawn(IEntitySource source)
        {
            hasEnemyHooked = Player.GetModPlayer<FrostHookDaggerPlayer>().hasEnemyHooked;
            prim = new PrimTrail();
            primTexture = (Texture2D)ModContent.Request<Texture2D>("Insignia/Assets/Effects/GlowTrail");
            primPoints = Projectile.oldPos.ToList();
            if (Projectile.ai[0] == 1)
                upSwing = true;

            angle = MathHelper.ToRadians(startingAngle);

            handler = new(KeyFrameInterpolationCurve.Slerp, "Insignia/Content/Items/Weapons/Sets/Glacial/SwingPoints2",
                (int)(20 / Player.GetAttackSpeed(DamageClass.Melee)));

            flipVertically = ((Player.direction == 1) && upSwing) || ((Player.direction == -1) && !upSwing);

            points = handler.GetPoints(25, flipVertically, Color.White);
            mouse = Player.Center.DirectionTo(Main.MouseWorld);

            rotationRatios = handler.CalculateRatios();
            rotationRatios.Insert(0, 0);
            rotationRatios.Add(100);

            Projectile.netUpdate = true;

            i = 0;
        }
        private int whichSectionOfCurve = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = flipVertically ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float destinationAngle = MathHelper.Pi + MathHelper.ToRadians(startingAngle);
            whichSectionOfCurve = 0;
            for (int j = 1; j < rotationRatios.Count; j++)
            {
                if (i >= rotationRatios[j] * points.Count + (j * rotationRatios[j - 1] * points.Count) && i < rotationRatios[j] * points.Count + (rotationRatios[j + 1] * points.Count))
                {
                    whichSectionOfCurve++;
                    break;
                }
            }
            //Main.NewText(whichSectionOfCurve);
            angle += (destinationAngle - MathHelper.ToRadians(startingAngle)) / points.Count * (float)((rotationRatios.Count - 2) * rotationRatios[whichSectionOfCurve + 1]);
            float scaleComponent = (float)Math.Sqrt(MathF.Sin(angle) * MathF.Sin(angle) / 3) + (MathF.Cos(angle) * MathF.Cos(angle));
            //Main.NewText((float)((rotationRatios.Count - 2) * rotationRatios[whichSectionOfCurve + 1]));
            Vector2 scale = new(scaleComponent, scaleComponent);

            Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(Texture);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, 0, tex.Width, tex.Height), lightColor, Projectile.rotation, new Vector2(tex.Width / 2, tex.Height), scale, spriteEffects);

            float rotation = flipVertically ? destinationAngle + MathHelper.Pi - angle : angle;
            Vector2 swordTip = ((rotation + MathHelper.Pi).ToRotationVector2() * new Vector2(1, 0.5f)).RotatedBy(mouse.ToRotation());
            primPoints.Insert(0, Player.Center + swordTip * 50);
            if (primPoints.Count >= Projectile.oldPos.Length - 1)
            {
                primPoints.RemoveAt(primPoints.Count - 1);
            }

            prim.Width = 20;
            prim.Color = lightColor * 0.1f;
            if (hasEnemyHooked || Player.GetModPlayer<FrostHookDaggerPlayer>().hasBigEnemyHooked)
            {
                prim.Width = 30;
                prim.Color = Color.AliceBlue * 0.5f;
            }
            prim.Texture = primTexture;
            prim.WidthFallOff = true;
            prim.Points = primPoints.ToArray();
            prim.Pixelated = false;
            if (Projectile.timeLeft == timeleft - 1)
                prim.Initialize();

            prim.Draw();

            return false;
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.AliceBlue.ToVector3() / 5);
            Vector2 mouse2 = Player.Center + mouse;
            handler.SetAiDefaults(Projectile, Player, mouse2);

            Vector2 offset = new(5 * Player.direction, 0);

            float rotOffset = MathHelper.ToRadians(-70);
            if (flipVertically)
                rotOffset = MathHelper.ToRadians(155);

            Projectile.Center = handler.CalculateSwordSwingPointsAndApplyRotation(Projectile, mouse2, Player, points, ref i, offset, upSwing, rotOffset);

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = Player.direction;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.GetModPlayer<FrostHookDaggerPlayer>().hasBigEnemyHooked && target.type != NPCID.TargetDummy)
            {
                Player.velocity += Player.Center.DirectionFrom(target.Center) * 15;
            }
        }
    }
    public class FrostHookProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 120;
            Projectile.knockBack = 5;
            Projectile.penetrate = -1;
            Projectile.idStaticNPCHitCooldown = 30;
            Projectile.usesIDStaticNPCImmunity = true;
        }
        float i = 0;
        Player Player => Main.player[Projectile.owner];
        Vector2 targetPoint;
        Vector2 controlPoint;
        bool returning = false;
        PrimTrail prim;
        int startingDirection = 0;
        NPC grabbedNPC;
        public override void OnSpawn(IEntitySource source)
        {
            prim = new() { Shader = GameShaders.Misc["TrailShader"].Shader };
            int maxDist = 300;
            float distance = MathHelper.Clamp(Vector2.Distance(Player.Center, Main.MouseWorld), 0, maxDist);
            targetPoint = Player.Center.DirectionTo(Main.MouseWorld) * distance;
            controlPoint = Player.Center.DirectionTo(Main.MouseWorld).RotatedBy(-MathHelper.PiOver2 * Player.direction) * distance / 2;

            Projectile.rotation = Player.Center.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.PiOver2 * Player.direction;
            startingDirection = Player.direction;
        }
        public override void AI()
        {
            Player.direction = startingDirection;

            int killdist = 30;
            if (Player.Center.DistanceSQ(Projectile.Center) < killdist * killdist && returning)
                Projectile.Kill();
            
            if (Player.Center.DistanceSQ(Projectile.Center) < killdist * killdist + 2 && returning)
                Player.GetModPlayer<InsigniaPlayer>().immuneTimeAdd += 1;

            int pointCount = 25;
            Projectile.netUpdate = true;

            Projectile.Center = EasingFunctions.Bezier([Player.Center, Player.Center + controlPoint, Player.Center + targetPoint], i / pointCount);

            if (i < pointCount)
            { 
                i++; 
            }                      

            if (i >= pointCount)
            {
                returning = true;
            }

            if (returning)
            {
                Projectile.velocity += Projectile.Center.DirectionTo(Player.Center) * 15;
            }

            if (Player.GetModPlayer<FrostHookDaggerPlayer>().hasEnemyHooked)
            {
                grabbedNPC.Center = Projectile.Center + Projectile.velocity;
            }
            if (Player.GetModPlayer<FrostHookDaggerPlayer>().hasBigEnemyHooked)
            {
                Player.Center += Player.DirectionTo(grabbedNPC.Center) * 15;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (returning && target.type != NPCID.TargetDummy && !Player.GetModPlayer<FrostHookDaggerPlayer>().hasEnemyHooked && !Player.GetModPlayer<FrostHookDaggerPlayer>().hasBigEnemyHooked)
            {
                if (!target.boss && ((float)target.defense / target.lifeMax) <= 0.1f * (Main.GameMode + 1) && target.knockBackResist > 0.4f && target.lifeMax < 5000)
                {
                    grabbedNPC = target;
                    Player.GetModPlayer<FrostHookDaggerPlayer>().hasEnemyHooked = true;
                }
                else 
                {
                    grabbedNPC = target;
                    Player.GetModPlayer<FrostHookDaggerPlayer>().hasBigEnemyHooked = true;
                }
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            return returning;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> points = [];
            for (float i = 0; i < 20; i++)
            {
                Vector2 controlPoint = Player.Center + (Player.Center.DirectionTo(Projectile.Center) * (100 - this.i * 5)).RotatedBy(-MathHelper.PiOver2 * Player.direction);
                Dust.QuickDust(controlPoint.ToPoint(), Color.Red);
                points.Add(EasingFunctions.Bezier([Player.Center, controlPoint, Projectile.Center + Player.Center.DirectionTo(Projectile.Center) * 10], i / 20));
            }
            var world = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            prim.Texture = (Texture2D)ModContent.Request<Texture2D>("Insignia/Content/Items/Weapons/Sets/Glacial/FrostHookTrail");
            prim.Shader.Parameters["wvp"].SetValue(world * view * projection);
            prim.Shader.Parameters["trailTexture"].SetValue(prim.Texture);
            prim.Shader.Parameters["colorMult"].SetValue(lightColor.ToVector4() + Color.White.ToVector4() * 0.3f);
            prim.Width = 3;
            prim.Color = Color.White;
            prim.Points = [.. points];
            if (Projectile.timeLeft == 119)
                prim.Initialize();
            prim.Draw();
            ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, new Color(lightColor.ToVector4() + Color.White.ToVector4() * 0.3f), Vector2.One, Player.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None);

            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Player.GetModPlayer<FrostHookDaggerPlayer>().hasEnemyHooked = false;
            Player.GetModPlayer<FrostHookDaggerPlayer>().hasBigEnemyHooked = false;
        }
    }
    public class FrostHookDaggerPlayer : ModPlayer
    {
        public bool hasEnemyHooked;
        public bool hasBigEnemyHooked;
    }
}
