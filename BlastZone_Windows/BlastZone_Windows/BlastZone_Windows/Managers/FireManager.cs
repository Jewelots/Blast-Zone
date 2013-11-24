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
    /// A class to handle fire spread and particle emission
    /// </summary>
    class FireManager
    {
        /// <summary>
        /// 2D array of float to designate what's on fire (>0) and time left
        /// </summary>
        float[,] fireArea;

        TileObjectManager tileObjectManager;

        //Reference to level's solid area (hacky)
        bool[,] solidArea;

        public FireManager(TileObjectManager tileObjectManager)
        {
            this.tileObjectManager = tileObjectManager;
        }

        /// <summary>
        /// Reset to clear the grid of fire
        /// </summary>
        public void Reset()
        {
            fireArea = new float[GlobalGameData.gridSizeX, GlobalGameData.gridSizeY];
        }

        /// <summary>
        /// Update all the fire
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            for (int y = 0; y < GlobalGameData.gridSizeY; ++y)
            {
                for (int x = 0; x < GlobalGameData.gridSizeX; ++x)
                {
                    if (fireArea[x, y] > 0)
                    {
                        //Tile on fire, decrease fire time so it takes 0.5 seconds for them to disappear
                        fireArea[x, y] -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                    }
                }
            }
        }

        /// <summary>
        /// Generate an explosion
        /// </summary>
        /// <param name="gx">Grid position x</param>
        /// <param name="gy">Grid position y</param>
        /// <param name="explodeSize">Radius of explosion in tiles</param>
        public void ExplodeFrom(int gx, int gy, int explodeSize)
        {
            //If you can't set the current tile on fire, don't do anything
            if (!SetTileOnFire(gx, gy)) return;

            //Go through the west branch and stop if you hit an obstacle
            for (int i = 1; i <= explodeSize; ++i)
            {
                if (!SetTileOnFire(gx - i, gy)) break;
            }

            //Go through the east branch and stop if you hit an obstacle
            for (int i = 1; i <= explodeSize; ++i)
            {
                if (!SetTileOnFire(gx + i, gy)) break;
            }

            //Go through the north branch and stop if you hit an obstacle
            for (int i = 1; i <= explodeSize; ++i)
            {
                if (!SetTileOnFire(gx, gy - i)) break;
            }

            //Go through the south branch and stop if you hit an obstacle
            for (int i = 1; i <= explodeSize; ++i)
            {
                if (!SetTileOnFire(gx, gy + i)) break;
            }
        }

        /// <summary>
        /// Returns true if the tile is solid
        /// </summary>
        /// <param name="gx">Tile position x</param>
        /// <param name="gy">Tile position y</param>
        /// <returns></returns>
        bool IsTileSolid(int gx, int gy)
        {
            //Check if in bounds
            if (!GlobalGameData.IsInBounds(gx, gy)) return true;

            return solidArea[gx, gy];
        }

        /// <summary>
        /// Set a tile on fire
        /// </summary>
        /// <param name="gx">Tile position x</param>
        /// <param name="gy">Tile position y</param>
        /// <returns>Returns false if obstacle in the way</returns>
        bool SetTileOnFire(int gx, int gy)
        {
            //Check if in bounds
            if (!GlobalGameData.IsInBounds(gx, gy)) return false;

            //Check if solid
            if (IsTileSolid(gx, gy)) return false;

            //Check if there's a solid object
            if (tileObjectManager.SolidAt(gx, gy))
            {
                //If there is, spread fire to it, and return false
                tileObjectManager.FireSpreadTo(gx, gy);
                return false;
            }
            else
            {
                //Otherwise, just spread fire to it
                tileObjectManager.FireSpreadTo(gx, gy);
            }

            //Set the fire timer for that grid square
            fireArea[gx, gy] = 1;

            //Calculate particle emit position
            Vector2 levelOffsetPos = new Vector2(GlobalGameData.windowWidth / 2 - GlobalGameData.levelSizeX / 2, GlobalGameData.windowHeight / 2 - GlobalGameData.levelSizeY / 2);
            Vector2 emitPos = levelOffsetPos + new Vector2(gx, gy) * GlobalGameData.tileSize * GlobalGameData.drawRatio;
            emitPos.X += (GlobalGameData.tileSize * GlobalGameData.drawRatio) / 2;
            emitPos.Y += (GlobalGameData.tileSize * GlobalGameData.drawRatio) / 2;

            //Emit particles
            if (GlobalGameData.LowQualityParticles)
            {
                Managers.ParticleManager.Emit("ExplosionFast", emitPos);
            }
            else
            {
                Managers.ParticleManager.Emit("Explosion", emitPos);
            }

            return true;
        }

        /// <summary>
        /// Set the solid area that the fire interacts with
        /// </summary>
        /// <param name="solidArea">A 2D array of bools representing if a tile is solid or not</param>
        public void SetSolidArea(bool[,] solidArea)
        {
            this.solidArea = solidArea;
        }

        /// <summary>
        /// Get if a tile is on fire
        /// </summary>
        /// <param name="gx">Tile position x</param>
        /// <param name="gy">Tile position y</param>
        /// <returns>True if the tile is on fire, false otherwise</returns>
        public bool IsOnFire(int gx, int gy)
        {
            return fireArea[gx, gy] > 0;
        }
    }
}
