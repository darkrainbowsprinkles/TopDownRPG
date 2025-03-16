using System;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RPG.Graphics
{
    public class GraphicsManager : MonoBehaviour, ISaveable
    {
        [SerializeField] VolumeProfile volumeProfile;
        [SerializeField] GraphicSettingConfig[] defaultSettings;
        Dictionary<GraphicSetting, Dictionary<Type, int>> graphicSettingsLookup = null;
        public event Action onRestored;

        [System.Serializable]
        struct GraphicSettingConfig
        {
            public GraphicSetting graphicSetting;
            public int value;
        }

        public IEnumerable<KeyValuePair<GraphicSetting, Dictionary<Type, int>>> GetGraphicPairs()
        {
            if(graphicSettingsLookup == null)
            {
                BuildLookup();
            }

            return graphicSettingsLookup;
        }

        public void SetGraphicSetting(GraphicSetting graphicSetting, int value)
        {
            if(graphicSettingsLookup == null)
            {
                BuildLookup();
            }

            if(!graphicSettingsLookup.ContainsKey(graphicSetting))
            {
                Debug.LogError($"Type for graphic setting: {graphicSetting} not found");
                return;
            }

            graphicSettingsLookup[graphicSetting][GetFromGraphicSetting(graphicSetting)] = value;
            SetGraphic(graphicSetting);
        }

        public int GetSelectedOption(GraphicSetting graphicSetting)
        {
            if(graphicSettingsLookup == null)
            {
                BuildLookup();
            }

            if(!graphicSettingsLookup.ContainsKey(graphicSetting))
            {
                Debug.LogError($"Type for graphic setting: {graphicSetting} not found");
                return -1;
            }

            return graphicSettingsLookup[graphicSetting][GetFromGraphicSetting(graphicSetting)];
        }

        void Start()
        {
            foreach(var config in defaultSettings)
            {
                SetGraphicSetting(config.graphicSetting, config.value);
            }
        }

        void BuildLookup()
        {
            graphicSettingsLookup = new Dictionary<GraphicSetting, Dictionary<Type, int>>();

            foreach(var config in defaultSettings)
            {
                var innerLookup = new Dictionary<Type, int>();

                innerLookup[GetFromGraphicSetting(config.graphicSetting)] = config.value;
                graphicSettingsLookup[config.graphicSetting] = innerLookup;
            }
        }

        void SetGraphic(GraphicSetting graphicSetting)
        {
            int selectedOption = GetSelectedOption(graphicSetting);

            switch(graphicSetting)
            {
                case GraphicSetting.FullScreen:
                    Screen.fullScreenMode = (FullScreenMode) selectedOption;
                    break;

                case GraphicSetting.VSync:
                    QualitySettings.vSyncCount = selectedOption;
                    break;

                case GraphicSetting.ShadowResolution:
                    QualitySettings.shadowResolution = (UnityEngine.ShadowResolution) selectedOption;
                    break;

                case GraphicSetting.Antialiasing:
                   QualitySettings.antiAliasing = selectedOption;
                   break;

                case GraphicSetting.Bloom:
                    if(!volumeProfile.TryGet(out Bloom bloom)) return;
                    bloom.intensity.value = selectedOption;
                    break;

                case GraphicSetting.DepthOfField:
                    if(!volumeProfile.TryGet(out DepthOfField depthOfField)) return;
                    depthOfField.mode.value = (DepthOfFieldMode) selectedOption;
                    break;
            }

        }

        Type GetFromGraphicSetting(GraphicSetting graphicSetting)
        {
            switch(graphicSetting)
            {
                case GraphicSetting.FullScreen:
                    return typeof(FullScreenMode);
    
                case GraphicSetting.VSync:
                    return typeof(VSyncMode);

                case GraphicSetting.ShadowResolution:
                    return typeof(UnityEngine.ShadowResolution);

                case GraphicSetting.Antialiasing:
                    return typeof(AntialiasingShortMode);

                case GraphicSetting.Bloom:
                    return typeof(BloomMode);

                case GraphicSetting.DepthOfField:
                    return typeof(DepthOfFieldMode);                
            }

            return null;
        }

        object ISaveable.CaptureState()
        {
            var saveObject = new Dictionary<int, int>();

            if(graphicSettingsLookup == null)
            {
                BuildLookup();
            }

            foreach(var pair in graphicSettingsLookup)
            {
                foreach(var innerPair in pair.Value)
                {   
                    saveObject[(int) pair.Key] = innerPair.Value;
                }
            }

            return saveObject;
        }

        void ISaveable.RestoreState(object state)
        {
            var saveObject = (Dictionary<int, int>) state;

            graphicSettingsLookup.Clear();

            foreach(var pair in saveObject)
            {
                var innerLookup = new Dictionary<Type, int>();
                var setting = (GraphicSetting) pair.Key;

                innerLookup[GetFromGraphicSetting(setting)] = pair.Value;

                graphicSettingsLookup[setting] = innerLookup;  

                SetGraphic(setting);
            }

            onRestored?.Invoke();
        }
    }
}
