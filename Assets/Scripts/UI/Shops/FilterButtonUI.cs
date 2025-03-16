using UnityEngine;
using UnityEngine.UI;
using RPG.Inventories;
using RPG.Shops;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        [SerializeField] ItemCategory category = ItemCategory.None;
        Button button;
        Shop currentShop = null;

        public void SetShop(Shop currentShop)
        {
            this.currentShop = currentShop;
        }

        public void RefreshUI()
        {
            button.interactable = currentShop.GetFilter() != category;
        }

        void Awake()
        {
            button = GetComponent<Button>();

            button.onClick.AddListener(SelectFilter);
        }

        void SelectFilter()
        {
            currentShop.SelectFilter(category);
        }
    }
}
