using RPG.Control;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Inventories
{
    [RequireComponent(typeof(ItemDropper))]
    public class Chest : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] LootConfig[] loot;
        [SerializeField] UnityEvent onChestOpened;
        ItemDropper itemDropper;
        Animation openAnimation;
        bool alreadyLooted = false;

        [System.Serializable]
        struct LootConfig
        {
            public InventoryItem item;
            [Min(1)] public int number;
        }

        void Awake()
        {
            itemDropper = GetComponent<RandomDropper>();
            openAnimation = GetComponent<Animation>();
            itemDropper = GetComponent<ItemDropper>();
        }

        void LootChest()
        {
            alreadyLooted = true;
            
            foreach(var slot in loot)
            {
                itemDropper.DropItem(slot.item, slot.number);
            }
        }

        CursorType IRaycastable.GetCursorType()
        {
            return CursorType.Pickup;
        }

        bool IRaycastable.HandleRaycast(PlayerController callingController)
        {
            if(!alreadyLooted)
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
            return alreadyLooted;
        }

        void ISaveable.RestoreState(object state)
        {
            alreadyLooted = (bool)state;

            if(alreadyLooted)
            {
                openAnimation.Play();
            }
        }
    }
}
