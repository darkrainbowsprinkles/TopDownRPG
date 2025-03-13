using System;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using GameDevTV.Utils;
using PsychoticLab;

namespace GameDevTV.Inventories
{
    /// <summary>
    /// Provides a store for the items equipped to a player. Items are stored by
    /// their equip locations.
    /// 
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Equipment : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [SerializeField] DefaultMeshLocation[] defaultMeshes;

        // STATE
        Dictionary<EquipLocation, EquipableItem> equippedItems = new Dictionary<EquipLocation, EquipableItem>();
        Dictionary<EquipLocation, string[]> defaultMeshesLookup = new Dictionary<EquipLocation, string[]>();
        CharacterCustomizer customizer;

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action equipmentUpdated;
        public event Action<EquipLocation, EquipableItem> onItemAdded;
        public event Action<EquipLocation> onItemRemoved;

        /// <summary>
        /// Return the item in the given equip location.
        /// </summary>
        public EquipableItem GetItemInSlot(EquipLocation equipLocation)
        {
            if (!equippedItems.ContainsKey(equipLocation))
            {
                return null;
            }

            return equippedItems[equipLocation];
        }

        /// <summary>
        /// Add an item to the given equip location. Do not attempt to equip to
        /// an incompatible slot.
        /// </summary>
        public void AddItem(EquipLocation slot, EquipableItem item)
        {
            Debug.Assert(item.CanEquip(slot, this));

            equippedItems[slot] = item;

            SetDefaultMesh(slot, false);

            item.ToggleCharacterParts(customizer, true);

            equipmentUpdated?.Invoke();
            onItemAdded?.Invoke(slot, item);
        }

        /// <summary>
        /// Remove the item for the given slot.
        /// </summary>
        public void RemoveItem(EquipLocation slot)
        {
            GetItemInSlot(slot).ToggleCharacterParts(customizer, false);

            equippedItems.Remove(slot);

            SetDefaultMesh(slot, true);

            equipmentUpdated?.Invoke();
            onItemRemoved?.Invoke(slot);
        }

        /// <summary>
        /// Enumerate through all the slots that currently contain items.
        /// </summary>
        public IEnumerable<EquipLocation> GetAllPopulatedSlots()
        {
            return equippedItems.Keys;
        }

        // PRIVATE

        [System.Serializable]
        class DefaultMeshLocation
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
            if(!defaultMeshesLookup.ContainsKey(slot)) return;
            
            foreach(string part in defaultMeshesLookup[slot])
            {
                customizer.SetCharacterPart(part, enabled);
            }
        }

        object ISaveable.CaptureState()
        {
            var equippedItemsForSerialization = new Dictionary<EquipLocation, string>();
            foreach (var pair in equippedItems)
            {
                equippedItemsForSerialization[pair.Key] = pair.Value.GetItemID();
            }
            return equippedItemsForSerialization;
        }

        void ISaveable.RestoreState(object state)
        {
            equippedItems = new Dictionary<EquipLocation, EquipableItem>();

            var equippedItemsForSerialization = (Dictionary<EquipLocation, string>)state;

            foreach (var pair in equippedItemsForSerialization)
            {
                var item = (EquipableItem)InventoryItem.GetFromID(pair.Value);
                if (item != null)
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