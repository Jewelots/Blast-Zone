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

namespace BlastZone_Windows.States
{
    class GameplayState : GameState
    {
        Level.Level level;

        /// <summary>
        /// Texture to render the background
        /// </summary>
        Texture2D gameBackground;

        /// <summary>
        /// Texture to render the borders
        /// </summary>
        Texture2D rectFillTex;
        ScoreRenderer scoreRenderer;

        /// <summary>
        /// Render target to render the level to for later offset
        /// </summary>
        RenderTarget2D levelRenderTarget;

        /// <summary>
        /// Stores graphics device
        /// </summary>
        GraphicsDevice graphicsDevice;

        int playerCount = 1;
        int[] playerInputTypes;

        double startTime;

        public override void Enter()
        {
            startTime = -1;

            level.Reset(playerCount, playerInputTypes[0], playerInputTypes[1], playerInputTypes[2], playerInputTypes[3]);
        }

        public override void Exit()
        {
        }

        public GameplayState(GameStateManager gameStateManager, GraphicsDevice graphicsDevice)
            : base(gameStateManager)
        {
            level = new Level.Level(MoveToWinState, MoveToTieState);
            scoreRenderer = new ScoreRenderer();

            //Create the render target to the level size
            //Initialise it with Bgr565 (no need for alpha) and no mipmap or depth buffer
            levelRenderTarget = new RenderTarget2D(graphicsDevice, GlobalGameData.levelSizeX, GlobalGameData.levelSizeY, false, SurfaceFormat.Bgr565, DepthFormat.None);

            this.graphicsDevice = graphicsDevice;

            playerInputTypes = new int[4];
        }

        void MoveToWinState(int winningPlayerIndex)
        {
            scoreRenderer.AddScore(winningPlayerIndex);

            WinScreenState wss = manager.GetState(StateType.WINSCREEN) as WinScreenState;
            wss.SetLevelData(playerCount, playerInputTypes[0], playerInputTypes[1], playerInputTypes[2], playerInputTypes[3]);
            wss.SetWinData(winningPlayerIndex);

            manager.SwapStateWithTransition(StateType.WINSCREEN);
        }

        void MoveToTieState()
        {
            TieScreenState tss = manager.GetState(StateType.TIESCREEN) as TieScreenState;
            tss.SetLevelData(playerCount, playerInputTypes[0], playerInputTypes[1], playerInputTypes[2], playerInputTypes[3]);

            manager.SwapStateWithTransition(StateType.TIESCREEN);
        }

        public override void LoadContent(ContentManager Content)
        {
            level.LoadContent(Content);
            scoreRenderer.LoadContent(Content);

            gameBackground = Content.Load<Texture2D>("Images/Game/game_background");
            rectFillTex = Content.Load<Texture2D>("Images/1px");
        }

        public override void Update(GameTime gameTime)
        {
            if (startTime == -1)
            {
                startTime = gameTime.TotalGameTime.TotalMilliseconds;
            }

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

            level.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Render the level to a rendertarget:
            graphicsDevice.SetRenderTarget(levelRenderTarget); //Set the render target to the level's render target
            graphicsDevice.Clear(Color.Black); //Clear it
            level.Draw(spriteBatch, gameTime); //Draw the level
            graphicsDevice.SetRenderTarget(null); //Revert the target to default to continue drawing

            spriteBatch.Begin();

            //Draw the background
            spriteBatch.Draw(gameBackground, new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight), Color.White);

            const int borderSize = 3;

            //Draw the borders (White and Black) around the level (aesthetics)
            spriteBatch.Draw(rectFillTex, new Rectangle((int)(GlobalGameData.windowWidth / 2 - borderSize * 2 - GlobalGameData.levelSizeX / 2), (int)(GlobalGameData.windowHeight / 2 - borderSize * 2 - GlobalGameData.levelSizeY / 2), GlobalGameData.levelSizeX + borderSize * 4, GlobalGameData.levelSizeY + borderSize * 4), Color.White);
            spriteBatch.Draw(rectFillTex, new Rectangle((int)(GlobalGameData.windowWidth / 2 - borderSize - GlobalGameData.levelSizeX / 2), (int)(GlobalGameData.windowHeight / 2 - borderSize - GlobalGameData.levelSizeY / 2), GlobalGameData.levelSizeX + borderSize * 2, GlobalGameData.levelSizeY + borderSize * 2), Color.Black);

            //Draw the level itself
            spriteBatch.Draw(levelRenderTarget, new Vector2(GlobalGameData.windowWidth / 2, GlobalGameData.windowHeight / 2), null, Color.White, 0f, new Vector2(GlobalGameData.levelSizeX / 2, GlobalGameData.levelSizeY / 2), 1f, SpriteEffects.None, 1f);

            //Draw the score
            scoreRenderer.Draw(spriteBatch, level.GetPlayerCount());

            spriteBatch.End();
        }

        public void SetLevelData(int playerCount, int p1ControlType, int p2ControlType, int p3ControlType, int p4ControlType)
        {
            this.playerCount = playerCount;
            this.playerInputTypes[0] = p1ControlType;
            this.playerInputTypes[1] = p2ControlType;
            this.playerInputTypes[2] = p3ControlType;
            this.playerInputTypes[3] = p4ControlType;
        }
    }
}
