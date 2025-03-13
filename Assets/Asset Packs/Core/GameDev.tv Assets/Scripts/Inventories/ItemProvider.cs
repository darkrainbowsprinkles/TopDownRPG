using GameDevTV.Utils;
using UnityEngine;

namespace GameDevTV.Inventories
{
    public class ItemProvider : MonoBehaviour, IActionPerfomer
    {
        void GiveItemToPlayer(string itemID, int number)
        {
            Inventory playerInventory = Inventory.GetPlayerInventory();
            InventoryItem item = InventoryItem.GetFromID(itemID);
            playerInventory.AddToFirstEmptySlot(item, number);
        }

        void IActionPerfomer.PerformAction(EAction action, string[] parameters)
        {
            switch(action)
            {
                case EAction.GiveItem:
                    string itemID = parameters[0];
                    int number = int.Parse(parameters[1]);
                    GiveItemToPlayer(itemID, number);
                    break;
            }
        }
    }
}