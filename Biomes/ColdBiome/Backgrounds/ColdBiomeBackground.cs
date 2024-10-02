using Insignia.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Threading;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.Modules;

namespace Insignia.Biomes.ColdBiome.Backgrounds
{
    public class ColdBiomeBackGround : ModSystem
    {
        RenderTarget2D lightMap;
        RenderTarget2D screenRT;
        //RenderTarget2D zoomRT;
        public override void Load()
        {
            On_Main.DrawBackgroundBlackFill += On_Main_DrawBackgroundBlackFill;
            Terraria.Graphics.Light.On_TileLightScanner.GetTileLight += On_TileLightScanner_GetTileLight;
            Main.OnResolutionChanged += Main_OnResolutionChanged;
            
            Main.QueueMainThreadAction(() =>
            {
                lightMap = new(Main.graphics.GraphicsDevice, Main.screenWidth / 16, Main.screenHeight / 16);
                screenRT = new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                //zoomRT = new(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
        }
        private void Main_OnResolutionChanged(Vector2 obj)
        {
            lightMap.Dispose();
            lightMap = new(Main.graphics.GraphicsDevice, (int)(obj.X / 16), (int)(obj.Y / 16));
        }
        private void On_TileLightScanner_GetTileLight(Terraria.Graphics.Light.On_TileLightScanner.orig_GetTileLight orig, Terraria.Graphics.Light.TileLightScanner self, int x, int y, out Vector3 outputColor)
        {
            orig(self, x, y, out outputColor);
            if (Main.LocalPlayer.GetModPlayer<InsigniaPlayer>().inColdBiome) 
                outputColor += new Vector3(0.01f);
        }
        private void On_Main_DrawBackgroundBlackFill(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            if (Main.gameMenu || Main.dedServ || Main.LocalPlayer == null || !Main.LocalPlayer.GetModPlayer<InsigniaPlayer>().inColdBiome)
            {
                orig(self);
                return;
            }
            orig(self);

            Color[] tileLightColors = new Color[lightMap.Width * lightMap.Height];
            Texture2D tex = ModContent.Request<Texture2D>("Insignia/Biomes/ColdBiome/Backgrounds/UndergroundBG").Value;

            FastParallel.For(0, lightMap.Width * lightMap.Height, (from, to, context) =>
            {
                for (int i = 0; i < to; i++)
                {
                    int x = (int)Main.screenPosition.X;
                    int y = (int)Main.screenPosition.Y;
                    tileLightColors[i] = new Color(Lighting.GetSubLight(new Vector2(x + i % lightMap.Width * 16, y + i / lightMap.Width * 16)));
                }
            });
            lightMap.SetData(tileLightColors);
            
            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            RenderTargetBinding[] previousRTs = Main.graphics.GraphicsDevice.GetRenderTargets();
            Main.graphics.GraphicsDevice.SetRenderTarget(screenRT);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, default, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            Vector2 scale = screenRT.Size() / tex.Size();
            Vector2 drawPos = Parallax(0.25f);
            Main.spriteBatch.Draw(tex, drawPos, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            Vector2 drawPos2 = screenRT.Bounds.TopRight() + drawPos;
            Main.spriteBatch.Draw(tex, drawPos2, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();

            /*Main.graphics.GraphicsDevice.SetRenderTarget(zoomRT);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.Transform);
            Main.spriteBatch.Draw(lightMap, Vector2.Zero, null, Color.White);
            Main.spriteBatch.End();*/

            Main.graphics.GraphicsDevice.SetRenderTargets(previousRTs);

            GameShaders.Misc["BackgroundDrawing"].Shader.Parameters["uImage0"].SetValue(screenRT);
            GameShaders.Misc["BackgroundDrawing"].Shader.Parameters["lightMap"].SetValue(lightMap);
            //GameShaders.Misc["BackgroundDrawing"].Shader.Parameters["intensity"].SetValue(1f);
            //GameShaders.Misc["BackgroundDrawing"].Shader.Parameters["zoomMult"].SetValue(Main.GameZoomTarget);
            //GameShaders.Misc["BackgroundDrawing"].Shader.Parameters["screenSize"].SetValue(Main.ScreenSize.ToVector2());

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, GameShaders.Misc["BackgroundDrawing"].Shader, Main.Transform);

            Main.spriteBatch.Draw(screenRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, default, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
        }
        private Vector2 Parallax(float speedMult) // returns the position to draw the texture at 
        {
            return Vector2.Zero + new Vector2(-Main.screenPosition.X * speedMult % screenRT.Width, 0);
        }
        public override void Unload()
        {
            On_Main.DrawBackgroundBlackFill -= On_Main_DrawBackgroundBlackFill;
            Terraria.Graphics.Light.On_TileLightScanner.GetTileLight -= On_TileLightScanner_GetTileLight;
            Main.OnResolutionChanged -= Main_OnResolutionChanged;
            Main.QueueMainThreadAction(() =>
            {
                lightMap.Dispose();
                screenRT.Dispose();
            });
        }
    }
    public class ColdBiomeBackground : ModUndergroundBackgroundStyle
    {
        public override void FillTextureArray(int[] textureSlots)
        {
            for (int i = 0; i < textureSlots.Length; i++)
            {
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot("Insignia/Biomes/ColdBiome/Backgrounds/Empty");
            }
        }
    }
} 