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

using BlastZone_Windows.Drawing;

namespace BlastZone_Windows.States
{
    class LobbyState : GameState
    {
        TiledTexture bgtex;
        SpriteFont lobbyFont;
        SpriteFont lobbyFontSmall;

        Texture2D ControllerBG;
        Texture2D ControllerBGReady;
        Texture2D ControllerBorder;
        Texture2D ControllerKeyboard;
        Texture2D ControllerGamepad;

        int playerCount;

        Vector2[] iconPos;
        int[] controller;
        bool[] ready;

        int keyboardCount;

        KeyboardState lastKeyboardState;
        GamePadState[] lastGamepadState;

        public override void Enter()
        {
            playerCount = 0;
            for (int i = 0; i < 4; ++i)
            {
                ready[i] = false;
            }

            keyboardCount = 0;
        }

        public override void Exit()
        {
        }

        public LobbyState(GameStateManager gameStateManager)
            : base(gameStateManager)
        {
            bgtex = new TiledTexture(new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight));

            iconPos = new Vector2[4];
            controller = new int[4];
            ready = new bool[4];

            lastGamepadState = new GamePadState[4];

            int spacing = 20;
            int iconVerticalPos = GlobalGameData.windowHeight / 2 - 128;
            float icon2Offset = -256 - spacing / 2;
            float icon3Offset = spacing / 2;
            iconPos[0] = new Vector2(GlobalGameData.windowWidth / 2 + icon2Offset - 256 - spacing, iconVerticalPos);
            iconPos[1] = new Vector2(GlobalGameData.windowWidth / 2 + icon2Offset, iconVerticalPos);
            iconPos[2] = new Vector2(GlobalGameData.windowWidth / 2 + icon3Offset, iconVerticalPos);
            iconPos[3] = new Vector2(GlobalGameData.windowWidth / 2 + icon3Offset + 256 + spacing, iconVerticalPos);
        }

        public override void LoadContent(ContentManager Content)
        {
            bgtex.SetTexture(Content.Load<Texture2D>("Images/Menu/bg"));

            ControllerBG = Content.Load<Texture2D>("Images/Lobby/ControllerBG");
            ControllerBGReady = Content.Load<Texture2D>("Images/Lobby/ControllerBGReady");
            ControllerBorder = Content.Load<Texture2D>("Images/Lobby/ControllerBorder");
            ControllerKeyboard = Content.Load<Texture2D>("Images/Lobby/ControllerKeyboard");
            ControllerGamepad = Content.Load<Texture2D>("Images/Lobby/ControllerGamepad");

            lobbyFont = Content.Load<SpriteFont>("Fonts/Badaboom");
            lobbyFontSmall = Content.Load<SpriteFont>("Fonts/BadaboomSmall");
        }

