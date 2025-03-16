using UnityEngine;
using TMPro;
using RPG.Attributes;

namespace RPG.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] RectTransform foreground;
        [SerializeField] TMP_Text healthText;

        void Update()
        {
            foreground.localScale = new Vector3(health.GetHealthPercentage(), 1, 1);

            if(healthText != null)
            {
                healthText.text = $"{health.GetCurrentHealth()} / {health.GetMaxHealth()}";
            }
        }
    }
}
