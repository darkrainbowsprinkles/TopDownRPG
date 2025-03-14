using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace RPG.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] AudioMixer audioMixer;
        const string rootGroup = "Master";

        public IEnumerable<AudioMixerGroup> GetAudioMixerGroups()
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
    }
}
