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
        List<TileObject>[,] tileObjectGrid;

        /// <summary>
        /// A factory to create TileObjects
        /// </summary>
        TileObjectFactory tileObjectFactory;

        /// <summary>
        /// Array of active bombs per-player
        /// </summary>
        int[] activeBombs;

        /// <summary>
        /// A dictionary of what player owns what bomb
        /// </summary>
        Dictionary<TileObject, int> playerOwnedBombs;

        /// <summary>
        /// A reference to the level the manager is in
        /// </summary>
        public Level.Level level;

        int gridSizeX, gridSizeY;

        /// <summary>
        /// Create a new TileObject manager
        /// </summary>
        /// <param name="level">The level in which to tie the manager to</param>
        public TileObjectManager(Level.Level level)
        {
            this.gridSizeX = GlobalGameData.gridSizeX;
            this.gridSizeY = GlobalGameData.gridSizeY;

            tileObjectGrid = new List<TileObject>[gridSizeX, gridSizeY];

            tileObjectFactory = new TileObjectFactory();

            this.level = level;
        }

        /// <summary>
        /// Reset the tile object manager and initialise
        /// </summary>
        public void Reset()
        {
            int levelType = GlobalGameData.rand.Next(4);
            //Clear grid and spawn soft blocks
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    tileObjectGrid[x, y] = new List<TileObject>();

                    //Spawn soft blocks in a checker pattern
                    if (x > 0 && x < GlobalGameData.gridSizeX - 1 && y > 0 && y < GlobalGameData.gridSizeY - 1)
                    {
                        if (x % 2 != y % 2)
                        {
                            tileObjectGrid[x, y].Add(tileObjectFactory.CreateSoftBlock(this, x, y, levelType));
                        }
                    }
                }
            }

            //Clear blocks near players
            tileObjectGrid[2, 1].Clear();
            tileObjectGrid[1, 2].Clear();

            tileObjectGrid[GlobalGameData.gridSizeX - 3, 1].Clear();
            tileObjectGrid[GlobalGameData.gridSizeX - 2, 2].Clear();

            tileObjectGrid[2, GlobalGameData.gridSizeY - 2].Clear();
            tileObjectGrid[1, GlobalGameData.gridSizeY - 3].Clear();

            tileObjectGrid[GlobalGameData.gridSizeX - 3, GlobalGameData.gridSizeY - 2].Clear();
            tileObjectGrid[GlobalGameData.gridSizeX - 2, GlobalGameData.gridSizeY - 3].Clear();

            //Reset active bombs
            activeBombs = new int[4];
            playerOwnedBombs = new Dictionary<TileObject, int>();
        }

        public void LoadContent(ContentManager Content)
        {
            tileObjectFactory.LoadContent(Content);
        }

        /// <summary>
        /// Remove any objects at (x, y)
        /// </summary>
        /// <param name="x">Grid position x</param>
        /// <param name="y">Grid position y</param>
        public void RemoveAt(int x, int y)
        {
            //If not in bounds, return
            if (!GlobalGameData.IsInBounds(x, y)) return;

            //Get a list of all objects at (x, y)
            List<TileObject> objects = tileObjectGrid[x, y];

            //Loop through all objects on tile
            foreach (TileObject obj in objects)
            {
                //If the player owns the object on the tile
                if (playerOwnedBombs.ContainsKey(obj))
                {
                    //Decrease the player's active bomb count and remove the object
                    activeBombs[playerOwnedBombs[obj]] -= 1;
                    playerOwnedBombs.Remove(obj);
                }
            }

            //Clear the tile
            tileObjectGrid[x, y].Clear();
        }

        //Remove a specific object
        public void RemoveObject(TileObject obj)
        {
            //Loop through the grid to find the object
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    //Check if the tile contains the object
                    if (tileObjectGrid[x, y].Contains(obj))
                    {
                        //If the player owns the object
                        if (playerOwnedBombs.ContainsKey(obj))
                        {
                            //Decrease the player's active bomb count and remove the object
                            activeBombs[playerOwnedBombs[obj]] -= 1;
                            playerOwnedBombs.Remove(obj);
                        }

                        //Remove the object from the tile
                        tileObjectGrid[x, y].Remove(obj);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Update all TileObjects
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //Loop through the grid
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    //Get a list of all checked objcts on the tile
                    List<TileObject> checkedObjects = new List<TileObject>();

                    int currentIndex = 0;
                    
                    //A fairly hacky method to loop through and update them without breaking when an object removes itself mid-update
                    while (currentIndex < tileObjectGrid[x, y].Count)
                    {
                        TileObject t = tileObjectGrid[x, y][currentIndex];
                        t.Update(gameTime);
                        checkedObjects.Add(t);
                        if (tileObjectGrid[x, y].Contains(t))
                        {
                            currentIndex += 1;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    foreach (TileObject to in tileObjectGrid[x, y])
                    {
                        to.Draw(spriteBatch, gameTime);
                    }
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
            if (tileObjectGrid[gx, gy].Count > 0) return false; //Spot already occupied, can't place bomb here

            if (activeBombs[playerIndex] < level.getMaxBombs(playerIndex))
            {
                //Create bomb
                Bomb b = tileObjectFactory.CreateBomb(this, gx, gy, power);

                //Add the bomb to be tracked
                playerOwnedBombs[b] = playerIndex;
                activeBombs[playerIndex] += 1;

                //Add bomb to tile object grid
                tileObjectGrid[gx, gy].Add(b);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Create a bomb at a tile, with power 3
        /// </summary>
        /// <param name="playerIndex">Player who owns the bomb</param>
        /// <param name="gx">X Position in terms of tiles</param>
        /// <param name="gy">Y Position in terms of tiles</param>
        public void CreateBomb(int playerIndex, int gx, int gy)
        {
            CreateBomb(playerIndex, gx, gy, 3);
        }

        /// <summary>
        /// Get if there's a solid object on tile (gx, gy)
        /// </summary>
        /// <param name="gx">X Position in terms of tiles</param>
        /// <param name="gy">Y Position in terms of tiles</param>
        public bool SolidAt(int gx, int gy)
        {
            //Return solid if out of bounds
            if (!GlobalGameData.IsInBounds(gx, gy)) return true;

            bool solid = false;

            //Loop through each object on the tile, if one of them's solid, return solid
            foreach (TileObject t in tileObjectGrid[gx, gy])
            {
                if (t.Solid)
                {
                    solid = true;
                }
            }

            return solid;
        }

        /// <summary>
        /// Spread fire to a tile and tell all the objects on that tile
        /// </summary>
        /// <param name="gx">X position in terms of tiles</param>
        /// <param name="gy">Y position in terms of tiles</param>
        public void FireSpreadTo(int gx, int gy)
        {
            //Return if out of bounds
            if (!GlobalGameData.IsInBounds(gx, gy)) return;

            bool tileSolid = false;
            foreach (TileObject t in tileObjectGrid[gx, gy])
            {
                if (t.Solid)
                {
                    tileSolid = true;
                }
            }

            List<TileObject> checkedObjects = new List<TileObject>();

            int currentIndex = 0;

            while (currentIndex < tileObjectGrid[gx, gy].Count)
            {
                TileObject t = tileObjectGrid[gx, gy][currentIndex];
                if (t.Solid == tileSolid)
                {
                    t.FireSpread();
                }
                checkedObjects.Add(t);
                if (tileObjectGrid[gx, gy].Contains(t))
                {
                    currentIndex += 1;
                }
            }
        }

        public TileObject NonSolidObjectAt(int gx, int gy)
        {
            foreach (TileObject t in tileObjectGrid[gx, gy])
            {
                if (!t.Solid)
                {
                    return t;
                }
            }

            return null;
        }

        public void SpawnPowerup(int tilePositionX, int tilePositionY)
        {
            if (GlobalGameData.rand.Next(0, 2) != 0) return;

            PowerupType pType;

            int randNum = GlobalGameData.rand.Next(0, 3);
            pType = (PowerupType)randNum;

            tileObjectGrid[tilePositionX, tilePositionY].Add(tileObjectFactory.CreatePowerup(this, tilePositionX, tilePositionY, pType));
        }
    }
}
