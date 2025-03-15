using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Core;
using RPG.Control;
using RPG.Audio;
using GameDevTV.Utils;
using System.Collections.Generic;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour, IRaycastable
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int sceneToLoad;
        [SerializeField] float fadeOutTime = 0.5f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeWaitTime = 0.5f;
        [SerializeField] float fadeInMusicTime = 3;
        [SerializeField] float fadeOutMusicTime = 3;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] Condition unlockCondition;
        IEnumerable<IPredicateEvaluator> predicates;

        void Awake()
        {
            predicates = GameObject.FindWithTag("Player").GetComponents<IPredicateEvaluator>();
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        }

        IEnumerator Transition()
        {
            if(sceneToLoad < 0) 
            { 
                Debug.LogError("Scene to load not set"); 
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            AudioManager audioManager = FindObjectOfType<AudioManager>();
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();

            ToggleControl(false);

            yield return fader.FadeOut(fadeOutTime);
            yield return audioManager.FadeOutMaster(fadeOutMusicTime);

            wrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            
            ToggleControl(false);

            wrapper.Load();

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            wrapper.Save();

            yield return audioManager.FadeInMaster(fadeInMusicTime);
            yield return new WaitForSeconds(fadeWaitTime);

            fader.FadeIn(fadeInTime);

            ToggleControl(true);

            Destroy(gameObject);
        }


        Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if(portal == this)
                {
                    continue;
                } 

                if(portal.destination != destination) 
                {
                    continue;
                }

                return portal;
            }

            return null;
        }

        void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");

            player.GetComponent<NavMeshAgent>().enabled = false;

            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;

            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        void ToggleControl(bool state)
        {
            PlayerController.GetPlayerController().ToggleInput(state);
            Cursor.visible = state;
        }

        bool IRaycastable.HandleRaycast(PlayerController callingController)
        {
            if(unlockCondition.Check(predicates))
            {
                if(Input.GetKeyDown(callingController.GetInteractionKey()))
                {
                    StartCoroutine(Transition());
                }
            }

            return true;
        }

        CursorType IRaycastable.GetCursorType()
        {
            if(unlockCondition.Check(predicates))
            {
                return CursorType.Door;
            }
            else
            {
                return CursorType.Locked;
            }
        }
    }
}

