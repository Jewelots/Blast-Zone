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
    class EventTimer
    {
        double currentTime;
        double maxTime;

        bool isFinished;
        bool paused;

        public delegate void TimerEndHandler();
        public event TimerEndHandler OnEnd;

        public EventTimer(double startTime, double endTime, bool startPaused = false)
        {
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

        public void Update(GameTime gameTime)
        {
            if (!paused)
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

        public void Reset()
        {
            currentTime = 0;
            isFinished = false;
        }

        public double GetRatio()
        {
            return currentTime / maxTime;
        }
    }
}
