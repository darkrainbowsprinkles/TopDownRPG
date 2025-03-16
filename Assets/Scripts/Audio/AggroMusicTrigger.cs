using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Audio
{
    [RequireComponent(typeof(SphereCollider))]
    public class AggroMusicTrigger : MonoBehaviour
    {
        [SerializeField] AudioClip aggroMusic;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 0.5f;
        List<AIController> enemies = new();
        MusicPlayer musicPlayer;

        void Start()
        {
            musicPlayer = FindObjectOfType<MusicPlayer>();
        }

        void Update()
        {
            bool aggravated = CheckIfAggravated();

            if(aggravated && musicPlayer.GetCurrentMusic() != aggroMusic)
            {
                StartCoroutine(musicPlayer.FadeMusic(aggroMusic, fadeOutTime, fadeInTime));
            }
            else if(!aggravated && musicPlayer.GetCurrentMusic() == aggroMusic)
            {
                StartCoroutine(musicPlayer.FadeMusic(musicPlayer.GetDefaultMusic(), fadeOutTime, fadeInTime));
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out AIController controller))
            {
                if(enemies.Contains(controller))
                {
                    return;
                }

                Health health = controller.GetComponent<Health>();

                if(health.IsDead())
                {
                    return;
                }

                enemies.Add(controller);

                health.onDie.AddListener(() => enemies.Remove(controller));
            }
        }

        void OnTriggerExit(Collider other)
        {
            if(other.TryGetComponent(out AIController controller))
            {
                Health health = controller.GetComponent<Health>();

                enemies.Remove(controller);
                
                health.onDie.RemoveListener(() => enemies.Remove(controller));
            }
        }

        bool CheckIfAggravated()
        {
            foreach(var enemie in enemies)
            {
                if(enemie.IsAggravated())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
