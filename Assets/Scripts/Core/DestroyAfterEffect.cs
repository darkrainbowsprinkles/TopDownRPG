using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        AudioSource audioSource;
        new ParticleSystem particleSystem;

        void Awake()
        {
            audioSource = GetComponentInChildren<AudioSource>();
            particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        void Update()
        {
            if(!particleSystem.IsAlive() && !audioSource.isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }
}