﻿using System;
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

        Dictionary<int, int> playerToController; //-1 = keyboard, 0-3 is gamepads

        int playerCount = 1;

        public Level()
        {
            aesthetics = new LevelAesthetics();

            tileObjectManager = new TileObjectManager(this);
            fireManager = new FireManager(tileObjectManager);

            solidArea = new bool[GlobalGameData.gridSizeX, GlobalGameData.gridSizeY];

            gridNodeMap = new GridNodeMap();

            playerToController = new Dictionary<int, int>();

            players = new Player[4];

            for (int i = 0; i < 4; ++i)
            {
                players[i] = new Player(gridNodeMap, tileObjectManager.CreateBomb);
            }

            playerInputControllers = new PlayerInputController[4];
            for (int i = 0; i < 4; ++i)
            {
                playerInputControllers[i] = new PlayerInputController(players[i]);
            }
        }

        void SetKeyType1(PlayerInputController controller)
        {
            controller.SetKeyIdentifiers(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space);
        }

        void SetKeyType2(PlayerInputController controller)
        {
            controller.SetKeyIdentifiers(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Enter);
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

            for (int i = 0; i < 4; ++i)
            {
                players[i].LoadContent(Content);
            }

            fireManager.SetSolidArea(solidArea);
        }

        void InitialisePlayerToKeyboard(int playerIndex, ref int currentKeyboards)
        {
            playerToController[playerIndex] = -1;

            if (currentKeyboards == 0)
            {
                SetKeyType1(playerInputControllers[playerIndex]);
            }
            else if (currentKeyboards == 1)
            {
                SetKeyType2(playerInputControllers[playerIndex]);
            }
            else
            {
                throw new Exception("Cannot initialise more than two players with a keyboard!");
            }

            currentKeyboards += 1;
        }

        public void SetPlayerControlIdentifiers(int playerCount, int p1, int p2, int p3, int p4)
        {
            int currentKeyboards = 0;

            if (p1 == -1)
            {
                InitialisePlayerToKeyboard(0, ref currentKeyboards);
            }
            else
            {
                playerToController[0] = p1;
            }

            if (playerCount < 2) return;

            if (p2 == -1)
            {
                InitialisePlayerToKeyboard(1, ref currentKeyboards);
            }
            else
            {
                playerToController[1] = p2;
            }

            if (playerCount < 3) return;

            if (p3 == -1)
            {
                InitialisePlayerToKeyboard(2, ref currentKeyboards);
            }
            else
            {
                playerToController[2] = p3;
            }

            if (playerCount < 4) return;

            if (p4 == -1)
            {
                InitialisePlayerToKeyboard(3, ref currentKeyboards);
            }
            else
            {
                playerToController[4] = p4;
            }
        }

        public void Reset(int playerCount, int p1ControlType, int p2ControlType, int p3ControlType, int p4ControlType)
        {
            this.playerCount = playerCount;

            tileObjectManager.Reset();

            players[0].Reset(2, 1);
            players[1].Reset(GlobalGameData.gridSizeX - 2, 1);
            players[2].Reset(1, GlobalGameData.gridSizeY - 2);
            players[3].Reset(GlobalGameData.gridSizeX - 2, GlobalGameData.gridSizeY - 2);

            SetPlayerControlIdentifiers(playerCount, p1ControlType, p2ControlType, p3ControlType, p4ControlType);
        }

        public void Update(GameTime gameTime)
        {
            tileObjectManager.Update(gameTime);

            fireManager.Update(gameTime);

            gridNodeMap.SetSolid(GetSolid());

            for (int i = 0; i < playerCount; ++i)
            {
                if (playerToController[i] == -1)
                {
                    playerInputControllers[i].GetKeyInput(Keyboard.GetState());
                }
                else
                {
                    playerInputControllers[i].GetPadInput(GamePad.GetState((PlayerIndex)playerToController[i]));
                }
                players[i].Update(gameTime);
            }
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
