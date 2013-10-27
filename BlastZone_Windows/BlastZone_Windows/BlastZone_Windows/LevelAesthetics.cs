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
    class LevelAesthetics
    {
        Texture2D tileTexture;
        Texture2D hardBlockTexture;
        Texture2D rectFillTex;

        int tileSize = 16;
        int drawRatio = 3;

        int gridSizeX, gridSizeY;

        struct TextureTile
        {
            public int x;
            public int y;
            public Texture2D tex;

            public TextureTile(int x, int y, Texture2D tex) { this.x = x; this.y = y; this.tex = tex; }
            public TextureTile(Vector2 o, Texture2D tex) { this.x = (int)o.X; this.y = (int)o.Y; this.tex = tex; }
        }

        Vector2[] TileTypes;

        enum TileType
        {
            Border,
            LeftBorder,
            TopShadow,
            LeftShadow,
            TopLeftShadow,
            UnderBlockShadow,
            RightBlockShadow,
            DiagonalBlockShadow
        }

        TextureTile[,] textureTileGrid;

        public LevelAesthetics(int gridSizeX, int gridSizeY)
        {
            this.gridSizeX = gridSizeX;
            this.gridSizeY = gridSizeY;

            textureTileGrid = new TextureTile[gridSizeX, gridSizeY];

            TileTypes = new Vector2[Enum.GetNames(typeof(TileType)).Length];
            TileTypes[(int)TileType.Border] = new Vector2(0, 0);
            TileTypes[(int)TileType.LeftBorder] = new Vector2(0, 16);
            TileTypes[(int)TileType.TopShadow] = new Vector2(32, 0);
            TileTypes[(int)TileType.LeftShadow] = new Vector2(16, 16);
            TileTypes[(int)TileType.TopLeftShadow] = new Vector2(16, 0);
            TileTypes[(int)TileType.UnderBlockShadow] = new Vector2(16, 32);
            TileTypes[(int)TileType.RightBlockShadow] = new Vector2(32, 16);
            TileTypes[(int)TileType.DiagonalBlockShadow] = new Vector2(32, 32);
        }

        bool IsInsideBorder(int x, int y)
        {
            return (x > 0 && x < gridSizeX - 1 && y > 0 && y < gridSizeY - 1);
        }

        public void GenerateTiles(bool[,] solidArea)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    if (IsInsideBorder(x, y))
                    {
                        //If first row inside border, cast top shadow
                        if (y == 1)
                        {
                            //If first block inside row, corner shadow
                            if (x == 1)
                                textureTileGrid[x, y] = new TextureTile(TileTypes[(int)TileType.TopLeftShadow], tileTexture);
                            else
                                textureTileGrid[x, y] = new TextureTile(TileTypes[(int)TileType.TopShadow], tileTexture);

                            continue;
                        }

                        //If first column inside border, cast left shadow
                        if (x == 1)
                        {
                            textureTileGrid[x, y] = new TextureTile(TileTypes[(int)TileType.LeftShadow], tileTexture);
                            continue;
                        }

                        //Hard block every 2 spaces
                        if (x % 2 == 0 && y % 2 == 0)
                        {
                            textureTileGrid[x, y] = new TextureTile(0, 0, hardBlockTexture);
                            solidArea[x, y] = true;
                        }
                        else //Make shadow for block
                        {
                            if (x % 2 == y % 2)
                                textureTileGrid[x, y] = new TextureTile(TileTypes[(int)TileType.DiagonalBlockShadow], tileTexture);
                            else if (x % 2 == 0 && y % 2 != 0)
                                textureTileGrid[x, y] = new TextureTile(TileTypes[(int)TileType.UnderBlockShadow], tileTexture);
                            else
                                textureTileGrid[x, y] = new TextureTile(TileTypes[(int)TileType.RightBlockShadow], tileTexture);
                        }
                    }
                    else //The border
                    {
                        if (x == 0 && y != 0 && y != gridSizeY - 1)
                            textureTileGrid[x, y] = new TextureTile(TileTypes[(int)TileType.LeftBorder], tileTexture);
                        else
                            textureTileGrid[x, y] = new TextureTile(TileTypes[(int)TileType.Border], tileTexture);

                        solidArea[x, y] = true;
                    }
                }
            }
        }

        public void LoadContent(ContentManager Content)
        {
            tileTexture = Content.Load<Texture2D>("tiles2");
            hardBlockTexture = Content.Load<Texture2D>("hardblocks");
            rectFillTex = Content.Load<Texture2D>("1px");
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 windowSize)
        {
            Vector2 Position = windowSize / 2;
            Vector2 LevelSize = new Vector2(gridSizeX * tileSize * drawRatio, gridSizeY * tileSize * drawRatio);

            int borderSize = 3;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            spriteBatch.Draw(rectFillTex, new Rectangle((int)(Position.X - borderSize * 2 - LevelSize.X / 2), (int)(Position.Y - borderSize * 2 - LevelSize.Y / 2), (int)LevelSize.X + borderSize * 4, (int)LevelSize.Y + borderSize * 4), Color.White);
            spriteBatch.Draw(rectFillTex, new Rectangle((int)(Position.X - borderSize - LevelSize.X / 2), (int)(Position.Y - borderSize - LevelSize.Y / 2), (int)LevelSize.X + borderSize * 2, (int)LevelSize.Y + borderSize * 2), Color.Black);

            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    TextureTile t = textureTileGrid[x, y];
                    if (t.tex == null) continue;
                    
                    spriteBatch.Draw(t.tex, new Vector2((int)(Position.X + x * tileSize * drawRatio - LevelSize.X / 2), (int)(Position.Y + y * tileSize * drawRatio - LevelSize.Y / 2)), new Rectangle(t.x, t.y, tileSize, tileSize), Color.White, 0, new Vector2(), drawRatio, SpriteEffects.None, 1f);
                }
            }

            spriteBatch.End();
        }
    }
}
