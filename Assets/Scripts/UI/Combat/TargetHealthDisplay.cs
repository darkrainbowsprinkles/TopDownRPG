using UnityEngine;
using TMPro;
using RPG.Attributes;
using RPG.Stats;
using RPG.Combat;

namespace RPG.UI.Combat
{
    public class TargetHealthDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        Fighter playerFighter;

        void Awake()
        {
            playerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        void Update()
        {
            Health target = playerFighter.GetCurrentTarget();

            if(target == null)
            {
                text.enabled = false;
                return;
            }

            text.enabled = true;
            text.text = $"{GetTargetClassText(target)}:{GetTargetHealthText(target)}";
        }

        string GetTargetHealthText(Health target)
        {
            return $"{target.GetCurrentHealth():0}/{target.GetMaxHealth():0}";
        }

        string GetTargetClassText(Health target)
        {
            BaseStats targetStats = target.GetComponent<BaseStats>();
            return targetStats.GetCharacterClass().ToString();
        }
    }   
}
