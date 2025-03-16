using UnityEngine;
using RPG.Utils.UI;

namespace RPG.UI.Inventories
{
    [RequireComponent(typeof(IItemHolder))]
    public class ItemTooltipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            var item = GetComponent<IItemHolder>().GetItem();

            if(item == null) 
            {
                return false;
            }

            return true;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            var itemTooltip = tooltip.GetComponent<ItemTooltip>();

            if(itemTooltip == null) 
            {
                return;
            }

            var item = GetComponent<IItemHolder>().GetItem();

            if(item == null) 
            {
                return;
            }

            itemTooltip.Setup(item);
        }
    }
}