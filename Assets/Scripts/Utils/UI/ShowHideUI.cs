using RPG.Control;
using UnityEngine;

namespace RPG.Utils.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] PlayerAction toggleAction;
        [SerializeField] GameObject uiContainer;
        InputReader inputReader;

        public void Toggle()
        {
            uiContainer.SetActive(!uiContainer.activeSelf);
        }

        void Awake()
        {
            inputReader = InputReader.GetPlayerInputReader();
        }

        void Start()
        {
            uiContainer.SetActive(false);
        }

        void Update()
        {
            if(Input.GetKeyDown(inputReader.GetKeyCode(toggleAction)))
            {
                Toggle();
            }
        }
    }
}