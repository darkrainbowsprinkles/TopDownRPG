using System;
using UnityEngine;

namespace RPG.Abilities.Effects
{
    [CreateAssetMenu(menuName = "RPG/Abilities/Effects/Spawn Prefab Effect")]
    public class SpawnPrefabEffect : EffectStrategy
    {
        [SerializeField] GameObject effectPrefab;

        public override void StartEffect(AbilityData data, Action finished)
        {
            GameObject effectInstance = Instantiate(effectPrefab);

            effectInstance.transform.position = data.GetTargetedPoint();

            finished();
        }
    }
}
