using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Insignia.Core.Particles;
using ReLogic.Content;

namespace Insignia.Core.Particles
{
    public abstract class Particle
    {
        public Texture2D Texture;
        public string TextureName;
        public Vector2 Position;
        public Vector2 Velocity;
        public float Angle;
        public float AngularVelocity;
        public Color Color;
        public float Size;
        public int TimeLeft;
        public Rectangle? Frame;
        public int? Alpha;
        public bool ShouldCustomDraw { get; private set; }
        public virtual void CustomDraw(SpriteBatch sb) { ShouldCustomDraw = true; }
        public virtual void Update() { }
        public virtual bool Kill => false;
    }
}
