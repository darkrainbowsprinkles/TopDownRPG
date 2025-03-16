using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] GameObject textContainer;
        [SerializeField] TextMeshProUGUI itemNumber;

        public void SetItem(InventoryItem item)
        {
            SetItem(item, 0);
        }

        public void SetItem(InventoryItem item, int number)
        {
            var iconImage = GetComponent<Image>();

            if (item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = item.GetIcon();
            }

            if(itemNumber)
            {
                if(number <= 1)
                {
                    textContainer.SetActive(false);
                }
                else
                {
                    textContainer.SetActive(true);
                    itemNumber.text = number.ToString();
                }
            }
        }
    }
}