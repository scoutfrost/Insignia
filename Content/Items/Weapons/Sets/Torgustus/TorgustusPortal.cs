using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.DataStructures;
using System;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Insignia.Helpers;
using Insignia.Content.Items.Weapons.Sets.Torgustus;

namespace Insignia.Content.Items.Weapons.Sets.Torgustus
{
    public class TorgustusPortal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.scale = 1;
        }
        public override bool? CanDamage() => false;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Center = Main.MouseWorld;
        }
        Player player => Main.player[Projectile.owner];
        Projectile otherPortal;
        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            List<Projectile> portals = player.GetModPlayer<TorgustusPortalPlayer>().portalsActive;
            //alt implementation
            /*var otherPortals = Main.projectile.SkipLast(1).Where(portal => portal.active && portal.whoAmI != Projectile.whoAmI && portal.owner == Projectile.owner && portal.type == ModContent.ProjectileType<TorgustusPortal>());
            if (otherPortals.ToArray().FirstOrDefault() != null && otherPortals != null)
            {
                otherPortal = otherPortals.ToArray()[0];
            }*/
            if (portals.Count > 1)
            {
                int index = player.GetModPlayer<TorgustusPortalPlayer>().portalsActive.IndexOf(Projectile);
                if (portals[index] == portals.FirstOrDefault()) {
                    otherPortal = portals[index + 1];
                }
                else {
                    otherPortal = portals[index - 1];
                }
                //if (portals.any())
                var shotArrow = Main.projectile.SkipLast(1).Where(arrow => arrow.active && arrow.whoAmI != Projectile.whoAmI && arrow.owner == Projectile.owner
                && arrow.type == player.GetModPlayer<TorgustusBowPlayer>().arrowType && arrow.getRect().Intersects(Projectile.getRect()) && arrow.ai[1] != 1 && arrow.damage != 0);
                foreach (var arrow in shotArrow)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        arrow.Center = otherPortal.Center;
                        arrow.ai[1] = 1;
                        arrow.damage += 30;
                        arrow.velocity *= 1.2f;
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, Color.Orange, 1);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            player.GetModPlayer<TorgustusPortalPlayer>().portalsActive.Remove(Projectile);
        }
    }
    public class TorgustusPortalPlayer : ModPlayer
    {
        public List<Projectile> portalsActive = new();
    }
}