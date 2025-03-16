using System.Collections.Generic;
using UnityEngine;

namespace RPG.Utils
{
    [System.Serializable]
    public class ActionConfig
    {
        [SerializeField] EAction action;
        [SerializeField] string[] parameters;

        public void PerformAction(IEnumerable<IActionPerfomer> performers)
        {   
            foreach(var performer in performers)
            {
                performer.PerformAction(action, parameters);
            }
        }
    }
}