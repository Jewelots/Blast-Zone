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
        int playerCount;
        int[] playerInputTypes;

        int winningPlayerIndex;

        Song winSong;

        public void SetLevelData(int playerCount, int p1, int p2, int p3, int p4)
        {
            this.playerCount = playerCount;
            playerInputTypes[0] = p1;
            playerInputTypes[1] = p2;
            playerInputTypes[2] = p3;
            playerInputTypes[3] = p4;
        }

        public void SetWinData(int winningPlayerIndex)
        {
            this.winningPlayerIndex = winningPlayerIndex;

            playerWinAnimation.SetTexture("player" + (winningPlayerIndex + 1) + "Win");
        }

        public override void Enter()
        {
            MediaPlayer.Play(winSong);
            MediaPlayer.IsRepeating = false;
        }

        public override void Exit()
        {
        }

        public WinScreenState(GameStateManager gameStateManager)
            : base(gameStateManager)
        {
            bgtex = new TiledTexture(new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight));

            playerInputTypes = new int[4];
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

            winSong = Content.Load<Song>("Music/victory");
        }

        public override void Update(GameTime gameTime)
        {
            bool someonePressedBack = false;
            bool someonePressedGo = false;

            //Check if quit or continue
            for (int i = 0; i < playerCount; ++i)
            {
                if (playerInputTypes[i] == -1)
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        someonePressedBack = true;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter) || Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        someonePressedGo = true;
                    }
                }
                else
                {
                    GamePadState gps = GamePad.GetState((PlayerIndex)playerInputTypes[i]);

                    if (!gps.IsConnected) continue;

                    if (gps.IsButtonDown(Buttons.Back))
                    {
                        someonePressedBack = true;
                    }

                    if (gps.IsButtonDown(Buttons.Start) || gps.IsButtonDown(Buttons.A))
                    {
                        someonePressedGo = true;
                    }
                }
            }

            if (someonePressedBack)
            {
                manager.SwapStateWithTransitionMusic(StateType.MENU);
            }

            if (someonePressedGo)
            {
                GameplayState gps = manager.GetState(StateType.GAME) as GameplayState;
                gps.SetLevelData(playerCount, playerInputTypes[0], playerInputTypes[1], playerInputTypes[2], playerInputTypes[3]);

                manager.SwapStateWithTransitionMusic(StateType.GAME);
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
