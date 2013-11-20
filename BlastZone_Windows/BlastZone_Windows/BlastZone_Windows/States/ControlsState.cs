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
    class ControlsState : GameState
    {
        Texture2D keyboardImage;
        Texture2D gamepadImage;
        TiledTexture bgtex;

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public ControlsState(GameStateManager gameStateManager)
            : base(gameStateManager)
        {
            bgtex = new TiledTexture(new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight));
        }

        public override void LoadContent(ContentManager Content)
        {
            bgtex.SetTexture(Content.Load<Texture2D>("Images/Menu/bg"));
            keyboardImage = Content.Load<Texture2D>("Images/Controls/KeyboardControls");
            gamepadImage = Content.Load<Texture2D>("Images/Controls/GamepadControls");
        }

        public override void Update(GameTime gameTime)
        {
            //Check if quit
            bool gamePadPressedBack = false;
            bool gamePadPressedA = false;
            for (int i = 0; i < 4; ++i)
            {
                GamePadState gps = GamePad.GetState((PlayerIndex)i);

                if (!gps.IsConnected) continue;

                if (gps.IsButtonDown(Buttons.Back))
                {
                    gamePadPressedBack = true;
                }

                if (gps.IsButtonDown(Buttons.A))
                {
                    gamePadPressedA = true;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.Enter) || gamePadPressedBack || gamePadPressedA)
            {
                manager.SwapStateWithTransition(StateType.MENU);
            }

            bgtex.ShiftOffset(new Vector2(50f * (float)gameTime.ElapsedGameTime.TotalSeconds, 50f * (float)gameTime.ElapsedGameTime.TotalSeconds));
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            bgtex.Draw(spriteBatch);

            spriteBatch.Begin();
            spriteBatch.Draw(keyboardImage, new Vector2(GlobalGameData.windowWidth / 2 - keyboardImage.Width / 2, GlobalGameData.windowHeight / 4 - keyboardImage.Height / 2), Color.White);
            spriteBatch.Draw(gamepadImage, new Vector2(GlobalGameData.windowWidth / 2 - gamepadImage.Width / 2, GlobalGameData.windowHeight - GlobalGameData.windowHeight / 4 - gamepadImage.Height / 2), Color.White);
            spriteBatch.End();
        }
    }
}
