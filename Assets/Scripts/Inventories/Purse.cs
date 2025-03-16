using RPG.Saving;
using UnityEngine;

namespace RPG.Inventories
{
    public class Purse : MonoBehaviour, ISaveable, IItemStore
    {
        [SerializeField] float startingBalance = 400;
        float balance = 0;

        public float GetBalance()
        {
            return balance;
        }

        public void UpdateBalance(float amount)
        {
            balance += amount;
        }

        void Awake()
        {
            balance = startingBalance;
        }

        object ISaveable.CaptureState()
        {
            return balance;
        }

        void ISaveable.RestoreState(object state)
        {
            balance = (float)state;
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
    }
}