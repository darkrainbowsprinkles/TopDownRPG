using UnityEngine;

namespace RPG.Audio
{
    public class SoundEffectPlayer : MonoBehaviour
    {
        [SerializeField] AudioClip[] soundEffects;
        [SerializeField] bool playOnStart = false;
        AudioSource audioSource;

        // Called in Unity Events
        public void PlaySoundEffect()
        {
            int randomIndex = Random.Range(0, soundEffects.Length);
            audioSource.PlayOneShot(soundEffects[randomIndex]);
            
        }

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            if(playOnStart)
            {
                PlaySoundEffect();
            }
        }
    }
}
