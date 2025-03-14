using System.Collections;
using UnityEngine;

namespace RPG.Audio
{
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] float fadeOutMusicTime = 1;
        [SerializeField] float fadeInMusicTime = 0.5f;
        [SerializeField] MusicConfig ambientMusic;
        [SerializeField] MusicConfig combatMusic;
        AudioSource audioSource;
        AudioManager audioManager;
        MusicConfig currentMusic;
        
        [System.Serializable]
        struct MusicConfig
        {
            public AudioClip clip;
            [HideInInspector] public float resumeTime;
        }

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            audioManager = FindObjectOfType<AudioManager>();
            PlayMusic(ambientMusic);
        }

        IEnumerator FadeInMusicRoutine(MusicConfig music)
        {
            yield return audioManager.FadeSnapshot("FadeOutMusic", fadeOutMusicTime);
            PlayMusic(music);
            yield return audioManager.FadeSnapshot("FadeInMusic", fadeInMusicTime);
        }

        void PlayMusic(MusicConfig music)
        {
            currentMusic.resumeTime = audioSource.time;
            audioSource.Stop();
            audioSource.clip = music.clip;
            audioSource.time = music.resumeTime;
            audioSource.Play();
        }
    }
}
