using RPG.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Menus
{
    public class AudioRowUI : MonoBehaviour
    {
        [SerializeField] Slider volumeSlider;
        [SerializeField] TMP_Text audioSettingText;
        AudioManager audioManager;
        string groupName;

        public void Setup(AudioManager audioManager, string groupName)
        {
            this.audioManager = audioManager;
            this.groupName = groupName;
        }

        void Start()
        {
            volumeSlider.value = Mathf.Pow(10, audioManager.GetVolume(groupName) / 20f);
            audioSettingText.text = $"{groupName} volume";
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        void SetVolume(float volume)
        {
            float dbValue = volume > 0 ? Mathf.Log10(volume) * 20f : -80f;
            audioManager.SetVolume(groupName, dbValue);
        }
    }
}
