using System;
using GameDevTV.Saving;
using GameDevTV.Utils;
using RPG.Attributes;
using UnityEngine;

namespace RPG.TimeManagement
{   
    // Optimization TO-DO
    public class Clock : MonoBehaviour, ISaveable, IAttributeProvider, IPredicateEvaluator
    {
        [SerializeField] float timeScale = 1;
        [SerializeField] [Range(0,24)] float initialTime = 6;
        float currentTime = 0;

        public float GetCurrentTime()
        {
            return currentTime;
        }

        void Awake()
        {
            currentTime = initialTime;
        }

        void Update()                                                                                      
        {   
            UpdateTime();
        }

        void UpdateTime()
        {
            currentTime += Time.deltaTime * timeScale;

            if(currentTime >= 24)
            {
                currentTime -= 24;
            }
        }

        object ISaveable.CaptureState()
        {
            return currentTime;
        }

        void ISaveable.RestoreState(object state)
        {
            currentTime = (float) state;
        }

        float IAttributeProvider.GetMaxValue()
        {
            return 0;
        }

        float IAttributeProvider.GetCurrentValue()
        {
            return currentTime;
        }

        // TO-DO: Clock condition optimization with events instead of predicates
        bool? IPredicateEvaluator.Evaluate(EPredicate predicate, string[] parameters)
        {
            // switch(predicate)
            // {

            // }

            return null;
        }
    }
}
