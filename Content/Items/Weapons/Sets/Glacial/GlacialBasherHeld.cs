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
using Terraria.DataStructures;
using Insignia.Core.Particles;
using Insignia.Core.Common.Systems;
using System.Runtime.CompilerServices;
using Terraria.Graphics.Shaders;
using System.Data.OleDb;
using System.Transactions;

namespace Insignia.Content.Items.Weapons.Sets.Glacial
{
    internal class GlacialBasherHeld : ModProjectile
    {
        public override string Texture => "Insignia/Content/Items/Weapons/Sets/Glacial/GlacialBasher";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.light = 0.5f;
            Projectile.timeLeft = 60 * 60;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }
        GenericPrimTrail primTrail2;
        Vector2 rand; 
        List<Vector2> oldpos = new(10);
        int i = 0;
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 10; i++)
            {
                oldpos.Add(Projectile.Center);
            }
            /*primTrail2 = new(Color.White, Projectile.oldPos, 20, true, default, true)
            {
                Texture = (Texture2D)ModContent.Request<Texture2D>("Insignia/Content/Items/Weapons/Sets/Glacial/FrostHookTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                //ShouldCustomDraw = true
            };*/
            primTrail2 = (GenericPrimTrail)PrimHandler.CreateTrail<GenericPrimTrail>(false, default);
            primTrail2.Texture = (Texture2D)ModContent.Request<Texture2D>("Insignia/Assets/Effects/GlowTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            primTrail2.Color = Color.HotPink;
            primTrail2.Points = Projectile.oldPos;
            primTrail2.WidthFallOff = true;
            //primTrail2.colorFallOff = true;
            primTrail2.ColorChangeDelegate = new((float progress, Color color) =>
            {
                return Color.Lerp(color, Color.White, progress) * progress * progress;
            });
            primTrail2.Width = 40;  
            primTrail2.Pixelated = true;
            primTrail2.WidthFallOff = false;
            primTrail2.Initialize();
            rand = Main.rand.NextVector2Circular(500, 500);
        }
        public override void AI()
        {
            if (i++ == 50)
            {
                //Main.NewText("w");
            }
            //oldpos.Insert(0, Projectile.Center);
            //oldpos.RemoveAt(9);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                //Projectile.oldPos[i] = Projectile.Center + (Projectile.oldPos[i] - Projectile.Center);
            }
            Projectile.Center = Main.MouseWorld + rand;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            //primTrail2.Width = Main.rand.Next(10, 20);
            primTrail2.Draw();
            /*var world = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);*/

            //primTrail.Shader.Parameters["outlineColor"].SetValue(Color.WhiteSmoke.ToVector4());


            return true;
        }
        public override void OnKill(int timeLeft)
        {
            primTrail2.kill = true;
        }
    }
}
