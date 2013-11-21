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

        int[] activeBombs;
        Dictionary<TileObject, int> playerOwnedBombs;

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

        public void Reset()
        {
            int levelType = GlobalGameData.rand.Next(4);
            //Clear grid and spawn soft blocks
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    tileObjectGrid[x, y] = null;

                    //Spawn soft blocks in a checker pattern
                    if (x > 0 && x < GlobalGameData.gridSizeX - 1 && y > 0 && y < GlobalGameData.gridSizeY - 1)
                    {
                        if (x % 2 != y % 2)
                        {
                            tileObjectGrid[x, y] = tileObjectFactory.CreateSoftBlock(this, x, y, levelType);
                        }
                    }

                    //Clear blocks near players
                    tileObjectGrid[2, 1] = null;
                    tileObjectGrid[1, 2] = null;

                    tileObjectGrid[GlobalGameData.gridSizeX - 3, 1] = null;
                    tileObjectGrid[GlobalGameData.gridSizeX - 2, 2] = null;

                    tileObjectGrid[2, GlobalGameData.gridSizeY - 2] = null;
                    tileObjectGrid[1, GlobalGameData.gridSizeY - 3] = null;

                    tileObjectGrid[GlobalGameData.gridSizeX - 3, GlobalGameData.gridSizeY - 2] = null;
                    tileObjectGrid[GlobalGameData.gridSizeX - 2, GlobalGameData.gridSizeY - 3] = null;
                }
            }

            //Reset active bombs
            activeBombs = new int[4];
            playerOwnedBombs = new Dictionary<TileObject, int>();
        }

        public void LoadContent(ContentManager Content)
        {
            tileObjectFactory.LoadContent(Content);
        }

        public void RemoveAt(int x, int y)
        {
            if (!GlobalGameData.IsInBounds(x, y)) return;

            TileObject obj = tileObjectGrid[x, y];

            if (playerOwnedBombs.ContainsKey(obj))
            {
                activeBombs[playerOwnedBombs[obj]] -= 1;
                playerOwnedBombs.Remove(obj);
            }

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

        /// <summary>
        /// Create a bomb at a tile, with a power
        /// </summary>
        /// <param name="playerIndex">Player who owns the bomb</param>
        /// <param name="gx">X Position in terms of tiles</param>
        /// <param name="gy">Y Position in terms of tiles</param>
        /// <param name="power">Power of bomb (tiles of explosion radius)</param>
        /// <returns></returns>
        public bool CreateBomb(int playerIndex, int gx, int gy, int power)
        {
            if (!GlobalGameData.IsInBounds(gx, gy)) return false;
            if (tileObjectGrid[gx, gy] != null) return false; //Spot already occupied, can't place bomb here

            if (activeBombs[playerIndex] < level.getMaxBombs(playerIndex))
            {
                //Create bomb
                Bomb b = tileObjectFactory.CreateBomb(this, gx, gy, power);

                //Add the bomb to be tracked
                playerOwnedBombs[b] = playerIndex;
                activeBombs[playerIndex] += 1;

                //Add bomb to tile object grid
                tileObjectGrid[gx, gy] = b;

                return true;
            }

            return false;
        }

        public void CreateBomb(int playerIndex, int gx, int gy)
        {
            if (!GlobalGameData.IsInBounds(gx, gy)) return;
            if (tileObjectGrid[gx, gy] != null) return; //Spot already occupied, can't place bomb here

            if (activeBombs[playerIndex] < level.getMaxBombs(playerIndex))
            {
                //Create bomb
                Bomb b = tileObjectFactory.CreateBomb(this, gx, gy, 3);

                //Add the bomb to be tracked
                playerOwnedBombs[b] = playerIndex;
                activeBombs[playerIndex] += 1;

                //Add bomb to tile object grid
                tileObjectGrid[gx, gy] = b;
            }
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

        public TileObject ObjectAt(int gx, int gy)
        {
            return tileObjectGrid[gx, gy];
        }

        public void SpawnPowerup(int tilePositionX, int tilePositionY)
        {
            if (GlobalGameData.rand.Next(0, 2) != 0) return;

            PowerupType pType;

            int randNum = GlobalGameData.rand.Next(0, 3);
            pType = (PowerupType)randNum;

            tileObjectGrid[tilePositionX, tilePositionY] = tileObjectFactory.CreatePowerup(this, tilePositionX, tilePositionY, pType);
        }
    }
}
