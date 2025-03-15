using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    [RequireComponent(typeof(PlayableDirector))]
    public class CinematicTrigger : MonoBehaviour
    {
        bool alreadyTriggered = false;

        void OnTriggerEnter(Collider other)
        {
            if(!alreadyTriggered && other.CompareTag("Player"))
            {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
            }
        }
    }
}
