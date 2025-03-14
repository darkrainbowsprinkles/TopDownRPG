using System;
using System.Collections;
using Cinemachine;
using RPG.Attributes;
using RPG.Audio;
using RPG.Movement;
using RPG.SceneManagement;
using UnityEngine;

namespace RPG.Control
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] Transform respawnLocation;
        [SerializeField] float respawnDelay = 2;
        [SerializeField] float fadeTime = 0.5f;
        [SerializeField] float fadeMusicTime = 2f;
        [SerializeField][Range(0,100)] float playerHealthRegenPercentage = 20;
        [SerializeField][Range(0,100)] float enemyHealthRegenPercentage = 20;
        Health health;

        void Awake()
        {
            health = GetComponent<Health>();
            health.onDie.AddListener(Respawn);
        }

        void Start()
        {
            if(health.IsDead())
            {
                Respawn();
            }
        }

        void Respawn()
        {
            StartCoroutine(RespawnRoutine());
        }

        IEnumerator RespawnRoutine()
        {
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();

            yield return new WaitForSeconds(respawnDelay);

            Fader fader = FindObjectOfType<Fader>();
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            
            yield return fader.FadeOut(fadeTime);
            yield return audioManager.FadeOutMaster(fadeMusicTime);

            RespawnPlayer();
            ResetEnemies();

            savingWrapper.Save();

            yield return audioManager.FadeInMaster(fadeMusicTime);
            yield return fader.FadeIn(fadeTime);
        }

        void RespawnPlayer()
        {
            Vector3 positionDelta = respawnLocation.position - transform.position;

            GetComponent<Mover>().Teleport(respawnLocation.position);
            health.Heal(health.GetMaxValue() * playerHealthRegenPercentage / 100);
            var activeCamera = FindObjectOfType<CinemachineBrain>().ActiveVirtualCamera;

            if(activeCamera.Follow == transform)
            {
                activeCamera.OnTargetObjectWarped(transform, positionDelta);
            }
        }

        void ResetEnemies()
        {
            foreach(AIController enemyController in FindObjectsOfType<AIController>())
            {
                Health enemyHealth = enemyController.GetComponent<Health>();

                if(enemyHealth != null && !enemyHealth.IsDead())
                {
                    enemyHealth.Heal(health.GetMaxValue() * enemyHealthRegenPercentage / 100); 
                    enemyController.Reset();
                }
            }
        }
    }
}
