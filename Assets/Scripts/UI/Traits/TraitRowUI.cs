using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Traits
{
    public class TraitRowUI : MonoBehaviour
    {
        [SerializeField] Trait trait;
        [SerializeField] TextMeshProUGUI valueText;
        [SerializeField] Button minusButton;
        [SerializeField] Button plusButton;
        TraitStore playerTraitStore;

        public void Allocate(int points)
        {
            playerTraitStore.AssignPoints(trait, points);
        }
        
        void Start()
        {
            playerTraitStore = TraitStore.GetPlayerTraitStore();
            minusButton.onClick.AddListener(() => Allocate(-1));
            plusButton.onClick.AddListener(() => Allocate(+1));
        }

        void Update()
        {
            minusButton.interactable = playerTraitStore.CanAsignPoints(trait, -1);
            plusButton.interactable = playerTraitStore.CanAsignPoints(trait, +1);
            valueText.text = playerTraitStore.GetProposedPoints(trait).ToString();
        }
    }
}
