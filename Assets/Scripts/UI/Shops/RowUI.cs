using UnityEngine;
using RPG.Shops;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] ItemHolder itemHolder;
        [SerializeField] Image image;
        [SerializeField] TextMeshProUGUI nameField;
        [SerializeField] TextMeshProUGUI availabilityField;
        [SerializeField] TextMeshProUGUI priceField;
        [SerializeField] TextMeshProUGUI quantityField;
        [SerializeField] Button minusButton;
        [SerializeField] Button plusButton;
        Shop currentShop;
        ShopItem item;

        public void Setup(Shop currentShop, ShopItem item)
        {
            this.currentShop = currentShop;
            this.item = item;

            itemHolder.Setup(item.GetInventoryItem());
            image.sprite = item.GetIcon();
            nameField.text = item.GetName();
            availabilityField.text = $"{item.GetAvailability()}";
            priceField.text = $"${item.GetPrice():N2}";
            quantityField.text = $"{item.GetQuantityInTransaction()}";
        }

        void Start()
        {
            plusButton.onClick.AddListener(Add);
            minusButton.onClick.AddListener(Remove);
        }

        void Add()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), 1);
        }

        void Remove()
        {
            currentShop.AddToTransaction(item.GetInventoryItem(), -1);
        }
    }
}