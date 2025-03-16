using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] BaseStats stats;
        [SerializeField] TMP_Text levelText;

        void OnEnable()
        {   
            stats.onLevelUp += DisplayLevel;
        }

        void OnDisable()
        {
            stats.onLevelUp -= DisplayLevel;
        }

        void Start()
        {
            DisplayLevel();
        }

        void DisplayLevel()
        {
            levelText.text = $"Lvl: {stats.GetLevel()}";
        }
    }
}
