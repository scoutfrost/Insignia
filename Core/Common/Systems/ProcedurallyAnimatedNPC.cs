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

namespace Insignia.Core.Common.Systems
{
    public class Limb
    {
        public float[] lengthOfLimbSegments;//maybe could combine lengths and textures into one dictionary? the textures would be the keys and the lengths the values
        public Texture2D[] limbSegmentTextures;
        public Vector2 endPos;
        public Vector2 attachedJointPos;
        public Vector2 destinationTile;
        public bool bendsFowards = true;
        internal bool ismoving = false;
        public Limb(Vector2 attachedJoint, Vector2 endPos, Texture2D[] limbTextures, float[] lengthOfLimbSegments)
        {
            attachedJointPos = attachedJoint;
            this.endPos = endPos;
            this.lengthOfLimbSegments = lengthOfLimbSegments;
            limbSegmentTextures = limbTextures;
            destinationTile = new();
        }
        
    }
	public abstract class ProcedurallyAnimatedNPC : ModNPC
	{
        int i = 0;
        protected List<List<Limb>> WhichLegsMoveInSuccession = [];
        protected List<Limb> Limbs = [];
        int maxDistance;
        public override void OnSpawn(IEntitySource source)
        {
            SafeOnSpawn(source);
            for (int i = 0; i < WhichLegsMoveInSuccession[0].Count; i++)
            {
                Limb limb = WhichLegsMoveInSuccession[0][i];
                limb.destinationTile = GetDestinationTile(limb);
                //Main.player[0].Center = limb.destinationTile;
            }
        }
        public sealed override void AI()
        {
            SafeAI();
            int next = i + 1;
            if (next >= WhichLegsMoveInSuccession.Count)
                next = 0;

            for (int j = 0; j < WhichLegsMoveInSuccession[i].Count; j++)
            {
                Limb limb = WhichLegsMoveInSuccession[i][j];
                maxDistance = ((int)limb.lengthOfLimbSegments[0] / 20) + ((int)limb.lengthOfLimbSegments[1] / 20); // the drawn scale, see predraw hook

                if (limb.destinationTile == limb.endPos)
                {
                    if (limb.attachedJointPos.Distance(limb.destinationTile) >= maxDistance)
                    {
                        for (int k = 0; k < WhichLegsMoveInSuccession[next].Count; k++)
                        {
                            Limb nextMovingLimb = WhichLegsMoveInSuccession[next][k];
                            nextMovingLimb.destinationTile = GetDestinationTile(nextMovingLimb);
                        }
                        i++;
                        if (i >= WhichLegsMoveInSuccession.Count)
                            i = 0;
                    }
                }
                LegMovement(ref limb, limb.destinationTile);
            }
        }
        //hardcoded but uhhh ummm
        //i'll cross that bridge when i get to it

