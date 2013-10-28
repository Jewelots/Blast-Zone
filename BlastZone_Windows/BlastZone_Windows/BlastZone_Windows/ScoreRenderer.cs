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
    class ScoreRenderer
    {
        SpriteFont scoreFont;

        Color[] backColors;
        Color[] frontColors;
        int[] scores;

        Vector2[] renderPositions;
        float[] scoreOffset;
        string[] text;

        public ScoreRenderer()
        {
            backColors = new Color[4];
            frontColors = new Color[4];
            scores = new int[4];

            renderPositions = new Vector2[4];
            scoreOffset = new float[4];
            text = new string[4];

            backColors[0] = Color.Black;
            frontColors[0] = Color.White;

            backColors[1] = new Color(99, 95, 95);
            frontColors[1] = new Color(34, 34, 34);

            backColors[2] = Color.Black;
            frontColors[2] = new Color(215, 0, 0);

            backColors[3] = Color.Black;
            frontColors[3] = new Color(29, 125, 223);

            text[0] = "Player One";
            text[1] = "Player Two";
            text[2] = "Player Three";
            text[3] = "Player Four";

            float xOffset = 10;
            float yOffset = 10;

            //Strange magic numbers are to account for standard font rendering offsets (padding on actual font characters)
            renderPositions[0] = new Vector2(xOffset, yOffset - 10);
            renderPositions[1] = new Vector2(GlobalGameData.windowWidth - xOffset, yOffset - 10);
            renderPositions[2] = new Vector2(xOffset, GlobalGameData.windowHeight + yOffset);
            renderPositions[3] = new Vector2(GlobalGameData.windowWidth - xOffset, GlobalGameData.windowHeight + yOffset);
        }

        public void LoadContent(ContentManager Content)
        {
            scoreFont = Content.Load<SpriteFont>("Badaboom");

            scoreOffset[0] = scoreFont.MeasureString(text[0]).Y - 10;
            scoreOffset[1] = scoreFont.MeasureString(text[1]).Y - 10;
            scoreOffset[2] = -(scoreFont.MeasureString(text[2]).Y - 10);
            scoreOffset[3] = -(scoreFont.MeasureString(text[3]).Y - 10);
        }

        public void SetScore(int player, int score)
        {
            scores[player] = score;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw Text
            DrawTextExtension.DrawTextOutline(spriteBatch, scoreFont, text[0], backColors[0], frontColors[0], renderPositions[0], 3f);
            DrawTextExtension.DrawTextOutline(spriteBatch, scoreFont, text[1], backColors[1], frontColors[1], renderPositions[1], 3f, HorizontalAlign.AlignRight);
            DrawTextExtension.DrawTextOutline(spriteBatch, scoreFont, text[2], backColors[2], frontColors[2], renderPositions[2], 3f, HorizontalAlign.AlignLeft,  VerticalAlign.AlignBottom);
            DrawTextExtension.DrawTextOutline(spriteBatch, scoreFont, text[3], backColors[3], frontColors[3], renderPositions[3], 3f, HorizontalAlign.AlignRight, VerticalAlign.AlignBottom);

            //Draw Scores
            DrawTextExtension.DrawTextOutline(spriteBatch, scoreFont, scores[0].ToString(), backColors[0], frontColors[0], renderPositions[0] + new Vector2(0, scoreOffset[0]), 3f);
            DrawTextExtension.DrawTextOutline(spriteBatch, scoreFont, scores[1].ToString(), backColors[1], frontColors[1], renderPositions[1] + new Vector2(0, scoreOffset[1]), 3f, HorizontalAlign.AlignRight);
            DrawTextExtension.DrawTextOutline(spriteBatch, scoreFont, scores[2].ToString(), backColors[2], frontColors[2], renderPositions[2] + new Vector2(0, scoreOffset[2]), 3f, HorizontalAlign.AlignLeft, VerticalAlign.AlignBottom);
            DrawTextExtension.DrawTextOutline(spriteBatch, scoreFont, scores[3].ToString(), backColors[3], frontColors[3], renderPositions[3] + new Vector2(0, scoreOffset[3]), 3f, HorizontalAlign.AlignRight, VerticalAlign.AlignBottom);
        }
    }
}
