using System;
using System.Collections.Generic;
using GameDevTV.Saving;
using UnityEngine;

namespace RPG.Control
{
    public class InputReader : MonoBehaviour, ISaveable
    {
        [SerializeField] InputAction[] inputActions;
        Dictionary<PlayerAction, KeyCode> keyLookup;
        public event Action onChange;

        public static InputReader GetPlayerInputReader()
        {
            return GameObject.FindWithTag("Player").GetComponent<InputReader>();
        }

        public IEnumerable<KeyValuePair<PlayerAction, KeyCode>> GetActionPairs()
        {
            if(keyLookup == null)
            {
                BuildLookup();
            }

            return keyLookup;
        }

        public KeyCode GetKeyCode(PlayerAction action)
        {
            if(keyLookup == null)
            {
                BuildLookup();
            }

            if(!isActiveAndEnabled)
            {
                return KeyCode.None;
            }

            if(!keyLookup.ContainsKey(action)) 
            {
                Debug.LogError($"Keybind for Input Action: {action} not found");
                return default;
            }

            return keyLookup[action];
        }

        public void SetKeyCode(PlayerAction action, KeyCode keyCode)
        {   
            if(keyLookup == null)
            {
                BuildLookup();
            }

            if(!keyLookup.ContainsKey(action)) 
            {
                Debug.LogError($"Keybind for Input Action: {action} not found");
                return;
            }
            
            keyLookup[action] = keyCode;
            onChange?.Invoke();
        }

        public bool HasRepeatedKeyCode(KeyCode keyCode)
        {
            if(keyLookup == null)
            {
                BuildLookup();
            }

            HashSet<KeyCode> usedKeyCodes = new HashSet<KeyCode>();
            foreach(KeyCode candidate in keyLookup.Values)
            {
                if(candidate != KeyCode.None && !usedKeyCodes.Add(candidate))
                {
                    if(candidate == keyCode)
                    {
                        return true;
                    }
                }
            }
            return false;
        }       

        [System.Serializable]
        struct InputAction
        {
            public PlayerAction action;
            public KeyCode keyCode;
        }

        void BuildLookup()
        {
            keyLookup = new();

            foreach(var inputAction in inputActions)
            {
                if(keyLookup.ContainsKey(inputAction.action))
                {
                    Debug.LogError($"Duplicated Input Action for {inputAction.action} & {inputAction.keyCode}");
                }

                keyLookup[inputAction.action] = inputAction.keyCode;
            }
        }

        object ISaveable.CaptureState()
        {
            var saveObject = new Dictionary<int, int>();

            foreach(var pair in keyLookup)
            {
                saveObject[(int) pair.Key] = (int) pair.Value;
            }

            return saveObject;
        }

        void ISaveable.RestoreState(object state)
        {
            var saveObject = (Dictionary<int, int>) state;

            keyLookup = new Dictionary<PlayerAction, KeyCode>();

            foreach(var pair in saveObject)
            {
                keyLookup[(PlayerAction) pair.Key] = (KeyCode) pair.Value;
            }

            onChange?.Invoke();
        }
    }
}