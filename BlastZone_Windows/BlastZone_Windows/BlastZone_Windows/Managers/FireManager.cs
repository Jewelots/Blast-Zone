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

        //Reference to level's solid area
        bool[,] solidArea;

        public FireManager(TileObjectManager tileObjectManager)
        {
            this.tileObjectManager = tileObjectManager;
        }

        public void Reset()
        {
            fireArea = new float[GlobalGameData.gridSizeX, GlobalGameData.gridSizeY];
        }

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

        public void ExplodeFrom(int gx, int gy, int explodeSize)
        {
            if (!SetTileOnFire(gx, gy)) return;

            for (int i = 1; i <= explodeSize; ++i)
            {
                if (!SetTileOnFire(gx - i, gy)) break;
            }

            for (int i = 1; i <= explodeSize; ++i)
            {
                if (!SetTileOnFire(gx + i, gy)) break;
            }

            for (int i = 1; i <= explodeSize; ++i)
            {
                if (!SetTileOnFire(gx, gy - i)) break;
            }

            for (int i = 1; i <= explodeSize; ++i)
            {
                if (!SetTileOnFire(gx, gy + i)) break;
            }
        }

        bool IsTileSolid(int gx, int gy)
        {
            if (!GlobalGameData.IsInBounds(gx, gy)) return true;

            return solidArea[gx, gy];
        }

        bool SetTileOnFire(int gx, int gy)
        {
            if (!GlobalGameData.IsInBounds(gx, gy)) return false;
            if (IsTileSolid(gx, gy)) return false;

            if (tileObjectManager.SolidAt(gx, gy))
            {
                tileObjectManager.FireSpreadTo(gx, gy);
                return false;
            }
            else
            {
                tileObjectManager.FireSpreadTo(gx, gy);
            }

            fireArea[gx, gy] = 1;

            Vector2 levelOffsetPos = new Vector2(GlobalGameData.windowWidth / 2 - GlobalGameData.levelSizeX / 2, GlobalGameData.windowHeight / 2 - GlobalGameData.levelSizeY / 2);
            Vector2 emitPos = levelOffsetPos + new Vector2(gx, gy) * GlobalGameData.tileSize * GlobalGameData.drawRatio;
            emitPos.X += (GlobalGameData.tileSize * GlobalGameData.drawRatio) / 2;
            emitPos.Y += (GlobalGameData.tileSize * GlobalGameData.drawRatio) / 2;
            Managers.ParticleManager.Emit("ExplosionFast", emitPos);

            return true;
        }

        public void SetSolidArea(bool[,] solidArea)
        {
            this.solidArea = solidArea;
        }

        public bool IsOnFire(int gx, int gy)
        {
            return fireArea[gx, gy] > 0;
        }
    }
}
