using UnityEngine;
using RPG.Utils;

namespace RPG.Core
{
    public class ConditionEnabler : MonoBehaviour
    {
        [SerializeField] EnableCondition[] enableConditions;

        [System.Serializable]
        struct EnableCondition
        {
            public GameObject target;
            public Condition condition;
        }

        void Start()
        {
            var evaluators = GameObject.FindWithTag("Player").GetComponents<IPredicateEvaluator>();

            foreach(var enableCondition in enableConditions)
            {
                enableCondition.target.SetActive(enableCondition.condition.Check(evaluators));
            }
        }
    }
}