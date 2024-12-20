﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Insignia.Content.Items.Weapons.Sets.Glacial
{
    internal class GlacialBasher : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.damage = 34;
            Item.DamageType = DamageClass.Melee;
            Item.rare = ItemRarityID.Green;

			Item.useTime = 8; 
			Item.useAnimation = 8; 
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = false;

			Item.knockBack = 5f; 
			Item.noMelee = true; 
            Item.noUseGraphic = true;

            Item.shoot = ModContent.ProjectileType<GlacialBasherHeld>();
		}
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 1; i++)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback);
            }
            return false;
        }
    }
}
