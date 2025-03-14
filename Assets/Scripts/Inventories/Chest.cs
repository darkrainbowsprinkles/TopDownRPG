using System;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Control;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Inventories
{
    [RequireComponent(typeof(RandomDropper))]
    public class Chest : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] LootConfig[] loot;
        [SerializeField] UnityEvent onChestOpened;
        bool looted = false;
        RandomDropper dropper;
        Animation openAnimation;

        [System.Serializable]
        struct LootConfig
        {
            public InventoryItem item;
            [Min(1)] public int number;
        }

        void Awake()
        {
            dropper = GetComponent<RandomDropper>();
            openAnimation = GetComponent<Animation>();

        }

        void LootChest()
        {
            looted = true;
            
            foreach(var slot in loot)
            {
                dropper.DropItem(slot.item, slot.number);
            }
        }

        CursorType IRaycastable.GetCursorType()
        {
            return CursorType.Pickup;
        }

        bool IRaycastable.HandleRaycast(PlayerController callingController)
        {
            if(!looted)
            {
                if(Input.GetKeyDown(callingController.GetInteractionKey()))
                {
                    onChestOpened?.Invoke();
                    openAnimation.Play();
                    LootChest();
                }

                return true;
            }

            return false;
        }

        object ISaveable.CaptureState()
        {
            return looted;
        }

        void ISaveable.RestoreState(object state)
        {
            looted = (bool)state;

            if(looted)
            {
                openAnimation.Play();
            }
        }
    }
}
