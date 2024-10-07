using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Insignia.Core.Particles;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.UI;
using Insignia.Prim;
using System.IO;

namespace Insignia.Core.Common.Systems
{
    internal class PrimHandler : ModSystem
    {
        const int maxTrails = 50;
        static internal List<PrimTrail> deadPool = []; 
        static internal List<PrimTrail> activePool = [];

        static internal DynamicVertexBuffer dynamicVertexBuffer;
        static internal DynamicIndexBuffer dynamicIndexBuffer;

        static GraphicsDevice GD = Main.graphics.GraphicsDevice;
        static internal VertexPositionColorTexture[] Vertices;
        static internal short[] Indices;

        const int maxVertices = 10000;
        static internal int lastActiveVertexIndex;
        public override void Load()
        {
            for (int i = 0; i < maxTrails; i++)
            {
                deadPool[i] = new PrimTrail();
            }
            lastActiveVertexIndex = 0;

            Vertices = new VertexPositionColorTexture[maxVertices];
            Indices = new short[maxVertices + maxTrails * 2];
            dynamicVertexBuffer = new(GD, typeof(VertexPositionTexture), maxVertices, BufferUsage.WriteOnly);
            dynamicIndexBuffer = new(GD, typeof(short), Indices.Length, BufferUsage.WriteOnly);// two extra indeces for each trail; for degenerate triangles

            GD.SetVertexBuffer(dynamicVertexBuffer);
            GD.Indices = dynamicIndexBuffer;
        }
        public static PrimTrail CreateTrail(PrimData p)
        {
            PrimTrail returnTrail = deadPool[0];
            lastActiveVertexIndex += returnTrail.Vertices.Length;

            deadPool.RemoveAt(0);

            returnTrail.Pixelated = p.Pixelated;
            returnTrail.Color = p.Color;
            returnTrail.Points = p.Points;
            returnTrail.Width = p.Width;
            returnTrail.Shader = p.Shader;
            returnTrail.Texture = p.Texture;

            activePool.Add(returnTrail);
            return returnTrail;
        }
        /// <summary>
        /// Creates a trail without adding it to the active pool. This is so you can set its data before adding it to the pool yourself.
        /// </summary>
        /// <returns></returns>
        public static PrimTrail CreateTrail()
        {
            PrimTrail returnTrail = deadPool[0];
            deadPool.RemoveAt(0);
            return returnTrail;
        }
        public override void PostUpdateProjectiles()
        {
            for (int i = 0; i < activePool.Count; i++)
            {
                PrimTrail trail = activePool[i];
                trail.isActive = false;
            }
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            for (int i = 0; i < activePool.Count; i++)
            {
                PrimTrail trail = activePool[i];
                if (trail.kill || !trail.isActive)
                {
                    activePool.Remove(trail);

                    trail.Vertices = null;
                    trail.indices = null;
                    trail.Pixelated = false;
                    trail.Width = 0;
                    trail.Color = Color.White;
                    trail.Points = null;
                    trail.Shader = null;
                    trail.Texture = null;
                    trail.texCoordBottomOffset = Vector2.Zero;
                    trail.texCoordTopOffset = Vector2.Zero;
                    trail.startVertexIndex = 0;

                    lastActiveVertexIndex -= trail.Vertices.Length;
                    deadPool.Add(trail);
                    continue;
                }

                RasterizerState rasterizerState = new()
                {
                    CullMode = CullMode.None
                };
                GD.RasterizerState = rasterizerState;
                if (!trail.Pixelated)
                {
                    foreach (EffectPass pass in trail.Shader.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GD.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, trail.Vertices, 0, trail.Vertices.Length, trail.indices, 0, trail.Vertices.Length - 2);
                    }
                }
                Main.graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

                RenderTargetBinding[] previousRTs = GD.GetRenderTargets();
                RenderTarget2D pixelationTarget = new(GD, GD.PresentationParameters.BackBufferWidth / 2, GD.PresentationParameters.BackBufferHeight / 2);

                GD.SetRenderTarget(pixelationTarget);
                GD.Clear(Color.Transparent);

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                    RasterizerState.CullNone, null);
                             
