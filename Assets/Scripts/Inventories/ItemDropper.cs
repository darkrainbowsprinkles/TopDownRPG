using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Inventories
{
    public class ItemDropper : MonoBehaviour, ISaveable
    {
        List<Pickup> droppedItems = new();
        List<DropRecord> otherSceneDroppedItems = new();

        public void DropItem(InventoryItem item, int number)
        {
            SpawnPickup(item, GetDropLocation(), number);
        }

        public void DropItem(InventoryItem item)
        {
            SpawnPickup(item, GetDropLocation(), 1);
        }

        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        [System.Serializable]
        struct DropRecord
        {
            public string itemID;
            public SerializableVector3 position;
            public int number;
            public int sceneBuildIndex;
        }

        void SpawnPickup(InventoryItem item, Vector3 spawnLocation, int number)
        {
            var pickup = item.SpawnPickup(spawnLocation, number);
            droppedItems.Add(pickup);
        }

        void RemoveDestroyedDrops()
        {
            var activeDrops = new List<Pickup>();

            foreach(var item in droppedItems)
            {
                if(item != null)
                {
                    activeDrops.Add(item);
                }
            }

            droppedItems = activeDrops;
        }

        object ISaveable.CaptureState()
        {
            RemoveDestroyedDrops();

            List<DropRecord> dropRecords = new();

            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            foreach(var pickup in droppedItems)
            {
                DropRecord droppedItem = new()
                {
                    itemID = pickup.GetItem().GetItemID(),
                    position = new SerializableVector3(pickup.transform.position),
                    number = pickup.GetNumber(),
                    sceneBuildIndex = buildIndex
                };

                dropRecords.Add(droppedItem);
            }

            dropRecords.AddRange(otherSceneDroppedItems);

            return dropRecords;
        }

        void ISaveable.RestoreState(object state)
        {
            var dropRecords = (List<DropRecord>)state;

            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            otherSceneDroppedItems.Clear();

            foreach(var item in dropRecords)
            {
                if(item.sceneBuildIndex != buildIndex)
                {
                    otherSceneDroppedItems.Add(item);
                    continue;
                }

                InventoryItem pickupItem = InventoryItem.GetFromID(item.itemID);
                Vector3 position = item.position.ToVector();
                int number = item.number;

                SpawnPickup(pickupItem, position, number);
            }
        }
    }
}