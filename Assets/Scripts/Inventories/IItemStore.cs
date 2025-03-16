namespace RPG.Inventories
{
    public interface IItemStore
    {
        int AddItems(InventoryItem item, int number);
    }
}