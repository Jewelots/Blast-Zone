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

        public override void Enter()
        {
            playerCount = 1;
        }

        public override void Exit()
        {
        }

        public LobbyState(GameStateManager gameStateManager)
            : base(gameStateManager)
        {
            bgtex = new TiledTexture(new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight));
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
            bgtex.ShiftOffset(new Vector2(50f * (float)gameTime.ElapsedGameTime.TotalSeconds, 50f * (float)gameTime.ElapsedGameTime.TotalSeconds));
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                GameplayState gps = manager.GetState(StateType.GAME) as GameplayState;
                gps.SetLevelData(2, -1, -1, 0, 1); //Temp, makes p1 and p2 use keyboard

                manager.SwapStateWithTransition(StateType.GAME);
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

            int spacing = 20;
            int iconVerticalPos = GlobalGameData.windowHeight / 2 - 128;

            float icon2Offset = -256 - spacing / 2;
            float icon3Offset = spacing / 2;
            DrawController(spriteBatch, new Vector2(GlobalGameData.windowWidth / 2 + icon2Offset - 256 - spacing, iconVerticalPos), 0, true);
            DrawController(spriteBatch, new Vector2(GlobalGameData.windowWidth / 2 + icon2Offset, iconVerticalPos), 0, false);
            DrawController(spriteBatch, new Vector2(GlobalGameData.windowWidth / 2 + icon3Offset, iconVerticalPos), 1, true);
            DrawController(spriteBatch, new Vector2(GlobalGameData.windowWidth / 2 + icon3Offset + 256 + spacing, iconVerticalPos), -1, false);

            Vector2 playerCountPos = new Vector2(GlobalGameData.windowWidth / 2, 75);
            Vector2 startGamePos = new Vector2(GlobalGameData.windowWidth / 2, GlobalGameData.windowHeight - 75);
            Vector2 resetInfoPos = new Vector2(GlobalGameData.windowWidth / 2, 125);
            Vector2 buttonInfoPos = new Vector2(GlobalGameData.windowWidth / 2, GlobalGameData.windowHeight - 200);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFont, "Player Count: " + playerCount, Color.Black, Color.White, playerCountPos, 3f, HorizontalAlign.AlignCenter, VerticalAlign.AlignCenter);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFontSmall, "Press R (Keyboard) or Back (GamePad) to reset", Color.Black, Color.White, resetInfoPos, 3f, HorizontalAlign.AlignCenter);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFontSmall, "Press space to add one keyboard", Color.Black, Color.White, buttonInfoPos, 3f, HorizontalAlign.AlignCenter);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFontSmall, "Press enter to add second keyboard", Color.Black, Color.White, buttonInfoPos + new Vector2(0, 32), 3f, HorizontalAlign.AlignCenter);
            DrawTextExtension.DrawTextOutline(spriteBatch, lobbyFont, "Press Bomb on all controllers to begin", Color.Black, Color.White, startGamePos, 3f, HorizontalAlign.AlignCenter, VerticalAlign.AlignCenter);
            spriteBatch.End();
        }
    }
}
