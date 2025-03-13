using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(menuName = "RPG/Abilities/Targeting/Delayed Player Targeting (AI)")]
    public class DelayedPlayerTargeting : TargetingStrategy
    {
        [SerializeField] GameObject targetingEffectPrefab;
        [SerializeField] float areaAffectRadius = 5;
        [SerializeField] float heightEffectOffset = 0.31f;
        [SerializeField] float timeToBlast = 5;
        [SerializeField] string targetingAnimationTrigger = "";
        GameObject targetingEffectInstance;
        GameObject playerReference;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            playerReference = GameObject.FindWithTag("Player");
            data.StartCoroutine(Targeting(data, finished));
        }

        IEnumerator Targeting(AbilityData data, Action finished)
        {
            data.GetUser().GetComponent<AIController>().enabled = false;

            if(targetingEffectInstance == null)
            {
                targetingEffectInstance = Instantiate(targetingEffectPrefab);
            }
            else
            {
                targetingEffectInstance.SetActive(true);
            }

            if(targetingAnimationTrigger != "")
            {
                data.GetUser().GetComponent<Animator>().SetTrigger(targetingAnimationTrigger);
            }

            targetingEffectInstance.transform.localScale = new Vector3(areaAffectRadius * 2, 1, areaAffectRadius * 2);

            while(!data.IsCancelled())
            {
                var targetingEffectOffset = Vector3.up * heightEffectOffset;
                targetingEffectInstance.transform.position = playerReference.transform.position + targetingEffectOffset;

                yield return new WaitForSeconds(timeToBlast);

                break;
            }

            data.SetTargetedPoint(targetingEffectInstance.transform.position);
            data.SetTargets(GetGameObjectsInRadius(targetingEffectInstance.transform.position));
            targetingEffectInstance.SetActive(false);
            data.GetUser().GetComponent<AIController>().enabled = true;
            finished();
        }

        IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 point)
        {
            RaycastHit[] hits = Physics.SphereCastAll(point, areaAffectRadius, Vector3.up, 0);

            foreach(var hit in hits)
            {
                yield return hit.collider.gameObject;
            }          
        }
    }
}
