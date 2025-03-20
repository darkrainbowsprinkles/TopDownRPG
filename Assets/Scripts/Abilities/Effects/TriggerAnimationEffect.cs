using System;
using System.Collections;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(menuName = "RPG/Abilities/Effects/Trigger Animation Effect")]
    public class TriggerAnimationEffect : EffectStrategy
    {
        [SerializeField] string animationTrigger = "";
        [SerializeField, Range(0, 0.9f)] float animationEndTime = 0.9f;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.GetUser().GetComponent<Animator>().SetTrigger(animationTrigger);
            data.StartCoroutine(WaitForAnimationFinished(data, finished));
            finished();
        }

        IEnumerator WaitForAnimationFinished(AbilityData data, Action finished)
        {
            Animator animator = data.GetUser().GetComponent<Animator>();

            animator.ResetTrigger("cancelAbility");

            ToggleControl(data.GetUser(), false);

            while(!AnimationOver(animator) && !data.IsCancelled())
            {
                yield return null;
            }

            animator.SetTrigger("cancelAbility");

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

            if(currentInfo.IsTag("ability") && !animator.IsInTransition(0))
            {
                return currentInfo.normalizedTime >= animationEndTime;
            }

            return false;
        }
    }
}
