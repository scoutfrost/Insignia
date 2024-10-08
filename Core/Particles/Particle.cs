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
        public Vector2 Velocity;
        public Vector2 Position;
        public float Angle;
        public float AngularVelocity;
        public Color Color;
        public float Size;
        public int TimeLeft;
        public Rectangle? Frame;
        public int? Alpha;
        public float[] Ai = new float[2];
        public BlendState BlendState = BlendState.AlphaBlend;

        public bool isMetaBall = false;
        public Color outlineColor;
        //public float outlineWidth;

        public virtual void CustomDraw(SpriteBatch sb) { }
        public virtual void Update() { }
        public virtual bool ShouldCustomDraw => false;
        public virtual bool Kill => false;
    }
}

