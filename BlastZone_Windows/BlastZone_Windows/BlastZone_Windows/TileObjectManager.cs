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
    /// <summary>
    /// A manager to manage TileObjects
    /// </summary>
    class TileObjectManager
    {
        /// <summary>
        /// A grid of TileObject's
        /// </summary>
        TileObject[,] tileObjectGrid;

        /// <summary>
        /// A factory to create TileObjects
        /// </summary>
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

        public void Update(GameTime gameTime)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    TileObject to = tileObjectGrid[x, y];
                    if (to == null) continue;

                    to.Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    TileObject to = tileObjectGrid[x, y];
                    if (to == null) continue;

                    to.Draw(spriteBatch, gameTime);
                }
            }
        }

        public void Reset()
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    tileObjectGrid[x, y] = null;
                }
            }

            tileObjectGrid[2, 1] = tileObjectFactory.CreateBomb(this, 2, 1);
        }
    }
}
