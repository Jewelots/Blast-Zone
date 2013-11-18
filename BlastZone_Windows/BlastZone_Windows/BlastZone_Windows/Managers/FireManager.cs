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

        Texture2D px; ///TEMP

        public FireManager(TileObjectManager tileObjectManager)
        {
            fireArea = new float[GlobalGameData.gridSizeX, GlobalGameData.gridSizeY];

            this.tileObjectManager = tileObjectManager;
        }

        public void LoadContent(ContentManager Content)
        {
            px = Content.Load<Texture2D>("Images/1px");
        }

        public void Update(GameTime gameTime)
        {
            for (int y = 0; y < GlobalGameData.gridSizeY; ++y)
            {
                for (int x = 0; x < GlobalGameData.gridSizeX; ++x)
                {
                    if (fireArea[x, y] > 0)
                    {
                        //Tile on fire, decrease fire time
                        fireArea[x, y] -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                }
            }
        }

        public void ExplodeFrom(int gx, int gy, int explodeSize = 3)
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

            return true;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int y = 0; y < GlobalGameData.gridSizeY; ++y)
            {
                for (int x = 0; x < GlobalGameData.gridSizeX; ++x)
                {
                    if (!solidArea[x, y])
                    {
                        int factor = GlobalGameData.tileSize * GlobalGameData.drawRatio;
                        Color color = new Color(fireArea[x, y] / 1, 0, 0) * 0.8f;

                        int border = 5;
                    
                        Rectangle destrect = new Rectangle(x * factor + border, y * factor + border, factor - border * 2, factor - border * 2);
                        spriteBatch.Draw(px, destrect, color);
                    }
                }
            }
        }

        public void SetSolidArea(bool[,] solidArea)
        {
            this.solidArea = solidArea;
        }
    }
}
