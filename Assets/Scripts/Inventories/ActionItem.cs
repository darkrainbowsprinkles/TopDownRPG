using UnityEngine;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventories/Action Item")]
    public class ActionItem : InventoryItem
    {
        [SerializeField] bool consumable = false;

        public virtual bool Use(GameObject user)
        {
            return false;
        }

        public bool IsConsumable()
        {
            return consumable;
        }
    }
}