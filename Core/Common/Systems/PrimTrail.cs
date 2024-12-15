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
using System.Transactions;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json.Linq;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;

namespace Insignia.Core.Common.Systems
{
    internal class PrimHandler : ModSystem
    {
        static readonly int maxTrails = 500;
        static internal List<PrimTrail> deadPool = []; 
        static internal List<PrimTrail> activePool = [];

        static internal DynamicVertexBuffer dynamicVertexBuffer;
        static internal DynamicIndexBuffer dynamicIndexBuffer;

        static readonly GraphicsDevice GD = Main.graphics.GraphicsDevice;

        static internal List<VertexPositionColorTexture> vertices; 
        static internal List<short> indices;

        static readonly int maxVertices = 10000; 
        static internal Dictionary<Tuple<Type, Texture2D, bool>, int> lastActiveVertexIndices; //same as below but with vertices
        static internal Dictionary<Tuple<Type, Texture2D, bool>, int> lastActiveIndices; //keys store the type of trail, texture, and if its pixellated or not. values store the last index of list of trails of a certain type. 
        static internal int lastActiveIndex; // for indices
        static internal int lastActiveVertexIndex;
        static internal RenderTarget2D pixellationTarget;
        static internal List<PrimTrail> trailTypes;
        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                for (int i = 0; i < maxTrails; i++)
                {
                    deadPool.Add(new PrimTrail());
                }
                lastActiveVertexIndex = 0;

                vertices = new(maxVertices);
                indices = new(maxVertices + maxTrails * 2);// two extra indeces for each trail; for degenerate triangles
                trailTypes = [];

                lastActiveVertexIndices = [];
                lastActiveIndices = [];

                dynamicVertexBuffer = new(GD, typeof(VertexPositionColorTexture), maxVertices, BufferUsage.WriteOnly);
                dynamicIndexBuffer = new(GD, typeof(short), maxVertices + maxTrails * 2, BufferUsage.WriteOnly);

