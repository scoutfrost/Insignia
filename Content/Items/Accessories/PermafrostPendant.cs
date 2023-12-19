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
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<AccessoryPlayer>().OnHitNPCEvent += PermafrostPendant_OnHitNPCEvent;
        }

        private void PermafrostPendant_OnHitNPCEvent(NPC target, NPC.HitInfo hit, int damageDone, Player player)
        {
            if (hit.Crit)
            {
                Projectile.NewProjectile(player.GetSource_Accessory(Entity), target.Center, Vector2.Zero, ModContent.ProjectileType<PermafrostPendantExplosion>(), 75, 0);
            }
        }
    }
    public class PermafrostPendantExplosion : ModProjectile
    {
        public override string Texture => "Insignia/Content/Items/Weapons/Sets/Torgustus/TorgustusArrow";
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
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
        public override void OnSpawn(IEntitySource source)
        {
        }
        int timer = 0;
        bool shouldDamage = false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (timer++ >= 120)
                shouldDamage = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.velocity 
        }
        public override bool? CanDamage()
        {
            return shouldDamage;
        }
    }
    internal class AccessoryPlayer : ModPlayer 
    {
        //using events because if it was a method i would have to add each function call to the modplayer and thats annoying and not super clean 
        //could also use a list/array of delegates then add to that but i like this better 
        public event Action<NPC, NPC.HitInfo, int, Player> OnHitNPCEvent;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitNPCEvent?.Invoke(target, hit, damageDone, Entity);
            base.OnHitNPC(target, hit, damageDone);
        }
    }

}
