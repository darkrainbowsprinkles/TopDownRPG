using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text experienceText;
        Experience experience;

        void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        void Update()
        {
            experienceText.text = $"XP: {experience.GetPoints()}";
        }
    }
}