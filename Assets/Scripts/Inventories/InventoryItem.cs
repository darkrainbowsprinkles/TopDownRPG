using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public abstract class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] string itemID;
        [SerializeField] string displayName;
        [SerializeField][TextArea] string description;
        [SerializeField] Sprite icon;
        [SerializeField] Pickup pickup;
        [SerializeField] bool stackable = false;
        [SerializeField] float price = 0;
        [SerializeField] ItemCategory category = ItemCategory.None;
        [SerializeField] ItemRarity rarity = ItemRarity.Common;
        static Dictionary<string, InventoryItem> itemLookup;

        public static InventoryItem GetFromID(string itemID)
        {
            if(itemLookup == null)
            {
                FillItemLookup();
            }

            if(itemID == null || !itemLookup.ContainsKey(itemID)) 
            {
                return null;
            }

            return itemLookup[itemID];
        }
        
        public Pickup SpawnPickup(Vector3 position, int number)
        {
            var pickup = Instantiate(this.pickup);
            pickup.transform.position = position;
            pickup.Setup(this, number);
            return pickup;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public string GetItemID()
        {
            return itemID;
        }

        public bool IsStackable()
        {
            return stackable;
        }
        
        public string GetDisplayName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return description;
        }

        public float GetPrice()
        {
            return price;
        }

        public ItemCategory GetItemCategory()
        {
            return category;
        }

        public ItemRarity GetRarity()
        {
            return rarity;
        }

        static void FillItemLookup()
        {
            itemLookup = new Dictionary<string, InventoryItem>();

            var items = Resources.LoadAll<InventoryItem>("");

            foreach(var item in items)
            {
                if(itemLookup.ContainsKey(item.itemID))
                {
                    Debug.LogError($"Duplicate item ID for: {itemLookup[item.itemID]} and {item}");
                    continue;
                }

                itemLookup[item.itemID] = item;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if(string.IsNullOrWhiteSpace(itemID))
            {
                itemID = Guid.NewGuid().ToString();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}
