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
using SpritesheetAnimation;

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
        public FloatingAnimationManager floatingAnimationManager;

        LevelAesthetics aesthetics;

        AnimatedSprite playerDeathAnimation;

        GridNodeMap gridNodeMap;

        Player[] players;
        PlayerInputController[] playerInputControllers;

        Dictionary<int, int> playerToController; //-1 = keyboard, 0-3 is gamepads

        int playerCount = 1;

        Action<int> onWin;
        Action onTie;

        EventTimer checkWinState;

        SoundEffect playerDeathSound;

        bool gameOver;

        public Level(Action<int> onWin, Action onTie)
        {
            this.onWin = onWin;
            this.onTie = onTie;

            aesthetics = new LevelAesthetics();

            tileObjectManager = new TileObjectManager(this);
            fireManager = new FireManager(tileObjectManager);

            solidArea = new bool[GlobalGameData.gridSizeX, GlobalGameData.gridSizeY];

            gridNodeMap = new GridNodeMap();

            playerToController = new Dictionary<int, int>();

            players = new Player[4];

            for (int i = 0; i < 4; ++i)
            {
                players[i] = new Player(gridNodeMap, i, tileObjectManager.CreateBomb);
            }

            playerInputControllers = new PlayerInputController[4];
            for (int i = 0; i < 4; ++i)
            {
                playerInputControllers[i] = new PlayerInputController(players[i]);
            }

            floatingAnimationManager = new FloatingAnimationManager();
        }

        public void Reset(int playerCount, int p1ControlType, int p2ControlType, int p3ControlType, int p4ControlType)
        {
            this.playerCount = playerCount;

            tileObjectManager.Reset();
            fireManager.Reset();

            //Reset players to positions
            players[0].Reset(1, 1);
            players[1].Reset(GlobalGameData.gridSizeX - 2, 1);
            players[2].Reset(1, GlobalGameData.gridSizeY - 2);
            players[3].Reset(GlobalGameData.gridSizeX - 2, GlobalGameData.gridSizeY - 2);

            SetPlayerControlIdentifiers(playerCount, p1ControlType, p2ControlType, p3ControlType, p4ControlType);

            gameOver = false;

            checkWinState = new EventTimer(0, 1, true);
            checkWinState.OnEnd += checkIfWinOrTie;
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

            AnimationSheet playerDeathAnimationSheet = new AnimationSheet();
            playerDeathAnimationSheet.Load(Content, "Spritesheets\\deathsprite");
            playerDeathAnimation = new AnimatedSprite(playerDeathAnimationSheet, "Death");

            playerDeathSound = Content.Load<SoundEffect>("SFX/death");

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

        void SetKeyType1(PlayerInputController controller)
        {
            controller.SetKeyIdentifiers(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space);
        }

        void SetKeyType2(PlayerInputController controller)
        {
            controller.SetKeyIdentifiers(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.Enter);
        }

        void InitialisePlayerToJoystick(int playerIndex, int joystickIndex)
        {
            playerToController[playerIndex] = joystickIndex;
            playerInputControllers[playerIndex].SetJoyIdentifiers();
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
                InitialisePlayerToJoystick(0, p1);
            }

            if (playerCount < 2) return;

            if (p2 == -1)
            {
                InitialisePlayerToKeyboard(1, ref currentKeyboards);
            }
            else
            {
                InitialisePlayerToJoystick(1, p2);
            }

            if (playerCount < 3) return;

            if (p3 == -1)
            {
                InitialisePlayerToKeyboard(2, ref currentKeyboards);
            }
            else
            {
                InitialisePlayerToJoystick(2, p3);
            }

            if (playerCount < 4) return;

            if (p4 == -1)
            {
                InitialisePlayerToKeyboard(3, ref currentKeyboards);
            }
            else
            {
                InitialisePlayerToJoystick(4, p4);
            }
        }

        public void Update(GameTime gameTime)
        {
            checkWinState.Update(gameTime);

            tileObjectManager.Update(gameTime);

            fireManager.Update(gameTime);

            //Set what's solid for collision
            gridNodeMap.SetSolid(GetSolid());

            int playersAlive = 0;
            for (int i = 0; i < playerCount; ++i)
            {
                //Only update if player not dead
                if (players[i].IsDead == false)
                {
                    //Update input
                    if (playerToController[i] == -1)
                    {
                        playerInputControllers[i].GetKeyInput(Keyboard.GetState());
                    }
                    else
                    {
                        playerInputControllers[i].GetPadInput(GamePad.GetState((PlayerIndex)playerToController[i]));
                    }

                    //Update players
                    players[i].Update(gameTime);

                    //Check if player is in fire and kill them if so
                    int gx, gy;
                    players[i].GetGridPosition(out gx, out gy);

                    if (fireManager.IsOnFire(gx, gy))
                    {
                        players[i].IsDead = true;

                        AnimatedSprite newAnim = playerDeathAnimation;
                        newAnim.SetTexture("player" + (i + 1) + "Death");

                        SoundEffectInstance playerDeathSoundInstance = playerDeathSound.CreateInstance();
                        playerDeathSoundInstance.Volume = GlobalGameData.SFXVolume;
                        playerDeathSoundInstance.Play();

                        floatingAnimationManager.Add(newAnim, players[i].GetPosition());
                    }
                        
                    //Get item player standing on
                    TileObject obj = tileObjectManager.ObjectAt(gx, gy);

                    if (obj != null)
                    {
                        obj.PlayerCollision(players[i]);
                    }

                    if (players[i].IsDead == false)
                    {
                        //Count how many players are alive
                        playersAlive += 1;
                    }
                }
            }

            //If game not over check if it's won
            if (gameOver == false)
            {
                if (playersAlive < 2)
                {
                    checkWinState.UnPause();
                    gameOver = true;
                }
            }
        }

        private void checkIfWinOrTie()
        {
            int playersAlive = 0;
            for (int i = 0; i < playerCount; ++i)
            {
                if (players[i].IsDead == false)
                {
                    //Count how many players are alive
                    playersAlive += 1;
                }
            }

            //Check if 1 or 0 players remaining and move to win/tie screen
            if (playersAlive == 1)
            {
                //Get the living player index
                int alivePlayerIndex = -1;
                for (int i = 0; i < playerCount; ++i)
                {
                    if (players[i].IsDead == false)
                    {
                        alivePlayerIndex = i;
                        break;
                    }
                }

                onWin(alivePlayerIndex);
            }

            else if (playersAlive == 0)
            {
                onTie();
            }
            else
            {
                //More than one player alive or negative somehow? Go back to how it was. Should never be reached though!
                gameOver = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Begin sprite batch with nearest neighbor interpolation state enabled (no filtering, stops muddy pixels)
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            aesthetics.Draw(spriteBatch, gameTime);
            tileObjectManager.Draw(spriteBatch, gameTime);

            for (int i = 0; i < playerCount; ++i)
            {
                players[i].Draw(spriteBatch, gameTime);
            }

            floatingAnimationManager.Draw(spriteBatch, gameTime);

            spriteBatch.End();
        }

        public int getMaxBombs(int playerIndex)
        {
            return players[playerIndex].GetMaxBombs();
        }

        public int GetPlayerCount()
        {
            return playerCount;
        }
    }
}
