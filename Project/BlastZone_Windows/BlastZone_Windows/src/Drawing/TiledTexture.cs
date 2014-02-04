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
    /// <summary>
    /// Texture that tiles over a destination rectangle
    /// </summary>
    class TiledTexture
    {
        Texture2D tex;
        
        Rectangle bounds;
        Vector2 offset = new Vector2();

        Vector2 tileCount = new Vector2();

        public Vector2 Offset { get { return offset; } set { offset = value; } }

        /// <summary>
        /// Create a tiled texture with boundaries and a texture
        /// </summary>
        /// <param name="tex">The texture to tile</param>
        /// <param name="bounds">Boundary to draw</param>
        public TiledTexture(Texture2D tex, Rectangle bounds)
        {
            this.tex = tex;
            this.bounds = bounds;
            CalculateTileCount();
        }

        /// <summary>
        /// Create a tiled texture
        /// </summary>
        public TiledTexture() : this(null, new Rectangle())
        {
        } 

        /// <summary>
        /// Create a tiled texture with boundary
        /// </summary>
        /// <param name="bounds">Boundary to draw</param>
        public TiledTexture(Rectangle bounds) : this(null, bounds)
        {
        }

        /// <summary>
        /// Create a tiled texture
        /// </summary>
        /// <param name="tex">The texture to tile</param>
        public TiledTexture(Texture2D tex) : this(tex, new Rectangle())
        {
        }

        /// <summary>
        /// Get the boundary of the tiled texture
        /// </summary>
        /// <returns></returns>
        public Rectangle GetBounds()
        {
            return bounds;
        }

        /// <summary>
        /// Set the boundary of the tiled texture
        /// </summary>
        /// <param name="bounds">The boundary rectangle to draw it inside</param>
        public void SetBounds(Rectangle bounds)
        {
            this.bounds = bounds;
            CalculateTileCount();
        }

        /// <summary>
        /// Set the texture to tile
        /// </summary>
        /// <param name="tex">The texture to tile</param>
        public void SetTexture(Texture2D tex)
        {
            this.tex = tex;
            CalculateTileCount();
        }

        /// <summary>
        /// Calculate the tile count for the current bounds/texture
        /// </summary>
        void CalculateTileCount()
        {
            if (tex == null) return;
            if (bounds.Width == 0 || bounds.Height == 0) return;

            tileCount = new Vector2((float)Math.Ceiling((double)bounds.Width / (double)tex.Width), (float)Math.Ceiling((double)bounds.Height / (double)tex.Height));
        }

        /// <summary>
        /// Set the position of the boundary
        /// </summary>
        /// <param name="position">Position of the boundary</param>
        public void SetPosition(Vector2 position)
        {
            this.bounds.Location = new Point((int)position.X, (int)position.Y);
        }

        /// <summary>
        /// Shift the boundary by a displacement
        /// </summary>
        /// <param name="displacement">Displacement to shift by</param>
        public void ShiftPosition(Vector2 displacement)
        {
            this.bounds.Offset(new Point((int)displacement.X, (int)displacement.Y));
        }

        /// <summary>
        /// Shift the texture offset
        /// </summary>
        /// <param name="displacement">Texture offset</param>
        public void ShiftOffset(Vector2 displacement)
        {
            this.offset += displacement;
        }

        /// <summary>
        /// Draw the texture
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Don't draw if no texture
            if (tex == null) return;
            //Don't draw if no boundary
            if (bounds.Width == 0 || bounds.Height == 0) return;

            //Begin spritebatch in wrapping mode to tile the texture
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);

            //Loop through tiles
            for (int y = 0; y < tileCount.Y; ++y)
            {
                for (int x = 0; x < tileCount.X; ++x)
                {
                    //Calculate source and destination rectangles
                    Rectangle source = new Rectangle((int)Offset.X, (int)Offset.Y, tex.Width, tex.Height);
                    Rectangle newBounds = new Rectangle(bounds.X + tex.Width * x, bounds.Y + tex.Height * y, tex.Width, tex.Height);
                    
                    //Clip if on horizontal edge
                    if (x == tileCount.X - 1)
                    {
                        int clippedAmount = newBounds.Right - Math.Min(newBounds.Right, bounds.Right);
                        float clipPercent = (float)(newBounds.Width - clippedAmount) / (float)newBounds.Width;
                        newBounds.Width -= clippedAmount;

                        source.Width = (int)(source.Width * clipPercent);
                    }

                    //Clip if on vertical edge
                    if (y == tileCount.Y - 1)
                    {
                        int clippedAmount = newBounds.Bottom - Math.Min(newBounds.Bottom, bounds.Bottom);
                        float clipPercent = (float)(newBounds.Height - clippedAmount) / (float)newBounds.Height;
                        newBounds.Height -= clippedAmount;

                        source.Height = (int)(source.Height * clipPercent);
                    }

                    //Draw the tile
                    spriteBatch.Draw(tex, newBounds, source, Color.White);
                }
            }

            spriteBatch.End();
        }
    }
}
