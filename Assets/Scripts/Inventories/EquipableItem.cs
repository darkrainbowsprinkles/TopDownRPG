using UnityEngine;
using RPG.Utils;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "RPG/Inventories/Equipable Item")]
    public class EquipableItem : InventoryItem
    {
        [SerializeField] Condition equipCondition;
        [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;
        [SerializeField] string[] characterParts;

        public bool CanEquip(EquipLocation equipLocation, Equipment equipment)
        {
            if(equipLocation != allowedEquipLocation) 
            {
                return false;
            }

            return equipCondition.Check(equipment.GetComponents<IPredicateEvaluator>());
        }

        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }

        public string[] GetCharacterParts()
        {
            return characterParts;
        }
    }
}