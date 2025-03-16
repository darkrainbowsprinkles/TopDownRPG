using UnityEngine;
using TMPro;
using RPG.Inventories;

namespace RPG.UI.Inventories
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TMP_Text balanceText;
        Purse playerPurse;

        void Awake()
        {
            playerPurse = GameObject.FindWithTag("Player").GetComponent<Purse>();
        }

        void Update()
        {
            balanceText.text = $"{playerPurse.GetBalance():0}";
        }
    }
}