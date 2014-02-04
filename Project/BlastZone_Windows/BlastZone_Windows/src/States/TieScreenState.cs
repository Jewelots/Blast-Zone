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
    class TieScreenState : GameState
    {
        SpriteFont tieTextFont;
        TiledTexture bgtex;

        //Player count and player input types to pass back to game state
        int playerCount;
        int[] playerInputTypes;

        Song tieSong;

        public void SetLevelData(int playerCount, int p1, int p2, int p3, int p4)
        {
            this.playerCount = playerCount;
            playerInputTypes[0] = p1;
            playerInputTypes[1] = p2;
            playerInputTypes[2] = p3;
            playerInputTypes[3] = p4;
        }

        public override void Enter()
        {
            MediaPlayer.Play(tieSong);
            MediaPlayer.IsRepeating = true;
        }

        public override void Exit()
        {
        }

        public TieScreenState(GameStateManager gameStateManager)
            : base(gameStateManager)
        {
            bgtex = new TiledTexture(new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight));

            playerInputTypes = new int[4];
        }

        public override void LoadContent(ContentManager Content)
        {
            bgtex.SetTexture(Content.Load<Texture2D>("Images/Menu/bg"));
            tieTextFont = Content.Load<SpriteFont>("Fonts/Badaboom");

            tieSong = Content.Load<Song>("Music/tie");
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
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            bgtex.Draw(spriteBatch);

            Vector2 textPos = new Vector2(GlobalGameData.windowWidth / 2, GlobalGameData.windowHeight / 2);

            spriteBatch.Begin();
            DrawTextExtension.DrawTextOutline(spriteBatch, tieTextFont, "It's a tie! So close!", Color.Black, Color.White, textPos, 3f, HorizontalAlign.AlignCenter, VerticalAlign.AlignCenter);
            spriteBatch.End();
        }
    }
}
