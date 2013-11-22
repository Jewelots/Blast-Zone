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
    class MenuState : GameState
    {
        Texture2D[] Blast = new Texture2D[4];
        Texture2D[] Zone = new Texture2D[4];

        int fallingBombCount = 5;
        Texture2D fallingBombTex;
        Vector2[] fallingBombPos;
        Vector2[] fallingBombSpeed;
        float[] fallingBombRotation;
        float[] fallingBombRotationalVelocity;
        float fallingBombGravity = 500f;

        Vector2 blastPos;
        Vector2 zonePos;

        TiledTexture bgtex;
        Texture2D menuTextTex;

        Song menuSong;
        SoundEffect itemSwitchSound, itemSelectSound;
        SoundEffectInstance itemSwitchSoundInstance, itemSelectSoundInstance;

        SpriteFont menuFont;

        int menuBeginPosY;
        int menuOffset;

        KeyboardState oldState;
        GamePadState[] oldGamepadStates;

        int curSelected;
        bool selectedMenuOption;

        public override void Enter()
        {
            curSelected = 0;
            selectedMenuOption = false;

            Random r = GlobalGameData.rand;

            fallingBombPos = new Vector2[fallingBombCount];
            fallingBombSpeed = new Vector2[fallingBombCount];
            fallingBombRotation = new float[fallingBombCount];
            fallingBombRotationalVelocity = new float[fallingBombCount];

            for (int i = 0; i < fallingBombCount; ++i)
            {
                InitBomb(r, i);
            }

            MediaQueue mediaQueue = MediaPlayer.Queue;

            //Play music if not playing
            if (mediaQueue.ActiveSong == null || mediaQueue.ActiveSong != menuSong)
            {
                MediaPlayer.Play(menuSong);
                MediaPlayer.IsRepeating = true;
            }

            itemSelectSoundInstance.Volume = GlobalGameData.SFXVolume;
            itemSwitchSoundInstance.Volume = GlobalGameData.SFXVolume;
        }

        void InitBomb(Random r, int bombIndex)
        {
            int columnWidth = GlobalGameData.windowWidth / 2 - 100;

            bool leftOrRight = r.NextDouble() > 0.5;
            float posOffset = leftOrRight ? GlobalGameData.windowWidth - columnWidth : 0;
            fallingBombPos[bombIndex] = new Vector2(r.Next(columnWidth) + posOffset, -100);
            fallingBombSpeed[bombIndex] = new Vector2(r.Next(200) - 100, -r.Next(0, 1000));

            fallingBombRotation[bombIndex] = (float)r.NextDouble() * 360;
            fallingBombRotationalVelocity[bombIndex] = (float)(r.NextDouble() - 0.5) * 500;
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
            menuBeginPosY = 250;
            menuOffset = 100;

            oldGamepadStates = new GamePadState[4];
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

            fallingBombTex = Content.Load<Texture2D>("Images/Game/bomb");

            bgtex.SetTexture(Content.Load<Texture2D>("Images/Menu/bg"));

            menuTextTex = Content.Load<Texture2D>("Images/Menu/text");

            menuFont = Content.Load<SpriteFont>("Fonts/Badaboom");

            //Load music
            menuSong = Content.Load<Song>("Music/title");

            //Load SFX
            itemSwitchSound = Content.Load<SoundEffect>("SFX/menuitemchange");
            itemSelectSound = Content.Load<SoundEffect>("SFX/menuitemselect");

            itemSwitchSoundInstance = itemSwitchSound.CreateInstance();
            itemSelectSoundInstance = itemSelectSound.CreateInstance();
        }

        public override void Update(GameTime gameTime)
        {
            bgtex.ShiftOffset(new Vector2(50f * (float)gameTime.ElapsedGameTime.TotalSeconds, 50f * (float)gameTime.ElapsedGameTime.TotalSeconds));

            for (int i = 0; i < fallingBombCount; ++i)
            {
                fallingBombPos[i] += fallingBombSpeed[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;
                fallingBombSpeed[i].Y += fallingBombGravity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (fallingBombPos[i].Y > GlobalGameData.windowHeight + 150)
                {
                    InitBomb(GlobalGameData.rand, i);
                }

                fallingBombRotation[i] += fallingBombRotationalVelocity[i] * (float)gameTime.ElapsedGameTime.TotalSeconds;

                float rotInRad = (float)Math.PI * fallingBombRotation[i] / 180.0f;
                Vector2 fireOffset = new Vector2(60, -80);
                fireOffset = Vector2.Transform(fireOffset, Matrix.CreateRotationZ(rotInRad));
                //Managers.ParticleManager.Emit("Fire", fallingBombPos[i] + fireOffset);

                if (GlobalGameData.LowQualityParticles)
                {
                    Managers.ParticleManager.AddEmissionPoint("FireFast", fallingBombPos[i] + fireOffset);
                }
                else
                {
                    Managers.ParticleManager.AddEmissionPoint("Fire", fallingBombPos[i] + fireOffset);
                }
            }

            KeyboardState newState = Keyboard.GetState();
            GamePadState[] currentGamepadStates = new GamePadState[4];

            for (int i = 0; i < 4; ++i)
            {
                currentGamepadStates[i] = GamePad.GetState((PlayerIndex)i);
            }

            //Check controller inputs
            bool gamepadPressedA = false;
            bool gamepadPressedUp = false;
            bool gamepadPressedDown = false;
            for (int i = 0; i < 4; ++i)
            {
                GamePadState gps = currentGamepadStates[i];
                GamePadState ogps = oldGamepadStates[i];

                if (!gps.IsConnected) continue;

                if (gps.IsButtonDown(Buttons.A) && ogps.IsButtonUp(Buttons.A))
                {
                    gamepadPressedA = true;
                }

                if ((gps.IsButtonDown(Buttons.DPadUp) && ogps.IsButtonUp(Buttons.DPadUp)) || (gps.ThumbSticks.Left.Y > 0.25 && Math.Abs(ogps.ThumbSticks.Left.Y) < 0.25))
                {
                    gamepadPressedUp = true;
                }

                if ((gps.IsButtonDown(Buttons.DPadDown) && ogps.IsButtonUp(Buttons.DPadDown)) || (gps.ThumbSticks.Left.Y < -0.25 && Math.Abs(ogps.ThumbSticks.Left.Y) < 0.25))
                {
                    gamepadPressedDown = true;
                }
            }

            if ((newState.IsKeyDown(Keys.Enter) || newState.IsKeyDown(Keys.Space) || gamepadPressedA) && !manager.IsTransitioning() && !selectedMenuOption)
            {
                itemSelectSoundInstance.Play();

                selectedMenuOption = true;
                switch (curSelected)
                {
                    case 0: //Lobby button
                        OnSelectStartGame();
                        break;
                    case 1: //Controls button
                        OnSelectControls();
                        break;
                    case 2: //Options button
                        OnSelectOptions();
                        break;
                    case 3: //Quit Button
                        MediaPlayer.Stop();
                        manager.QuitGame();
                        break;
                    default:
                        break;
                }
            }

            if ((newState.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up)) || (newState.IsKeyDown(Keys.W) && oldState.IsKeyUp(Keys.W)) || gamepadPressedUp)
            {
                itemSwitchSoundInstance.Play();
                curSelected -= 1;
            }
            if ((newState.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down)) || (newState.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S)) || gamepadPressedDown)
            {
                itemSwitchSoundInstance.Play();
                curSelected += 1;
            }

            curSelected %= 4;
            if (curSelected < 0) curSelected = 3;

            oldState = newState;

            for (int i = 0; i < 4; ++i)
            {
                oldGamepadStates[i] = currentGamepadStates[i];
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            bgtex.Draw(spriteBatch);

            Vector2 newZonePos = zonePos + new Vector2((float)Math.Cos((double)gameTime.TotalGameTime.TotalMilliseconds / 150f) * 2f, (float)Math.Sin((double)gameTime.TotalGameTime.TotalMilliseconds / 200f) * 2f);
            Vector2 newBlastPos = blastPos + new Vector2((float)Math.Sin((double)gameTime.TotalGameTime.TotalMilliseconds / 150f + 0.3f) * 2f, (float)Math.Cos((double)gameTime.TotalGameTime.TotalMilliseconds / 200f + 0.23f) * 2f);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            for (int i = 0; i < fallingBombCount; ++i)
            {
                spriteBatch.Draw(fallingBombTex, fallingBombPos[i], null, Color.White, (float)Math.PI * fallingBombRotation[i] / 180.0f, new Vector2(8, 8), 10f, SpriteEffects.None, 1f);
            }

            spriteBatch.End();

            //Draw particles
            Managers.ParticleManager.Draw(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            spriteBatch.Draw(Zone[0], newZonePos, null, Color.White * 0.5f, 0f, new Vector2(30, 0), 5f, SpriteEffects.None, 1f);
            spriteBatch.Draw(Blast[0], newBlastPos, null, Color.White * 0.5f, 0f, new Vector2(30, 0), 5f, SpriteEffects.None, 1f);

            for (int i = 0; i < 3; ++i)
            {
                spriteBatch.Draw(Zone[i + 1], newZonePos, null, Color.White, 0f, new Vector2(30, 0), 5f, SpriteEffects.None, 1f);
                spriteBatch.Draw(Blast[i + 1], newBlastPos, null, Color.White, 0f, new Vector2(30, 0), 5f, SpriteEffects.None, 1f);
            }

            int viewW = GlobalGameData.windowWidth;

            //Lobby
            DrawTextExtension.DrawTextOutline(spriteBatch, menuFont, "Lobby", Color.Black, (curSelected == 0) ? Color.Yellow : Color.White, new Vector2(viewW / 2, menuBeginPosY), 3f, HorizontalAlign.AlignCenter);

            //Controls
            DrawTextExtension.DrawTextOutline(spriteBatch, menuFont, "Controls", Color.Black, (curSelected == 1) ? Color.Yellow : Color.White, new Vector2(viewW / 2, menuBeginPosY + menuOffset), 3f, HorizontalAlign.AlignCenter);

            //Options
            DrawTextExtension.DrawTextOutline(spriteBatch, menuFont, "Options", Color.Black, (curSelected == 2) ? Color.Yellow : Color.White, new Vector2(viewW / 2, menuBeginPosY + menuOffset * 2), 3f, HorizontalAlign.AlignCenter);

            //Quit
            DrawTextExtension.DrawTextOutline(spriteBatch, menuFont, "Quit", Color.Black, (curSelected == 3) ? Color.Yellow : Color.White, new Vector2(viewW / 2, menuBeginPosY + menuOffset * 3), 3f, HorizontalAlign.AlignCenter);

            spriteBatch.End();
        }

        void OnSelectStartGame()
        {
            manager.SwapStateWithTransition(StateType.LOBBY);
        }

        void OnSelectControls()
        {
            manager.SwapStateWithTransition(StateType.CONTROLS);
        }

        void OnSelectOptions()
        {
            manager.SwapStateWithTransition(StateType.OPTIONS);
        }
    }
}
