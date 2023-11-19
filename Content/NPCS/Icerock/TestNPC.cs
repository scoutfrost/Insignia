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

namespace Insignia.Content.NPCS.Icerock
{
	public class TestNPC : ProcedurallyAnimatedNPC
	{
        public override string Texture => "Insignia/Content/Items/Weapons/Sets/Torgustus/TorgustusArrow";
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
                Limb limb = new(NPC.Center, NPC.Center + new Vector2(0, 10), new Texture2D[2] {texture, texture}, new float[2] { 20, 20 });
                limb.endPos = NPC.Center + new Vector2(0, 10);
                Limbs.Add(limb);
                WhichLegsMoveInSuccession.Add(new List<Limb>());
                WhichLegsMoveInSuccession[i].Add(limb);
            }
        }
        public override void SafeAI()
        {
            NPC.velocity = NPC.Center.DirectionTo(Main.MouseWorld) * 10;
            for (int i = 0; i < Limbs.Count; i++)
            {
                Limb l = Limbs[i];
                l.attachedJointPos = NPC.Center;

                Dust d = Dust.NewDustPerfect(l.attachedJointPos, DustID.Adamantite, Vector2.Zero);
                d.noGravity = true;

                Dust d1 = Dust.NewDustPerfect(l.destinationTile, DustID.Adamantite, Vector2.Zero);
                d1.noGravity = true;

            }
            //NPC.Center = Main.MouseWorld;
        }
        public override Vector2 GetDestinationTile(ref Limb limb)
        {
            Dust d = Dust.NewDustPerfect(limb.endPos + new Vector2(20, 0), DustID.SparksMech, Vector2.Zero);
            d.noGravity = true;
            Main.NewText("AAAAA");
            return limb.endPos + new Vector2(200, 0) * NPC.velocity;
        }
        float t = 0;
        //TODO: restrict end pos 
        public override void LegMovement(ref Limb limb, Vector2 targetTile)
        {
            //Main.NewText(limb.destinationTile);
            //Main.NewText("CRAXWAXAXAXWXWAX");
            Dust d = Dust.NewDustPerfect(targetTile, DustID.Adamantite, Vector2.Zero);
            d.noGravity = true;
            t += 0.01f;
            if (limb.endPos.Distance(NPC.Center) > 50)
            {
                limb.endPos = Vector2.Lerp(limb.endPos, targetTile, t);
                //Main.NewText("CRAXWAXAXAXWXWAX");
            }
            if (t >= 1)
                t = 0;
            //Main.NewText(limb.endPos);
            //Main.NewText(t);
            Dust dust = Dust.NewDustPerfect(limb.endPos, DustID.Adamantite, Vector2.Zero);
            dust.noGravity = true;
        }
    }
}

