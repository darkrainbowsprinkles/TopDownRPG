using RPG.Inventories;
using UnityEngine;

namespace RPG.UI.Inventories
{
    public class PlayerUI : MonoBehaviour
    {
        Equipment playerEquipment;

        void Awake()
        {
            playerEquipment = GameObject.FindWithTag("Player").GetComponent<Equipment>();
        }

        void OnEnable()
        {
            playerEquipment.equipmentUpdated += ReplicateEquipment;
        }

        void OnDisable()
        {
            playerEquipment.equipmentUpdated -= ReplicateEquipment;
        }

        void ReplicateEquipment()
        {
            // TO-DO
            print("Equipment changed");
        }
    }
}
