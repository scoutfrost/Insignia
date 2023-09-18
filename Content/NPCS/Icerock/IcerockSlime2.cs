using Insignia.Content.Items.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Insignia.Content.NPCS.Icerock
{
    public class IcerockSlime2 : ModNPC
    {

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[2];
            NPCID.Sets.TrailCacheLength[NPC.type] = 3;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            var value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            { // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);

        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 20;
            NPC.damage = 12;
            NPC.lifeMax = 62;
            NPC.value = Item.buyPrice(0, 0, 5, 44);
            NPC.aiStyle = 1;
            NPC.HitSound = SoundID.DD2_CrystalCartImpact;
            NPC.DeathSound = SoundID.NPCDeath7;
            AIType = NPCID.IlluminantSlime;
            AnimationType = NPCID.GreenSlime;
            NPC.netAlways = true;
            NPC.netUpdate = true;
            NPC.defense = 3;

        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Color color = GetAlpha(Color.LightBlue) ?? Color.LightYellow;

            if (NPC.IsABestiaryIconDummy)
                color = Color.LightYellow;

        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneSnow)
                return SpawnCondition.Overworld.Chance * 0.16f;

            return base.SpawnChance(spawnInfo);
        }
        private int dustTimer;

        public override void AI()
        {
            Lighting.AddLight(NPC.position, r: 0.1f, g: 0.15f, b: 0.3f) ;

            NPC.TargetClosest(true);
            dustTimer++;

            Player player = Main.player[NPC.target];


            NPC.velocity *= 1.013f;
            NPC.spriteDirection = NPC.direction;

            if (dustTimer >= 30)


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
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the spawning conditions of this NPC that is listed in the bestiary.
                   BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("Tough as nails, these slimes have been bouncing around the Arctic Orogeny for decades, getting frozen with ice and hard rubble"),


            });
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

                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);

                var d = Dust.NewDustPerfect(NPC.position, DustID.Ice, speed * 5, Scale: 1f);
                ;
                d.noGravity = true;
            }
        }
    }
}
