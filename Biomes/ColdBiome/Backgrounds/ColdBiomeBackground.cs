using Insignia.Core.ModPlayers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Threading;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Insignia.Biomes.ColdBiome.Backgrounds
{
    public class ColdBiomeBackGround : ModSystem
    {
        RenderTarget2D rtTileCoords;
        RenderTarget2D screenRT;
        public override void Load()
        {
            On_Main.DrawBackgroundBlackFill += On_Main_DrawBackgroundBlackFill;
            Terraria.Graphics.Light.On_TileLightScanner.GetTileLight += On_TileLightScanner_GetTileLight;
            Main.QueueMainThreadAction(() =>
            {
                rtTileCoords = new(Main.graphics.GraphicsDevice, Main.graphics.PreferredBackBufferWidth / 16, Main.graphics.PreferredBackBufferHeight / 16);
                screenRT = new(Main.graphics.GraphicsDevice, Main.graphics.PreferredBackBufferWidth, Main.graphics.PreferredBackBufferHeight);
            });
        }
        private void On_TileLightScanner_GetTileLight(Terraria.Graphics.Light.On_TileLightScanner.orig_GetTileLight orig, Terraria.Graphics.Light.TileLightScanner self, int x, int y, out Vector3 outputColor)
        {
            orig(self, x, y, out outputColor);

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

            Color[] tileLightColors = new Color[rtTileCoords.Width * rtTileCoords.Height];
            Texture2D tex = ModContent.Request<Texture2D>("Insignia/Biomes/ColdBiome/Backgrounds/UndergroundBG").Value;

            FastParallel.For(0, rtTileCoords.Width * rtTileCoords.Height, (from, to, context) =>
            {
                for (int i = 0; i < to; i++)
                {
                    int x = (int)Main.screenPosition.X;
                    int y = (int)Main.screenPosition.Y;
                    tileLightColors[i] = new Color(Lighting.GetSubLight(new Vector2(x + i % rtTileCoords.Width * 16, y + i / rtTileCoords.Width * 16)));
                }
            });
            rtTileCoords.SetData(tileLightColors);
            
            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            RenderTargetBinding[] previousRTs = Main.graphics.GraphicsDevice.GetRenderTargets();
            Main.graphics.GraphicsDevice.SetRenderTarget(screenRT);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, default, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            Vector2 scale = screenRT.Size() / tex.Size();
            Vector2 drawPos = Parallax();
            Main.spriteBatch.Draw(tex, drawPos, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            Vector2 drawPos2 = screenRT.Bounds.TopRight() + drawPos;
            Main.spriteBatch.Draw(tex, drawPos2, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(previousRTs);

            GameShaders.Misc["BackgroundDrawing"].Shader.Parameters["uImage0"].SetValue(screenRT);
            GameShaders.Misc["BackgroundDrawing"].Shader.Parameters["lightMap"].SetValue(rtTileCoords);
            GameShaders.Misc["BackgroundDrawing"].Shader.Parameters["intensity"].SetValue(1.5f);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, GameShaders.Misc["BackgroundDrawing"].Shader);

            Main.spriteBatch.Draw(screenRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, default, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
        }
        private Vector2 Parallax() // returns the position to draw the texture at 
        {
            return Vector2.Zero + new Vector2(-Main.screenPosition.X / 4 % screenRT.Width, 0);
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