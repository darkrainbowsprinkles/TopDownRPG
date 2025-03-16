using RPG.Attributes;
using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable, IItemStore, IAttributeProvider
    {
        [SerializeField] float startingBalance = 400;
        float balance = 0;

        void Awake()
        {
            balance = startingBalance;
        }

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
        }

        object ISaveable.CaptureState()
        {
            return balance;
        }

        void ISaveable.RestoreState(object state)
        {
            balance = (float) state;
        }

        int IItemStore.AddItems(InventoryItem item, int number)
        {
            if(item is CurrencyItem)
            {
                UpdateBalance(item.GetPrice() * number);
                return number;
            }

            return 0;
        }

        float IAttributeProvider.GetMaxValue()
        {
            return 0;
        }

        float IAttributeProvider.GetCurrentValue()
        {
            return GetBalance();
        }
    }
}