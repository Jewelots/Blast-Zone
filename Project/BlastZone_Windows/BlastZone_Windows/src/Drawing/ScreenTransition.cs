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
        /// <summary>
        /// A 1px wide texture to draw the transition with
        /// </summary>
        Texture2D rectTex;

        /// <summary>
        /// Number of tiles in X and Y dimensions to transition with
        /// </summary>
        Vector2 tileCount;

        /// <summary>
        /// Size of each tile to transition with
        /// </summary>
        Vector2 tileSize;

        /// <summary>
        /// A timer to keep track of the transition, and call an event
        /// </summary>
        EventTimer timer;

        /// <summary>
        /// Reverse the transition from in to out
        /// </summary>
        bool reverse;

        /// <summary>
        /// Args to get passed to the TransitionEnd event handler
        /// </summary>
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

        /// <summary>
        /// Create a Screen Transition with 13x8 tiles, no reversing, and 1 second to transition
        /// </summary>
        public ScreenTransition() : this(13, 8, false, 1)
        {
        }

        /// <summary>
        /// Set the arguments to be passed into the OnTransitionEnd event
        /// </summary>
        /// <param name="e">Arguments</param>
        public void SetEventArgs(EventArgs e)
        {
            args = e;
        }

        /// <summary>
        /// Reset the transition
        /// </summary>
        public void Reset()
        {
            timer.Reset();

            OnTransitionEnd = null;
        }

        /// <summary>
        /// Load the images needed
        /// </summary>
        /// <param name="Content"></param>
        public void LoadContent(ContentManager Content)
        {
            rectTex = Content.Load<Texture2D>("Images/1px");
        }

        /// <summary>
        /// Fires the OnTransitionEnd event
        /// </summary>
        void TransitionEnd()
        {
            if (OnTransitionEnd != null)
                OnTransitionEnd(args);
        }

        /// <summary>
        /// Update's the transition
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
        }

        /// <summary>
        /// Draws the transition
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Return if transition over
            if (timer.IsFinished()) return;

            //Get the percent complete
            float percentDone = (float)timer.GetRatio();

            //Loop through the tiles to draw
            for (int y = 0; y < tileCount.Y; ++y)
            {
                for (int x = 0; x < tileCount.X; ++x)
                {
                    //Create their position
                    Vector2 tilePos = new Vector2(x * tileSize.X + tileSize.X / 2f, y * tileSize.Y + tileSize.Y / 2f);

                    //Create their size
                    float ratio = ((tileCount.X - (x + 1)) + (y + 1)) / (tileCount.X + tileCount.Y) - 1;
                    ratio += percentDone * 2;
                    ratio = Math.Max(Math.Min(ratio, 1), 0);

                    //Reverse it if neccesary
                    if (reverse)
                        ratio = 1 - ratio;

                    //Don't draw if they're under 0 in size
                    if (ratio <= 0) continue;
                    
                    //Create the destination rectangle and draw the square
                    Rectangle destRect = new Rectangle((int)tilePos.X, (int)tilePos.Y, (int)Math.Round(tileSize.X * ratio) + 1, (int)Math.Round(tileSize.Y * ratio) + 1);
                    spriteBatch.Draw(rectTex, destRect, null, Color.Black, 0f, new Vector2(0.5f, 0.5f), SpriteEffects.None, 1f);
                }
            }
        }

        /// <summary>
        /// Get the ratio/percent complete
        /// </summary>
        /// <returns>Percent complete</returns>
        public float Ratio()
        {
            return (float)timer.GetRatio();
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

        /// <summary>
        /// Create a two way Screen Transition with 13x8 tiles, and 0.75s transition time
        /// </summary>
        public ScreenTransitionInOut() : this(13, 8, 0.75f)
        {
        }

        /// <summary>
        /// Initialise the transitions and hook functions
        /// </summary>
        void Init()
        {
            stIn.OnTransitionEnd += SwapTransition;
            stOut.OnTransitionEnd += TransitionsFinished;

            currentTransition = stIn;
        }

        /// <summary>
        /// Set the arguments to call when transition finished
        /// </summary>
        /// <param name="e"></param>
        public void SetEventArgs(EventArgs e)
        {
            stIn.SetEventArgs(e);
            stOut.SetEventArgs(e);
        }

        /// <summary>
        /// Load the content
        /// </summary>
        /// <param name="Content"></param>
        public void LoadContent(ContentManager Content)
        {
            stIn.LoadContent(Content);
            stOut.LoadContent(Content);
        }

        /// <summary>
        /// Swap the transition from in to out (hooked onto OnTransitionEnd of the inward transition)
        /// </summary>
        /// <param name="e"></param>
        void SwapTransition(EventArgs e)
        {
            TransitionChange(e);
            currentTransition = stOut;
        }

        /// <summary>
        /// Fire OnTransition event when the transition is halfway through (screen completely black)
        /// </summary>
        /// <param name="e"></param>
        void TransitionChange(EventArgs e)
        {
            if (OnTransition != null)
                OnTransition(e);
        }

        /// <summary>
        /// Fire OnTransitionFinished event when the transition is completely done (screen completely uncovered)
        /// </summary>
        /// <param name="e"></param>
        void TransitionsFinished(EventArgs e)
        {
            if (OnTransitionFinished != null)
                OnTransitionFinished(e);
        }

        /// <summary>
        /// Reset the transition
        /// </summary>
        public void Reset()
        {
            stIn.Reset();
            stOut.Reset();

            OnTransition = null;
            OnTransitionFinished = null;

            Init();
        }

        /// <summary>
        /// Update the transition
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            currentTransition.Update(gameTime);
        }

        /// <summary>
        /// Draw the transition
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            currentTransition.Draw(spriteBatch);
        }

        /// <summary>
        /// Get the current amount of fade on the screen
        /// </summary>
        /// <returns>Current amount of fade</returns>
        public float FadeAmount()
        {
            if (currentTransition == stIn)
            {
                return 1 - stIn.Ratio();
            }
            else
            {
                return stOut.Ratio();
            }
        }
    }
}
