using System;
using System.Collections;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(menuName = "RPG/Abilities/Effects/Wait for Animation Effect")]
    public class WaitForAnimationEffect : EffectStrategy
    {
        [SerializeField] string animationTag = "";
        [SerializeField, Range(0, 0.9f)] float endTime = 0.9f;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(WaitForAnimationFinished(data, finished));
        }

        IEnumerator WaitForAnimationFinished(AbilityData data, Action finished)
        {
            Animator animator = data.GetUser().GetComponent<Animator>();

            ToggleControl(data.GetUser(), false);

            while(!AnimationOver(animator) && !data.IsCancelled())
            {
                yield return null;
            }

            ToggleControl(data.GetUser(), true);

            finished();
        }

        void ToggleControl(GameObject user, bool enabled)
        {
            if(user.TryGetComponent(out PlayerController playerController))
            {
                playerController.enabled = enabled;
            }

            if(user.TryGetComponent(out AIController aiController))
            {
                aiController.enabled = enabled;
            }
        }

        bool AnimationOver(Animator animator)
        {
            var currentInfo = animator.GetCurrentAnimatorStateInfo(0);

            if(currentInfo.IsTag(animationTag) && !animator.IsInTransition(0))
            {
                return currentInfo.normalizedTime >= endTime;
            }

            return false;
        }
    }
}
