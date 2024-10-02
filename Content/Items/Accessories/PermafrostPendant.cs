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
using Terraria.Graphics.Shaders;
using System.Diagnostics;
using Terraria.GameContent.Golf;
using System.Net;
using Microsoft.Build.ObjectModelRemoting;
using Insignia.Content.Items.Weapons.Sets.Glacial;
using Terraria.Graphics.CameraModifiers;
using Microsoft.CodeAnalysis;
using Humanizer;

namespace Insignia.Content.Items.Accessories
{
    internal class PermafrostPendant : ModItem
    {
        public override string Texture => "Insignia/Content/Items/Accessories/PermafrostPendant";
        public override void SetDefaults()
        {
            Item.rare = ItemRarityID.Blue;
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
        }
        bool hasSubscribed = false;
        bool equipped = false;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            equipped = true;
            if (!hasSubscribed)
            {
                player.GetModPlayer<AccessoryPlayer>().UpDateEquipEvent += PermafrostPendant_UpDateEquipEvent;
                player.GetModPlayer<AccessoryPlayer>().OnHitNPCEvent += PermafrostPendant_OnHitNPCEvent;
                player.GetModPlayer<AccessoryPlayer>().ResetEffectsEvent += PermafrostPendant_ResetEffectsEvent;
                hasSubscribed = true;
            }
        }
        void PermafrostPendant_ResetEffectsEvent()
        {
            equipped = false;
        }
        void PermafrostPendant_UpDateEquipEvent(Player player)
        {
            if (hasSubscribed && !equipped)
            {
                hasSubscribed = false;
                player.GetModPlayer<AccessoryPlayer>().OnHitNPCEvent -= PermafrostPendant_OnHitNPCEvent;
            }
        }
        void PermafrostPendant_OnHitNPCEvent(NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {
            if (hit.Crit && !target.active && !target.CountsAsACritter)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Entity), target.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostPendantExplosion>(), 90, 0, Main.player.ToList().IndexOf(player));
            }
        }

    }
    public class PermafrostPendantExplosion : ModProjectile
    {
        public override string Texture => GeneralHelper.Empty;
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = false;
            Projectile.damage = 50;
        }
        PrimTrail primTrail;
        List<Projectile> particles = [];
        bool initialize = false;
        public override void OnSpawn(IEntitySource source)
        {
            initialize = true;
            Projectile.netUpdate = true;

            primTrail = new PrimTrail()
            {
                Texture = (Texture2D)ModContent.Request<Texture2D>("Insignia/Assets/Effects/GlowTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad)
            };

        }
        bool shouldDamage = false;
        int maxdist = 200;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            shouldDamage = false;

            if (radius <= -20)
                shouldDamage = true;

            if (radius <= -maxdist)
                Projectile.Kill();

            IEnumerable<NPC> targets = Main.npc.Where(npc => npc.active && npc.CanBeChasedBy() && !npc.boss && Projectile.Center.DistanceSQ(npc.Center) < maxdist * maxdist);

            foreach (var npc in targets)
            {
                if (shouldDamage)
                    npc.velocity += npc.DirectionFrom(Projectile.Center).RotatedByRandom(MathHelper.ToRadians(10)) * 2;
                else
                    npc.velocity += npc.DirectionTo(Projectile.Center).RotatedByRandom(MathHelper.ToRadians(10)) * 0.3f;
            }
        }
        float timer = 0;
        static int pointCount = 32;
        Vector2[] points = new Vector2[pointCount + 1];
        float radius;
        bool explode;
        public override bool PreDraw(ref Color lightColor)
        {
            timer++;
            float startingRadius = 180;
            float timeMult = 2;
            radius = startingRadius - timer * timeMult;
            float radiusSpeed = timeMult * 7;
            float time = maxdist / radiusSpeed;
            int trailWidthExplosion = 350;
            if (radius <= 0)
            {
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(Main.screenPosition, Main.rand.NextFloat(MathHelper.Pi).ToRotationVector2(), 1, 15, 5));
                explode = true;
                radius *= 6;
            }
            for (int i = 0; i < pointCount; i++)
            {
                points[i] = Projectile.Center + Vector2.UnitY.RotatedBy((Math.Tau / pointCount * i) + MathHelper.ToRadians(timer)) * radius;
                if (explode && primTrail.Width < 25)
                {
                    ParticleSystem.GenerateParticle(new SparkleParticle(
                        Color.Azure, 0.2f, points[i] - Vector2.UnitY.RotatedBy((float)Projectile.Center.DirectionTo(points[i]).ToRotation()), Projectile.Center.DirectionTo(points[i]), 0, 130, 0.99f));
                }
                if (timer == 1)
                {
                    if (i % 2 == 0)
                    {
                        Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), points[i], Vector2.Zero, ModContent.ProjectileType<PermafrostPendantShard>(), 10, 0);
                        p.timeLeft = 230;
                        p.alpha = 175;
                        particles.Add(p);
                        //Particle p = new SparkleParticle(Color.Azure, 0.5f, points[i], Vector2.Zero, 0, 360, 1); 
                        //ParticleSystem.GenerateParticle(p);
                    }
                }
            }
            points[^1] = points[0];

            for (int i = 0; i < particles.Count; i++)
            {
                Projectile p = particles[i];
                p.tileCollide = false;
                if (explode)
                {
                    p.tileCollide = true;
                    p.alpha += 2;
                    p.velocity += p.Center.DirectionTo(points[i * 2 + 1]);
                    continue;
                }
                p.Center = points[i * 2] + Vector2.Normalize(Projectile.Center.DirectionTo(points[i * 2])) * (float)Math.Sin(timer / 10) * 30;
            }

            if (radius < 0 && radius > -25)
            {
                primTrail.Width = trailWidthExplosion;
            }
            primTrail.Points = points;
            primTrail.Color = new Color(100, 150, 170);
            primTrail.Width = explode == true ? primTrail.Width - (trailWidthExplosion / time) : 40;
            primTrail.Pixelated = true;
            if (initialize)
            {
                primTrail.Initialize();
                initialize = false;
            }
            primTrail.Draw();

            return false;
        }
        public override bool? CanDamage()
        {
            return shouldDamage;
        }
    }
    public class PermafrostPendantShard : ModProjectile
    {
        public override string Texture => GeneralHelper.Empty;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
            Projectile.damage = 7;
        }
        //PrimTrail prim;
        /*public override void OnSpawn(IEntitySource source)
        {
            prim = new(Projectile.oldPos, Color.Azure * 0.1f, 5)
            {
                WidthFallOff = true
            };
        }
        public override bool PreDraw(ref Color lightColor)
        {
            prim.Draw();
            return true;
        } commented because it lags for like a frame when the projectile spawns i have no clue why, nothing else does this*/
        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            SpawnParticles(1, Projectile.timeLeft);
        }
        public override void OnKill(int timeLeft)
        {
            SpawnParticles(10, 100);
        }
        private void SpawnParticles(int count, int timeleft)
        {
            for (int i = 0; i < count; i++)
            {
                SparkleParticle p = new(Color.Azure, Projectile.scale, Projectile.Center, Vector2.Zero, Projectile.alpha, timeleft, 0.95f);
                ParticleSystem.GenerateParticle(p);
            }
        }
    }
}
