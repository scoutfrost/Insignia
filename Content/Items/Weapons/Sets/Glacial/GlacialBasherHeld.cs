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
        PrimTrail primTrail2;
        Vector2 rand;
        public override void OnSpawn(IEntitySource source)
        {
            /*primTrail2 = new(Color.White, Projectile.oldPos, 20, true, default, true)
            {
                Texture = (Texture2D)ModContent.Request<Texture2D>("Insignia/Content/Items/Weapons/Sets/Glacial/FrostHookTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad),
                //ShouldCustomDraw = true
            };*/
            primTrail2 = PrimHandler.CreateTrail<PrimTrail>(false, default);
            primTrail2.Texture = (Texture2D)ModContent.Request<Texture2D>("Insignia/Assets/Effects/GlowTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            primTrail2.Color = Color.White;
            primTrail2.Points = Projectile.oldPos;
            primTrail2.Width = 40;
            primTrail2.Pixelated = true;
            primTrail2.WidthFallOff = false;
            primTrail2.Initialize();
            rand = Main.rand.NextVector2Circular(500, 500);
        }
        public override void AI()
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.Center + (Projectile.oldPos[i] - Projectile.Center);
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
