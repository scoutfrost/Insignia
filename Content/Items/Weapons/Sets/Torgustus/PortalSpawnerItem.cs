using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insignia.Content.Items.Weapons.Sets.Torgustus
{
    public class PortalSpawnerItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 9;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 7500;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item11;
            Item.noMelee = true;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.shoot = ModContent.ProjectileType<TorgustusPortal>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile portal = Projectile.NewProjectileDirect(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback);
            player.GetModPlayer<TorgustusPortalPlayer>().portalsActive.Add(portal);
            List<Projectile> portals = player.GetModPlayer<TorgustusPortalPlayer>().portalsActive;

            if (portals.Count > 2)
            {
                Projectile firstPortal = player.GetModPlayer<TorgustusPortalPlayer>().portalsActive.FirstOrDefault();
                firstPortal.Kill();
                player.GetModPlayer<TorgustusPortalPlayer>().portalsActive.Remove(firstPortal);
            }
            return false;
        }
    }
}