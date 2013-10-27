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
        Texture2D tiles;
        Texture2D hardblocks;
        Texture2D softblocks;
        Texture2D fillTex;

        int tileSize = 16;
        int drawRatio = 3;
        Vector2 gridSize = new Vector2(15, 11);

        struct Tile
        {
            public int x;
            public int y;
            public Texture2D tex;

            public Tile(int x, int y, Texture2D tex) { this.x = x; this.y = y; this.tex = tex; }
        }

        Tile[,] tileGrid;

        public Level()
        {
            tileGrid = new Tile[(int)gridSize.X, (int)gridSize.Y];
        }

        public void LoadContent(ContentManager Content)
        {
            tiles = Content.Load<Texture2D>("tiles2");
            hardblocks = Content.Load<Texture2D>("hardblocks");
            softblocks = Content.Load<Texture2D>("softblocks");
            fillTex = Content.Load<Texture2D>("1px");

            GenerateTiles();
        }

        void GenerateTiles()
        {
            for (int y = 0; y < (int)gridSize.Y; ++y)
            {
                for (int x = 0; x < (int)gridSize.X; ++x)
                {
                    if (x > 0 && x < (int)gridSize.X - 1 && y > 0 && y < (int)gridSize.Y - 1)
                    {
                        if (y > 1)
                        {
                            if (x == 1)
                                tileGrid[x, y] = new Tile(16, 16, tiles);
                            else
                            {
                                if (x % 2 == 0 && y % 2 == 0)
                                    tileGrid[x, y] = new Tile(0, 0, hardblocks);
                                else //Shadow
                                {
                                    if (x % 2 == y % 2)
                                        tileGrid[x, y] = new Tile(32, 32, tiles);
                                    else if (x % 2 == 0 && y % 2 != 0)
                                        tileGrid[x, y] = new Tile(16, 32, tiles);
                                    else
                                        tileGrid[x, y] = new Tile(32, 16, tiles);
                                }
                            }
                        }
                        else
                        {
                            if (x == 1)
                                tileGrid[x, y] = new Tile(16, 0, tiles);
                            else
                                tileGrid[x, y] = new Tile(32, 0, tiles);
                        }
                    }
                    else
                    {
                        tileGrid[x, y] = new Tile(0, 0, tiles); //Border
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 windowSize)
        {
            Vector2 Position = windowSize / 2;
            Vector2 LevelSize = new Vector2(gridSize.X * tileSize * drawRatio, gridSize.Y * tileSize * drawRatio);

            int borderSize = 3;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            spriteBatch.Draw(fillTex, new Rectangle((int)(Position.X - borderSize * 2 - LevelSize.X / 2), (int)(Position.Y - borderSize * 2 - LevelSize.Y / 2), (int)LevelSize.X + borderSize * 4, (int)LevelSize.Y + borderSize * 4), Color.White);
            spriteBatch.Draw(fillTex, new Rectangle((int)(Position.X - borderSize     - LevelSize.X / 2), (int)(Position.Y - borderSize     - LevelSize.Y / 2), (int)LevelSize.X + borderSize * 2, (int)LevelSize.Y + borderSize * 2), Color.Black);

            for (int y = 0; y < (int)gridSize.Y; ++y)
            {
                for (int x = 0; x < (int)gridSize.X; ++x)
                {
                    Tile t = tileGrid[x, y];
                    if (t.tex != null)
                        spriteBatch.Draw(t.tex, new Vector2((int)(Position.X + x * tileSize * drawRatio - LevelSize.X / 2), (int)(Position.Y + y * tileSize * drawRatio - LevelSize.Y / 2)), new Rectangle(t.x, t.y, tileSize, tileSize), Color.White, 0, new Vector2(), drawRatio, SpriteEffects.None, 1f);
                }
            }

            spriteBatch.End();
        }
    }
}
