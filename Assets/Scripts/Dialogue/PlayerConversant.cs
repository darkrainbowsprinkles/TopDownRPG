using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPG.Utils;
using RPG.Control;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName = "";
        Dialogue currentDialogue;
        DialogueNode currentNode;
        AIConversant currentConversant;
        bool isChoosing = false;
        public event Action onConversationUpdated;

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            GetComponent<PlayerController>().ToggleInput(false);

            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = GetRootNode();

            TriggerEnterAction();

            onConversationUpdated?.Invoke();
        }

        public void Quit()
        {
            GetComponent<PlayerController>().ToggleInput(true);

            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            currentConversant = null;

            TriggerExitAction();

            onConversationUpdated?.Invoke();
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if(currentNode != null) 
            {
                return currentNode.GetText();
            }

            return "";
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            if(!HasNext())
            {
                Quit();
                return;
            }

            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();

            if(numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated?.Invoke();
                return;
            }

            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            
            TriggerExitAction();

            if(children.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, children.Count());
                currentNode = children[randomIndex];
            }

            TriggerEnterAction();

            onConversationUpdated?.Invoke();
        }

        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        public string GetCurrentConversantName()
        {
            if(isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetName();
            }
        }

        DialogueNode GetRootNode()
        {
            foreach(var rootNode in FilterOnCondition(currentDialogue.GetRootNodes()))
            {
                return rootNode;
            }

            return null;
        }

        IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach(var node in inputNode)
            {
                if(node.CheckCondition(GetComponents<IPredicateEvaluator>()))
                {
                    yield return node;
                }
            }
        }

        void TriggerEnterAction()
        {
            if(currentNode != null)
            {
                foreach(var action in currentNode.GetOnEnterActions())
                {
                    action.PerformAction(currentConversant.GetComponents<IActionPerfomer>());
                }
            }
        }

        void TriggerExitAction()
        {
            if(currentNode != null)
            {
                foreach(var action in currentNode.GetOnExitActions())
                {
                    action.PerformAction(currentConversant.GetComponents<IActionPerfomer>());
                }
            }
        }
    }
}