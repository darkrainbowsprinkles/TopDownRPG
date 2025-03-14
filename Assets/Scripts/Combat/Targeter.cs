using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Targeter : MonoBehaviour
    {
        [SerializeField] UnityEvent onTargetsAggrevated;
        [SerializeField] UnityEvent onNoTargets;
        List<CombatTarget> targets = new();
        bool alreadyInCombat = false;

        void OnTriggerEnter(Collider other)
        {
            var target = other.GetComponent<CombatTarget>();

            if(target != null)
            {
                AddTarget(target);
            }
        }

        void OnTriggerExit(Collider other)
        {
            var target = other.GetComponent<CombatTarget>();

            if(target != null)
            {
                RemoveTarget(target);
            }
        }

        void AddTarget(CombatTarget target)
        {
            var health = target.GetComponent<Health>();
            var controller = target.GetComponent<AIController>();

            if(health != null && controller != null && !health.IsDead())
            {
                targets.Add(target);
                health.onDie.AddListener(() => RemoveTarget(target));
                controller.onAggrevated.AddListener(OnAggrevated);
            }
        }

        void RemoveTarget(CombatTarget target)
        {
            var health = target.GetComponent<Health>();
            var controller = target.GetComponent<AIController>();

            if(health != null && controller != null)
            {
                targets.Remove(target);
                health.onDie.RemoveListener(() => RemoveTarget(target));
                controller.onAggrevated.RemoveListener(OnAggrevated);

                if(targets.Count == 0 && alreadyInCombat)
                {
                    OnNoTargets();
                } 
            }
        }

        void OnAggrevated()
        {
            if(!alreadyInCombat) 
            {
                onTargetsAggrevated?.Invoke();
                alreadyInCombat = true;
            }
        }

        void OnNoTargets()
        {
            onNoTargets?.Invoke();
            alreadyInCombat = false;
        }
    }
}