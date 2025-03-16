using RPG.Inventories;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] InventorySlotUI InventoryItemPrefab;
        Inventory playerInventory;

        void Awake() 
        {
            playerInventory = Inventory.GetPlayerInventory();
            playerInventory.inventoryUpdated += Redraw;
        }

        void Start()
        {
            Redraw();
        }

        void Redraw()
        {
            foreach(Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for(int i = 0; i < playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(InventoryItemPrefab, transform);
                itemUI.Setup(playerInventory, i);
            }
        }
    }
}