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

            keyFrameHandler = new(KeyFrameInterpolationCurve.Slerp, "Insignia/Content/Items/Weapons/Sets/Glacial/SwingPoints", 20);
            mousew = Main.MouseWorld;
            vectorToMouse = player.Center.DirectionTo(mousew);
            keyFrameHandler.SetAiDefaults(Projectile, player, mousew);
            keypoints = keyFrameHandler.GetPoints(45);

            if ((player.direction == -1 && !upswing) || (player.direction == 1 && upswing))
            {
                keypoints = keyFrameHandler.GetPoints(55);
                i = keypoints.Count - 1;
            }
        }
        bool upswing = false;
        //float rot;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            mouse = player.Center + vectorToMouse;
            keyFrameHandler.SetAiDefaults(Projectile, player, mouse);

            if (player.GetModPlayer<TestProjPlayer>().SwingCount % 2 != 0)
                upswing = true;
            else
                upswing = false;
            
            Projectile.Center = keyFrameHandler.CalculateSwordSwingPointsAndApplyRotation(Projectile, mouse, player, keypoints, ref i, new Vector2(-5, -5), upswing);

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.Pi + MathHelper.PiOver4 * player.direction);
        }
    }
    public class TestProjPlayer : ModPlayer
    {
        public int SwingCount = 0;
    }
}
