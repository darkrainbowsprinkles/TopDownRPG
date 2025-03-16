using System;
using System.Collections.Generic;
using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] bool raycastable = true;
        [SerializeField] string shopName = "";
        [SerializeField] StockItemConfig[] stockConfig;
        [SerializeField, Range(0,100)] float sellingDiscountPercentage = 80f;
        [SerializeField, Range(0,100)] float maximumBarterDiscount = 80f;
        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();
        Shopper currentShopper = null;
        bool isBuyingMode = true;
        ItemCategory filter = ItemCategory.None;
        public event Action onChange;

        [System.Serializable]
        struct StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0, 100)] public float buyingDiscountPercentage;
            public int levelToUnlock;
        }

        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }

        public string GetShopName()
        {
            return shopName;
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            Dictionary<InventoryItem, float> prices = GetPrices();
            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();

            foreach(InventoryItem item in availabilities.Keys)
            {
                if(availabilities[item] <= 0) 
                {
                    continue;
                }

                float price = prices[item];

                transaction.TryGetValue(item, out int quantityInTransaction);

                int availability = availabilities[item];

                yield return new ShopItem(item, availability, price, quantityInTransaction);
            }
        }

        public IEnumerable<ShopItem> GetFilteredItems() 
        {
            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();

                if(filter == ItemCategory.None || item.GetItemCategory() == filter)
                {
                    yield return shopItem;
                }
            }
        }

        public void SelectFilter(ItemCategory category) 
        { 
            filter = category;
            onChange?.Invoke();
        }

        public ItemCategory GetFilter() 
        { 
            return filter; 
        }

        public void SelectMode(bool isBuying) 
        { 
            isBuyingMode = isBuying;
            onChange?.Invoke();
        }

        public bool IsBuyingMode() 
        { 
            return isBuyingMode;     
        }

        public bool HasSufficientFunds()
        {
            if(!IsBuyingMode())
            {
                return true;
            } 

            Purse shopperPurse = currentShopper.GetComponent<Purse>();

            if(shopperPurse == null) 
            {
                return false;
            }

            return shopperPurse.GetBalance() >= TransactionTotal();
        }

        public bool HasInventorySpace()
        {
            if(!IsBuyingMode()) 
            {
                return true;
            }
            
            List<InventoryItem> flatItems = new();
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();

            if(shopperInventory == null) 
            {
                return false;
            }

            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();

                for(int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }

        public bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }

        public float TransactionTotal() 
        { 
            float total = 0;

            foreach(ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }

            return total;
        }

        public bool CanTransact() 
        { 
            if(IsTransactionEmpty()) 
            {
                return false;
            }

            if(!HasSufficientFunds()) 
            {
                return false;
            }

            if(!HasInventorySpace()) 
            {
                return false;
            }

            return true;
        }

        public void AddToTransaction(InventoryItem item, int quantity) 
        { 
            if(!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            int availability = GetAvailabilities()[item];

            if(transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
            {
                transaction[item] += quantity;
            }

            if(transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            onChange?.Invoke();
        }

        public void ConfirmTransaction() 
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();

            if(shopperInventory == null || shopperPurse == null) 
            {
                return;
            }

            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                float price = shopItem.GetPrice();

                for(int i = 0; i < quantity; i++)
                {
                    if(IsBuyingMode())
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }

                }
            }

            onChange?.Invoke();
        }

        void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if(shopperPurse.GetBalance() < price) 
            {
                return;
            }

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);

            if(success)
            {
                AddToTransaction(item, -1);

                if(!stockSold.ContainsKey(item))
                {
                    stockSold[item] = 0;
                }

                stockSold[item]++;
                shopperPurse.UpdateBalance(-price);
            }
        }

        void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);

            if(slot == -1) 
            {
                return;
            }

            AddToTransaction(item, -1);

            shopperInventory.RemoveFromSlot(slot, 1);

            if(!stockSold.ContainsKey(item))
            {
                stockSold[item] = 0;
            }

            stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }

        int CountItemsInInventory(InventoryItem item)
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();

            if(shopperInventory == null)
            {
                return 0;
            } 

            int total = 0;

            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if(shopperInventory.GetItemInSlot(i) == item)
                {
                    total += shopperInventory.GetNumberInSlot(i);
                }
            }

            return total;
        }

        int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if(shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }

            return -1;
        }

        Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();

            foreach(var config in GetAvailableConfigs())
            {
                if(IsBuyingMode())
                {
                    if(!availabilities.ContainsKey(config.item))
                    {
                        stockSold.TryGetValue(config.item, out int soldAmount);

                        availabilities[config.item] = -soldAmount;
                    }

                    availabilities[config.item] += config.initialStock;
                }
                else
                {
                    availabilities[config.item] = CountItemsInInventory(config.item);
                }
            }

            return availabilities;
        }

        Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();

            foreach(var config in GetAvailableConfigs())
            {
                if(IsBuyingMode())
                {
                    if(!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = config.item.GetPrice() * GetBarterDiscount();
                    }

                    prices[config.item] *= 1 - config.buyingDiscountPercentage / 100;
                }
                else
                {
                    prices[config.item] = config.item.GetPrice() * (sellingDiscountPercentage / 100);
                }
            }

            return prices;
        }

        float GetBarterDiscount()
        {
            BaseStats baseStats = currentShopper.GetComponent<BaseStats>();
            float discount = baseStats.GetStat(Stat.BuyingDiscountPercentage);
            return 1 - Mathf.Min(discount, maximumBarterDiscount) / 100;
        }

        IEnumerable<StockItemConfig> GetAvailableConfigs()
        {      
            foreach(var config in stockConfig)
            {
                if(config.levelToUnlock > GetShopperLevel()) continue;
                
                yield return config;
            }
        }

        int GetShopperLevel()
        {
            BaseStats stats = currentShopper.GetComponent<BaseStats>();

            if(stats == null) 
            {
                return 0;
            }

            return stats.GetLevel();
        }

        bool IRaycastable.HandleRaycast(PlayerController callingController)
        {
            if(!raycastable)
            {
                return false;
            } 

            if(Input.GetKeyDown(callingController.GetInteractionKey()))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }

            return true;
        }

        CursorType IRaycastable.GetCursorType()
        {
            return CursorType.Shop;
        }

        object ISaveable.CaptureState()
        {
            Dictionary<string, int> saveObject = new();

            foreach(var pair in stockSold)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }

            return saveObject;
        }

        void ISaveable.RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>) state;

            stockSold.Clear();

            foreach(var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
        }
    }
}