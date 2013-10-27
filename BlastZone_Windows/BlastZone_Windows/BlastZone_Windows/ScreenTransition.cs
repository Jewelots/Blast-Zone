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
    class ScreenTransition
    {
        Texture2D rectTex;
        Vector2 tileCount;
        Vector2 tileSize;

        EventTimer timer;

        bool reverse;

        public delegate void TransitionEndHandler();
        public event TransitionEndHandler OnTransitionEnd;

        public ScreenTransition(int tilesX, int tilesY, int screenW, int screenH, bool reverseTransition = false, double transitionTime = 1)
        {
            tileCount = new Vector2(tilesX, tilesY);
            tileSize = new Vector2(screenW / tileCount.X, screenH / tileCount.Y);

            reverse = reverseTransition;

            timer = new EventTimer(0, transitionTime);
            timer.OnEnd += TransitionEnd;
        }

        public void Reset()
        {
            timer.Reset();
        }

        public void LoadContent(ContentManager Content)
        {
            rectTex = Content.Load<Texture2D>("1px");
        }

        public void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
        }

        void TransitionEnd()
        {
            if (OnTransitionEnd != null)
                OnTransitionEnd();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
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

                    Rectangle destRect = new Rectangle((int)tilePos.X, (int)tilePos.Y, (int)Math.Round(tileSize.X * ratio), (int)Math.Round(tileSize.Y * ratio));
                    spriteBatch.Draw(rectTex, destRect, null, Color.White, 0f, new Vector2(0.5f, 0.5f), SpriteEffects.None, 1f);
                }
            }
        }
    }
}
