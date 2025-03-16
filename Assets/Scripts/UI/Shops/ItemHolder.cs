using UnityEngine;
using RPG.Inventories;
using RPG.UI.Inventories;

namespace RPG.UI.Shops
{
    public class ItemHolder : MonoBehaviour, IItemHolder
    {
        InventoryItem item;

        public void Setup(InventoryItem item)
        {
            this.item = item;
        }

        public InventoryItem GetItem()
        {
            return item;
        }
    }
}
