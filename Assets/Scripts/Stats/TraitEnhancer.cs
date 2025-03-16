using UnityEngine;
using RPG.Utils;

namespace RPG.Stats
{
    public class TraitEnhancer : MonoBehaviour, IActionPerfomer
    {
        TraitStore playerTraitStore;

        void Awake()
        {
            playerTraitStore = TraitStore.GetPlayerTraitStore();
        }

        void IActionPerfomer.PerformAction(EAction action, string[] parameters)
        {
            switch(action)
            {
                case EAction.SetTraitEnhancer:
                    playerTraitStore.SetActiveEnhancer(true);
                    break;
            }
        }
    }
}