using System.Collections;
using GameDevTV.Saving;
using UnityEngine;
using UnityEngine.Audio;

namespace RPG.Audio
{
    public class AudioManager : MonoBehaviour, ISaveable
    {
        [SerializeField] AudioMixer audioMixer;
        const string rootGroup = "Master";

        public AudioMixerGroup[] GetAudioMixerGroups()
        {
            return audioMixer.FindMatchingGroups(rootGroup);
        }

        public float GetVolume(string groupName)
        {
            if(!audioMixer.GetFloat(groupName, out float value))
            {
                return 0f;
            }

            return value;
        }

        public void SetVolume(string groupName, float value)
        {
            audioMixer.SetFloat(groupName, value);
        }

        public Coroutine FadeOutMaster(float time)
        {
            return StartCoroutine(FadeSnapshot("FadeOutMaster", time));
        }

        public Coroutine FadeInMaster(float time)
        {
            return StartCoroutine(FadeSnapshot("FadeInMaster", time));
        }
        
        public IEnumerator FadeSnapshot(string snapshotName, float time)
        {
            var snapshot = audioMixer.FindSnapshot(snapshotName);

            if(snapshot == null)
            {
                Debug.LogError($"Snapshot '{snapshotName}' not found");
                yield break;
            }

            snapshot.TransitionTo(time);

            yield return new WaitForSeconds(time);
        }

        object ISaveable.CaptureState()
        {
            var groups = GetAudioMixerGroups();
            var volumes = new float[groups.Length];

            for(int i = 0; i < groups.Length; i++)
            {
                volumes[i] = GetVolume(groups[i].name);
            }

            return volumes;
        }

        void ISaveable.RestoreState(object state)
        {
            var groups = GetAudioMixerGroups();
            var volumes = (float[])state;

            for(int i = 0; i < groups.Length; i++)
            {
                SetVolume(groups[i].name, volumes[i]);
            }
        }
    }
}
