using UnityEngine;
using TMPro;

namespace RPG.UI.Combat
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TMP_Text damageText;

        public void DestroyText()
        {
            Destroy(gameObject);
        }

        public void SetValue(float amount)
        {
            damageText.text = string.Format("{0:0}", amount);
        }
    }
}
