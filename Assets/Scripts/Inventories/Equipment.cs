using System;
using System.Collections.Generic;
using UnityEngine;
using PsychoticLab;
using RPG.Saving;
using RPG.Utils;

namespace RPG.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [SerializeField] DefaultMeshLocation[] defaultMeshes;
        Dictionary<EquipLocation, EquipableItem> equippedItems = new();
        Dictionary<EquipLocation, string[]> defaultMeshesLookup = new();
        CharacterCustomizer customizer;
        public event Action equipmentUpdated;

        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return equippedItems[equipLocation];
        }

        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            Debug.Assert(item.CanEquip(slot, this));

            equippedItems[slot] = item;

            SetDefaultMesh(slot, false);

            item.ToggleCharacterParts(customizer, true);

            equipmentUpdated?.Invoke();
        }

        public void RemoveItem(EquipLocation slot)
        {
            GetItemInSlot(slot).ToggleCharacterParts(customizer, false);

            equippedItems.Remove(slot);

            SetDefaultMesh(slot, true);

            equipmentUpdated?.Invoke();
        }

        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        }

        [System.Serializable]
        struct DefaultMeshLocation
        {
            public EquipLocation slot;
            public string[] characterParts;
        }

        void Awake()
        {
            customizer = GetComponent<CharacterCustomizer>();
        }

        void Start()
        {
            foreach(var mesh in defaultMeshes)
            {
                defaultMeshesLookup[mesh.slot] = mesh.characterParts;
                SetDefaultMesh(mesh.slot, true);
            }
        }

        void SetDefaultMesh(EquipLocation slot, bool enabled)
        {
            if(!defaultMeshesLookup.ContainsKey(slot)) 
            {
                return;
            }
            
            foreach(string mesh in defaultMeshesLookup[slot])
            {
                customizer.SetCharacterPart(mesh, enabled);
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
                    AddItem(item.GetAllowedEquipLocation(), item);
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