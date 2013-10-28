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
    /// EventTimer counts from a start time to an end time (in seconds) and triggers an event on end.
    /// </summary>
    class EventTimer
    {
        double startTime;

        double currentTime;
        double maxTime;

        bool isFinished;
        bool paused;

        public delegate void TimerEndHandler();
        public event TimerEndHandler OnEnd;

        public EventTimer(double startTime, double endTime, bool startPaused = false)
        {
            this.startTime = startTime;

            currentTime = startTime;
            maxTime = endTime;

            paused = startPaused;
        }

        public void Pause()
        {
            paused = true;
        }

        public void UnPause()
        {
            paused = false;
        }

        public void TogglePause()
        {
            paused = !paused;
        }

        public bool IsFinished()
        {
            return isFinished;
        }

        /// <summary>
        /// Update EventTimer, triggering OnEnd if timer is complete
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (!paused && !isFinished)
            {
                if (currentTime < maxTime)
                {
                    currentTime += gameTime.ElapsedGameTime.TotalSeconds;

                    if (currentTime > maxTime && !isFinished)
                    {
                        End();
                        isFinished = true;
                    }
                }
                else
                {
                    currentTime -= gameTime.ElapsedGameTime.TotalSeconds;

                    if (currentTime < maxTime && !isFinished)
                    {
                        End();
                        isFinished = true;
                    }
                }
            }
        }

        void End()
        {
            if (OnEnd != null)
                OnEnd();
        }

        /// <summary>
        /// Reset the timer back to original starting time
        /// </summary>
        public void Reset()
        {
            currentTime = startTime;
            isFinished = false;
        }

        public double GetRatio()
        {
            return currentTime / maxTime;
        }
    }
}
