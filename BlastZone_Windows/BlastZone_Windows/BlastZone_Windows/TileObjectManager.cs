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
    class TileObjectManager
    {
        TileObject[,] tileObjectGrid;
        TileObjectFactory tileObjectFactory;

        int gridSizeX, gridSizeY;

        public TileObjectManager(int gridSizeX, int gridSizeY)
        {
            tileObjectGrid = new TileObject[gridSizeX, gridSizeY];

            tileObjectFactory = new TileObjectFactory();

            this.gridSizeX = gridSizeX;
            this.gridSizeY = gridSizeY;
        }

        public void LoadContent(ContentManager Content)
        {
            tileObjectFactory.LoadContent(Content);
            tileObjectGrid[2, 1] = tileObjectFactory.CreateBomb(this, 2, 1);
        }

        public void RemoveAt(int x, int y)
        {
            tileObjectGrid[x, y] = null;
        }

        public void RemoveObject(TileObject o)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    if (tileObjectGrid[x, y] == o)
                    {
                        RemoveAt(x, y);
                        return;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 windowSize)
        {
            int tileSize = 16;
            int drawRatio = 3;

            Vector2 levelSize = new Vector2();
            levelSize.X = windowSize.X / 2 - gridSizeX / 2 * tileSize * drawRatio;
            levelSize.Y = windowSize.Y / 2 - gridSizeY / 2 * tileSize * drawRatio;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
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
