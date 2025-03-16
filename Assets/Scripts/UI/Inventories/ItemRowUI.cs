using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    public class ItemRowUI : MonoBehaviour
    {
        [SerializeField] TMP_Text itemNameText;
        [SerializeField] Image itemIcon;
        public event Action onRemoved; 

        public void Remove()
        {
            onRemoved?.Invoke();
            Destroy(gameObject);
        }
        
        public void Setup(InventoryItem item, int number)
        {
            itemNameText.text = $"{item.GetDisplayName()} x {number}";
            itemIcon.sprite = item.GetIcon();
        }
    }
}