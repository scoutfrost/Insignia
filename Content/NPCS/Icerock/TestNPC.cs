using Insignia.Core.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insignia.Content.NPCS.Icerock
{
    public class TestNPC : ProcedurallyAnimatedNPC
    {
        public override string Texture => Helpers.Helper.Empty;

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 36;
            NPC.aiStyle = -1;
            NPC.damage = 7;
            NPC.defense = 2000;
            NPC.lifeMax = 2500;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 25f;
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

        public override void SafeOnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 2; i++)
            {
                Limb limb = new(NPC.Center, NPC.Center + new Vector2(0, 10), new Texture2D[2] { texture, texture }, new float[2] { 20, 20 });

                float maxLimbDist = limb.lengthOfLimbSegments[0] + limb.lengthOfLimbSegments[1];
                float length = MathHelper.Clamp(Vector2.Distance(limb.attachedJointPos, limb.endPos), Math.Abs(limb.lengthOfLimbSegments[0] - limb.lengthOfLimbSegments[1]), maxLimbDist - 0.01f); //subtracting by 0.01 to avoid NaN errors
                //limb.endPos = limb.attachedJointPos + limb.attachedJointPos.DirectionTo(limb.endPos) * length;

                Limbs.Add(limb);
                WhichLegsMoveInSuccession.Add(new List<Limb>());
                WhichLegsMoveInSuccession[i].Add(limb);
            }
        }

        public override void SafeAI()
        {
            //NPC.velocity = (Main.MouseWorld - NPC.Center).SafeNormalize(Vector2.Zero) * 10;
            NPC.Center = Main.MouseWorld;
            for (int i = 0; i < Limbs.Count; i++)
            {
                Limb l = Limbs[i];
                l.attachedJointPos = NPC.Center;

                Dust d = Dust.NewDustPerfect(l.attachedJointPos, DustID.Adamantite, Vector2.Zero);
                d.noGravity = true;

                Dust d1 = Dust.NewDustPerfect(l.destinationTile, DustID.AmberBolt, Vector2.Zero);
                d1.noGravity = true;

                Dust d2 = Dust.NewDustPerfect(l.endPos, DustID.ArgonMoss, Vector2.Zero);
                d2.noGravity = true;
            }
            //NPC.Center = Main.MouseWorld;
        }

        public override Vector2 GetDestinationTile(Limb limb)
        {
            //Vector2 vel = NPC.position - NPC.oldPosition;

            float maxLimbDist = limb.lengthOfLimbSegments[0] + limb.lengthOfLimbSegments[1] - 0.01f;

            return limb.endPos + new Vector2(Helpers.Helper.Pythagoras(default, distanceFromGround, maxLimbDist), 0);// * vel;
        }

        private float t = 0;
        private float distanceFromGround = 20;

        public override void LegMovement(ref Limb limb, Vector2 targetTile)
        {
            float maxLimbDist = limb.lengthOfLimbSegments[0] + limb.lengthOfLimbSegments[1] - 0.01f; //subtracting by 0.01 to avoid NaN errors

            t += 0.01f;
            if (limb.endPos.Distance(NPC.Center) >= maxLimbDist)
            {
                limb.endPos = Vector2.Lerp(limb.endPos, targetTile, t);
            }

            //limb.endPos *= length;
            if (t >= 1)
                t = 0;

            //Main.NewText(limb.endPos);
            //Main.NewText(t);
            //Main.NewText(limb.destinationTile);
            //Main.NewText("CRAXWAXAXAXWXWAX");
        }
    }
}