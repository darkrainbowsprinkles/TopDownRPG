using System;
using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI shopName;
        [SerializeField] Transform listRoot;
        [SerializeField] Button quitButton;
        [SerializeField] Button confirmButton;
        [SerializeField] Button switchButton;
        [SerializeField] RowUI rowPrefab;
        [SerializeField] TextMeshProUGUI totalField;
        Shopper shopper;
        Shop currentShop;
        Color originalTotalTextColor;

        void Start()
        {
            originalTotalTextColor = totalField.color;
            shopper = GameObject.FindWithTag("Player").GetComponent<Shopper>();

            if(shopper != null)
            {
                shopper.activeShopChanged += ShopChanged;
                ShopChanged();
            }

            quitButton.onClick.AddListener(Close);     
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchButton.onClick.AddListener(SwitchMode);
        }

        void ShopChanged()
        {
            if(currentShop != null)
            {
                currentShop.onChange -= RefreshUI;
            }

            currentShop = shopper.GetActiveShop();

            gameObject.SetActive(currentShop != null);

            foreach(FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetShop(currentShop);
            }

            if(currentShop != null)   
            {
                shopName.text = currentShop.GetShopName();
                currentShop.onChange += RefreshUI;
                RefreshUI();
            }
        }

        void RefreshUI()
        {
            foreach(Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach(ShopItem item in currentShop.GetFilteredItems())
            {
                RowUI row = Instantiate<RowUI>(rowPrefab, listRoot);
                row.Setup(currentShop, item);
            }

            totalField.text = $"Total: ${currentShop.TransactionTotal():N2}";

            totalField.color = currentShop.HasSufficientFunds() ? originalTotalTextColor : Color.red;

            confirmButton.interactable = currentShop.CanTransact();

            var switchText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
            var confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();

            if(currentShop.IsBuyingMode())
            {
                switchText.text = "Switch to Selling";
                confirmText.text = "Buy";
            }
            else
            {
                switchText.text = "Switch to Buying";
                confirmText.text = "Sell";
            }

            foreach(FilterButtonUI button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.Redraw();
            }
        }

        void Close()
        {
            shopper.SetActiveShop(null);
        }

        void ConfirmTransaction()
        {
            currentShop.ConfirmTransaction();
        }

        void SwitchMode()
        {
            currentShop.SelectMode(!currentShop.IsBuyingMode());
        }
    }
}