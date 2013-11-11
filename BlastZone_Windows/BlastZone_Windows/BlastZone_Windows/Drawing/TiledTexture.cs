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

namespace BlastZone_Windows.Drawing
{
    class TiledTexture
    {
        Texture2D tex;
        
        Rectangle bounds;
        Vector2 offset = new Vector2();

        Vector2 tileCount = new Vector2();

        public Vector2 Offset { get { return offset; } set { offset = value; } }

        public TiledTexture()
        {
            this.tex = null;
            this.bounds = new Rectangle();
            CalculateTileCount();
        } 

        public TiledTexture(Rectangle bounds)
        {
            this.tex = null;
            this.bounds = bounds;
            CalculateTileCount();
        }

        public TiledTexture(Texture2D tex)
        {
            this.tex = tex;
            this.bounds = new Rectangle();
            CalculateTileCount();
        }

        public TiledTexture(Texture2D tex, Rectangle bounds)
        {
            this.tex = tex;
            this.bounds = bounds;
            CalculateTileCount();
        }

        public Rectangle GetBounds()
        {
            return bounds;
        }

        public void SetBounds(Rectangle bounds)
        {
            this.bounds = bounds;
            CalculateTileCount();
        }

        public void SetTexture(Texture2D tex)
        {
            this.tex = tex;
            CalculateTileCount();
        }

        void CalculateTileCount()
        {
            if (tex == null) return;
            if (bounds.Width == 0 || bounds.Height == 0) return;

            tileCount = new Vector2((float)Math.Ceiling((double)bounds.Width / (double)tex.Width), (float)Math.Ceiling((double)bounds.Height / (double)tex.Height));
        }

        public void SetPosition(Vector2 position)
        {
            this.bounds.Location = new Point((int)position.X, (int)position.Y);
        }

        public void ShiftPosition(Vector2 displacement)
        {
            this.bounds.Offset(new Point((int)displacement.X, (int)displacement.Y));
        }

        public void ShiftOffset(Vector2 displacement)
        {
            this.offset += displacement;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (tex == null) return;
            if (bounds.Width == 0 || bounds.Height == 0) return;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);
            //spriteBatch.Draw(tex, new Rectangle(bounds.X - 5, bounds.Y - 5, bounds.Width + 10, bounds.Height + 10), Color.Black);

            for (int y = 0; y < tileCount.Y; ++y)
            {
                for (int x = 0; x < tileCount.X; ++x)
                {
                    Rectangle source = new Rectangle((int)Offset.X, (int)Offset.Y, tex.Width, tex.Height);
                    Rectangle newBounds = new Rectangle(bounds.X + tex.Width * x, bounds.Y + tex.Height * y, tex.Width, tex.Height);

                    if (x == tileCount.X - 1)
                    {
                        int clippedAmount = newBounds.Right - Math.Min(newBounds.Right, bounds.Right);
                        float clipPercent = (float)(newBounds.Width - clippedAmount) / (float)newBounds.Width;
                        newBounds.Width -= clippedAmount;

                        source.Width = (int)(source.Width * clipPercent);
                    }

                    if (y == tileCount.Y - 1)
                    {
                        int clippedAmount = newBounds.Bottom - Math.Min(newBounds.Bottom, bounds.Bottom);
                        float clipPercent = (float)(newBounds.Height - clippedAmount) / (float)newBounds.Height;
                        newBounds.Height -= clippedAmount;

                        source.Height = (int)(source.Height * clipPercent);
                    }

                    spriteBatch.Draw(tex, newBounds, source, Color.White);
                }
            }

            spriteBatch.End();
        }
    }
}
