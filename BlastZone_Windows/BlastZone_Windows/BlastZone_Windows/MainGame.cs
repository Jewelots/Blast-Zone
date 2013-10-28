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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Level level;

        Texture2D gameBackground;
        ScoreRenderer scoreRenderer;

        RenderTarget2D levelRenderTarget;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = GlobalGameData.windowWidth;
            graphics.PreferredBackBufferHeight = GlobalGameData.windowHeight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            level = new Level();
            scoreRenderer = new ScoreRenderer();

            levelRenderTarget = new RenderTarget2D(GraphicsDevice, GlobalGameData.levelSizeX, GlobalGameData.levelSizeY);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            level.LoadContent(Content);
            scoreRenderer.LoadContent(Content);

            gameBackground = Content.Load<Texture2D>("game_background");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            scoreRenderer.SetScore(0, (int)gameTime.TotalGameTime.TotalMilliseconds / 500);
            scoreRenderer.SetScore(1, (int)gameTime.TotalGameTime.TotalMilliseconds / 1000);
            scoreRenderer.SetScore(2, (int)gameTime.TotalGameTime.TotalMilliseconds / 1500);
            scoreRenderer.SetScore(3, (int)gameTime.TotalGameTime.TotalMilliseconds / 2000);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(levelRenderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            level.Draw(spriteBatch, gameTime);
            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(gameBackground, new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight), Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.Draw(levelRenderTarget, new Vector2(GlobalGameData.windowWidth / 2, GlobalGameData.windowHeight / 2), null, Color.White, 0f, new Vector2(GlobalGameData.levelSizeX / 2, GlobalGameData.levelSizeY / 2), 1f, SpriteEffects.None, 1f);
            spriteBatch.End();

            scoreRenderer.Draw(spriteBatch);

            base.Draw(gameTime);
        }
    }
}
