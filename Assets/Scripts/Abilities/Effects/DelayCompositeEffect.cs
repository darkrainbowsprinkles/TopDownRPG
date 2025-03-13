using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(menuName = "RPG/Abilities/Effects/Delay Effect")]
    public class DelayCompositeEffect : EffectStrategy
    {
        [SerializeField] float delay = 0;
        [SerializeField] EffectStrategy[] delayedEffects;
        [SerializeField] bool abortIfCancelled = false;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(DelayedEffects(data, finished));
        }

        public override IEnumerable<string> GetEffectInfo()
        {
            foreach(var effect in delayedEffects)
            {
                if(effect.GetEffectInfo() == null) 
                {
                    continue;
                }
                
                foreach(var info in effect.GetEffectInfo())
                {
                    yield return info;
                }
            }
        }

        IEnumerator DelayedEffects(AbilityData data, Action finished)
        {
            yield return new WaitForSeconds(delay);

            if(data.IsCancelled() && abortIfCancelled) 
            {
                yield break;
            }
            
            foreach(var effect in delayedEffects)
            {
                effect.StartEffect(data, finished);
            }
        }
    }
}
