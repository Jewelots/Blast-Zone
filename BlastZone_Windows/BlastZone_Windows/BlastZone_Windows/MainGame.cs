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

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //Set window size
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

            //Create the render target to the level size
            //Initialise it with Bgr565 (no need for alpha) and no mipmap or depth buffer
            levelRenderTarget = new RenderTarget2D(GraphicsDevice, GlobalGameData.levelSizeX, GlobalGameData.levelSizeY, false, SurfaceFormat.Bgr565, DepthFormat.None);

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
            rectFillTex = Content.Load<Texture2D>("1px");
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

            level.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Render the level to a rendertarget:
            GraphicsDevice.SetRenderTarget(levelRenderTarget); //Set the render target to the level's render target
            GraphicsDevice.Clear(Color.Black); //Clear it
            level.Draw(spriteBatch, gameTime); //Draw the level
            GraphicsDevice.SetRenderTarget(null); //Revert the target to default to continue drawing

            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            
            //Draw the background
            spriteBatch.Draw(gameBackground, new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight), Color.White);

            const int borderSize = 3;

            //Draw the borders (White and Black) around the level (aesthetics)
            spriteBatch.Draw(rectFillTex, new Rectangle((int)(GlobalGameData.windowWidth / 2 - borderSize * 2 - GlobalGameData.levelSizeX / 2), (int)(GlobalGameData.windowHeight / 2 - borderSize * 2 - GlobalGameData.levelSizeY / 2), GlobalGameData.levelSizeX + borderSize * 4, GlobalGameData.levelSizeY + borderSize * 4), Color.White);
            spriteBatch.Draw(rectFillTex, new Rectangle((int)(GlobalGameData.windowWidth / 2 - borderSize     - GlobalGameData.levelSizeX / 2), (int)(GlobalGameData.windowHeight / 2 - borderSize     - GlobalGameData.levelSizeY / 2), GlobalGameData.levelSizeX + borderSize * 2, GlobalGameData.levelSizeY + borderSize * 2), Color.Black);

            //Draw the level itself
            spriteBatch.Draw(levelRenderTarget, new Vector2(GlobalGameData.windowWidth / 2, GlobalGameData.windowHeight / 2), null, Color.White, 0f, new Vector2(GlobalGameData.levelSizeX / 2, GlobalGameData.levelSizeY / 2), 1f, SpriteEffects.None, 1f);
            
            //Draw the score
            scoreRenderer.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
