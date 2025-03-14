using RPG.Audio;
using UnityEngine;

namespace RPG.UI.Menus
{
    public class AudioMenuUI : MonoBehaviour
    {
        [SerializeField] Transform listRoot;
        [SerializeField] AudioRowUI audioRowPrefab;
        AudioManager audioManager;
 
        void Start()
        {
            audioManager = FindObjectOfType<AudioManager>();
            Redraw();
        }   

        void Redraw()
        {
            foreach(Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach(var audioGroup in audioManager.GetAudioMixerGroups())
            {
                var rowInstance = Instantiate(audioRowPrefab, listRoot);
                rowInstance.Setup(audioManager, audioGroup.name);
            }
        }
    }
}