        public override void Update(GameTime gameTime)
        {
            //Check if quit
            bool gamePadPressedBack = false;
            for (int i = 0; i < 4; ++i)
            {
                GamePadState gps = GamePad.GetState((PlayerIndex)i);

                if (!gps.IsConnected) continue;

                if (gps.IsButtonDown(Buttons.Back))
                {
                    gamePadPressedBack = true;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || gamePadPressedBack)
            {
                manager.SwapStateWithTransition(StateType.MENU);
            }

            bgtex.ShiftOffset(new Vector2(50f * (float)gameTime.ElapsedGameTime.TotalSeconds, 50f * (float)gameTime.ElapsedGameTime.TotalSeconds));

            CheckForPlayerInput();

            //Check if everyone's ready and start game if so
            int totalReady = 0;
            for (int i = 0; i < playerCount; ++i)
            {
                if (ready[i])
                {
                    totalReady += 1;
                }
            }

            if (totalReady >= 2 && totalReady == playerCount)
            {
                GameplayState gps = manager.GetState(StateType.GAME) as GameplayState;
                gps.SetLevelData(playerCount, controller[0], controller[1], controller[2], controller[3]);

                manager.SwapStateWithTransition(StateType.GAME);
            }

            //Set last states
            lastKeyboardState = Keyboard.GetState();

            for (int i = 0; i < 4; ++i)
            {
                GamePadState gps = GamePad.GetState((PlayerIndex)i);
                if (!gps.IsConnected) continue;

                lastGamepadState[i] = gps;
            }
        }

        void DrawController(SpriteBatch spriteBatch, Vector2 controlPos, int controllerType, bool ready)
        {
            if (ready)
            {
                spriteBatch.Draw(ControllerBGReady, controlPos, Color.White);
            }
            else
            {
                spriteBatch.Draw(ControllerBG, controlPos, Color.White);
            }

            if (controllerType == 0) //Keyboard
            {
                spriteBatch.Draw(ControllerKeyboard, controlPos, Color.White);
            }
            else if (controllerType == 1) //Gamepad
            {
                spriteBatch.Draw(ControllerGamepad, controlPos, Color.White);
            }

            spriteBatch.Draw(ControllerBorder, controlPos, Color.White);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            bgtex.Draw(spriteBatch);

            spriteBatch.Begin();

            for (int i = 0; i < 4; ++i)
            {
                int controllerType;

                if (i > playerCount - 1)
                {
                    controllerType = -1; //No controller
                }
                else
                {
                    if (controller[i] == -1)
                    {
                        controllerType = 0; //Keyboard
                    }
                    else
                    {
                        controllerType = 1; //Gamepad
                    }
                }

                DrawController(spriteBatch, iconPos[i], controllerType, ready[i]);
            }

            Vector2 playerCountPos = new Vector2(GlobalGameData.windowWidth / 2, 75);
            Vector2 resetInfoPos = new Vector2(GlobalGameData.windowWidth / 2, 125);
            Vector2 buttonInfoPos = new Vector2(GlobalGameData.windowWidth / 2, GlobalGameData.windowHeight - 210);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFont, "Player Count: " + playerCount, Color.Black, Color.White, playerCountPos, 3f, HorizontalAlign.AlignCenter, VerticalAlign.AlignCenter);

            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFontSmall, "Press R (Keyboard) or B (GamePad) to reset", Color.Black, Color.White, resetInfoPos, 3f, HorizontalAlign.AlignCenter);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFontSmall, "Press space to add one keyboard", Color.Black, Color.White, buttonInfoPos, 3f, HorizontalAlign.AlignCenter);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFontSmall, "Press enter to add second keyboard", Color.Black, Color.White, buttonInfoPos + new Vector2(0, 50), 3f, HorizontalAlign.AlignCenter);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFontSmall, "Press Enter/Space, or the A button, to ready!", Color.Black, Color.White, buttonInfoPos + new Vector2(0, 100), 3f, HorizontalAlign.AlignCenter);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFontSmall, "Game will begin when all players ready (minimum two)!", Color.Black, Color.White, buttonInfoPos + new Vector2(0, 150), 3f, HorizontalAlign.AlignCenter);
            spriteBatch.End();
        }

        void CheckForPlayerInput()
        {
            KeyboardState k = Keyboard.GetState();

            //Reset if any player has pressed B, or key R is down
            for (int i = 0; i < 4; ++i)
            {
                GamePadState gps = GamePad.GetState((PlayerIndex)i);
                if (!gps.IsConnected) continue;

                if (gps.IsButtonDown(Buttons.B))
                {
                    Enter(); //Reset
                }
            }

            if (k.IsKeyDown(Keys.R) && lastKeyboardState.IsKeyUp(Keys.R))
            {
                Enter(); //Reset
            }

            if (playerCount > 0)
            {
                HandleReadyInput(k);
            }

            if (playerCount == 4) return; //Break out if 4 players reached

            //If Space is pressed and no keyboards; add one
            //If Enter pressed and one keyboard; add a second one
            if ((k.IsKeyDown(Keys.Space) && keyboardCount == 0) || (k.IsKeyDown(Keys.Enter) && keyboardCount == 1))
            {
                controller[playerCount] = -1;
                playerCount += 1;
                keyboardCount += 1;
            }

            //Check all 4 gamepads
            for (int i = 0; i < 4; ++i)
            {
                GamePadState gps = GamePad.GetState((PlayerIndex)i);
                if (!gps.IsConnected) continue;

                //Controller A button is pressed
                if (gps.IsButtonDown(Buttons.A))
                {
                    bool isFound = false;

                    //Check if controller unused
                    for (int p = 0; p < playerCount; ++p)
                    {
                        if (controller[p] == i)
                        {
                            isFound = true;
                        }
                    }

                    //If controller is found to already be a player, skip it
                    if (isFound) continue;

                    controller[playerCount] = i;
                    playerCount += 1;
                }
            }
        }

        void HandleReadyInput(KeyboardState k)
        {
            int foundKeyboards = 0;
            int[] keyboardIndices = new int[2];

            //Loop through players
            for (int i = 0; i < playerCount; ++i)
            {
                //Player is using a keyboard
                if (controller[i] == -1)
                {
                    keyboardIndices[foundKeyboards] = i;
                    foundKeyboards += 1;
                }
                else //Player is using a gamepad
                {
                    //Get gamepad state for that player's controller
                    GamePadState gps = GamePad.GetState((PlayerIndex)controller[i]);

                    //Check if A just pressed
                    if (gps.IsButtonDown(Buttons.A) && lastGamepadState[controller[i]].IsButtonUp(Buttons.A))
                    {
                        //Ready controller
                        ready[i] = !ready[i];
                    }
                }
            }

            if (foundKeyboards > 0)
            {
                if (k.IsKeyDown(Keys.Space) && lastKeyboardState.IsKeyUp(Keys.Space))
                {
                    ready[keyboardIndices[0]] = !ready[keyboardIndices[0]];
                }
            }

            if (foundKeyboards > 1)
            {
                if (k.IsKeyDown(Keys.Enter) && lastKeyboardState.IsKeyUp(Keys.Enter))
                {
                    ready[keyboardIndices[1]] = !ready[keyboardIndices[1]];
                }
            }
        }
    }
}
