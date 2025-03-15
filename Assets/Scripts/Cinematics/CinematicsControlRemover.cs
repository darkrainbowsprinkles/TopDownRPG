using RPG.Control;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CinematicsControlRemover : MonoBehaviour
    {
        PlayableDirector playableDirector;
        PlayerController playerController;

        void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
            playerController = PlayerController.GetPlayerController();
        }

        void OnEnable()
        {
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;
        }

        void OnDisable()
        {
            playableDirector.played -= DisableControl;
            playableDirector.stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector playableDirector)
        {
            playerController.ToggleInput(false);
        }

        void EnableControl(PlayableDirector playableDirector)
        {
            playerController.ToggleInput(true);
        }
    }
}