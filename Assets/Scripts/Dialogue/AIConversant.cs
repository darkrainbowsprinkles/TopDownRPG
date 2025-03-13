using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] string conversantName = "";

        public Dialogue GetDialogue()
        {
            return dialogue;
        }

        public string GetDialogueName()
        {
            return dialogue.name;
        }

        public string GetName()
        {
            return conversantName;
        }

        public void ShowDialogue(PlayerController callingController)
        {
            callingController.GetComponent<PlayerConversant>().StartDialogue(this, dialogue);
        }

        bool IRaycastable.HandleRaycast(PlayerController callingController)
        {
            if(TryGetComponent(out Health health))
            {
                if(health.IsDead) 
                {
                    return false;
                } 
            }

            if(dialogue == null)
            {
                return false;
            }

            if(Input.GetKeyDown(callingController.GetInteractionKey()))
            {
                ShowDialogue(callingController);
            }

            return true;
        }

        CursorType IRaycastable.GetCursorType()
        {
            return CursorType.Dialogue;
        }
    }
}