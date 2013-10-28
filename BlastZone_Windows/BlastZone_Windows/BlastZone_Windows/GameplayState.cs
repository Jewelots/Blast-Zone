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

namespace BlastZone_Windows
{
    class GameplayState : GameState
    {
        Level level;

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

        double startTime;

        public override void Enter()
        {
            startTime = -1;

            level.Reset();
        }

        public override void Exit()
        {
        }

        public GameplayState(GameStateManager gameStateManager, GraphicsDevice graphicsDevice)
            : base(gameStateManager)
        {
            level = new Level();
            scoreRenderer = new ScoreRenderer();

            //Create the render target to the level size
            //Initialise it with Bgr565 (no need for alpha) and no mipmap or depth buffer
            levelRenderTarget = new RenderTarget2D(graphicsDevice, GlobalGameData.levelSizeX, GlobalGameData.levelSizeY, false, SurfaceFormat.Bgr565, DepthFormat.None);

            this.graphicsDevice = graphicsDevice;
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

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                manager.SwapStateWithTransition(StateType.MENU);
            }

            scoreRenderer.SetScore(0, (int)(gameTime.TotalGameTime.TotalMilliseconds - startTime) / 500);
            scoreRenderer.SetScore(1, (int)(gameTime.TotalGameTime.TotalMilliseconds - startTime) / 1000);
            scoreRenderer.SetScore(2, (int)(gameTime.TotalGameTime.TotalMilliseconds - startTime) / 1500);
            scoreRenderer.SetScore(3, (int)(gameTime.TotalGameTime.TotalMilliseconds - startTime) / 2000);

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
            scoreRenderer.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
