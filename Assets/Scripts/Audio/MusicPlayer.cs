using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] AudioClip defaultMusic;
        AudioSource audioSource;
        AudioManager audioManager;
        Dictionary<AudioClip, float> resumeTimes = new();

        public AudioClip GetDefaultMusic()
        {
            return defaultMusic;
        }

        public AudioClip GetCurrentMusic()
        {
            return audioSource.clip;
        }

        public IEnumerator FadeMusic(AudioClip music, float fadeOutTime, float fadeInTime)
        {
            yield return audioManager.FadeOutMusic(fadeOutTime);

            PlayMusic(music);

            yield return audioManager.FadeInMusic(fadeInTime);
        }

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            audioManager = FindObjectOfType<AudioManager>();

            PlayMusic(defaultMusic);
        }

        void PlayMusic(AudioClip music)
        {
            if(music == GetCurrentMusic())
            {
                return;
            }
            
            if(audioSource.clip != null)
            {
                resumeTimes[audioSource.clip] = audioSource.time;
            }

            if(resumeTimes.ContainsKey(music))
            {
                audioSource.time = resumeTimes[music];
            }

            audioSource.Stop();

            audioSource.clip = music;

            audioSource.Play();
        }
    }
}
