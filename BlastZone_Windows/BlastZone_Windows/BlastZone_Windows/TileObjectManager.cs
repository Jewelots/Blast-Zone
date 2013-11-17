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

        public Level.Level level;

        int gridSizeX, gridSizeY;

        public TileObjectManager(Level.Level level)
        {
            this.gridSizeX = GlobalGameData.gridSizeX;
            this.gridSizeY = GlobalGameData.gridSizeY;

            tileObjectGrid = new TileObject[gridSizeX, gridSizeY];

            tileObjectFactory = new TileObjectFactory();

            this.level = level;
        }

        public void LoadContent(ContentManager Content)
        {
            tileObjectFactory.LoadContent(Content);
        }

        public void RemoveAt(int x, int y)
        {
            if (!GlobalGameData.IsInBounds(x, y)) return;

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

            CreateBomb(3, 1, 5);
            CreateBomb(3, 5, 2);
        }

        public void CreateBomb(int gx, int gy, int power = 3)
        {
            if (!GlobalGameData.IsInBounds(gx, gy)) return;
            if (tileObjectGrid[gx, gy] != null) return; //Spot already occupied, can't place bomb here

            tileObjectGrid[gx, gy] = tileObjectFactory.CreateBomb(this, gx, gy, power);
        }

        public bool SolidAt(int gx, int gy)
        {
            if (!GlobalGameData.IsInBounds(gx, gy)) return true;

            TileObject t = tileObjectGrid[gx, gy];

            if (t == null) return false;

            return t.Solid;
        }

        public void FireSpreadTo(int gx, int gy)
        {
            if (!GlobalGameData.IsInBounds(gx, gy)) return;

            TileObject t = tileObjectGrid[gx, gy];

            if (t == null) return;

            t.FireSpread();
        }
    }
}
