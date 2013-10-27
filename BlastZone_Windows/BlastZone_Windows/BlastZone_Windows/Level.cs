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
        const int gridSizeX = 15;
        const int gridSizeY = 11;

        bool[,] solidArea;

        TileObjectManager tileObjectManager;
        LevelAesthetics aesthetics;

        public Level()
        {
            aesthetics = new LevelAesthetics(gridSizeX, gridSizeY);

            solidArea = new bool[gridSizeX, gridSizeY];
            tileObjectManager = new TileObjectManager(gridSizeX, gridSizeY);
        }

        public void LoadContent(ContentManager Content)
        {
            aesthetics.LoadContent(Content);
            aesthetics.GenerateTiles(solidArea);

            tileObjectManager.LoadContent(Content);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 windowSize)
        {
            aesthetics.Draw(spriteBatch, gameTime, windowSize);

            tileObjectManager.Draw(spriteBatch, gameTime, windowSize);
        }
    }
}
