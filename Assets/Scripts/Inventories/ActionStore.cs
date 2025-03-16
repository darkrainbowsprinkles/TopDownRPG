using System;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Inventories
{
    public class ActionStore : MonoBehaviour, ISaveable
    {
        Dictionary<int, DockedItemSlot> dockedItems = new();
        public event Action storeUpdated;

        public ActionItem GetAction(int index)
        {
            if(dockedItems.ContainsKey(index))
            {
                return dockedItems[index].item;
            }

            return null;
        }

        public int GetNumber(int index)
        {
            if(dockedItems.ContainsKey(index))
            {
                return dockedItems[index].number;
            }

            return 0;
        }

        public void AddAction(InventoryItem item, int index, int number)
        {
            if(dockedItems.ContainsKey(index))
            {  
                if(ReferenceEquals(item, dockedItems[index].item))
                {
                    dockedItems[index].number += number;
                }
            }
            else
            {
                var slot = new DockedItemSlot();
                slot.item = item as ActionItem;
                slot.number = number;
                dockedItems[index] = slot;
            }

            storeUpdated?.Invoke();
        }

        public bool Use(int index, GameObject user)
        {
            if(dockedItems.ContainsKey(index))
            {
                bool wasUsed = dockedItems[index].item.Use(user);

                if (wasUsed && dockedItems[index].item.IsConsumable())
                {
                    RemoveItems(index, 1);
                }

                return true;
            }

            return false;
        }

        public void RemoveItems(int index, int number)
        {
            if(dockedItems.ContainsKey(index))
            {
                dockedItems[index].number -= number;

                if(dockedItems[index].number <= 0)
                {
                    dockedItems.Remove(index);
                }

                storeUpdated?.Invoke();
            }
            
        }

        public int MaxAcceptable(InventoryItem item, int index)
        {
            var actionItem = item as ActionItem;

            if(actionItem == null) 
            {
                return 0;
            }

            if(dockedItems.ContainsKey(index) && !ReferenceEquals(item, dockedItems[index].item))
            {
                return 0;
            }

            if(actionItem.IsConsumable())
            {
                return int.MaxValue;
            }

            if(dockedItems.ContainsKey(index))
            {
                return 0;
            }

            return 1;
        }

        class DockedItemSlot 
        {
            public ActionItem item;
            public int number;
        }

        [System.Serializable]
        struct DockedItemRecord
        {
            public string itemID;
            public int number;
        }

        object ISaveable.CaptureState()
        {
            Dictionary<int, DockedItemRecord> itemRecords = new();

            foreach (var pair in dockedItems)
            {
                DockedItemRecord record = new()
                {
                    itemID = pair.Value.item.GetItemID(),
                    number = pair.Value.number
                };

                itemRecords[pair.Key] = record;
            }
            return itemRecords;
        }

        void ISaveable.RestoreState(object state)
        {
            var itemRecords = (Dictionary<int, DockedItemRecord>)state;

            foreach(var pair in itemRecords)
            {
                AddAction(InventoryItem.GetFromID(pair.Value.itemID), pair.Key, pair.Value.number);
            }

            storeUpdated?.Invoke();
        }
    }
}