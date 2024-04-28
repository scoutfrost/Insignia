using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Core.Common.Systems;
using Terraria.GameContent;

namespace Insignia.Content.NPCS.Fish
{
    public class AuratailEel : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.CountsAsCritter[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 60;
            NPC.height = 25;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.lifeMax = 5;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = .35f;
            NPC.aiStyle = 16;
            NPC.noGravity = true;
            NPC.npcSlots = 0.5f;
            AIType = NPCID.Goldfish;
            NPC.dontCountMe = true;
            NPC.alpha = 255;
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            NPC.alpha++;

            Player target = Main.player[NPC.target];
            int distance = (int)Math.Sqrt((NPC.Center.X - target.Center.X) * (NPC.Center.X - target.Center.X) + (NPC.Center.Y - target.Center.Y) * (NPC.Center.Y - target.Center.Y));
            if (distance < 200 && NPC.wet)
            {
                NPC.alpha = 20;
                Vector2 vel = NPC.DirectionFrom(target.Center);
                vel.Normalize();
                vel *= 6f;
                NPC.velocity = vel;
                NPC.rotation = NPC.velocity.X * .03f;
                if (target.position.X > NPC.position.X)
                {
                    NPC.spriteDirection = -1;
                    NPC.direction = -1;
                    NPC.netUpdate = true;
                    NPC.alpha = 20;
                }
                else if (target.position.X < NPC.position.X)
                {
                    NPC.spriteDirection = 1;
                    NPC.direction = 1;
                    NPC.netUpdate = true;
                    NPC.alpha = 255;
                }

            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var pos = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);
            spriteBatch.Draw(TextureAssets.Npc[NPC.type].Value, pos, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) => GlowMaskSystem.DrawNPCGlowMask(spriteBatch, NPC, ModContent.Request<Texture2D>(Texture + "_Glow", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, screenPos);

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.18f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
    }
}
