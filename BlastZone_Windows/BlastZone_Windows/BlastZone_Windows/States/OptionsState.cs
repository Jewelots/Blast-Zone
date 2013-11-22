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
    class OptionsState : GameState
    {
        TiledTexture bgtex;
        SpriteFont optionsFont;

        Texture2D checkboxTex;
        Texture2D sliderTex;

        KeyboardState oldState;
        GamePadState[] oldGamepadStates;

        SoundEffect itemSwitchSound;
        SoundEffectInstance itemSwitchSoundInstance;

        int curSelected;

        public override void Enter()
        {
            curSelected = 0;
            itemSwitchSoundInstance.Volume = GlobalGameData.SFXVolume;
        }
        public override void Exit()
        {
            GlobalGameData.SaveSettings();
        }

        public OptionsState(GameStateManager gameStateManager)
            : base(gameStateManager)
        {
            bgtex = new TiledTexture(new Rectangle(0, 0, GlobalGameData.windowWidth, GlobalGameData.windowHeight));

            oldState = Keyboard.GetState();

            oldGamepadStates = new GamePadState[4];
        }

        public override void LoadContent(ContentManager Content)
        {
            optionsFont = Content.Load<SpriteFont>("Fonts/Badaboom");
            bgtex.SetTexture(Content.Load<Texture2D>("Images/Menu/bg"));
            sliderTex = Content.Load<Texture2D>("Images/Options/slider");
            checkboxTex = Content.Load<Texture2D>("Images/Options/checkbox");

            itemSwitchSound = Content.Load<SoundEffect>("SFX/menuitemchange");
            itemSwitchSoundInstance = itemSwitchSound.CreateInstance();
        }

        public override void Update(GameTime gameTime)
        {
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

            bgtex.ShiftOffset(new Vector2(50f * (float)gameTime.ElapsedGameTime.TotalSeconds, 50f * (float)gameTime.ElapsedGameTime.TotalSeconds));

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
            bool gamepadPressedLeft = false;
            bool gamepadPressedRight = false;

            float mostJoyAmount = 0;
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

                if (gps.IsButtonDown(Buttons.DPadLeft))
                {
                    gamepadPressedLeft = true;
                }

                if (gps.IsButtonDown(Buttons.DPadRight))
                {
                    gamepadPressedRight = true;
                }

                if (Math.Abs(mostJoyAmount) < Math.Abs(gps.ThumbSticks.Left.X))
                {
                    mostJoyAmount = gps.ThumbSticks.Left.X;
                }
            }

            if ((newState.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter)) || (newState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space)) || gamepadPressedA)
            {
                switch (curSelected)
                {
                    case 0: //SFX Volume
                        break;
                    case 1: //Music volume
                        break;
                    case 2: //Toggle Low Quality Particles
                        GlobalGameData.LowQualityParticles = !GlobalGameData.LowQualityParticles;
                        itemSwitchSoundInstance.Play();
                        break;
                    default:
                        break;
                }
            }

            if (newState.IsKeyDown(Keys.A) || newState.IsKeyDown(Keys.Left) || gamepadPressedLeft)
            {
                switch (curSelected)
                {
                    case 0: //SFX Volume
                        GlobalGameData.SFXVolume -= 0.3f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (GlobalGameData.SFXVolume < 0)
                        {
                            GlobalGameData.SFXVolume = 0;
                        }

                        itemSwitchSoundInstance.Volume = GlobalGameData.SFXVolume;
                        itemSwitchSoundInstance.Play();
                        break;
                    case 1: //Music volume
                        GlobalGameData.MusicVolume -= 0.3f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (GlobalGameData.MusicVolume < 0)
                        {
                            GlobalGameData.MusicVolume = 0;
                        }
                        MediaPlayer.Volume = GlobalGameData.MusicVolume;
                        break;
                    case 2: //Toggle Low Quality Particles
                        break;
                    default:
                        break;
                }
            }

            if (newState.IsKeyDown(Keys.D) || newState.IsKeyDown(Keys.Right) || gamepadPressedRight)
            {
                switch (curSelected)
                {
                    case 0: //SFX Volume
                        GlobalGameData.SFXVolume += 0.3f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (GlobalGameData.SFXVolume > 1)
                        {
                            GlobalGameData.SFXVolume = 1;
                        }

                        itemSwitchSoundInstance.Volume = GlobalGameData.SFXVolume;
                        itemSwitchSoundInstance.Play();
                        break;
                    case 1: //Music volume
                        GlobalGameData.MusicVolume += 0.3f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (GlobalGameData.MusicVolume > 1)
                        {
                            GlobalGameData.MusicVolume = 1;
                        }
                        MediaPlayer.Volume = GlobalGameData.MusicVolume;
                        break;
                    case 2: //Toggle Low Quality Particles
                        break;
                    default:
                        break;
                }
            }

            //Handle joystick
            if (mostJoyAmount != 0)
            {
                switch (curSelected)
                {
                    case 0: //SFX Volume
                        GlobalGameData.SFXVolume -= 0.6f * -mostJoyAmount * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (GlobalGameData.SFXVolume < 0)
                        {
                            GlobalGameData.SFXVolume = 0;
                        }

                        if (GlobalGameData.SFXVolume > 1)
                        {
                            GlobalGameData.SFXVolume = 1;
                        }

                        itemSwitchSoundInstance.Volume = GlobalGameData.SFXVolume;
                        itemSwitchSoundInstance.Play();
                        break;
                    case 1: //Music volume
                        GlobalGameData.MusicVolume -= 0.6f * -mostJoyAmount * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (GlobalGameData.MusicVolume < 0)
                        {
                            GlobalGameData.MusicVolume = 0;
                        }

                        if (GlobalGameData.MusicVolume > 1)
                        {
                            GlobalGameData.MusicVolume = 1;
                        }
                        MediaPlayer.Volume = GlobalGameData.MusicVolume;
                        break;
                    case 2: //Toggle Low Quality Particles
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

            curSelected %= 3;
            if (curSelected < 0) curSelected = 2;

            oldState = newState;

            for (int i = 0; i < 4; ++i)
            {
                oldGamepadStates[i] = currentGamepadStates[i];
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            bgtex.Draw(spriteBatch);

            float option1Height = 150;
            float sfxSliderHeight = 300;
            float option2Height = 325;
            float musicSliderHeight = 475;
            float option3Height = 500;

            string lowQPartText = "Low Quality Particles: ";
            float lowQWidth = optionsFont.MeasureString(lowQPartText).X;
            float totalLowQWidth = lowQWidth + checkboxTex.Width/2;

            Vector2 lowQPartTextPos = new Vector2(GlobalGameData.windowWidth / 2 - (totalLowQWidth/2), option3Height);
            Vector2 checkboxPos = new Vector2(GlobalGameData.windowWidth / 2 + lowQWidth - (totalLowQWidth / 2), option3Height - 10);

            Rectangle checkboxSrcRect = new Rectangle(GlobalGameData.LowQualityParticles ? 0 : checkboxTex.Width / 2, 0, checkboxTex.Width / 2, checkboxTex.Height);

            spriteBatch.Begin();
            DrawTextExtension.DrawTextOutline(spriteBatch, optionsFont, "SFX Volume", Color.Black, (curSelected == 0) ? Color.Yellow : Color.White, new Vector2(GlobalGameData.windowWidth / 2, option1Height), 3f, HorizontalAlign.AlignCenter);
            DrawTextExtension.DrawTextOutline(spriteBatch, optionsFont, "Music Volume", Color.Black, (curSelected == 1) ? Color.Yellow : Color.White, new Vector2(GlobalGameData.windowWidth / 2, option2Height), 3f, HorizontalAlign.AlignCenter);
            
            #if WINDOWS
            DrawTextExtension.DrawTextOutline(spriteBatch, optionsFont, lowQPartText, Color.Black, (curSelected == 2) ? Color.Yellow : Color.White, lowQPartTextPos, 3f);
            spriteBatch.Draw(checkboxTex, checkboxPos, checkboxSrcRect, Color.White);
            #endif

            spriteBatch.End();

            //Use for nearest neighbor
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

            DrawSlider(spriteBatch, new Vector2(GlobalGameData.windowWidth / 2, sfxSliderHeight), 500, GlobalGameData.SFXVolume);
            DrawSlider(spriteBatch, new Vector2(GlobalGameData.windowWidth / 2, musicSliderHeight), 500, GlobalGameData.MusicVolume);

            spriteBatch.End();
        }

        void DrawSlider(SpriteBatch spriteBatch, Vector2 pos, int width, float percentComplete)
        {
            Rectangle sliderDest = new Rectangle((int)pos.X - width / 2, (int)pos.Y, width, 3);
            //Draw the slider itself
            spriteBatch.Draw(sliderTex, sliderDest, new Rectangle(0, 77, 1, 3), Color.White);

            //Draw the arrow itself
            spriteBatch.Draw(sliderTex, pos + new Vector2((percentComplete - 0.5f) * width - 14, -80), new Rectangle(0, 0, 29, 77), Color.White);
        }
    }
}
