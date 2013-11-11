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

        //Reference to level's solid area
        bool[,] solidArea;

        Texture2D px; ///TEMP

        public FireManager(int gridSizeX, int gridSizeY)
        {
            fireArea = new float[gridSizeX, gridSizeY];

            fireArea[5, 5] = 1;
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


            ///Debug:

            int offsetX, offsetY;
            offsetX = GlobalGameData.windowWidth  / 2 - GlobalGameData.levelSizeX / 2;
            offsetY = GlobalGameData.windowHeight / 2 - GlobalGameData.levelSizeY / 2;

            MouseState mState = Mouse.GetState();

            if (mState.LeftButton == ButtonState.Pressed)
            {
                int factor = GlobalGameData.tileSize * GlobalGameData.drawRatio;

                int gx, gy;

                gx = (mState.X - offsetX) / factor;
                gy = (mState.Y - offsetY) / factor;

                //SetTileOnFire(gx, gy);
                ExplodeFrom(gx, gy);
            }
        }

        void ExplodeFrom(int gx, int gy, int explodeSize = 3)
        {
            if (IsTileSolid(gx, gy)) return;

            SetTileOnFire(gx, gy);

            for (int i = 1; i <= explodeSize; ++i)
            {
                int newgx = gx - i;

                if (IsTileSolid(newgx, gy)) break;
                SetTileOnFire(newgx, gy);
            }

            for (int i = 1; i <= explodeSize; ++i)
            {
                int newgx = gx + i;

                if (IsTileSolid(newgx, gy)) break;
                SetTileOnFire(newgx, gy);
            }

            for (int i = 1; i <= explodeSize; ++i)
            {
                int newgy = gy - i;

                if (IsTileSolid(gx, newgy)) break;
                SetTileOnFire(gx, newgy);
            }

            for (int i = 1; i <= explodeSize; ++i)
            {
                int newgy = gy + i;

                if (IsTileSolid(gx, newgy)) break;
                SetTileOnFire(gx, newgy);
            }
        }

        bool IsTileSolid(int gx, int gy)
        {
            if (!GlobalGameData.IsInBounds(gx, gy)) return true;

            return solidArea[gx, gy];
        }

        void SetTileOnFire(int gx, int gy)
        {
            if (!GlobalGameData.IsInBounds(gx, gy)) return;

            fireArea[gx, gy] = 1;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int y = 0; y < GlobalGameData.gridSizeY; ++y)
            {
                for (int x = 0; x < GlobalGameData.gridSizeX; ++x)
                {
                    int factor = GlobalGameData.tileSize * GlobalGameData.drawRatio;
                    Color color = new Color(fireArea[x, y] / 1, 0, 0) * 0.5f;

                    int border = 20;

                    Rectangle destrect = new Rectangle(x * factor + border, y * factor + border, factor - border * 2, factor - border * 2);
                    spriteBatch.Draw(px, destrect, color);
                }
            }
        }

        public void SetSolidArea(bool[,] solidArea)
        {
            this.solidArea = solidArea;
        }
    }
}
