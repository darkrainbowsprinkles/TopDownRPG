using System;
using System.Collections;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(menuName = "RPG/Abilities/Effects/Spawn Target Prefab Effect")]
    public class SpawnTargetPrefabEffect : EffectStrategy
    {
        [SerializeField] GameObject effectPrefab;
        [SerializeField] float destroyDelay  = -1;

        public override void StartEffect(AbilityData data, Action finished)
        {
            data.StartCoroutine(Effect(data, finished));
        }

        IEnumerator Effect(AbilityData data, Action finished)
        {
            GameObject effectInstance = Instantiate(effectPrefab);

            effectInstance.transform.position = data.GetTargetedPoint();

            if(destroyDelay > 0)
            {
                yield return new WaitForSeconds(destroyDelay);
                Destroy(effectInstance);
            }

            finished();
        }
    }
}
