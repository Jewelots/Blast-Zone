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

namespace BlastZone_Windows.Drawing
{
    /// <summary>
    /// Class used for transitioning between different screens, covers entire screen, can be reversed to unclear the screen
    /// Provides an event OnTransitionEnd to change between scenes
    /// </summary>
    class ScreenTransition
    {
        Texture2D rectTex;
        Vector2 tileCount;
        Vector2 tileSize;

        EventTimer timer;

        bool reverse;

        EventArgs args;

        public delegate void TransitionEndHandler(EventArgs e);
        /// <summary>
        /// An Event which is called when transition is finished
        /// </summary>
        public event TransitionEndHandler OnTransitionEnd;

        /// <summary>
        /// Create a Screen Transition
        /// </summary>
        /// <param name="tilesX">Horizontal Tile Count</param>
        /// <param name="tilesY">Vertical Tile Count</param>
        /// <param name="reverseTransition">Bool to specify whether to reverse the transition or not</param>
        /// <param name="transitionTime">Time to complete transition</param>
        public ScreenTransition(int tilesX, int tilesY, bool reverseTransition, double transitionTime)
        {
            tileCount = new Vector2(tilesX, tilesY);
            tileSize = new Vector2(GlobalGameData.windowWidth / tileCount.X, GlobalGameData.windowHeight / tileCount.Y);

            reverse = reverseTransition;

            timer = new EventTimer(0, transitionTime);
            timer.OnEnd += TransitionEnd;

            args = EventArgs.Empty;
        }

        public ScreenTransition()
        {
            tileCount = new Vector2(13, 8);
            tileSize = new Vector2(GlobalGameData.windowWidth / tileCount.X, GlobalGameData.windowHeight / tileCount.Y);

            reverse = false;

            timer = new EventTimer(0, 1);
            timer.OnEnd += TransitionEnd;

            args = EventArgs.Empty;
        }

        public void SetEventArgs(EventArgs e)
        {
            args = e;
        }

        public void Reset()
        {
            timer.Reset();

            OnTransitionEnd = null;
        }

        public void LoadContent(ContentManager Content)
        {
            rectTex = Content.Load<Texture2D>("Images/1px");
        }

        void TransitionEnd()
        {
            if (OnTransitionEnd != null)
                OnTransitionEnd(args);
        }

        public void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (timer.IsFinished()) return;

            float percentDone = (float)timer.GetRatio();

            for (int y = 0; y < tileCount.Y; ++y)
            {
                for (int x = 0; x < tileCount.X; ++x)
                {
                    Vector2 tilePos = new Vector2(x * tileSize.X + tileSize.X / 2f, y * tileSize.Y + tileSize.Y / 2f);

                    float ratio = ((tileCount.X - (x + 1)) + (y + 1)) / (tileCount.X + tileCount.Y) - 1;
                    ratio += percentDone * 2;
                    ratio = Math.Max(Math.Min(ratio, 1), 0);

                    if (reverse)
                        ratio = 1 - ratio;

                    if (ratio <= 0) continue;

                    Rectangle destRect = new Rectangle((int)tilePos.X, (int)tilePos.Y, (int)Math.Round(tileSize.X * ratio) + 1, (int)Math.Round(tileSize.Y * ratio) + 1);
                    spriteBatch.Draw(rectTex, destRect, null, Color.Black, 0f, new Vector2(0.5f, 0.5f), SpriteEffects.None, 1f);
                }
            }
        }
    }

    /// <summary>
    /// Class used to provide a wrapper over two ScreenTransitions to transition in then out
    /// Provides event OnTransition to change screens midway
    /// Provides event OnTransitionFinished to destroy object with when done
    /// </summary>
    class ScreenTransitionInOut
    {
        ScreenTransition stIn;
        ScreenTransition stOut;

        ScreenTransition currentTransition;

        public delegate void TransitionChangeHandler(EventArgs e);
        /// <summary>
        /// An Event which is called when transition In is finished (screen completely covered)
        /// </summary>
        public event TransitionChangeHandler OnTransition;

        public delegate void TransitionInOutFinishedHandler(EventArgs e);
        /// <summary>
        /// An Event which is called when transition Out is finished (screen completely uncovered, both transitions over)
        /// </summary>
        public event TransitionInOutFinishedHandler OnTransitionFinished;

        /// <summary>
        /// Create a two way Screen Transition
        /// </summary>
        /// <param name="tilesX">Horizontal Tile Count</param>
        /// <param name="tilesY">Vertical Tile Count</param>
        /// <param name="transitionTime">Time to complete transition (one way)</param>
        public ScreenTransitionInOut(int tilesX, int tilesY, double transitionTime)
        {
            stIn = new ScreenTransition(tilesX, tilesY, false, transitionTime);
            stOut = new ScreenTransition(tilesX, tilesY, true, transitionTime);

            Init();
        }

        public ScreenTransitionInOut()
        {
            stIn = new ScreenTransition(13, 8, false, 0.75f);
            stOut = new ScreenTransition(13, 8, true, 0.75f);

            Init();
        }

        void Init()
        {
            stIn.OnTransitionEnd += SwapTransition;
            stOut.OnTransitionEnd += TransitionsFinished;

            currentTransition = stIn;
        }

        public void SetEventArgs(EventArgs e)
        {
            stIn.SetEventArgs(e);
            stOut.SetEventArgs(e);
        }

        public void LoadContent(ContentManager Content)
        {
            stIn.LoadContent(Content);
            stOut.LoadContent(Content);
        }

        void SwapTransition(EventArgs e)
        {
            TransitionChange(e);
            currentTransition = stOut;
        }

        void TransitionChange(EventArgs e)
        {
            if (OnTransition != null)
                OnTransition(e);
        }

        void TransitionsFinished(EventArgs e)
        {
            if (OnTransitionFinished != null)
                OnTransitionFinished(e);
        }

        public void Reset()
        {
            stIn.Reset();
            stOut.Reset();

            OnTransition = null;
            OnTransitionFinished = null;

            Init();
        }

        public void Update(GameTime gameTime)
        {
            currentTransition.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentTransition.Draw(spriteBatch);
        }
    }
}
