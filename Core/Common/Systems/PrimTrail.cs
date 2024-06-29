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

namespace Insignia.Core.Common.Systems
{
    internal class PrimHandler : ModSystem
    {
        static internal List<PrimTrail> trails = [];
        public override void PostUpdateProjectiles()
        {
            for (int i = 0; i < trails.Count; i++)
            {
                PrimTrail trail = trails[i];
                if (trail.kill || trail == null)
                {
                    trails.Remove(trail);
                    continue;
                }
            }
            //Main.NewText(trails.Count);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            for (int i = 0; i < trails.Count; i++)
            {
                PrimTrail trail = trails[i];
                if (trail.pixelated)
                {
                    Main.graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

                    GraphicsDevice GD = Main.graphics.GraphicsDevice;
                    RenderTargetBinding[] previousRTs = GD.GetRenderTargets();
                    RenderTarget2D pixelationTarget = new(GD, GD.PresentationParameters.BackBufferWidth / 2, GD.PresentationParameters.BackBufferHeight / 2);

                    GD.SetRenderTarget(pixelationTarget);
                    GD.Clear(Color.Transparent);

                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                        RasterizerState.CullNone, null);
                    
                    RasterizerState rasterizerState = new()
                    {
                        CullMode = CullMode.None
                    };
                    GD.RasterizerState = rasterizerState;

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
    }
    public class PrimTrail
    {
        public BasicEffect basicEffect = new(GD);
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        static GraphicsDevice GD = Main.graphics.GraphicsDevice;
        internal protected bool pixelated = false;
        public VertexPositionColorTexture[] Vertices;
        internal protected short[] indices;
        public Color Color;
        public Vector2[] Points;
        public float Width;
        public Effect Shader;
        public Texture2D Texture;
        public bool kill = false;
        protected virtual void Update() { }
        //protected virtual void OnKill() { }
        protected virtual void CustomDraw(GraphicsDevice graphicsDevice) { }
        public bool ShouldCustomDraw = false;
        public bool WidthFallOff;
        public PrimTrail(Vector2[] points, Color color, int width)
        {
            Points = points;
            Color = color;
            Width = width;
            Initialize();
        }
        public PrimTrail() { }
        public void Initialize()
        {
            Vertices = new VertexPositionColorTexture[Points.Length * 2];
            indices = new short[Vertices.Length];
            vertexBuffer = new(GD, typeof(VertexPositionTexture), Vertices.Length, BufferUsage.WriteOnly);
            indexBuffer = new(GD, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
            vertexBuffer.SetData(Vertices);

            PrimHandler.trails.Add(this);
        }
        public void Draw()
        {
            if (Points.Length < 2)
                return;
            if (ShouldCustomDraw)
                CustomDraw(GD);

            if (!ShouldCustomDraw && Points != null)
            {
                GenerateVertices();
                GenerateIndeces();
            }

            GD.SetVertexBuffer(vertexBuffer);
            GD.Indices = indexBuffer;

            RasterizerState rasterizerState = new()
            {
                CullMode = CullMode.None
            };
            GD.RasterizerState = rasterizerState;

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
            foreach (EffectPass pass in Shader.CurrentTechnique.Passes)
            {
                if (!pixelated)
                {
                    pass.Apply();
                    GD.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, Vertices, 0, Vertices.Length, indices, 0, Vertices.Length - 2);
                }
            }
        }
        private void GenerateIndeces()
        {
            for (short i = 0; i < Vertices.Length; i++)
            {
                if (Vertices[i] == default)
                    return;
                indices[i] = i;
            }
        }
        private void GenerateVertices()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                if (Points[i].X <= 100)
                    Points[i] = Points[0];
                Color.A = 0;
                bool lastpoint = i == Points.Length - 1;
                Vector2 current = Points[i];
                Vector2 next = lastpoint ? Points[i - 1] : Points[i + 1];
                float progress = 1 - ((float)i / Points.Length);
                Vector3 normal = new(current.DirectionTo(next).RotatedBy(lastpoint ? MathHelper.PiOver2 : -MathHelper.PiOver2) * Width, 0);
                if (WidthFallOff)
                    normal *= progress;
                Vertices[i * 2] = new(new Vector3(current, 0) + normal, Color, i % 2 == 0 ? new(0, 0) : new(1, 0));
                Vertices[i * 2 + 1] = new(new Vector3(current, 0) - normal, Color, i % 2 == 0 ? new(0, 1) : new(1, 1));

                /*Dust d = Dust.NewDustPerfect(new Vector2((int)Vertices[i * 2].Position.X, (int)Vertices[i * 2].Position.Y), DustID.AmberBolt, Vector2.Zero);
                d.noGravity = true;
                Dust d1 = Dust.NewDustPerfect(new Vector2((int)Vertices[i * 2 + 1].Position.X, (int)Vertices[i * 2 + 1].Position.Y), DustID.Adamantite, Vector2.Zero);
                d1.noGravity = true;*/
            }
        }
    }
}