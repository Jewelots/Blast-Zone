using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using SpritesheetAnimation;

namespace BlastZone_Windows
{
    class FloatingAnimationManager
    {
        List<AnimatedSprite> floatingAnimationList;

        public FloatingAnimationManager()
        {
            floatingAnimationList = new List<AnimatedSprite>();
        }

        public void Reset()
        {
            floatingAnimationList.Clear();
        }

        public void Add(AnimatedSprite animation, Vector2 pos)
        {
            animation.position = pos;

            floatingAnimationList.Add(animation);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            List<int> indicesToRemove = new List<int>();

            for (int i = 0; i < floatingAnimationList.Count; ++i)
            {
                floatingAnimationList[i].Update(gameTime);
                floatingAnimationList[i].Draw(spriteBatch, GlobalGameData.drawRatio);
                if (floatingAnimationList[i].Animation == null)
                {
                    indicesToRemove.Add(i);
                }
            }

            //Sort in ascending order
            indicesToRemove.Sort();
            //Reverse so indices go from high to low
            indicesToRemove.Reverse();

            //Remove from high to low so removing doesn't affect previous ones
            for (int i = 0; i < indicesToRemove.Count; ++i)
            {
                floatingAnimationList.RemoveAt(indicesToRemove[i]);
            }
        }
    }
}
