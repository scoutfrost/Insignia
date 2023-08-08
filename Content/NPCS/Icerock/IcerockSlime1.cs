using Insignia.Content.Items.Misc;
using Insignia.Core.Common.Systems;
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
    public class IcerockSlime1 : ModNPC
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
            NPC.DeathSound = Soundd.GlacialChunkKillSound;
            NPC.aiStyle = NPCAIStyleID.Slime;
            AIType = NPCID.IlluminantSlime;
            AnimationType = NPCID.GreenSlime;
            NPC.netAlways = true;
            NPC.netUpdate = true;
            NPC.defense = 3;

        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 20)
                NPC.frameCounter = 0;
            NPC.frame.Y = (int)NPC.frameCounter / 10 * frameHeight;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

       

        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneSnow)
                return SpawnCondition.Overworld.Chance * 0.16f;

            return base.SpawnChance(spawnInfo);
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.position, r: 0.1f, g: 0.15f, b: 0.3f); ;

            NPC.TargetClosest(true);

            Player player = Main.player[NPC.target];


            NPC.velocity *= 0.98f;
            NPC.spriteDirection = NPC.direction;

          
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item9);

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
        public override bool CheckDead()
        {
            for (int i = 0; i < Main.rand.Next(1, 1); i++)
            {
                NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center, ModContent.NPCType<IcerockSlime2>(), ai3: 1).scale = 1f;
            }
            return true;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GlacialChunkItem>(), 2, 2, 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.IceBlock, 3, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 2, 4));

        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("IcerockSlimeGore1").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("IcerockSlimeGore2").Type, 1f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("IcerockSlimeGore3").Type, 1f);

            }
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
