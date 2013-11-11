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
    /// Horizontal Alignment of text.
    /// AlignLeft is standard.
    /// AlignCenter aligns text to center horizontally on the position.
    /// AlignRight aligns text to have the right edge at position.
    /// </summary>
    public enum HorizontalAlign
    {
        AlignLeft, AlignCenter, AlignRight
    }

    /// <summary>
    /// Vertical Alignment of text.
    /// AlignTop is standard.
    /// AlignCenter aligns text to center vertically on the position.
    /// AlignBottom aligns text to have the bottom edge at position.
    /// </summary>
    public enum VerticalAlign
    {
        AlignTop, AlignCenter, AlignBottom
    }

    class DrawTextExtension
    {
        /// <summary>
        /// Renders a string from a specified SpriteFont with an outline of specified colors into a SpriteBatch.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to render to</param>
        /// <param name="font">SpriteFont to render with</param>
        /// <param name="text">String to render</param>
        /// <param name="backColor">Outline Color</param>
        /// <param name="frontColor">Main Text Color</param>
        /// <param name="position">Position to render to</param>
        /// <param name="thickness">Thickness of the outline</param>
        /// <param name="hAlign">Horizontal Align</param>
        /// <param name="vAlign">Vertical Align</param>
        public static void DrawTextOutline(SpriteBatch spriteBatch, SpriteFont font, string text, Color backColor, Color frontColor, Vector2 position, float thickness, HorizontalAlign hAlign = HorizontalAlign.AlignLeft, VerticalAlign vAlign = VerticalAlign.AlignTop)
        {
            //Get the size of the text for offsetting
            Vector2 fullSize = font.MeasureString(text);

            //Offset Horizontal and Vertical aligns based on the specified enum value
            Vector2 alignOffset = new Vector2();
            
            switch (hAlign)
            {
                case HorizontalAlign.AlignCenter:
                    alignOffset.X = fullSize.X / 2;
                    break;
                case HorizontalAlign.AlignRight:
                    alignOffset.X = fullSize.X;
                    break;
                default:
                    break;
            }

            switch (vAlign)
            {
                case VerticalAlign.AlignCenter:
                    alignOffset.Y = fullSize.Y / 2;
                    break;
                case VerticalAlign.AlignBottom:
                    alignOffset.Y = fullSize.Y;
                    break;
                default:
                    break;
            }

            //Draw text in all 8 directions for hacky outline. Handled fine by sprite batching however so not a problem.
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(1 * thickness, 1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(-1 * thickness, -1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(-1 * thickness, 1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(1 * thickness, -1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(1 * thickness, 0), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(-1 * thickness, 0), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(0, 1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(0, -1 * thickness), backColor);

            //Draw the main text in the center
            spriteBatch.DrawString(font, text, position - alignOffset, frontColor);
        }
    }
}
