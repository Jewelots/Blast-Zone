using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace BlastZone_Windows.Drawing
{
    class Animation : Sprite
    {
        struct Frame
        {
            Rectangle bounds;
            public bool flipped;

            public Rectangle Bounds { get { return bounds; } private set { bounds = value; } }

            public Frame(int x, int y, int w, int h, bool aFlipped)
            {
                bounds = new Rectangle(x, y, w, h);
                flipped = aFlipped;
            }
        }

        List<Frame> frames;
        bool finished = false;
        bool firedAnimationEndEvent = false;

        public float Time { get; set; }
        public bool Loop { get; set; }
        public bool Paused { get; set; }

        public delegate void AnimationEndHandler(object sender, EventArgs e);
        public event AnimationEndHandler OnAnimationEnd;

        public Animation(AnimationDetails details, bool paused = false)
        {
            this.Time = 0;
            this.Paused = paused;

            this.Loop = details.loop;
            this.tex = details.tex;

            frames = new List<Frame>();

            foreach (AnimationFrameDetails frame in details.frames)
            {
                frames.Add(new Frame(frame.x, frame.y, frame.w, frame.h, frame.flip));
            }
        }

        public void Reset()
        {
            Time = 0;
        }

        public void AnimationEnd(EventArgs e)
        {
            if (!firedAnimationEndEvent)
            {
                if (OnAnimationEnd != null)
                    OnAnimationEnd(this, e);

                firedAnimationEndEvent = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 position, Color tint)
        {
            if (!Paused) Time += gameTime.ElapsedGameTime.Milliseconds/1000f;

            //int currentFrame = (int)(Time * 20); // 0.05s per frame
            int currentFrame = (int)(Time * 10); // 0.1s per frame

            if (Loop)
            {
                currentFrame %= frames.Count;
            }
            else
            {
                if (currentFrame >= frames.Count)
                {
                    finished = true;
                    AnimationEnd(EventArgs.Empty);
                }
            }

            if (!finished)
            {
                SpriteEffects flipped = frames[currentFrame].flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(tex, position, frames[currentFrame].Bounds, tint, 0, new Vector2(), 2f, flipped, 1f);
            }
        }
    }
}
