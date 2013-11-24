using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using SpritesheetAnimation;

namespace BlastZone_Windows
{
    /// <summary>
    /// A class to handle un-interactable floating animations
    /// </summary>
    class FloatingAnimationManager
    {
        /// <summary>
        /// The list of the animations it's currently holding
        /// </summary>
        List<AnimatedSprite> floatingAnimationList;

        /// <summary>
        /// Create a new floating animation manager
        /// </summary>
        public FloatingAnimationManager()
        {
            floatingAnimationList = new List<AnimatedSprite>();
        }

        /// <summary>
        /// Clear all floating animations
        /// </summary>
        public void Reset()
        {
            floatingAnimationList.Clear();
        }

        /// <summary>
        /// Add a new floating animation
        /// </summary>
        /// <param name="animation">Animation to float</param>
        /// <param name="pos">Position to float at</param>
        public void Add(AnimatedSprite animation, Vector2 pos)
        {
            animation.position = pos;

            floatingAnimationList.Add(animation);
        }
        
        /// <summary>
        /// Draw all floating animations
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            //Calculate indices to remove beforehand as to avoid list shuffling issues
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
