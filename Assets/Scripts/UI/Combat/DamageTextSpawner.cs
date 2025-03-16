using UnityEngine;

namespace RPG.UI.Combat
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab;

        public void Spawn(float damageAmount)
        {
            DamageText instance = Instantiate(damageTextPrefab, transform);
            instance.SetValue(damageAmount);
        }
    }
}
