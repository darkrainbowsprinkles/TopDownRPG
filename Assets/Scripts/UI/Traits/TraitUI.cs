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
        TraitStore playerTraitStore;
        Animator animator;

        void Awake()
        {
            animator = GetComponentInParent<Animator>();
        }

        void OnEnable()
        {
            animator.ResetTrigger("traitsFadeOut");
            animator.SetTrigger("traitsFadeIn");
        }

        void OnDisable()
        {
            animator.ResetTrigger("traitsFadeIn");
            animator.SetTrigger("traitsFadeOut");
        }

        void Start()
        {
            playerTraitStore = TraitStore.GetPlayerTraitStore();
            playerTraitStore.storeUpdated += Redraw;
            commitButton.onClick.AddListener(playerTraitStore.Commit);
            Redraw();
        }

        void Redraw()
        {
            gameObject.SetActive(playerTraitStore.ActiveEnhancer());
            commitButton.interactable = playerTraitStore.GetTotalStagedPoints() > 0;
            unassignedPointsText.text = $"{playerTraitStore.GetUnassignedPoints()}";
        }
    }
}
