using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Insignia.Prim;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Core.Common.Systems;
using Terraria.DataStructures;
using Microsoft.CodeAnalysis;

namespace Insignia.Content.Items.Weapons.Sets.Glacial
{
    public class TestProj : ModProjectile
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
        int i = 0;
        List<Vector2> keypoints = new();
        Vector2 mouse;
        Vector2 vectorToMouse;
        Vector2 mousew;
        ProjKeyFrameHandler keyFrameHandler;
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];

            keyFrameHandler = new(KeyFrameInterpolationCurve.Slerp, "Insignia/Content/Items/Weapons/Sets/Glacial/SwingPoints");
            mousew = Main.MouseWorld;
            vectorToMouse = player.Center.DirectionTo(mousew);
            keyFrameHandler.SetAiDefaults(Projectile, player, mousew);
            keypoints = keyFrameHandler.GetPoints(35);

            if (player.direction == -1)
            {
                keypoints = keyFrameHandler.GetPoints(45);
                i = keypoints.Count;
            }
        }
        //float rot;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            mouse = player.Center + vectorToMouse;

            //Dust.NewDustPerfect(mouse, DustID.Adamantite, Vector2.Zero);

            keyFrameHandler.SetAiDefaults(Projectile, player, mouse);
            Projectile.Center = keyFrameHandler.CalculateSwordSwingPointsAndApplyRotation(Projectile, mouse, player, keypoints, ref i);

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi + MathHelper.PiOver4 * player.direction);
        }
    }
}
