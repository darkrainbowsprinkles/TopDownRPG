using RPG.Abilities;
using RPG.Inventories;
using RPG.Utils.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Inventories
{
    public class ActionSlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon icon;
        [SerializeField] int index = 0;
        [SerializeField] Image cooldownOverlay;
        ActionStore actionStore;
        CooldownStore cooldownStore;

        public void AddItems(InventoryItem item, int number)
        {
            actionStore.AddAction(item, index, number);
        }

        public InventoryItem GetItem()
        {
            return actionStore.GetAction(index);
        }

        public int GetNumber()
        {
            return actionStore.GetNumber(index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return actionStore.MaxAcceptable(item, index);
        }

        public void RemoveItems(int number)
        {
            actionStore.RemoveItems(index, number);
        }

        void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            actionStore = player.GetComponent<ActionStore>();
            cooldownStore = player.GetComponent<CooldownStore>();
            actionStore.storeUpdated += UpdateIcon;
        }

        void Update()
        {
            cooldownOverlay.fillAmount = cooldownStore.GetFractionRemaining(GetItem());
        }

        void UpdateIcon()
        {
            icon.SetItem(GetItem(), GetNumber());
        }
    }
}
