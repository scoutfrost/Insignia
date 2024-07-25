using Insignia.Biomes.ColdBiome.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Insignia.Content.NPCS.Icerock
{
    public class IcerockSlime2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            var value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 20;
            NPC.damage = 11;
            NPC.lifeMax = 62;
            NPC.value = Item.buyPrice(0, 0, 5, 44);
            NPC.aiStyle = 1;
            NPC.HitSound = SoundID.DD2_CrystalCartImpact;
            NPC.DeathSound = SoundID.NPCDeath7;
            AIType = NPCID.IlluminantSlime;
            AnimationType = NPCID.GreenSlime;
            NPC.netAlways = true;
            NPC.netUpdate = true;
            NPC.defense = 1;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color color = GetAlpha(Color.LightBlue) ?? Color.LightBlue;

            if (NPC.IsABestiaryIconDummy)
                color = Color.LightBlue;
        }

        private int dustTimer;

        public override void AI()
        {
            Lighting.AddLight(NPC.position, r: 0.1f, g: 0.15f, b: 0.3f);

            NPC.TargetClosest(true);
            dustTimer++;

            Player player = Main.player[NPC.target];

            NPC.velocity *= 1.013f;
            NPC.spriteDirection = NPC.direction;

            if (dustTimer >= 50)

            {
                for (int i = 0; i < 60; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.Ice, speed * 3, Scale: 1f);
                    ;
                    d.noGravity = true;
                }

                dustTimer = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20)
                NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GlacialChunkItem>(), 2, 2, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.IceBlock, 3, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 2, 4));
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 23; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ice, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.6f);
            }
        }
    }
}