                foreach (EffectPass pass in trail.Shader.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GD.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, trail.Vertices, 0, trail.Vertices.Length, trail.indices, 0, trail.Vertices.Length - 2);
                }
                Main.spriteBatch.End();
                GD.SetRenderTargets(previousRTs);

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                    RasterizerState.CullNone, null);

                Main.spriteBatch.Draw(pixelationTarget, new Rectangle(0, 0, pixelationTarget.Width * 2, pixelationTarget.Height * 2), Color.White);
                Main.spriteBatch.End();

                pixelationTarget.Dispose();
                
            }
        }
    }
    public struct PrimData
    {
        public bool Pixelated;
        public Color Color;
        public Vector2[] Points;
        public float Width;
        public Effect Shader;
        public Texture2D Texture;
    }
    internal class PrimTrail
    {
        internal bool isActive;
        public BasicEffect basicEffect = new(GD);
        static GraphicsDevice GD = Main.graphics.GraphicsDevice;
        public bool Pixelated = false;
        public VertexPositionColorTexture[] Vertices;
        internal protected short[] indices;
        public Color Color;
        public Vector2[] Points;
        public float Width;
        public Effect Shader;
        public Texture2D Texture;
        public bool kill = false;
        public Vector2 texCoordTopOffset;
        public Vector2 texCoordBottomOffset;
        public int startVertexIndex;
        protected virtual void Update() { }
        //protected virtual void OnKill() { }
        protected virtual void CustomDraw(GraphicsDevice graphicsDevice) { }
        public bool ShouldCustomDraw = false;
        public bool WidthFallOff;
        public void Initialize()
        {
            Vertices = new VertexPositionColorTexture[Points.Length * 2];
            indices = new short[Vertices.Length];
            
            startVertexIndex = PrimHandler.lastActiveVertexIndex;
            Vertices.CopyTo(PrimHandler.Vertices, startVertexIndex);
            PrimHandler.dynamicVertexBuffer.SetData(Vertices, startVertexIndex, Vertices.Length, SetDataOptions.None);
            PrimHandler.dynamicIndexBuffer.SetData(indices, startVertexIndex, indices.Length);
            PrimHandler.activePool.Add(this);
        }
        public void Draw()
        {
            if (Points.Length < 2)
                return;
            if (ShouldCustomDraw)
                CustomDraw(GD);

            isActive = true;

            Update();

            if (!ShouldCustomDraw && Points != null)
            {
                GenerateVertices();
            }
            GenerateIndices();

            if (Shader == default || Shader.GetType() == basicEffect.GetType())
            {
                if (Texture != null)
                    basicEffect.TextureEnabled = true;
                basicEffect.VertexColorEnabled = true;

                basicEffect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
                basicEffect.View = Main.GameViewMatrix.TransformationMatrix;
                basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, GD.Viewport.Width, GD.Viewport.Height, 0, -1, 1);

                basicEffect.Texture = Texture;
                Shader = basicEffect;
            }
/*            foreach (EffectPass pass in Shader.CurrentTechnique.Passes)
            {
                if (!Pixelated)
                {
                    pass.Apply();
                    GD.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, Vertices, 0, Vertices.Length, indices, 0, Vertices.Length - 2);
                }
            }*/
        }
        private void GenerateIndices()
        {
            for (short i = 0; i < Vertices.Length + 2; i++)
            {
                if (Vertices[i] == default)
                    return;
                if (i == 0)
                {
                    indices[i] = (short)startVertexIndex;
                    return;
                }
                if (i == Vertices.Length + 1) 
                {
                    indices[i] = (short)(startVertexIndex + i - 2);
                    return;
                }

                indices[i] = (short)(startVertexIndex + i - 1);
            }
        }
        private void GenerateVertices()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                if (Points[i].X <= 100)
                {
                    Points[i] = Points[i - 1];
                    if (Points[i].X == 0)
                        return;
                }
                Color.A = 0;
                bool lastpoint = i == Points.Length - 1;
                Vector2 current = Points[i];
                Vector2 next = lastpoint ? Points[i - 1] : Points[i + 1];
                float progress = 1 - ((float)i / Points.Length);
                Vector3 normal = new(current.DirectionTo(next).RotatedBy(lastpoint ? MathHelper.PiOver2 : -MathHelper.PiOver2) * Width, 0);
                if (WidthFallOff)
                    normal *= progress;

                Vector2 texCoordsTop = i % 2 == 0 ? texCoordTopOffset : new Vector2(1, 0) + texCoordTopOffset; //0,0 : 1,0
                Vector2 texCoordsBottom = i % 2 == 0 ? new Vector2(0, 1) + texCoordBottomOffset : new Vector2(1, 1) + texCoordBottomOffset;//0,1 : 1,1

                Vertices[i * 2] = new(new Vector3(current, 0) + normal, Color, texCoordsTop);
                Vertices[i * 2 + 1] = new(new Vector3(current, 0) - normal, Color, texCoordsBottom);

                /*Dust d = Dust.NewDustPerfect(new Vector2((int)Vertices[i * 2].Position.X, (int)Vertices[i * 2].Position.Y), DustID.AmberBolt, Vector2.Zero);
                d.noGravity = true;
                Dust d1 = Dust.NewDustPerfect(new Vector2((int)Vertices[i * 2 + 1].Position.X, (int)Vertices[i * 2 + 1].Position.Y), DustID.Adamantite, Vector2.Zero);
                d1.noGravity = true;*/
            }
        }
    }
}