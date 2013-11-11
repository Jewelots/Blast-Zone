using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlastZone_Windows.Drawing
{
    class AnimationSwitcher
    {
        Dictionary<string, Animation> animDict;
        Animation currentAnimation;

        public AnimationSwitcher(List<AnimationDetails> animDetails, string defaultAnim = null)
        {
            animDict = new Dictionary<string, Animation>();
            currentAnimation = null;

            foreach (AnimationDetails detail in animDetails)
            {
                animDict[detail.name] = new Animation(detail);
            }

            if (defaultAnim != null)
            {
                currentAnimation = animDict.FirstOrDefault(x => x.Key == defaultAnim).Value;
            }
        }

        public void SwapAnimation(string animName)
        {
            currentAnimation = animDict.FirstOrDefault(x => x.Key == animName).Value;
        }

        public void SwapAnimationKeepSync(string animName)
        {
            if (currentAnimation == null)
            {
                SwapAnimation(animName);
                return;
            }

            float prevTime = currentAnimation.Time;
            currentAnimation = animDict.FirstOrDefault(x => x.Key == animName).Value;

            if (currentAnimation != null)
                currentAnimation.Time = prevTime;
        }

        public Animation GetCurrentAnimation()
        {
            return currentAnimation;
        }

        public void ResetAllAnimations()
        {
            foreach (KeyValuePair<string, Animation> kvp in animDict)
            {
                kvp.Value.Reset();
            }
        }
    }
}
