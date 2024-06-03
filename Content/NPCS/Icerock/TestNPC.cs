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
using Newtonsoft.Json.Serialization;

namespace Insignia.Content.NPCS.Icerock
{
	public class TestNPC : ProcedurallyAnimatedNPC
	{
        public override string Texture => Helpers.GeneralHelper.Empty;
        public override void SetDefaults()
        {
            NPC.width = 15;
            NPC.height = 15; 
            NPC.aiStyle = -1;
            NPC.damage = 7;
            NPC.defense = 2000; 
            NPC.lifeMax = 2500; 
            NPC.HitSound = SoundID.NPCHit1; 
            NPC.DeathSound = SoundID.NPCDeath1; 
            NPC.value = 25f;
            NPC.knockBackResist = 0;
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
            Limb limb = new(NPC.Center, NPC.Center + new Vector2(0, 10), [texture, texture], [60, 60]);
            Limb limb2 = new(NPC.Center, NPC.Center + new Vector2(-20, 10), [texture, texture], [60, 60]);

            limb.bendsFowards = false;
            limb2.bendsFowards = false;
            Limbs.Add(limb);
            Limbs.Add(limb2);
            WhichLegsMoveInSuccession.Add([]);
            WhichLegsMoveInSuccession.Add([]);
            WhichLegsMoveInSuccession[0].Add(limb); 
            WhichLegsMoveInSuccession[1].Add(limb2);
        }
        Vector2 gravity;
        public override void SafeAI()
        {
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R))
            {
                NPC.life = 0;
            }
            for (int i = 0; i < Limbs.Count; i++)
            {
                Limb l = Limbs[i];
                l.attachedJointPos = NPC.Center;

                Dust d1 = Dust.NewDustPerfect(l.destinationTile, DustID.AmberBolt, Vector2.Zero);
                d1.noGravity = true;

                Dust d2 = Dust.NewDustPerfect(l.endPos, DustID.ArgonMoss, Vector2.Zero);
                d2.noGravity = true;
            }

            int iterations = 9;
            int distOffGroundInTiles = 7;
            Vector2 pos = NPC.Center / 16;

            bool onground = false;

            for (int i = 0; i < iterations; i++)
            {
                if (WorldGen.TileEmpty((int)pos.X, (int)pos.Y) && !WorldGen.SolidTile((int)pos.X, (int)pos.Y))
                {
                    gravity = new Vector2(0, 10);
                    pos += new Vector2(0, 1);
                }
                else
                {
                    onground = true;
                    gravity = Vector2.Zero;
                    NPC.Center = (pos + new Vector2(0, -distOffGroundInTiles)) * 16;
                    break;
                }
            }
            
            NPC.Center += gravity + new Vector2(0, -10); // -10 to account to negate the default gravity on aistyle -1

            Dust d3 = Dust.NewDustPerfect(pos * 16, DustID.AmberBolt, Vector2.Zero);
            d3.noGravity = true;

            NPC.direction = Main.LocalPlayer.Center.X >= NPC.Center.X ? 1 : -1;

            if (!Helpers.GeneralHelper.DoesTileCollideWithLine(NPC.Center, Main.LocalPlayer.Center) && Vector2.DistanceSquared(NPC.Center, Main.LocalPlayer.Center) >= 10000 && onground)
            {
                NPC.Center += NPC.Center.DirectionTo(new Vector2(Main.LocalPlayer.Center.X, NPC.Center.Y)) * 4;
            }
            Main.NewText(NPC.direction);
        }
        Vector2 tile;
        public override Vector2 GetDestinationTile(Limb limb)
        {
            int stepdist = 20 * (int)NPC.velocity.LengthSquared() / 20 * NPC.direction;

            tile = (NPC.Center + new Vector2(stepdist, 0)) / 16;
            while (WorldGen.TileEmpty((int)tile.X, (int)tile.Y) && !WorldGen.SolidTile((int)tile.X, (int)tile.Y))
            {
                tile += new Vector2(0, 1);
            }
            return tile * 16;
        }
        float t = 0;
        public override void LegMovement(ref Limb limb, Vector2 targetTile)
        {
            Vector2 stepheight = new(0, -30);
            Vector2 controlPoint = limb.endPos + ((targetTile - limb.endPos) / 2) + stepheight;

            if (limb.endPos != targetTile)
            {
                t += 0.05f;
                limb.endPos = Helpers.EasingFunctions.Bezier([limb.endPos, controlPoint, targetTile], t);
            }
            else
            {
                t = 0;
            }
            if (t >= 1)
                t = 0;
        }
    }
}