        //VERY placeholder drawing- TODO: implement post/predraw for limbs drawing behind npc + more flexibility for implemented class
        public sealed override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (SafePreDraw(spriteBatch, screenPos, drawColor))
            {
                for (int i = 0; i < Limbs.Count; i++)
                {
                    Limb limb = Limbs[i];

                    //float maxLimbDist = limb.lengthOfLimbSegments[0] + limb.lengthOfLimbSegments[1];
                    //float length = MathHelper.Clamp(Vector2.Distance(limb.attachedJointPos, limb.endPos), Math.Abs(limb.lengthOfLimbSegments[0] - limb.lengthOfLimbSegments[1]), maxLimbDist - 0.01f); //subtracting by 0.01 to avoid NaN errors
                    

                    int leftOrRight = 1;
                    if (!limb.bendsFowards)
                        leftOrRight = -1;
                    if (NPC.direction == -1)
                        leftOrRight *= -1;

                    float limb1Length = limb.lengthOfLimbSegments[0];
                    float limb2Length = limb.lengthOfLimbSegments[1];

                    Texture2D texture = limb.limbSegmentTextures[0];
                    Texture2D texture2 = limb.limbSegmentTextures[1];

                    Vector2 joint1 = limb.attachedJointPos;
                    Vector2 joint2 = limb.endPos;

                    float[] rotations = TwoJoint2LimbIKSolver(limb1Length, limb2Length, joint1, ref joint2);

                    if (i == 0)
                    {
                        Main.EntitySpriteDraw(texture, joint1 - Main.screenPosition, texture.Bounds, Color.Black,
                                         rotations[0] * leftOrRight + joint1.DirectionTo(joint2).ToRotation(), texture.Size() / 2, new Vector2(1 * limb1Length / 20, 1), SpriteEffects.None, default);

                        Main.EntitySpriteDraw(texture2, joint2 - Main.screenPosition, texture2.Bounds, Color.Black,
                                        -rotations[1] * leftOrRight + joint2.DirectionTo(joint1).ToRotation(), texture.Size() / 2, new Vector2(1 * limb2Length / 20, 1), SpriteEffects.None, default);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(texture, joint1 - Main.screenPosition, texture.Bounds, drawColor,
                                         rotations[0] * leftOrRight + joint1.DirectionTo(joint2).ToRotation(), texture.Size() / 2, new Vector2(1 * limb1Length / 20, 1), SpriteEffects.None, default);

                        Main.EntitySpriteDraw(texture2, joint2 - Main.screenPosition, texture2.Bounds, drawColor,
                                        -rotations[1] * leftOrRight + joint2.DirectionTo(joint1).ToRotation(), texture.Size() / 2, new Vector2(1 * limb2Length / 20, 1), SpriteEffects.None, default);
                    }
                }
            }
            SafePreDraw(spriteBatch, screenPos, drawColor);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = 0; i < Limbs.Count; i++)
            {
                Limb limb = Limbs[i];
                float maxLimbDist = limb.lengthOfLimbSegments[0] + limb.lengthOfLimbSegments[1];
                float length = MathHelper.Clamp(Vector2.Distance(limb.attachedJointPos, limb.endPos), Math.Abs(limb.lengthOfLimbSegments[0] - limb.lengthOfLimbSegments[1]), maxLimbDist - 0.01f); 
                //limb.endPos = limb.attachedJointPos + limb.attachedJointPos.DirectionTo(limb.endPos) * length;
            }
        }
        public abstract void SafeOnSpawn(IEntitySource entitySource);
        public abstract void SafeAI();
        public virtual bool SafePreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) { return true; }
        public virtual void LegMovement(ref Limb limb, Vector2 targetTile) { }
        public virtual Vector2 GetDestinationTile(Limb limb) { return default; }
        //public virtual bool CustomDrawLimbs(SpriteBatch sb) { return false; }

        /// <param name="limbLength1">The length of the first limb segment.</param>
        /// <param name="limbLength2">The length of the second limb segment.</param>
        /// <param name="joint">Where the limb attatches to the object.</param>
        /// <param name="footpos">The end position of the limb- ie. a hand or a foot's position </param>
        /// <returns>Returns the correct rotations of the limbs in an array</returns>
        protected float[] TwoJoint2LimbIKSolver(float limbLength1, float limbLength2, Vector2 joint, ref Vector2 footpos)
        {
            float maxLimbDist = limbLength1 + limbLength2 - 0.01f; //subtracting by 0.01 to avoid NaN errors

            float length = MathHelper.Clamp(Vector2.Distance(joint, footpos), Math.Abs(limbLength1 - limbLength2), maxLimbDist); 
            footpos = joint + joint.DirectionTo(footpos) * length;

            float a = joint.Distance(footpos);

            if (a <= Math.Abs(limbLength1 - limbLength2))
                a = Math.Abs(limbLength1 - limbLength2);

            float rotation1 = (float)Math.Acos((limbLength1 * limbLength1 + a * a - limbLength2 * limbLength2) / (2 * limbLength1 * a));
            float rotation2 = (float)Math.Acos((-limbLength1 * limbLength1 + a * a + limbLength2 * limbLength2) / (2 * limbLength2 * a));

            return [rotation1, rotation2];
        }
        /*protected float[] TwoJoint2LimbIKSolver(float limbLength1, float limbLength2, Vector2 joint, Vector2 footpos) //not sure why i named it footpos instead of endpos lol
        {
            float a = joint.Distance(footpos);

            if (a <= Math.Abs(limbLength1 - limbLength2))
                a = Math.Abs(limbLength1 - limbLength2);

            float rotation1 = (float)Math.Acos((limbLength1 * limbLength1 + a * a - limbLength2 * limbLength2) / (2 * limbLength1 * a));
            float rotation2 = (float)Math.Acos((-limbLength1 * limbLength1 + a * a + limbLength2 * limbLength2) / (2 * limbLength2 * a));

            return new float[2] { rotation1, rotation2 };
        }*/
    }
}
