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

namespace Insignia.Content.Items.Weapons.Sets.Glacial
{
    internal class CrystalScythe : ModItem
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

            Item.shoot = ModContent.ProjectileType<CrystalScytheProjectile>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
        }
        ProjKeyFrameHandler handler;
        Vector2 mouse;
        List<Vector2> points;
        int i; 
        Vector2 vectorToMouse;
        Player Player => Main.player[Projectile.owner];
        public override void OnSpawn(IEntitySource source)
        {
            handler = new(KeyFrameInterpolationCurve.Slerp, "Insignia/Content/Items/Weapons/Sets/Glacial/SwingPoints", 23);
            points = handler.GetPoints(30);

            vectorToMouse = Player.Center.DirectionTo(Main.MouseWorld);
           
            i = points.Count - 1;
            if (Player.direction == 1)
                i = 0;
        }
        public override void AI()
        {
            float rotOffset = Player.direction == 1 ? MathHelper.Pi : 0;
            mouse = Player.Center + vectorToMouse;
            handler.SetAiDefaults(Projectile, Player, mouse);
            Projectile.Center = handler.CalculateSwordSwingPointsAndApplyRotation(Projectile, mouse, Player, points, ref i, Vector2.Zero, false, rotOffset);

            Player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + (Player.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver2 + MathHelper.PiOver4));
        }
        float time;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Player.direction == -1)
            {
                int i2 = points.Count - 1 - i;
                time = i2 / (points.Count / 1.5f);
                //Main.NewText(i2);
            }
            else
            {
                time = i / (points.Count / 1.5f);
                //Main.NewText(i);
            }
            time = EasingFunctions.EaseInOutQuad(time);

            //Main.NewText(time);
            GenericPrimTrail prim = new(new(100, 100, 100, 10), Projectile.oldPos, 10);
            prim.Draw();
            ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, lightColor, new Vector2(0.78f + time, time));
            return false;
        }
    }
}
