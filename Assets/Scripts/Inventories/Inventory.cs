using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Utils;

namespace RPG.Inventories
{
    public class Inventory : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [SerializeField] int inventorySize = 16;
        InventorySlot[] slots;
        public event Action inventoryUpdated;
        public event Action<InventoryItem, int> onItemAdded;

        public struct InventorySlot
        {
            public InventoryItem item;
            public int number;
        }

        public static Inventory GetPlayerInventory()
        {
            return GameObject.FindWithTag("Player").GetComponent<Inventory>();
        }

        public bool HasSpaceFor(InventoryItem item)
        {
            if(item is CurrencyItem)
            {
                return true;
            }
            
            return FindSlot(item) >= 0;
        }

        public bool HasSpaceFor(IEnumerable<InventoryItem> items)
        {
            int freeSlots = FreeSlots();

            List<InventoryItem> stackedItems = new List<InventoryItem>();

            foreach(var item in items)
            {
                if(item.IsStackable())
                {
                    if(HasItem(item)) 
                    {
                        continue;
                    }

                    if(stackedItems.Contains(item)) 
                    {
                        continue;
                    }

                    stackedItems.Add(item);
                }

                if(freeSlots <= 0) 
                {
                    return false;
                }

                freeSlots--;
            }

            return true;
        }

        public int FreeSlots()
        {
            int count = 0;

            foreach(InventorySlot slot in slots)
            {
                if(slot.number == 0)
                {
                    count++;
                }
            }

            return count;
        }

        public int GetSize()
        {
            return slots.Length;
        }

        public bool AddToFirstEmptySlot(InventoryItem item, int number)
        {
            if(item is CurrencyItem || item is ExperienceItem)
            {
                onItemAdded?.Invoke(item, number);
            }

            foreach(var store in GetComponents<IItemStore>())
            {
                number -= store.AddItems(item, number);
            }

            if(number <= 0) 
            {
                return true;
            }

            int slot = FindSlot(item);

            if(slot < 0)
            {
                return false;
            }

            slots[slot].item = item;
            slots[slot].number += number;

            onItemAdded?.Invoke(item, number);
            inventoryUpdated?.Invoke();

            return true;
        }

        public bool HasItem(InventoryItem item)
        {
            for(int i = 0; i < slots.Length; i++)
            {
                if(ReferenceEquals(slots[i].item, item))
                {
                    return true;
                }
            }

            return false;
        }

        public InventoryItem GetItemInSlot(int slot)
        {
            return slots[slot].item;
        }

        public int GetNumberInSlot(int slot)
        {
            return slots[slot].number;
        }

        public void RemoveFromSlot(int slot, int number)
        {
            slots[slot].number -= number;

            if(slots[slot].number <= 0)
            {
                slots[slot].number = 0;
                slots[slot].item = null;
            }

            inventoryUpdated?.Invoke();
        }

        public bool AddItemToSlot(int slot, InventoryItem item, int number)
        {
            if(slots[slot].item != null)
            {
                return AddToFirstEmptySlot(item, number); ;
            }

            int stack = FindStack(item);

            if (stack >= 0)
            {
                slot = stack;
            }

            slots[slot].item = item;
            slots[slot].number += number;

            inventoryUpdated?.Invoke();

            return true;
        }

        public int FindSlot(InventoryItem item)
        {
            int stack = FindStack(item);

            if(stack < 0)
            {
                stack = FindEmptySlot();
            }

            return stack;
        }

        void Awake()
        {
            slots = new InventorySlot[inventorySize];
        }

        int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    return i;
                }
            }
            return -1;
        }

        int FindStack(InventoryItem item)
        {
            if(item == null)
            {
                return -1;
            }

            if(!item.IsStackable())
            {
                return -1;
            }

            for(int i = 0; i < slots.Length; i++)
            {
                if(ReferenceEquals(slots[i].item, item))
                {
                    return i;
                }
            }

            return -1;
        }

        [System.Serializable]
        struct InventorySlotRecord
        {
            public string itemID;
            public int number;
        }
    
        object ISaveable.CaptureState()
        {
            var slotRecords = new InventorySlotRecord[inventorySize];

            for(int i = 0; i < inventorySize; i++)
            {
                if(slots[i].item != null)
                {
                    slotRecords[i].itemID = slots[i].item.GetItemID();
                    slotRecords[i].number = slots[i].number;
                }
            }

            return slotRecords;
        }

        void ISaveable.RestoreState(object state)
        {
            var slotRecords = (InventorySlotRecord[])state;

            for(int i = 0; i < inventorySize; i++)
            {
                slots[i].item = InventoryItem.GetFromID(slotRecords[i].itemID);
                slots[i].number = slotRecords[i].number;
            }

            inventoryUpdated?.Invoke();
        }

        public bool? Evaluate(EPredicate predicate, string[] parameters)
        {
            switch(predicate)
            {
                case EPredicate.HasItem:
                    return HasItem(InventoryItem.GetFromID(parameters[0]));
            }

            return null;
        }
    }
}