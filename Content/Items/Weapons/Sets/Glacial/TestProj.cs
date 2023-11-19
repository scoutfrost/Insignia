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
            Projectile.timeLeft = 3600;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
        }
        Chain chain;
        public static Texture2D texture;
        public override bool? CanDamage()
        {
            return false;
        }
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
            NPC.NewNPC(Projectile.GetSource_FromThis(), (int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, ModContent.NPCType<TestNPC>());
            /*Player player = Main.player[Projectile.owner];
            ChainCaller chaincaller = new();
            chain = new(player.Center, player.Center + new Vector2(-1, 10), 10, 15, default, new Vector2(0, 0.2f), true, true, false);
            chaincaller.Create(chain);*/
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            //chain.points[1].pos = player.Center;
        }
        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            float limb1Length = 60;
            float limb2Length = 100;
            int leftOrRight = -1;
            Vector2 joint1 = Projectile.Center;

            Vector2 joint2 = Projectile.Center + Projectile.Center.DirectionTo(Main.MouseWorld) * Vector2.Distance(joint1, Main.MouseWorld);
            float[] rotations = TwoJoint2LimbIKSolver(limb1Length, limb2Length, joint1, ref joint2);

            Main.EntitySpriteDraw(texture, joint1 - Main.screenPosition, texture.Bounds, lightColor,
                             rotations[0] * leftOrRight + joint1.DirectionTo(joint2).ToRotation(), texture.Size() / 2, new Vector2(limb1Length / 20, 1), SpriteEffects.None, default);

            Main.EntitySpriteDraw(texture, joint2 - Main.screenPosition, texture.Bounds, lightColor,
                            -rotations[1] * leftOrRight + joint2.DirectionTo(joint1).ToRotation(), texture.Size() / 2, new Vector2(limb2Length / 20, 1), SpriteEffects.None, default);
        }
        public float[] TwoJoint2LimbIKSolver(float limbLength1, float limbLength2, Vector2 joint, ref Vector2 footpos)
        {
            float maxLimbDist = limbLength1 + limbLength2;

            float length = MathHelper.Clamp(Vector2.Distance(joint, footpos), Math.Abs(limbLength1 - limbLength2), maxLimbDist - 0.1f);
            footpos = joint + joint.DirectionTo(footpos) * length;

            float a = joint.Distance(footpos);

            if (a <= Math.Abs(limbLength1 - limbLength2))
                a = Math.Abs(limbLength1 - limbLength2);

            float rotation1 = (float)Math.Acos((limbLength1 * limbLength1 + a * a - limbLength2 * limbLength2) / (2 * limbLength1 * a));
            float rotation2 = (float)Math.Acos((-limbLength1 * limbLength1 + a * a + limbLength2 * limbLength2) / (2 * limbLength2 * a));

            return new float[2] { rotation1, rotation2 };
        }

        /*float length = MathHelper.Clamp(Vector2.Distance(joint1 + joint1.DirectionTo(joint2), Main.MouseWorld), Math.Abs(limb1Length - limb2Length), maxLimbDist - 0.1f);
        joint2 = Projectile.Center + Projectile.Center.DirectionTo(Main.MouseWorld) * length;

        float a = joint1.Distance(joint2);

        if (a <= Math.Abs(limb1Length - limb2Length))
            a = Math.Abs(limb1Length - limb2Length);

        //Dust d = Dust.NewDustPerfect(joint2, DustID.Adamantite, Vector2.Zero);
        //d.noGravity = true;


        float rotation1 = (float)Math.Acos((limb1Length * limb1Length + a * a - limb2Length * limb2Length) / (2 * limb1Length * a));
        float rotation2 = (float)Math.Acos((-limb1Length * limb1Length + a * a + limb2Length * limb2Length) / (2 * limb2Length * a));*/
    }
}

