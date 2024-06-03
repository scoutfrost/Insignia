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

namespace Insignia.Core.Common.Systems
{
    public abstract class PrimTrail
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        readonly BasicEffect BasicEffect = new(GD);
        protected Effect Shader;
        protected static GraphicsDevice GD = Main.graphics.GraphicsDevice;
        protected VertexPositionColorTexture[] Vertices;
        protected short[] indices;
        public Color Color;
        public Vector2[] Points;
        public float Width;
        protected virtual void Update() { }
        protected virtual void SetShaders() { }
        protected virtual void OnKill() { }
        protected virtual void CustomDraw(GraphicsDevice graphicsDevice) { }
        protected virtual bool ShouldCustomDraw => false;
        public bool WidthFallOff;
        protected void Initialize()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                Points[i] += new Vector2(0.01f);
            }
            Vertices = new VertexPositionColorTexture[Points.Length * 2];
            indices = new short[3 * (Vertices.Length - 2)];
            vertexBuffer = new(GD, typeof(VertexPositionTexture), Vertices.Length, BufferUsage.WriteOnly);
            indexBuffer = new(GD, typeof(short), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
            vertexBuffer.SetData(Vertices);
        }
        public void Draw()
        {
            if (ShouldCustomDraw)
                CustomDraw(GD);

            if (!ShouldCustomDraw && Points != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (Points[i] == default)
                        return;
                }
                GenerateVertices();
                GenerateIndeces();
            }
            SetShaders();

            RasterizerState rasterizerState = new()
            {
                CullMode = CullMode.None
            };
            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
            BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
            BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, GD.Viewport.Width, GD.Viewport.Height, 0, -1, 1);
            GD.RasterizerState = rasterizerState;

            GD.SetVertexBuffer(vertexBuffer);
            GD.Indices = indexBuffer;

            if (Shader == default)
                Shader = BasicEffect;
            
            foreach (EffectPass pass in Shader.CurrentTechnique.Passes)
            {
                pass.Apply();
                GD.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, Vertices, 0, Vertices.Length, indices, 0, Vertices.Length - 2);
            }
        }
        private void GenerateIndeces()
        {
            for (short i = 0; i < Vertices.Length - 2; i++)
            {
                if (Vertices[i] == default)
                    return;
                indices[i * 3] = i;
                indices[i * 3 + 1] = (short)(i + 1);
                indices[i * 3 + 2] = (short)(i + 2);
            }
        }
        private void GenerateVertices()
        {
            for (int i = 0; i < Points.Length; i++)
            {
                if (Points[i] == default) 
                   return;

                bool lastpoint = i == Points.Length - 1;
                Vector2 current = Points[i];
                Vector2 next = lastpoint ? Points[i - 1] : Points[i + 1];
                float progress = 1 - ((float)i / Points.Length);
                Vector3 normal = new(current.DirectionTo(next).RotatedBy(lastpoint ? MathHelper.PiOver2 : -MathHelper.PiOver2) * Width, 0);
                if (WidthFallOff)
                    normal *= progress;
                Vertices[i * 2] = new(new Vector3(current, 0) + normal, Color, Vector2.One);
                Vertices[i * 2 + 1] = new(new Vector3(current, 0) - normal, Color, Vector2.One);
                
                /*Dust d = Dust.NewDustPerfect(new Vector2(Vertices[i].Position.X, Vertices[i].Position.Y), DustID.Ash, Vector2.Zero, 0, default, 1);
                Dust d1 = Dust.NewDustPerfect(new Vector2(Vertices[i + 1].Position.X, Vertices[i + 1].Position.Y), DustID.Ash, Vector2.Zero, 0, default, 1f);
                d.noGravity = true;
                d1.noGravity = true;*/
            }
        }
    }
}