                pixellationTarget = new(GD, GD.PresentationParameters.BackBufferWidth / 1, GD.PresentationParameters.BackBufferHeight / 1);
            });
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (activePool.Count == 0 || Main.gameInactive)
                return;

            for (int i = 0; i < activePool.Count; i++)
            {
                if (activePool.Count == 0)
                    return;

                PrimTrail trail = activePool[i];
                if (trail.kill || (!trail.isActive && activePool.Contains(trail)))
                {
                    KillTrail(trail);
                    continue;
                }

                trail.isActive = false; //should stay out of the above if statement 
            }
            RasterizerState rasterizerState = new()
            {
                CullMode = CullMode.None
            };
            GD.RasterizerState = rasterizerState;

            dynamicVertexBuffer.SetData(vertices.ToArray(), 0, vertices.Count, SetDataOptions.Discard);

            GD.SetVertexBuffer(dynamicVertexBuffer);
            GD.Indices = dynamicIndexBuffer;

            if (trailTypes.Count == 0 || activePool.Count == 0)
                return;

            GD.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;

            RenderTargetBinding[] previous = default;
            bool shouldDrawRT = false;

            for (int j = 0; j < 2; j++)//first iteration draws all of the pixelated ones, second the rest 
            {
                for (int i = 0; i < trailTypes.Count; i++)
                {
                    if (j == 0 && trailTypes[i].Pixelated && previous == default)
                    {
                        previous = GD.GetRenderTargets();
                        GD.SetRenderTarget(pixellationTarget);
                        GD.Clear(Color.Transparent);
                        shouldDrawRT = true;
                    }
                    if (j == 1)
                    {
                        GD.SetRenderTargets(previous);
                    }
                    if ((!trailTypes[i].Pixelated && j == 0) || (trailTypes[i].Pixelated && j == 1))
                        continue;

                    Tuple<Type, Texture2D, bool> previousTrailDrawData = null;
                    if (i != 0)
                    {
                        previousTrailDrawData = new(trailTypes[i - 1].Shader.GetType(), trailTypes[i - 1].Texture, trailTypes[i - 1].Pixelated);
                    }
                    Tuple<Type, Texture2D, bool> trailDrawData = new(trailTypes[i].Shader.GetType(), trailTypes[i].Texture, trailTypes[i].Pixelated);
                    int currentVI = lastActiveVertexIndices[trailDrawData];
                    int previousvI = previousTrailDrawData != null ? lastActiveVertexIndices[previousTrailDrawData] : 0;

                    int currentIndex = lastActiveIndices[trailDrawData];
                    int previousIndex = previousTrailDrawData != null ? lastActiveIndices[previousTrailDrawData] : 0;

                    int indicesToUse = currentIndex - previousIndex;
                    int verticesToDraw = currentVI - previousvI;

                    trailTypes[i].SetShaders();
                    foreach (EffectPass pass in trailTypes[i].Shader.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        GD.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, previousvI, verticesToDraw, previousIndex, indicesToUse - 2);
                    }
                }
            }
            if (shouldDrawRT)
            {
                //couldnt get the projections working when zoomed in and the rt half sized, idk why, so im just using a shader lol 
                GameShaders.Misc["Pixellation"].Shader.Parameters["uImage0"].SetValue(pixellationTarget);
                GameShaders.Misc["Pixellation"].Shader.Parameters["pixellationFactor"].SetValue(2);
                GameShaders.Misc["Pixellation"].Shader.Parameters["resolution"].SetValue(pixellationTarget.Size() / Main.GameViewMatrix.Zoom.X);

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, RasterizerState.CullNone, GameShaders.Misc["Pixellation"].Shader, Matrix.Identity);
                Main.spriteBatch.Draw(pixellationTarget, new Rectangle(0, 0, pixellationTarget.Width * 1, pixellationTarget.Height * 1), Color.White);
                Main.spriteBatch.End();

                GD.SetRenderTarget(pixellationTarget);
                GD.Clear(Color.Transparent);
                GD.SetRenderTargets(previous);
            }
        }
        static void KillTrail(PrimTrail trail)
        {
            activePool.Remove(trail);

            int trailIndexLength = trail.Vertices.Length + 2;

            lastActiveVertexIndex -= trail.Vertices.Length;
            lastActiveIndex -= trailIndexLength;
            vertices.RemoveRange(trail.startVertexIndex, trail.Vertices.Length);
            indices.RemoveRange(trail.startIndex, trailIndexLength);
            for (int j = trail.startIndex; j < indices.Count; j++)
            {
                indices[j] -= (short)trail.Vertices.Length; 
            }
            dynamicIndexBuffer.SetData(indices.ToArray(), 0, indices.Count, SetDataOptions.Discard);

            Tuple<Type, Texture2D, bool> trailDrawData = new(trail.Shader.GetType(), trail.Texture, trail.Pixelated);

            var arrayToScan = lastActiveIndices.ToArray();
            for (int j = 0; j < lastActiveIndices.Count; j++)
            {
                if (arrayToScan[j].Value > lastActiveIndices[trailDrawData])
                {
                    lastActiveIndices[arrayToScan[j].Key] -= trailIndexLength;
                }
            }
            arrayToScan = lastActiveVertexIndices.ToArray();
            for (int j = 0; j < lastActiveVertexIndices.Count; j++)
            {
                if (arrayToScan[j].Value > lastActiveVertexIndices[trailDrawData])
                {
                    lastActiveVertexIndices[arrayToScan[j].Key] -= trail.Vertices.Length;
                }
            }
            lastActiveVertexIndices[trailDrawData] -= trail.Vertices.Length;
            lastActiveIndices[trailDrawData] -= trailIndexLength;

            for (int j = 0; j < activePool.Count; j++) //adjusting last vertex data for other trails
            {
                PrimTrail otherTrail = activePool[j];
                if (otherTrail.startVertexIndex > trail.startVertexIndex)//if the startvertex is above, the startindex must be too so i can edit both
                {
                    otherTrail.startVertexIndex -= trail.Vertices.Length;
                    otherTrail.startIndex -= trailIndexLength;
                }
            }
            trail.Vertices = null;
            trail.Pixelated = false;
            trail.Width = 0;
            trail.Color = Color.White;
            trail.Points = null;
            trail.Shader = null;
            trail.Texture = null;
            trail.texCoordBottomOffset = Vector2.Zero;
            trail.texCoordTopOffset = Vector2.Zero;
            trail.startVertexIndex = 0;
            trail.startIndex = 0;
            trail.kill = true;
            deadPool.Add(trail);

            for (int j = 0; j < trailTypes.Count; j++)
            {
                Tuple<Type, Texture2D, bool> drawData = new(trailTypes[j].Shader.GetType(), trailTypes[j].Texture, trailTypes[j].Pixelated);
                Tuple<Type, Texture2D, bool> previousDrawData = null;
                if (j != 0)
                {
                    previousDrawData = new(trailTypes[j - 1].Shader.GetType(), trailTypes[j - 1].Texture, trailTypes[j - 1].Pixelated);
                }
                int previousVI = j != 0 ? lastActiveVertexIndices[previousDrawData] : 0;
                int vI = lastActiveVertexIndices[drawData];

                if (previousVI == vI)
                {
                    lastActiveVertexIndices.Remove(drawData);
                    lastActiveIndices.Remove(drawData);
                    trailTypes.RemoveAt(j);
                }
            }
        }
        /// <summary>
        /// Adds a new trail to the active pool.
        /// </summary>
        /// <param name="usePrimData">If false, call the PrimTrail.Initialize() method yourself.</param>
        /// <param name="p">Set this to default if you aren't using it.</param>
        /// <typeparam name="T">The type of the trail.</typeparam>
        /// <returns>The trail that got added.</returns>
        public static PrimTrail CreateTrail<T>(bool usePrimData, PrimData p) where T : PrimTrail
        {
            PrimTrail returnTrail;
            if (deadPool.Count == 0)
            {
                KillTrail(activePool[0]);
            }
            returnTrail = deadPool[0] as T;
            deadPool.RemoveAt(0);
            
            if (usePrimData)
            {
                returnTrail.Pixelated = p.Pixelated;
                returnTrail.Color = p.Color;
                returnTrail.Points = p.Points;
                returnTrail.Width = p.Width;
                returnTrail.Shader = p.Shader;
                returnTrail.Texture = p.Texture;

                returnTrail.Initialize();
            }
            returnTrail.isActive = true;
            return returnTrail;
        }
        internal static short[] GenerateIndices(PrimTrail trail)
        {
            short[] indices = new short[trail.Vertices.Length + 2];
            short startValue = (short)(PrimHandler.indices.Count == 0 ? 0 : (PrimHandler.indices[(short)MathHelper.Clamp(trail.startIndex - 1, 0, short.MaxValue)] + 1));
            for (short i = 0; i < indices.Length; i++)
            {
                if (trail.startIndex == 0)
                {
                    startValue = 0;
                }
                if (i == 0)
                {
                    indices[i] = startValue;
                    continue;
                }
                if (i == indices.Length - 1)
                {
                    indices[i] = (short)(startValue + trail.Vertices.Length - 1);
                    return indices;
                }

                indices[i] = (short)(startValue + i - 1);
            }
            return indices;
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
        public Color Color;
        public Vector2[] Points;
        public float Width;
        public Effect Shader;
        public Texture2D Texture;
        public bool kill = false;
        public Vector2 texCoordTopOffset;
        public Vector2 texCoordBottomOffset;
        internal int startVertexIndex;
        internal int startIndex;
        /// <summary>
        /// For when you want to set custom shader params without making a new trail type.
        /// </summary>
        public Action SetShadersDelegate;
        protected virtual void OnSpawn() { }
        protected virtual void Update() { }
        internal void SetShaders()
        {
            if (SetShadersDelegate == default)
                CheckBasicEffect();
            else
                SetShadersDelegate();
        }
        protected virtual void CustomDraw(GraphicsDevice graphicsDevice) { }
        public bool ShouldCustomDraw = false;
        public bool WidthFallOff;
        internal PrimTrail() { }
        public void Initialize()
        {
            isActive = true;
            Vertices = new VertexPositionColorTexture[Points.Length * 2];
            CheckBasicEffect();
            PrimHandler.activePool.Add(this);
            Tuple<Type, Texture2D, bool> key = new(Shader.GetType(), Texture, Pixelated);
            if (PrimHandler.lastActiveVertexIndices.TryGetValue(key, out int value))
            {
                startVertexIndex = PrimHandler.lastActiveVertexIndices[key];
                startIndex = PrimHandler.lastActiveIndices[key];
            }
            else
            {
                startIndex = PrimHandler.lastActiveIndex;
                startVertexIndex = PrimHandler.lastActiveVertexIndex;

                PrimHandler.lastActiveIndices.Add(key, startIndex);
                PrimHandler.lastActiveVertexIndices.Add(key, startVertexIndex);

                PrimTrail copy = new() 
                { 
                    Shader = Shader, 
                    Texture = Texture,
                    Pixelated = Pixelated,
                    SetShadersDelegate = SetShadersDelegate
                };
                PrimHandler.trailTypes.Add(copy);
            }
            PrimHandler.lastActiveVertexIndex += Vertices.Length;
            PrimHandler.lastActiveIndex += Vertices.Length + 2;
            var arrayToScan = PrimHandler.lastActiveIndices.ToArray();
            for (int j = 0; j < PrimHandler.lastActiveIndices.Count; j++)
            {
                if (arrayToScan[j].Value > PrimHandler.lastActiveIndices[key])
                {
                    PrimHandler.lastActiveIndices[arrayToScan[j].Key] += Vertices.Length + 2;
                }
            }
            arrayToScan = PrimHandler.lastActiveVertexIndices.ToArray();
            for (int j = 0; j < PrimHandler.lastActiveVertexIndices.Count; j++)
            {
                if (arrayToScan[j].Value > PrimHandler.lastActiveVertexIndices[key])
                {
                    PrimHandler.lastActiveVertexIndices[arrayToScan[j].Key] += Vertices.Length;
                }
            }
            PrimHandler.lastActiveIndices[key] += Vertices.Length + 2;
            PrimHandler.lastActiveVertexIndices[key] += Vertices.Length;

            PrimHandler.vertices.InsertRange(startVertexIndex, Vertices);

            short[] indicesToAdd = PrimHandler.GenerateIndices(this);
            PrimHandler.indices.InsertRange(startIndex, indicesToAdd);
            for (int i = startIndex + indicesToAdd.Length; i < PrimHandler.indices.Count; i++)
            {
                PrimHandler.indices[i] += (short)Vertices.Length;
            } 
            for (int j = 0; j < PrimHandler.activePool.Count; j++) //adjusting last vertex data for other trails
            {
                PrimTrail otherTrail = PrimHandler.activePool[j];
                if ((otherTrail.startVertexIndex >= startVertexIndex) && otherTrail != this)//if the startvertex is above, the startindex must be too so i can edit both
                {
                    otherTrail.startVertexIndex += Vertices.Length;
                    otherTrail.startIndex += Vertices.Length + 2;
                }
            }
            PrimHandler.dynamicIndexBuffer.SetData(PrimHandler.indices.ToArray(), 0, PrimHandler.indices.Count, SetDataOptions.Discard);
        }
        public void Draw()
        {
            if (Points == null) return;
            if (Points.Length < 2)
            {
                isActive = false;
                return;
            }
            if (ShouldCustomDraw)
                CustomDraw(GD);

            isActive = true;

            Update();

            if (!ShouldCustomDraw && Vertices != null)
            {
                GenerateVertices();
            }
            for (int i = 0; i < Vertices.Length; i++)
            {
                PrimHandler.vertices[startVertexIndex + i] = Vertices[i];
            }
        }
        private void CheckBasicEffect()
        {
            if (Shader == default || Shader.GetType() == basicEffect.GetType())
            {
                if (Texture != null)
                    basicEffect.TextureEnabled = true;
                basicEffect.VertexColorEnabled = true;

                basicEffect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
                basicEffect.View = Main.GameViewMatrix.TransformationMatrix;/*new
                    Matrix(Main.GameViewMatrix.Zoom.X, 0, 0, 0,
                    0, Main.GameViewMatrix.Zoom.Y, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                    );*/
                //Main.NewText(Main.GameViewMatrix.Zoom);
                basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, GD.PresentationParameters.BackBufferWidth, GD.PresentationParameters.BackBufferHeight, 0, -1, 1);
                
                basicEffect.Texture = Texture;
                Shader = basicEffect;
                /*Main.NewText(GD.Viewport.Bounds + " VIEWPORT");
                Main.NewText(GD.PresentationParameters.Bounds + " PRESENTPARAMS");
                Main.NewText(Main.ScreenSize + " SCREENSIZE");*/
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
               
                //for debugging
                /*GenericGlowParticle p = new(new Vector2((int)Vertices[i * 2].Position.X, (int)Vertices[i * 2].Position.Y), Vector2.Zero, Color.BlanchedAlmond, 0.04f, 2);
                GenericGlowParticle p1 = new(new Vector2((int)Vertices[i * 2 + 1].Position.X, (int)Vertices[i * 2 + 1].Position.Y), Vector2.Zero, Color.BlanchedAlmond, 0.04f, 2);
                ParticleSystem.GenerateParticle(p, p1);*/
            }
        }
    }
}