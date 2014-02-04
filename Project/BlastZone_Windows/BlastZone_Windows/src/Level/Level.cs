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

        /// <summary>
        /// The aesthetics (non-interactable level tiles)
        /// </summary>
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
        SoundEffectInstance[] playerDeathSoundInstance = new SoundEffectInstance[4]; //Array of soundEffectInstances so garbage collection won't collect them

        bool gameOver;

        /// <summary>
        /// Initialise a level
        /// </summary>
        /// <param name="onWin">Function to call when a player wins (parameter is winning player index)</param>
        /// <param name="onTie">Function to call when there's a tie</param>
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

            //Initialise players to the gridnodemap, with their index, and the bomb create function
            for (int i = 0; i < 4; ++i)
            {
                players[i] = new Player(gridNodeMap, i, tileObjectManager.CreateBomb);
            }

            playerInputControllers = new PlayerInputController[4];

            //Initialise player input controllers to the players
            for (int i = 0; i < 4; ++i)
            {
                playerInputControllers[i] = new PlayerInputController(players[i]);
            }

            floatingAnimationManager = new FloatingAnimationManager();
        }
        
        /// <summary>
        /// Stop player movement sounds
        /// </summary>
        public void StopPlayerSounds()
        {
            for (int i = 0; i < playerCount; ++i)
            {
                players[i].StopMove();
            }
        }

        /// <summary>
        /// Reset the level
        /// </summary>
        /// <param name="playerCount">Number of players</param>
        /// <param name="p1ControlType">Control type index for player 1</param>
        /// <param name="p2ControlType">Control type index for player 2</param>
        /// <param name="p3ControlType">Control type index for player 3</param>
        /// <param name="p4ControlType">Control type index for player 4</param>
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
                playerDeathSoundInstance[i] = playerDeathSound.CreateInstance();
                playerDeathSoundInstance[i].Volume = GlobalGameData.SFXVolume;
            }

            for (int i = 0; i < 4; ++i)
            {
                players[i].LoadContent(Content);
            }

            fireManager.SetSolidArea(solidArea);
        }

        /// <summary>
        /// Set keyboard types for player index based on current keyboard player count
        /// </summary>
        /// <param name="playerIndex">Index of player</param>
        /// <param name="currentKeyboards">Reference to number of keyboards</param>
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

        /// <summary>
        /// Set all player control identifiers
        /// </summary>
        /// <param name="playerCount">Number of players</param>
        /// <param name="p1">Control type index for player 1</param>
        /// <param name="p2">Control type index for player 2</param>
        /// <param name="p3">Control type index for player 3</param>
        /// <param name="p4">Control type index for player 4</param>
        public void SetPlayerControlIdentifiers(int playerCount, int p1, int p2, int p3, int p4)
        {
            int currentKeyboards = 0;

            if (p1 == -1) //Keyboard
            {
                InitialisePlayerToKeyboard(0, ref currentKeyboards);
            }
            else //Gamepad
            {
                InitialisePlayerToJoystick(0, p1);
            }

            if (playerCount < 2) return;

            if (p2 == -1) //Keyboard
            { 
                InitialisePlayerToKeyboard(1, ref currentKeyboards);
            }
            else //Gamepad
            {
                InitialisePlayerToJoystick(1, p2);
            }

            if (playerCount < 3) return;

            if (p3 == -1) //Keyboard
            {
                InitialisePlayerToKeyboard(2, ref currentKeyboards);
            }
            else //Gamepad
            {
                InitialisePlayerToJoystick(2, p3);
            }

            if (playerCount < 4) return;

            if (p4 == -1) //Keyboard
            {
                InitialisePlayerToKeyboard(3, ref currentKeyboards);
            }
            else //Gamepad
            {
                InitialisePlayerToJoystick(3, p4);
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

                    //If the tile is on fire that the player is standing on
                    if (fireManager.IsOnFire(gx, gy))
                    {
                        //Kill the player
                        players[i].StopMove();
                        players[i].IsDead = true;

                        //Play the player death sound
                        playerDeathSoundInstance[i].Play();

                        //Create a death animation effect
                        AnimatedSprite newAnim = playerDeathAnimation;
                        newAnim.SetTexture("player" + (i + 1) + "Death");

                        //Add it to the floating animation manager
                        floatingAnimationManager.Add(newAnim, players[i].GetPosition());
                    }
                        
                    //Get item player standing on
                    TileObject obj = tileObjectManager.NonSolidObjectAt(gx, gy);

                    //If object exists
                    if (obj != null)
                    {
                        //Collide with it
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

        /// <summary>
        /// Check what game end state it is and move to corresponding screen
        /// </summary>
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

        /// <summary>
        /// Get the max bombs for a player
        /// </summary>
        /// <param name="playerIndex">Index of player to get max bombs from</param>
        /// <returns>Max bombs for player</returns>
        public int getMaxBombs(int playerIndex)
        {
            return players[playerIndex].GetMaxBombs();
        }

        /// <summary>
        /// Get the player count
        /// </summary>
        /// <returns>Player count</returns>
        public int GetPlayerCount()
        {
            return playerCount;
        }
    }
}
