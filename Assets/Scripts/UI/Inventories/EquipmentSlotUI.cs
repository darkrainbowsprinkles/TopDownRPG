using UnityEngine;
using RPG.Inventories;
using RPG.Utils.UI;

namespace RPG.UI.Inventories
{
    public class EquipmentSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon icon = null;
        [SerializeField] EquipLocation equipLocation = EquipLocation.Weapon;
        Equipment playerEquipment;

        public int MaxAcceptable(InventoryItem item)
        {
            EquipableItem equipableItem = item as EquipableItem;

            if(equipableItem == null) 
            {
                return 0;
            }

            if(!equipableItem.CanEquip(equipLocation, playerEquipment))
            {
                return 0;
            } 

            if(GetItem() != null) 
            {
                return 0;
            }

            return 1;
        }

        public void AddItems(InventoryItem item, int number)
        {
            playerEquipment.AddItem(equipLocation, (EquipableItem) item);
        }

        public InventoryItem GetItem()
        {
            return playerEquipment.GetItemInSlot(equipLocation);
        }

        public int GetNumber()
        {
            if (GetItem() != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void RemoveItems(int number)
        {
            playerEquipment.RemoveItem(equipLocation);
        }

        void Awake() 
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            playerEquipment = player.GetComponent<Equipment>();
            playerEquipment.equipmentUpdated += RedrawUI;
        }

        void Start() 
        {
            RedrawUI();
        }

        void RedrawUI()
        {
            icon.SetItem(playerEquipment.GetItemInSlot(equipLocation));
        }
    }
}