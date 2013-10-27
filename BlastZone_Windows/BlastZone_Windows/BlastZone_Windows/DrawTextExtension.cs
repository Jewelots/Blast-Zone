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

namespace BlastZone_Windows
{
    public enum HorizontalAlign
    {
        AlignLeft, AlignCenter, AlignRight
    }

    public enum VerticalAlign
    {
        AlignTop, AlignCenter, AlignBottom
    }

    class DrawTextExtension
    {
        public static void DrawTextOutline(SpriteBatch spriteBatch, SpriteFont font, string text, Color backColor, Color frontColor, Vector2 position, float thickness, HorizontalAlign hAlign = HorizontalAlign.AlignLeft, VerticalAlign vAlign = VerticalAlign.AlignTop)
        {
            Vector2 fullSize = font.MeasureString(text);

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

            //Draw text in all 8 directions for hacky outline. Handled fine by sprite batching however.
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(1 * thickness, 1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(-1 * thickness, -1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(-1 * thickness, 1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(1 * thickness, -1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(1 * thickness, 0), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(-1 * thickness, 0), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(0, 1 * thickness), backColor);
            spriteBatch.DrawString(font, text, position - alignOffset + new Vector2(0, -1 * thickness), backColor);

            spriteBatch.DrawString(font, text, position - alignOffset, frontColor);
        }
    }
}
