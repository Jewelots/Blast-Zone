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
using SpritesheetAnimation;

namespace BlastZone_Windows.States
{
    class WinScreenState : GameState
    {
        AnimatedSprite playerWinAnimation;
        SpriteFont winTextFont;
        TiledTexture bgtex;

        //Player count and player input types to pass back to game state
        int playerCount, p1, p2, p3, p4;

        int winningPlayerIndex;

        public void SetLevelData(int playerCount, int p1, int p2, int p3, int p4)
        {
            this.playerCount = playerCount;
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
        }

        public void SetWinData(int winningPlayerIndex)
        {
            this.winningPlayerIndex = winningPlayerIndex;

            playerWinAnimation.SetTexture("player" + (winningPlayerIndex + 1) + "Win");
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public WinScreenState(GameStateManager gameStateManager)
            : base(gameStateManager)
        {
            bgtex = new TiledTexture(new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight));
        }

        public override void LoadContent(ContentManager Content)
        {
            bgtex.SetTexture(Content.Load<Texture2D>("Images/Menu/bg"));
            winTextFont = Content.Load<SpriteFont>("Fonts/Badaboom");

            AnimationSheet winSheet = new AnimationSheet();
            winSheet.Load(Content, "Spritesheets\\winsprite");

            playerWinAnimation = new AnimatedSprite(winSheet, "Win");
            playerWinAnimation.position = new Vector2(GlobalGameData.windowWidth / 2, GlobalGameData.windowHeight / 2);
            playerWinAnimation.frameRate = 2;
        }

        public override void Update(GameTime gameTime)
        {
            //Check if quit
            bool gamePadPressedBack = false;
            bool gamePadPressedGo = false;
            for (int i = 0; i < 4; ++i)
            {
                GamePadState gps = GamePad.GetState((PlayerIndex)i);

                if (!gps.IsConnected) continue;

                if (gps.IsButtonDown(Buttons.Back))
                {
                    gamePadPressedBack = true;
                }

                if (gps.IsButtonDown(Buttons.Start) || gps.IsButtonDown(Buttons.A))
                {
                    gamePadPressedGo = true;
                }
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || gamePadPressedBack)
            {
                manager.SwapStateWithTransition(StateType.MENU);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) || Keyboard.GetState().IsKeyDown(Keys.Enter) || gamePadPressedGo)
            {
                GameplayState gps = manager.GetState(StateType.GAME) as GameplayState;
                gps.SetLevelData(playerCount, p1, p2, p3, p4);

                manager.SwapStateWithTransition(StateType.GAME);
            }

            bgtex.ShiftOffset(new Vector2(50f * (float)gameTime.ElapsedGameTime.TotalSeconds, 50f * (float)gameTime.ElapsedGameTime.TotalSeconds));

            playerWinAnimation.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            bgtex.Draw(spriteBatch);

            Vector2 textPos = new Vector2(GlobalGameData.windowWidth / 2, GlobalGameData.windowHeight / 2 + 200);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            playerWinAnimation.Draw(spriteBatch, 15);
            //spriteBatch.Draw(playerWinImage, new Vector2(GlobalGameData.windowWidth / 2 - playerWinImage.Width / 2, GlobalGameData.windowHeight / 2 - playerWinImage.Height / 2), Color.White);
            DrawTextExtension.DrawTextOutline(spriteBatch, winTextFont, "Player " + (winningPlayerIndex + 1) + " wins!", Color.Black, Color.White, textPos, 3f, HorizontalAlign.AlignCenter);
            spriteBatch.End();
        }
    }
}
