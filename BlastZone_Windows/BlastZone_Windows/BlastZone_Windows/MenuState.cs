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
    class MenuState : GameState
    {
        Texture2D[] Blast = new Texture2D[4];
        Texture2D[] Zone = new Texture2D[4];

        Vector2 blastPos;
        Vector2 zonePos;

        TiledTexture bgtex;
        Texture2D menuTextTex;

        int menuBeginPosY;
        int menuOffset;

        KeyboardState oldState;

        int curSelected;

        public override void Enter()
        {
            curSelected = 0;
        }

        public override void Exit()
        {
        }

        public MenuState(GameStateManager gameStateManager)
            : base(gameStateManager)
        {
            blastPos = new Vector2(GlobalGameData.windowWidth / 2, 20);
            zonePos = blastPos + new Vector2(0, 60);

            bgtex = new TiledTexture(new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight));

            oldState = Keyboard.GetState();

            //menuBeginPosY = 250;
            menuBeginPosY = 300;
            menuOffset = 100;
        }

        public override void LoadContent(ContentManager Content)
        {
            Blast[0] = Content.Load<Texture2D>("Images/Logo/Blast_Shadow");
            Blast[1] = Content.Load<Texture2D>("Images/Logo/Blast_Orange");
            Blast[2] = Content.Load<Texture2D>("Images/Logo/Blast_Yellow");
            Blast[3] = Content.Load<Texture2D>("Images/Logo/Blast_Text");

            Zone[0] = Content.Load<Texture2D>("Images/Logo/Zone_Shadow");
            Zone[1] = Content.Load<Texture2D>("Images/Logo/Zone_Orange");
            Zone[2] = Content.Load<Texture2D>("Images/Logo/Zone_Yellow");
            Zone[3] = Content.Load<Texture2D>("Images/Logo/Zone_Text");

            bgtex.SetTexture(Content.Load<Texture2D>("Images/Menu/bg"));

            menuTextTex = Content.Load<Texture2D>("Images/Menu/text");
        }

        public override void Update(GameTime gameTime)
        {
            bgtex.ShiftOffset(new Vector2(50f * (float)gameTime.ElapsedGameTime.TotalSeconds, 50f * (float)gameTime.ElapsedGameTime.TotalSeconds));

            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
            {
                switch (curSelected)
                {
                    case 0: //Lobby button
                        OnSelectStartGame();
                        break;
                    case 1: //Options button
                        break;
                    case 2: //Quit Button
                        manager.QuitGame();
                        break;
                    default:
                        break;
                }
            }

            if (newState.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down)) curSelected += 1;
            if (newState.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up)) curSelected -= 1;

            curSelected %= 3;
            if (curSelected < 0) curSelected = 2;

            oldState = newState;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            bgtex.Draw(spriteBatch);

            Vector2 newZonePos = zonePos + new Vector2((float)Math.Cos((double)gameTime.TotalGameTime.TotalMilliseconds / 150f) * 2f, (float)Math.Sin((double)gameTime.TotalGameTime.TotalMilliseconds / 200f) * 2f);
            Vector2 newBlastPos = blastPos + new Vector2((float)Math.Sin((double)gameTime.TotalGameTime.TotalMilliseconds / 150f + 0.3f) * 2f, (float)Math.Cos((double)gameTime.TotalGameTime.TotalMilliseconds / 200f + 0.23f) * 2f);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            spriteBatch.Draw(Zone[0], newZonePos, null, Color.White * 0.5f, 0f, new Vector2(30, 0), 5f, SpriteEffects.None, 1f);
            spriteBatch.Draw(Blast[0], newBlastPos, null, Color.White * 0.5f, 0f, new Vector2(30, 0), 5f, SpriteEffects.None, 1f);

            for (int i = 0; i < 3; ++i)
            {
                spriteBatch.Draw(Zone[i + 1], newZonePos, null, Color.White, 0f, new Vector2(30, 0), 5f, SpriteEffects.None, 1f);
                spriteBatch.Draw(Blast[i + 1], newBlastPos, null, Color.White, 0f, new Vector2(30, 0), 5f, SpriteEffects.None, 1f);
            }

            Rectangle sourceRec;
            int viewW = GlobalGameData.windowWidth;

            //Lobby
            sourceRec = new Rectangle(0, (curSelected == 0) ? 37 : 0, 168, 36);
            spriteBatch.Draw(menuTextTex, new Vector2(viewW / 2, menuBeginPosY), sourceRec, Color.White, 0f, new Vector2(sourceRec.Width / 2, sourceRec.Height / 2), 1f, SpriteEffects.None, 1f);

            //Options
            sourceRec = new Rectangle(169, (curSelected == 1) ? 37 : 0, 218, 36);
            spriteBatch.Draw(menuTextTex, new Vector2(viewW / 2, menuBeginPosY + menuOffset), sourceRec, Color.White, 0f, new Vector2(sourceRec.Width / 2, sourceRec.Height / 2), 1f, SpriteEffects.None, 1f);

            //Quit
            sourceRec = new Rectangle(388, (curSelected == 2) ? 37 : 0, 126, 36);
            spriteBatch.Draw(menuTextTex, new Vector2(viewW / 2, menuBeginPosY + menuOffset * 2), sourceRec, Color.White, 0f, new Vector2(sourceRec.Width / 2, sourceRec.Height / 2), 1f, SpriteEffects.None, 1f);

            spriteBatch.End();
        }

        void OnSelectStartGame()
        {
            manager.SwapStateWithTransition(StateType.GAME);
        }
    }
}
