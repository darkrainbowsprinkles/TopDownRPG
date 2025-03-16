using UnityEngine;
using RPG.Saving;

namespace RPG.Inventories
{
    public class PickupSpawner : MonoBehaviour, ISaveable
    {
        [SerializeField] InventoryItem item;
        [SerializeField] int number = 1;

        public Pickup GetPickup() 
        { 
            return GetComponentInChildren<Pickup>();
        }

        public bool WasCollected() 
        { 
            return GetPickup() == null;
        }

        void Awake()
        {
            SpawnPickup();
        }

        void SpawnPickup()
        {
            var spawnedPickup = item.SpawnPickup(transform.position, number);
            spawnedPickup.transform.SetParent(transform);
        }

        void DestroyPickup()
        {
            if(GetPickup())
            {
                Destroy(GetPickup().gameObject);
            }
        }

        object ISaveable.CaptureState()
        {
            return WasCollected();
        }

        void ISaveable.RestoreState(object state)
        {
            bool shouldBeCollected = (bool)state;

            if(shouldBeCollected && !WasCollected())
            {
                DestroyPickup();
            }

            if(!shouldBeCollected && WasCollected())
            {
                SpawnPickup();
            }
        }
    }
}