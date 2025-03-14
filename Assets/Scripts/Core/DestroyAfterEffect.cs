using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestroy;
        new ParticleSystem particleSystem;

        void Awake()
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            if(!particleSystem.IsAlive())
            {
                if(targetToDestroy != null)
                {
                    Destroy(targetToDestroy);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}