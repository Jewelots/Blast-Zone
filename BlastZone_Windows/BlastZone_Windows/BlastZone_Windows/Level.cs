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
        TileObject[,] tileObjectGrid;

        Vector2 gridSize = new Vector2(15, 11);

        LevelAesthetics aesthetics;
        TileObjectFactory tileObjectFactory;

        public Level()
        {
            aesthetics = new LevelAesthetics(gridSize);

            solidArea = new bool[(int)gridSize.X, (int)gridSize.Y];
            tileObjectGrid = new TileObject[(int)gridSize.X, (int)gridSize.Y];

            tileObjectFactory = new TileObjectFactory();
        }

        public void LoadContent(ContentManager Content)
        {
            aesthetics.LoadContent(Content);
            aesthetics.GenerateTiles(solidArea);

            tileObjectFactory.LoadContent(Content);
            tileObjectGrid[2, 1] = tileObjectFactory.CreateBomb(new Vector2(2, 1));
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 windowSize)
        {
            aesthetics.Draw(spriteBatch, gameTime, windowSize);

            int tileSize = 16;
            int drawRatio = 3;

            Vector2 levelSize = new Vector2();
            levelSize.X = windowSize.X / 2 - gridSize.X / 2 * tileSize * drawRatio;
            levelSize.Y = windowSize.Y / 2 - gridSize.Y / 2 * tileSize * drawRatio;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            for (int y = 0; y < (int)gridSize.Y; ++y)
            {
                for (int x = 0; x < (int)gridSize.X; ++x)
                {
                    TileObject to = tileObjectGrid[x, y];
                    if (to == null) continue;

                    to.Draw(spriteBatch, gameTime, windowSize, levelSize);
                }
            }

            spriteBatch.End();
        }
    }
}
