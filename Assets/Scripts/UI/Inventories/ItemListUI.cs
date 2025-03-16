using UnityEngine;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    public class ItemListUI : MonoBehaviour
    {
        [SerializeField] Transform container;
        [SerializeField] Transform listRoot;
        [SerializeField] ItemRowUI itemRowPrefab;
        Inventory playerInventory;

        void Awake()
        {
            playerInventory = Inventory.GetPlayerInventory();
        }

        void Start()
        {
            container.gameObject.SetActive(false);

            foreach(Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            playerInventory.onItemAdded += AddItem;
        }

        void AddItem(InventoryItem item, int number)
        {
            container.gameObject.SetActive(true);
            var itemRowInstance = Instantiate(itemRowPrefab, listRoot);
            itemRowInstance.Setup(item, number);
            itemRowInstance.onRemoved += CheckIfEmpty;
        }

        void CheckIfEmpty()
        {
            container.gameObject.SetActive(listRoot.childCount > 1);
        }
    }
}