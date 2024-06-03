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
namespace Insignia.Content.Items.Accessories
{
    internal class PermafrostPendant : ModItem
    {
        public override string Texture => "Insignia/Content/Items/Accessories/PermafrostPendant";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.accessory = true;
        }
        //Player player;
        bool hasSubscribed = false;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //this.player = player;
            if (!hasSubscribed)
            {
                player.GetModPlayer<AccessoryPlayer>().OnHitNPCEvent += PermafrostPendant_OnHitNPCEvent;
                hasSubscribed = true;
            }
        }

        internal void PermafrostPendant_OnHitNPCEvent(NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {
            if (hit.Crit && !target.active && !target.CountsAsACritter)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Entity), target.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostPendantExplosion>(), 90, 0, Main.player.ToList().IndexOf(player));
            }
        }
    }
    public class PermafrostPendantExplosion : ModProjectile
    {
        public override string Texture => "Insignia/Content/Items/Weapons/Sets/Torgustus/TorgustusArrow";
        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.damage = 50;
        }
        public static Texture2D texture;
        public override void Load()
        {
            texture = (Texture2D)ModContent.Request<Texture2D>("Insignia/Content/Items/Weapons/Sets/Torgustus/TorgustusArrow", ReLogic.Content.AssetRequestMode.ImmediateLoad);
        }
        public override void Unload()
        {
            texture = null;
        }
        GenericPrimTrail primTrail;
        public override void OnSpawn(IEntitySource source)
        {
            primTrail = new(Color.LightBlue, points, 20, false);
        }
        int timer = 0;
        bool shouldDamage = false;
        public override void AI()
        {
            int maxdist = 200;
            Player player = Main.player[Projectile.owner];

            if (shouldDamage)
                Projectile.Kill();

            if (timer++ >= 120)
                shouldDamage = true;
            
            var targets = Main.npc.Where(npc => npc.active && npc.CanBeChasedBy() && !npc.boss && Projectile.Center.DistanceSQ(npc.Center) < maxdist * maxdist);

            foreach (var npc in targets)
            {
                if (shouldDamage)
                {
                    npc.velocity += npc.DirectionFrom(Projectile.Center).RotatedByRandom(MathHelper.ToRadians(10)) * 2;
                }
                else
                {
                    npc.velocity += npc.DirectionTo(Projectile.Center).RotatedByRandom(MathHelper.ToRadians(10)) * 0.3f;
                }
            }
        }
        static int pointCount = 150;
        Vector2[] points = new Vector2[pointCount];
        public override bool PreDraw(ref Color lightColor)
        {
            int pointcount = 16;
            float maxtime = 180;
            float timeMult = 1.3f;

            ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, lightColor, 1);
            for (int i = 0; i < pointcount; i++)
            {
                points[i] = Projectile.Center + Vector2.UnitY.RotatedBy(Math.Tau / (pointcount - 1.5f) * i) * (maxtime - timer * timeMult);
                //points[i] = points[i].RotatedBy(MathHelper.ToRadians(i * 10), Projectile.Center);
                //Dust d = Dust.NewDustPerfect(points[i], DustID.Adamantite, Vector2.Zero);
                //d.noGravity = true;
            }
            primTrail.Points = points;
            primTrail.Draw();
            return false;
        }
        public override bool? CanDamage()
        {
            return shouldDamage;
        }
    }
}
