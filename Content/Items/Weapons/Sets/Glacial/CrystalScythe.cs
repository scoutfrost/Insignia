using Insignia.Core.Common.Systems;
using Insignia.Helpers;
using Insignia.Prim;

namespace Insignia.Content.Items.Weapons.Sets.Glacial
{
    internal class CrystalScythe : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.damage = 25;
            Item.DamageType = DamageClass.Melee;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 50;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;

            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<CrystalScytheProjectile>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shotCount++;
            if (shotCount % 2 == 0)
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback);
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, -1, 1);
            
            return false;
        }
    }
    public class CrystalScytheProjectile : ModProjectile
    {
        public override string Texture => "Insignia/Content/Items/Weapons/Sets/Glacial/CrystalScythe";
        static Texture2D tex;
        public override void Load() => tex = (Texture2D)ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad);
        public override void Unload() => tex = null;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = timeleft;
        }
        ProjKeyFrameHandler handler;
        Vector2 mouse;
        List<Vector2> points;
        int i; 
        Vector2 vectorToMouse;
        Player Player => Main.player[Projectile.owner];
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[0] == 1)
                upSwing = true;

            handler = new(KeyFrameInterpolationCurve.Slerp, "Insignia/Content/Items/Weapons/Sets/Glacial/SwingPoints", (int)(23 / Player.GetAttackSpeed(DamageClass.Melee)));
            points = handler.GetPoints(30);

            vectorToMouse = Player.Center.DirectionTo(Main.MouseWorld);

            i = points.Count - 1;
            if (Player.direction == 1 && !upSwing || Player.direction == -1 && upSwing)
                i = 0;
        }
        public override void AI()
        {
            float rotOffset = 0;
            if (upSwing)
                rotOffset = Player.direction == 1 ? MathHelper.PiOver2 : -MathHelper.PiOver4;

            mouse = Player.Center + vectorToMouse;
            handler.SetAiDefaults(Projectile, Player, mouse);
            Projectile.Center = handler.CalculateSwordSwingPointsAndApplyRotation(Projectile, mouse, Player, points, ref i, Vector2.Zero, upSwing, rotOffset);
            if (hitNPC && ((Player.direction == 1 && !upSwing) || (Player.direction == -1 && upSwing)))
            {
                i--;
                freezeTime++;
            }
            else if (hitNPC)
            {
                i++;
                freezeTime++;
            }
            if (freezeTime >= 4)
            {
                Vector2 dustSpeed = Player.Center.DirectionTo(mouse).RotatedByRandom(MathHelper.PiOver4 / 4) * 8 * Main.rand.NextFloat(0.7f, 1.5f);
                Dust.NewDust(Main.rand.NextVector2FromRectangle(target.Hitbox), 10, 10, DustID.Blood, dustSpeed.X, dustSpeed.Y, default, default, 1.2f);

                hitNPC = false;
                freezeTime++;
            }

            foreach (var shard in shards)
            {
                if (shard.ai[0] != 1)
                {
                    shard.Center = Projectile.Center + new Vector2(time * 100, time * 5).RotatedBy(Player.Center.DirectionTo(mouse).ToRotation());
                    shard.scale = 0.2f + time / 8;
                }
            }

            if (upSwing)
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4);
            else
                Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + (Player.direction == 1 ? -MathHelper.PiOver2 - MathHelper.PiOver4 : MathHelper.Pi));
            
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            this.target = target;
            if (freezeTime >= 6 || freezeTime == 0)
            {
                hitNPC = true;
                freezeTime = 0;
            }
            target.AddBuff(ModContent.BuffType<BleedDebuff>(), 120);
            BleedDebuff debuff = (BleedDebuff)ModContent.GetModBuff(ModContent.BuffType<BleedDebuff>());
            debuff.reApplyBleed = ReApplyBleed;
        }
        private void ReApplyBleed(ref List<int> stackList, NPC npc, int index)
        {
            int stack = stackList[index];
            if (stack >= 10)
            {
                npc.lifeRegen -= 12;
            }
            if (stack >= 30 && npc.type != NPCID.TargetDummy)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 dustSpeed = new Vector2(0, -4).RotatedByRandom(MathHelper.ToRadians(20));
                    Dust.NewDust(Main.rand.NextVector2FromRectangle(npc.Hitbox), 10, 10, DustID.Blood, dustSpeed.X, dustSpeed.Y, default, default, 2f);
                }
                npc.life -= (int)(npc.lifeMax * 0.09f + npc.lifeMax / 10000);
                if (npc.life <= 0)
                    npc.life = 1;

                stack = 0;
                stackList[index] = stack;
            }
        }
        float time;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);

            if (Player.direction == -1)
            {
                int i2 = points.Count - 1 - i;
                time = i2 / (points.Count / 1.5f);
            }
            else
            {
                time = i / (points.Count / 1.5f);
            }
            time = EasingFunctions.EaseInOutQuad(time);

            Vector2 scale = upSwing ? new Vector2(time, time + 0.1f) : new Vector2(0.75f + time, time);

            if (Projectile.timeLeft == timeleft - 1)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    oldscale.Add(scale);
                }
            }
            if (oldscale.Count > Projectile.oldPos.Length)
            {
                oldscale.Remove(oldscale[0]);
            }

            oldpos = Projectile.oldPos.ToList();
            for (int i = 0; i < oldpos.Count; i++)
            {
                oldpos[i] += drawOrigin + new Vector2(time * 35, time * 10).RotatedBy(Player.Center.DirectionTo(mouse).ToRotation());
            }
            GenericPrimTrail prim = new(new(10, 30, 35, 10), oldpos.ToArray(), 5 + time * 10, true);
            prim.Draw();
            
            SpriteEffects spriteEffects = Player.direction == 1 && !upSwing || (Player.direction == -1 && upSwing) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, lightColor, scale, spriteEffects);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.2f;
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.oldRot[k] + ((Player.direction == 1 && upSwing) || (Player.direction == -1 && !upSwing) ? MathHelper.Pi : 0), drawOrigin, oldscale[k], spriteEffects, 0);
            }

            oldscale.Add(scale);
            return false;
        }
    }
}
