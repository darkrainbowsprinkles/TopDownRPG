using UnityEngine;
using TMPro;
using RPG.Attributes;

namespace RPG.UI
{
    public class ManaBar : MonoBehaviour
    {
        [SerializeField] RectTransform foreground;
        [SerializeField] TMP_Text manaText;
        Mana mana;

        void Awake()
        {
            mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
        }

        void Update()
        {
            foreground.localScale = new Vector3(mana.GetManaPercentage(), 1, 1);

            if(manaText != null)
            {
                manaText.text = $"{mana.GetCurrentMana():0} / {mana.GetMaxMana():0}";
            }
        }
    }
}