using UnityEngine;

namespace RPG.Inventories
{
    public class Pickup : MonoBehaviour
    {
        InventoryItem item;
        Inventory playerInventory;
        int number = 1;

        public void Setup(InventoryItem item, int number)
        {
            this.item = item;

            if(!item.IsStackable())
            {
                number = 1;
            }
            
            this.number = number;
        }

        public InventoryItem GetItem()
        {
            return item;
        }

        public int GetNumber()
        {
            return number;
        }

        public void PickupItem()
        {
            bool foundSlot = playerInventory.AddToFirstEmptySlot(item, number);

            if(foundSlot)
            {
                Destroy(gameObject);
            }
        }

        public bool CanBePickedUp()
        {
            return playerInventory.HasSpaceFor(item);
        }

        void Awake()
        {
            playerInventory = Inventory.GetPlayerInventory();
        }
    }
}