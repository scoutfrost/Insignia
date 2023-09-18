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
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.BlueSlime];
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
            NPC.damage = 9;
            NPC.lifeMax = 62;
            NPC.value = Item.buyPrice(0, 0, 5, 44);
            NPC.aiStyle = 1;
            NPC.HitSound = SoundID.DD2_CrystalCartImpact;
            NPC.DeathSound = SoundSystem.GlacialChunkKillSound;
            NPC.aiStyle = NPCAIStyleID.Slime;
            AIType = NPCID.BlueSlime;
            AnimationType = NPCID.BlueSlime;
            NPC.netAlways = true;
            NPC.netUpdate = true;
            NPC.defense = 3;

        }

       
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

       

        }
    

        public override void AI()
        {
          
            NPC.TargetClosest(true);

            Player player = Main.player[NPC.target];


            NPC.velocity *= 0.98f;
            NPC.spriteDirection = NPC.direction;

          
        }
    
        
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                   BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,

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

        }

        public override void HitEffect(NPC.HitInfo hit)
        {
                if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
                {
                   
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Left, NPC.velocity, Mod.Find<ModGore>("IcerockSlimeGore1").Type, 1.2f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("IcerockSlimeGore2").Type, 1.2f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Right, NPC.velocity, Mod.Find<ModGore>("IcerockSlimeGore3").Type, 1.2f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Top, NPC.velocity, Mod.Find<ModGore>("IcerockSlimeGore3").Type, 1.2f);


                }
            for (int i = 0; i < 23; i++)
            {

                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Stone, 2.5f * hit.HitDirection, -2.5f, 0, Color.White, 0.6f);

               
            }
        }
    }
}
