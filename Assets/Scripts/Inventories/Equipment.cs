using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Utils;
using RPG.Core;

namespace RPG.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [SerializeField] CharacterPartLocation[] defaultCharacterParts;
        Dictionary<EquipLocation, EquipableItem> equippedItems = new();
        Dictionary<EquipLocation, string[]> defaultPartsLookup = new();
        CharacterCustomizer customizer;
        public event Action equipmentUpdated;

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

            SetDefaultCharacterPart(slot, false);

            foreach(var characterPart in item.GetCharacterParts())
            {
                customizer.ToggleCharacterPart(characterPart, true);
            }

            equipmentUpdated?.Invoke();
        }

        public void RemoveItem(EquipLocation slot)
        {
            var item = GetItemInSlot(slot);

            foreach(var characterPart in item.GetCharacterParts())
            {
                customizer.ToggleCharacterPart(characterPart, false);
            }

            equippedItems.Remove(slot);

            SetDefaultCharacterPart(slot, true);

            equipmentUpdated?.Invoke();
        }

        [System.Serializable]
        struct CharacterPartLocation
        {
            public EquipLocation equipLocation;
            public string[] characterParts;
        }

        void Awake()
        {
            customizer = GetComponent<CharacterCustomizer>();
        }

        void Start()
        {
            foreach(var mesh in defaultCharacterParts)
            {
                defaultPartsLookup[mesh.equipLocation] = mesh.characterParts;
                SetDefaultCharacterPart(mesh.equipLocation, true);
            }
        }

        void SetDefaultCharacterPart(EquipLocation slot, bool enabled)
        {
            if(!defaultPartsLookup.ContainsKey(slot)) 
            {
                return;
            }
            
            foreach(string mesh in defaultPartsLookup[slot])
            {
                customizer.ToggleCharacterPart(mesh, enabled);
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