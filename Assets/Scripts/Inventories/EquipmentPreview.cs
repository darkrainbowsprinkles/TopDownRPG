using UnityEngine;

namespace RPG.Inventories
{
    public class EquipmentPreview : MonoBehaviour
    {
        Equipment playerEquipment;
        Equipment previewEquipment;

        void Awake()
        {
            playerEquipment = GameObject.FindWithTag("Player").GetComponent<Equipment>();
            previewEquipment = GetComponent<Equipment>();
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
            var prewiewSlots = previewEquipment.GetAllPopulatedSlots();
            var playerSlots = playerEquipment.GetAllPopulatedSlots();

            foreach(var previewSlot in prewiewSlots)
            {
                if(playerEquipment.GetItemInSlot(previewSlot))
                {
                    previewEquipment.RemoveItem(previewSlot);
                }
            }

            foreach(var slot in playerSlots)
            {
                var item = playerEquipment.GetItemInSlot(slot);

                previewEquipment.AddItem(slot, item);
            }
        }
    }
}