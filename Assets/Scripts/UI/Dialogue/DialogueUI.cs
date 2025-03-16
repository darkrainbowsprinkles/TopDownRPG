using System.Collections;
using RPG.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.UI.Dialogue
{
    public class DialogueUI : MonoBehaviour, IPointerDownHandler
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI AIText;
        [SerializeField] Button nextButton;
        [SerializeField] GameObject AIResponse;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;
        [SerializeField] TextMeshProUGUI conversantName;
        [SerializeField] float displayDelay = 0.03f;
        Coroutine displayCoroutine = null;
        bool skipDelay = false;

        void Start()
        {
            playerConversant = GameObject.FindWithTag("Player").GetComponent<PlayerConversant>();

            playerConversant.onConversationUpdated += UpdateUI;

            nextButton.onClick.AddListener(() => playerConversant.Next());

            UpdateUI();
        }

        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());

            if(!playerConversant.IsActive())
            {
                return;
            }

            conversantName.text = playerConversant.GetCurrentConversantName();

            AIResponse.SetActive(!playerConversant.IsChoosing());
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());

            if(playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                DisplayResponse();
            }
        }

        void DisplayResponse()
        {
            if(displayCoroutine != null)
            {
                StopCoroutine(displayCoroutine);
            }

            AIText.text = "";

            skipDelay = false;

            displayCoroutine = StartCoroutine(AITextRoutine());
        }

        IEnumerator AITextRoutine()
        {
            nextButton.gameObject.SetActive(false);

            for (int i = 0; i < playerConversant.GetText().Length; i++)
            {
                if(skipDelay)
                {
                    AIText.text = playerConversant.GetText();
                    break;
                }

                AIText.text += playerConversant.GetText()[i];
                yield return new WaitForSeconds(displayDelay);
            }

            nextButton.gameObject.SetActive(true);
        }

        void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);
            }

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var textComponent = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComponent.text = choice.GetText();

                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => 
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if(displayCoroutine != null)
            {
                skipDelay = true;
            }
        }
    }
}