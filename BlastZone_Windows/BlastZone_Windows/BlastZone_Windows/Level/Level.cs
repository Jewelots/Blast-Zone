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

using BlastZone_Windows.MovementGrid;

namespace BlastZone_Windows.Level
{
    /// <summary>
    /// Encapsulates the aesthetic tiles and all entities that interact.
    /// The game itself.
    /// </summary>
    class Level
    {
        /// <summary>
        /// 2D array of bools to designate what's solid and what's not
        /// </summary>
        bool[,] solidArea;

        TileObjectManager tileObjectManager;
        public FireManager fireManager;

        LevelAesthetics aesthetics;

        GridNodeMap gridNodeMap;
        //GridNodeMover gridNodeMover;

        Player[] players;
        PlayerInputController[] playerInputControllers;

        int playerCount = 1;

        public Level()
        {
            aesthetics = new LevelAesthetics();

            tileObjectManager = new TileObjectManager(this);
            fireManager = new FireManager(tileObjectManager);

            solidArea = new bool[GlobalGameData.gridSizeX, GlobalGameData.gridSizeY];

            gridNodeMap = new GridNodeMap();

            players = new Player[playerCount];
            players[0] = new Player(gridNodeMap, 5, 3);

            playerInputControllers = new PlayerInputController[playerCount];
            for (int i = 0; i < playerCount; ++i)
            {
                playerInputControllers[i] = new PlayerInputController(players[i]);
            }
        }

        /// <summary>
        /// Returns a compilation of Always-Solid walls and solid TileObjects
        /// </summary>
        /// <returns>A 2D array of booleans representing solid (true) or not solid.</returns>
        public bool[,] GetSolid()
        {
            bool[,] solid = new bool[GlobalGameData.gridSizeX, GlobalGameData.gridSizeY];

            for (int y = 0; y < GlobalGameData.gridSizeY; ++y)
            {
                for (int x = 0; x < GlobalGameData.gridSizeX; ++x)
                {
                    solid[x, y] = solidArea[x, y] || tileObjectManager.SolidAt(x, y);
                }
            }

            return solid;
        }

        public void LoadContent(ContentManager Content)
        {
            aesthetics.LoadContent(Content);
            aesthetics.GenerateTiles(solidArea);

            tileObjectManager.LoadContent(Content);

            ///////////////REPLACE LATER/////////////
            fireManager.LoadContent(Content);
            /////////////////////////////////////////

            for (int i = 0; i < playerCount; ++i)
            {
                players[i].LoadContent(Content);
            }

            fireManager.SetSolidArea(solidArea);
        }

        public void Reset()
        {
            tileObjectManager.Reset();
        }

        public void Update(GameTime gameTime)
        {
            tileObjectManager.Update(gameTime);

            fireManager.Update(gameTime);

            gridNodeMap.SetSolid(GetSolid());

            playerInputControllers[0].GetInput(Keyboard.GetState());

            for (int i = 0; i < playerCount; ++i)
            {
                players[i].Update(gameTime);
            }

            ///////////////REPLACE LATER/////////////
            int offsetX, offsetY;
            offsetX = GlobalGameData.windowWidth / 2 - GlobalGameData.levelSizeX / 2;
            offsetY = GlobalGameData.windowHeight / 2 - GlobalGameData.levelSizeY / 2;

            MouseState mState = Mouse.GetState();

            int factor = GlobalGameData.tileSize * GlobalGameData.drawRatio;

            int gx, gy;
            gx = (mState.X - offsetX) / factor;
            gy = (mState.Y - offsetY) / factor;

            if (GlobalGameData.IsInBounds(gx, gy) && solidArea[gx, gy] == false) //Area not solid
            {
                if (MouseManager.ButtonPressed(MouseButton.LEFT))
                {
                    fireManager.ExplodeFrom(gx, gy, 2);
                }

                if (MouseManager.ButtonDown(MouseButton.RIGHT))
                {
                    tileObjectManager.CreateBomb(gx, gy);
                }
            }
            /////////////////////////////////////////
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Begin sprite batch with nearest neighbor interpolation state enabled (no filtering, stops muddy pixels)
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            aesthetics.Draw(spriteBatch, gameTime);
            tileObjectManager.Draw(spriteBatch, gameTime);

            ///////////////REPLACE LATER/////////////
            fireManager.Draw(spriteBatch, gameTime);
            /////////////////////////////////////////

            for (int i = 0; i < playerCount; ++i)
            {
                players[i].Draw(spriteBatch, gameTime);
            }

            spriteBatch.End();
        }
    }
}
