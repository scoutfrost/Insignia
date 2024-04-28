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

namespace Insignia.Core.Common.Systems
{
    public abstract class PrimTrail
    {
        //TODO: rewrite/rework most of the code here as well as make it more optimized using vertex/index buffers and making it easier to use
        public static GraphicsDevice GD { get; protected set; } = Main.graphics.GraphicsDevice;
        public static BasicEffect BasicEffect { get; protected set; } = new(GD);
        public VertexPositionColorTexture[] Vertices { get; protected set; }
        public Color Color { get; protected set; }
        public Vector2[] Points { get; protected set; }
        public float Width { get; protected set; }
        public virtual void Update() { }
        public virtual void SetShaders() { }
        public virtual void OnKill() { }
        public virtual void CustomDraw(GraphicsDevice graphicsDevice) { }
        public virtual bool ShouldCustomDraw => false;
        public virtual bool ShouldBasicDraw => true;
        public bool WidthFallOff { get; protected set; }
        public void Draw()
        {
            Vertices = new VertexPositionColorTexture[Points.Length * 2];
            if (ShouldCustomDraw)
                CustomDraw(GD);

            if (ShouldBasicDraw && Points != null)
                GenerateVertices(Points, Width, Color);
            SetShaders();

            RasterizerState rasterizerState = new()
            {
                CullMode = CullMode.None
            };
            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
            BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
            Viewport viewport = GD.Viewport;
            BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 1);
            GD.RasterizerState = rasterizerState;
            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, Vertices, 0, Vertices.Length / 3);
            }
        }

        private void GenerateVertices(Vector2[] points, float width, Color color)
        {
            int max = points.Length;
            if (points.Length % 2 != 0)
                max = points.Length - 1;

            for (int i = 0; i < max; i += 2)
            {
                Vector2 current = points[i];
                Vector2 next = points[i + 1];
                float progress = 1 - ((float)i / points.Length);
                Vector3 normal = new((current.DirectionTo(next) * width).RotatedBy(MathHelper.PiOver2), 0);
                if (WidthFallOff)
                {
                    normal *= progress;
                }
                Vertices[i] = new(new Vector3(current, 0) + normal, color, Vector2.One);
                Vertices[i + 1] = new(new Vector3(current, 0) - normal, color, Vector2.One);

                /*Dust d = Dust.NewDustPerfect(new Vector2(Vertices[i].Position.X, Vertices[i].Position.Y), DustID.Ash, Vector2.Zero);
                Dust d1 = Dust.NewDustPerfect(new Vector2(Vertices[i + 1].Position.X, Vertices[i + 1].Position.Y), DustID.Ash, Vector2.Zero);
                d.noGravity = true;
                d1.noGravity = true;*/
            }
        }
    }
}