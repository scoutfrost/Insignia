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
using Insignia.Content.Items.Weapons.Sets.Torgustus;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Insignia.Core.Particles;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace Insignia
{
    public class ChainCaller : ModSystem
    {
        internal static List<Chain> chains = new();
        /*public void Create(Vector2 startpos, Vector2 endpos, int pointCount, float lengthBetweenPoints, float chainWeight, float drag = 0.77f, Vector2 gravity = default, bool isStartPosStationary = true)
        {
            Chain chain = new(startpos, endpos, pointCount, lengthBetweenPoints, chainWeight, drag, gravity, isStartPosStationary);
            chains.Add(chain);
        }*/
        public void Create(Chain chain)
        {
            chains.Add(chain);
        }
        public override void PostUpdateProjectiles()
        {
            foreach (Chain chain in chains)
            {
                chain.UpdateChain();
            }
        }
        public override void PostDrawTiles()
        {
            foreach (Chain chain in chains)
                chain.DrawChain();
        }
    }
    public class Chain
    {
        public ChainPoint[] points;
        Vector2 start;
        float drag;
        float lengthBetweenPoints;
        Texture2D texture;
        Vector2 gravity = new(0, 0.5f);
        bool collidesWithTiles;
        bool collidesWithPlayers;

        //private float weight;
        //might implement weight parameter later but probably not
        public Chain(Vector2 startpos, Vector2 endpos, int pointCount, float lengthBetweenPoints, /*float chainWeight,*/ float drag = 0.75f, Vector2 gravity = default, bool isStartPosStationary = true, bool collidesWithTiles = false, bool collidesWithPlayers = true)
        {
            points = new ChainPoint[pointCount];
            start = startpos;
            this.drag = drag;
            this.lengthBetweenPoints = lengthBetweenPoints;
            if (gravity != default)
                this.gravity = gravity;
            this.collidesWithPlayers = collidesWithPlayers;
            this.collidesWithTiles = collidesWithTiles;

            //weight = chainWeight;
            Vector2 len = startpos.DirectionTo(endpos);
            for (int i = 0; i < pointCount; i++)
            {
                points[i] = new();
                points[i].pos = points[i].oldPos = start + len * lengthBetweenPoints * i;
            }
            if (isStartPosStationary)
            {
                points[0].stationary = true;
            }
            texture = (Texture2D)ModContent.Request<Texture2D>("Insignia/Content/Items/Weapons/Sets/Torgustus/TorgustusArrow", ReLogic.Content.AssetRequestMode.ImmediateLoad);
        }
        //TODO: chains weigh more as i increases
        public void UpdateChain()
        {
            if (points.Length > 0)
            {
                Vector2 force = Vector2.Zero;
                for (int i = 0; i < points.Length - 1; i++)
                {
                    Vector2 grav = gravity;
                    ChainPoint point = points[i];
                    if (point.stationary)
                    {
                        continue;
                    }
                    if (collidesWithPlayers)
                    {
                        for (int j = 0; j < Main.player.Length; j++)
                        {
                            Player player = Main.player[j];
                            Rectangle hitboxworldpos = new((int)player.position.X, (int)player.position.Y, 20, 42);
                            if (hitboxworldpos.Contains(point.pos.ToPoint()))
                            {
                                force = player.velocity;
                            }
                        }
                    }
                    ChainPoint lastpoint = points[i - 1];
                    ChainPoint nextpoint = points[i + 1];
                    //gravity *= (points.Length - i) / (20 - weight);
                    Vector2 vel = (point.pos - point.oldPos) * drag;
                    force *= drag; 
                    if (collidesWithTiles)
                    {
                        if (Main.tile[(point.pos / 16).ToPoint()].HasTile)
                        {
                            grav = vel = force = Vector2.Zero;
                            //Dust d = Dust.NewDustPerfect(point.pos, DustID.Adamantite, Vector2.Zero);
                            //d.noGravity = true;
                        }
                    }
                    point.oldPos = point.pos;
                    point.pos += vel += force += grav;

                    //float var = 0.5f;

                    if (Vector2.Distance(point.pos, lastpoint.pos) != lengthBetweenPoints)
                        point.pos += (point.pos.DirectionTo(nextpoint.pos) * (Vector2.Distance(point.pos, nextpoint.pos) - lengthBetweenPoints) * drag);
                        //lastpoint.pos += (lastpoint.pos.DirectionTo(point.pos) * (Vector2.Distance(point.pos, lastpoint.pos) - lengthBetweenPoints * var) * drag);

                }
            }
        }
        public void DrawChain()
        {
            if (points.Length > 0)
            {
                for (int i = 1; i < points.Length; i++)
                {
                    ChainPoint point = points[i];
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                        RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    Main.EntitySpriteDraw(texture, point.pos - Main.screenPosition, texture.Bounds, Lighting.GetColor(new Point((int)point.pos.X / 16, (int)point.pos.Y / 16)),
                            default, texture.Size() / 2, 1, SpriteEffects.None, default);
                    Main.spriteBatch.End();
                }
            }
        }
    }

    public class ChainPoint
    {
        internal Vector2 pos;
        internal Vector2 oldPos;
        internal bool stationary;
    }
}

