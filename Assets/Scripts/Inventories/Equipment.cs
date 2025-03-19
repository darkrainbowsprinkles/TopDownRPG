using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Utils;
using PshychoticLab;

namespace RPG.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [SerializeField] EquipmentMeshSet[] defaultEquipmentMeshes;
        Dictionary<EquipLocation, EquipableItem> equippedItems = new();
        Dictionary<EquipLocation, string[]> defaultMeshes = new();
        CharacterCustomizer characterCustomizer;
        public event Action equipmentUpdated;

        [System.Serializable]
        struct EquipmentMeshSet
        {
            public EquipLocation slot;
            public string[] meshes;
        }

        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        }

        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if(!equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return equippedItems[equipLocation];
        }

        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            Debug.Assert(item.CanEquip(slot, this));

            equippedItems[slot] = item;
            
            SetEquipmentMesh(item, true);

            equipmentUpdated?.Invoke();
        }

        public void RemoveItem(EquipLocation slot)
        {
            var item = GetItemInSlot(slot);

            equippedItems.Remove(slot);

            SetEquipmentMesh(item, false);

            equipmentUpdated?.Invoke();
        }

        void Awake()
        {
            characterCustomizer = GetComponent<CharacterCustomizer>();
        }

        void Start()
        {
            foreach(var mesh in defaultEquipmentMeshes)
            {
                defaultMeshes[mesh.slot] = mesh.meshes;
                SetDefaultEquipmentMesh(mesh.slot, true);
            }
        }

        void SetEquipmentMesh(EquipableItem item, bool enabled)
        {   
            if(defaultMeshes.ContainsKey(item.GetEquipLocation())) 
            {
                SetDefaultEquipmentMesh(item.GetEquipLocation(), !enabled);
            }

            foreach(var characterPart in item.GetEquipmentMeshes())
            {
                characterCustomizer.ToggleCharacterMesh(characterPart, enabled);
            }
        }

        void SetDefaultEquipmentMesh(EquipLocation slot, bool enabled)
        {
            foreach(string mesh in defaultMeshes[slot])
            {
                characterCustomizer.ToggleCharacterMesh(mesh, enabled);
            }
        }

        object ISaveable.CaptureState()
        {
            Dictionary<EquipLocation, string> equippedItemsCache = new();

            foreach(var pair in equippedItems)
            {
                equippedItemsCache[pair.Key] = pair.Value.GetItemID();
            }

            return equippedItemsCache;
        }

        void ISaveable.RestoreState(object state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsCache = (Dictionary<EquipLocation, string>)state;

            foreach(var pair in equippedItemsCache)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);

                if(item != null)
                {
                    AddItem(item.GetEquipLocation(), item);
                }
            }

            equipmentUpdated?.Invoke();
        }

        bool? IPredicateEvaluator.Evaluate(EPredicate predicate, string[] parameters)
        {
            if(predicate == EPredicate.HasItemEquipped)
            {
                foreach(var item in equippedItems.Values)
                {
                    if(item.GetItemID() == parameters[0])
                    {
                        return true;
                    }
                }

                return false;
            }

            return null;
        }
    }
}