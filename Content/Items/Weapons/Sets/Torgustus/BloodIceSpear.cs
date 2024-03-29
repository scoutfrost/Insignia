using Insignia.Core.Common.Systems;
using Insignia.Helpers;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Insignia.Content.Items.Weapons.Sets.Torgustus
{
    public class BloodIceSpear : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        private int i = 0;
        private List<Vector2> keypoints = new();
        private Vector2 mouse;
        private Vector2 vectorToMouse;
        private Vector2 mousew;
        private ProjKeyFrameHandler keyFrameHandler;

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            keyFrameHandler = new(KeyFrameInterpolationCurve.Bezier, "Insignia/Content/Items/Weapons/Sets/Torgustus/SwingPointsParabola");

            mousew = Main.MouseWorld;
            vectorToMouse = player.Center.DirectionTo(mousew);

            keypoints = keyFrameHandler.GetPoints();
            if (player.direction == -1)
            {
                keypoints = keyFrameHandler.GetPoints();
                i = keypoints.Count;
            }

            keyFrameHandler.ChangePoints(ref keypoints, new ProjKeyFrameHandler.DesiredChange((Vector2 point, int i) =>
            {
                return point = new(point.X * 1.75f, point.Y);
            }));
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            mouse = player.Center + vectorToMouse;

            keyFrameHandler.SetAiDefaults(Projectile, player, mouse);

            Projectile.Center = keyFrameHandler.CalculateSwordSwingPointsAndApplyRotation(Projectile, mouse, player, keypoints, ref i, Vector2.Zero, false, MathHelper.ToRadians(21)) + new Vector2(-40, -12).RotatedBy(player.Center.DirectionTo(mouse).ToRotation());
            Projectile.rotation = MathHelper.Clamp(Projectile.rotation, vectorToMouse.RotatedBy(MathHelper.ToRadians(70)).ToRotation(), vectorToMouse.RotatedBy(MathHelper.ToRadians(-90)).ToRotation());

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi + MathHelper.ToRadians(-21) + MathHelper.PiOver4 * player.direction);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjectileDrawHelper.QuickDrawProjectile(Projectile, null, null, Texture, lightColor, 1);
            return false;
        }
    }
}