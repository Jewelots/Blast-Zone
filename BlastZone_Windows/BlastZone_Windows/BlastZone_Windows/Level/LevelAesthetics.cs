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

namespace BlastZone_Windows.Level
{
    /// <summary>
    /// Used to generate and display the purely aesthetic tiles of the level
    /// </summary>
    class LevelAesthetics
    {
        /// <summary>
        /// Spritemap for the tiles
        /// </summary>
        Texture2D tileTexture;

        /// <summary>
        /// Sprite for the hard blocks
        /// </summary>
        Texture2D hardBlockTexture;

        int gridSizeX, gridSizeY;

        /// <summary>
        /// A purely aesthetic textured tile that provides a position (in tiles) and a texture
        /// </summary>
        struct TextureTile
        {
            public int x;
            public int y;
            public Texture2D tex;
            
            public TextureTile(int x, int y, Texture2D tex) { this.x = x; this.y = y; this.tex = tex; }
            public TextureTile(Vector2 o, Texture2D tex) { this.x = (int)o.X; this.y = (int)o.Y; this.tex = tex; }
        }

        /// <summary>
        /// Tile type enumerator to make generating tiles more readable
        /// </summary>
        enum TileType
        {
            Border,
            LeftBorder,
            TopShadow,
            LeftShadow,
            TopLeftShadow,
            UnderBlockShadow,
            RightBlockShadow,
            DiagonalBlockShadow,
            _TILE_COUNT
        }

        /// <summary>
        /// An array of Vectors to link TileTypes to Texture Positions to easily create TextureTiles
        /// </summary>
        Vector2[] TileTypes;

        /// <summary>
        /// A 2D grid of textured tiles
        /// </summary>
        TextureTile[,] textureTileGrid;

        public LevelAesthetics()
        {
            this.gridSizeX = GlobalGameData.gridSizeX;
            this.gridSizeY = GlobalGameData.gridSizeY;

            textureTileGrid = new TextureTile[gridSizeX, gridSizeY];

            TileTypes = new Vector2[(int)TileType._TILE_COUNT];
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

        /// <summary>
        /// Generates tiles
        /// </summary>
        /// <param name="solidArea">A reference array that will be filled with the solid tiles</param>
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
            tileTexture = Content.Load<Texture2D>("Images/Game/tiles2");
            hardBlockTexture = Content.Load<Texture2D>("Images/Game/hardblocks");
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Loop through tiles
            for (int y = 0; y < gridSizeY; ++y)
            {
                for (int x = 0; x < gridSizeX; ++x)
                {
                    //Get the tile at x, y
                    TextureTile t = textureTileGrid[x, y];

                    //If the tile has no texture, don't draw it
                    if (t.tex == null) continue;

                    //Get the draw position
                    Vector2 drawPos = new Vector2();
                    drawPos.X = x * GlobalGameData.tileSize * GlobalGameData.drawRatio;
                    drawPos.Y = y * GlobalGameData.tileSize * GlobalGameData.drawRatio;

                    //Draw it
                    spriteBatch.Draw(
                        t.tex, //Texture
                        drawPos, //Position
                        new Rectangle(t.x, t.y, GlobalGameData.tileSize, GlobalGameData.tileSize), //Source
                        Color.White, //Color
                        0, //Rotation
                        new Vector2(), //Offset
                        GlobalGameData.drawRatio, //Scale
                        SpriteEffects.None, //Sprite Effect (Flip)
                        1f //Layer Depth
                    );
                }
            }
        }
    }
}
