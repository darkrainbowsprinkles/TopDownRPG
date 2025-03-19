using System;
using System.Collections;
using System.Text.RegularExpressions;
using RPG.Control;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Menus
{
    public class KeyBindRowUI : MonoBehaviour
    {
        [SerializeField] TMP_Text actionText;
        [SerializeField] Button keyBindButton;
        InputReader inputReader;
        PlayerAction playerAction;
        KeyCode keyCode;
        GameObject choosingText;

        public void Setup(InputReader inputReader, PlayerAction playerAction, KeyCode keyCode, GameObject choosingText)
        {
            this.inputReader = inputReader;
            this.playerAction = playerAction;
            this.keyCode = keyCode;
            this.choosingText = choosingText;
            Redraw();
        }

        void Start()
        {
            keyBindButton.onClick.AddListener(Rebind);
        }

        void Redraw()
        {
            string text = playerAction.ToString();
            string formattedText = Regex.Replace(text, @"([a-z])([A-Z0-9])", "$1 $2");
            TMP_Text buttonText = keyBindButton.GetComponentInChildren<TMP_Text>();

            actionText.text = formattedText;
            buttonText.text = keyCode.ToString();
            choosingText.SetActive(false);

            if(inputReader.HasRepeatedKeyCode(keyCode))
            {
                buttonText.color = Color.red;
            }   
        }

        void Rebind()
        {
            StartCoroutine(WaitForKeyPress());
        }

        IEnumerator WaitForKeyPress()
        {
            inputReader.enabled = false;
            keyBindButton.interactable = false;
            choosingText.SetActive(true);
            
            while(true)
            {
                foreach(KeyCode candidate in Enum.GetValues(typeof(KeyCode)))
                {
                    if(Input.GetKeyDown(candidate))
                    {
                        inputReader.enabled = true;
                        keyBindButton.interactable = true; 
                        inputReader.SetKeyCode(playerAction, candidate);       
                        yield break;
                    }
                }
                
                yield return null;
            }
        }
    }
}
