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
        public static GraphicsDevice GD { get; protected set; } = Main.graphics.GraphicsDevice;
        public static BasicEffect BasicEffect { get; protected set; } = new(GD);
        public VertexPositionColorTexture[] Vertices { get; protected set; }
        public Color Color { get; protected set; }
        public List<Vector2> Points { get; protected set; } = new();
        public PrimitiveType Type { get; protected set; }
        public int Width { get; protected set; }
        public int Opacity { get; protected set; }
        public virtual void Update() { }
        public virtual void SetShaders() { }
        public virtual void OnKill() { }
        public virtual void CustomDraw(GraphicsDevice graphicsDevice) { }
        public virtual bool ShouldCustomDraw => false;
        public virtual bool ShouldBasicDraw => true;
        public void Draw()
        {
            Vertices = new VertexPositionColorTexture[Points.Count];
            if (ShouldCustomDraw)
                CustomDraw(GD);

            if (ShouldBasicDraw)
                Vertices = GenerateVertices(Points, Width, Color);
                
            SetShaders();

            RasterizerState rasterizerState = new()
            {
                CullMode = CullMode.None
            };

            BasicEffect.VertexColorEnabled = true;
            BasicEffect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
            BasicEffect.View = Main.GameViewMatrix.TransformationMatrix;
            var viewport = GD.Viewport;
            BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 1);
            GD.RasterizerState = rasterizerState;
            foreach (EffectPass pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GD.DrawUserPrimitives(Type, Vertices, 0, Points.Count / 3);
            }
        }
        //warning: bad code ahead - read at your own risk
        private VertexPositionColorTexture[] GenerateVertices(List<Vector2> points, int width, Color color)//Vector2 pos, Color color)
        {
            List<VertexPositionColorTexture> vertices = new();
            //vertices[0] = new VertexPositionColorTexture(new Vector3(pos, 0), color, Vector2.One);
            for (int i = 1; i < points.Count; i++)
            {
                Vector2 previous = points[i - 1];
                Vector2 current = points[i];
                Vector3 normal = new(Vector2.Normalize(current - previous).RotatedBy(MathHelper.PiOver2, current) * width / i, 0);
                //float progress = i / points.Count;
                //first triangle 
                vertices[i] = new(normal, color, Vector2.One);
                vertices[i + 1] = new(-normal, color, Vector2.One);
                vertices[i + 2] = new(new(Vector2.Normalize(points[i + 1] - current).RotatedBy(MathHelper.PiOver2, current) * width / i, 0), color, Vector2.One);
                //second triangle 
                vertices[i + 3] = new(new(Vector2.Normalize(points[i + 1] - current).RotatedBy(-MathHelper.PiOver2, current) * width / i, 0), color, Vector2.One);
                vertices[i + 4] = vertices[i + 2];
                vertices[i + 5] = vertices[i + 1];
            }
            return vertices.ToArray();
        }
    }
}
