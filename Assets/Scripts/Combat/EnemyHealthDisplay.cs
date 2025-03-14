using UnityEngine;
using TMPro;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text text;
        Fighter fighter;

        void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        void Update()
        {
            if(fighter.GetCurrentTarget() == null)
            {
                text.enabled = false;
                return;
            }


            text.enabled = true;
            Health targetHealth = fighter.GetCurrentTarget();
            string health = string.Format("{0:0}/{1:0}", targetHealth.GetCurrentValue(), targetHealth.GetMaxValue());
            string characterClass = targetHealth.GetComponent<BaseStats>().GetCharacterClass().ToString() + ": ";
            text.text = $"{characterClass}: {health}";
        }
    }   
}
