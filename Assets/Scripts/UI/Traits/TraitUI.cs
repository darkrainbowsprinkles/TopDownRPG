using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Traits
{
    public class TraitUI : MonoBehaviour
    {
        [SerializeField] Button commitButton;
        [SerializeField] TextMeshProUGUI unassignedPointsText;
        [SerializeField] Transform traitsContainer;
        TraitStore playerTraitStore;

        void Start()
        {
            playerTraitStore = TraitStore.GetPlayerTraitStore();
            playerTraitStore.storeUpdated += RefreshUI;
            commitButton.onClick.AddListener(playerTraitStore.Commit);
            RefreshUI();
        }

        void RefreshUI()
        {
            traitsContainer.gameObject.SetActive(playerTraitStore.ActiveEnhancer());
            commitButton.interactable = playerTraitStore.GetTotalStagedPoints() > 0;
            unassignedPointsText.text = $"{playerTraitStore.GetUnassignedPoints()}";
        }
    }
}
