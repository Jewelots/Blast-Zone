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
    class Level
    {
        bool[,] solidArea;

        Vector2 gridSize = new Vector2(15, 11);

        LevelAesthetics aesthetics;

        public Level()
        {
            aesthetics = new LevelAesthetics(gridSize);
            solidArea = new bool[(int)gridSize.X, (int)gridSize.Y];
        }

        public void LoadContent(ContentManager Content)
        {
            aesthetics.LoadContent(Content);

            aesthetics.GenerateTiles(solidArea);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 windowSize)
        {
            aesthetics.Draw(spriteBatch, gameTime, windowSize);
        }
    }
}
