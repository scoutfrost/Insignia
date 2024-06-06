using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Core.Common.Systems;
using Insignia.Helpers;
using Insignia.Prim;
using Insignia.Core.Particles;
using Terraria.GameContent;
using Insignia.Content.Buffs;
using Insignia.Core.ModPlayers;

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
            Item.useTime = 20;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;

            Item.knockBack = 5f;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<CrystalScytheProjectile>();
        }

        private int shotCount = 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            shotCount++;
            if (shotCount % 2 == 0)
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback);
            else
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, -1, 1);

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ModContent.ItemType<CrystalScythe>());
            recipe.AddIngredient(ItemID.IceBlade);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            GeneralHelper.AddExpandableTooltip(ref tooltips, Mod, Color.Blue, Helpers.GeneralHelper.BleedDescription, Color.LightBlue);
        }
    }

    public class CrystalScytheProjectile : ModProjectile
    {
        public override string Texture => "Insignia/Content/Items/Weapons/Sets/Glacial/CrystalScythe";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 85;
            Projectile.height = 85;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = timeleft;
            Projectile.knockBack = 10;
        }

        private Player Player => Main.player[Projectile.owner];

        private ProjKeyFrameHandler handler;

        private bool upSwing = false;
        private bool hitNPC = false;

        private readonly int timeleft = 500;
        private int i;
        private int freezeTime = 0;
        private float time;

        private Vector2 vectorToMouse;
        private Vector2 mouse;

        private NPC target;

        private List<Vector2> oldpos = [];
        private List<Vector2> points;
        private List<Vector2> oldscale = [];

        private List<Projectile> shards = [];

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

            for (int i = 0; i < 3; i++)
            {
                shards.Add(Projectile.NewProjectileDirect(source, Projectile.Center + new Vector2(Main.rand.Next(30)), Vector2.Zero, ModContent.ProjectileType<CrystalShard>(), 7, 0));
            }
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
            Player.GetModPlayer<InsigniaPlayer>().BleedProc = true;
            int stack = stackList[index];
            if (stack >= 10)
            {
                npc.lifeRegen -= 12;
            }
            if (stack >= 30 / Player.GetModPlayer<InsigniaPlayer>().BleedBuildupMultiplier && npc.type != NPCID.TargetDummy)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 dustSpeed = new Vector2(0, -4).RotatedByRandom(MathHelper.ToRadians(20));
                    Dust.NewDust(Main.rand.NextVector2FromRectangle(npc.Hitbox), 10, 10, DustID.Blood, dustSpeed.X, dustSpeed.Y, default, default, 2f);
                }
                int bleedDamage = (int)(npc.lifeMax * 0.2f * (1 - npc.lifeMax / 10000) * Player.GetModPlayer<InsigniaPlayer>().BleedDamageMultiplier);

                if (bleedDamage < 0)
                    bleedDamage = 0;

                npc.life -= bleedDamage;
                CombatText.NewText(new((int)npc.Center.X, (int)npc.Center.Y, 10, 10), Color.DarkRed, bleedDamage);
                if (npc.life <= 0)
                    npc.life = 1;

                stack = 0;
                stackList[index] = stack;
            }
        }

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
            oldpos = [.. Projectile.oldPos];
            oldpos[0] = Projectile.Center;
            for (int i = 1; i < oldpos.Count; i++)
            {
                oldpos[i] = Projectile.oldPos[i];
                oldpos[i] += drawOrigin + new Vector2(Player.direction == 1 ? time * 80 : time * 35, 0).RotatedBy(Player.Center.DirectionTo(mouse).ToRotation());
            }
            GenericPrimTrail prim = new(new(5, 15, 17, 10), [.. oldpos], 5 + time * 10);
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

    public class CrystalShard : ModProjectile
    {
        public override string Texture => GeneralHelper.Empty;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = timeleft;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
            Projectile.damage = 7;
        }

        private readonly int timeleft = 300;

        public override void AI()
        {
            SparkleParticle p = new(Color.Azure, Projectile.scale, Projectile.Center, Vector2.Zero, 200, Projectile.timeLeft, 0.95f);
            ParticleSystem.GenerateParticle(p);
            if (Projectile.ai[0] != 1 && Projectile.timeLeft <= timeleft - 100)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BleedDebuff>(), 120);
            BleedDebuff debuff = (BleedDebuff)ModContent.GetModBuff(ModContent.BuffType<BleedDebuff>());
            debuff.reApplyBleed = ReApplyBleed;
            Projectile.Center = target.Center;
            Projectile.ai[0] = 1;
        }

        private void ReApplyBleed(ref List<int> stackList, NPC npc, int index)
        {
            Player player = Main.player[Projectile.owner];
            player.GetModPlayer<InsigniaPlayer>().BleedProc = true;
            int stack = stackList[index];
            if (stack >= 25)
            {
                npc.lifeRegen -= 10;
            }
            if (stack >= 60 / player.GetModPlayer<InsigniaPlayer>().BleedBuildupMultiplier && npc.type != NPCID.TargetDummy)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 dustSpeed = new Vector2(0, -4).RotatedByRandom(MathHelper.ToRadians(20));
                    Dust.NewDust(Main.rand.NextVector2FromRectangle(npc.Hitbox), 10, 10, DustID.Blood, dustSpeed.X, dustSpeed.Y, default, default, 2f);
                }

                int bleedDamage = (int)(npc.lifeMax * 0.12f * (1 - npc.lifeMax / 10000) * player.GetModPlayer<InsigniaPlayer>().BleedDamageMultiplier);

                if (bleedDamage < 0)
                    bleedDamage = 0;

                npc.life -= bleedDamage;
                CombatText.NewText(new((int)npc.Center.X, (int)npc.Center.Y, 10, 10), Color.DarkRed, bleedDamage);

                if (npc.life <= 0)
                    npc.life = 1;
                stack = 0;
                stackList[index] = stack;
            }
        }
    }
}