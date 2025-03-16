using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RPG.Utils;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        [SerializeField] bool isPlayerSpeaking = false;
        [SerializeField, TextArea] string text;
        [SerializeField] ActionConfig[] onEnterActions;
        [SerializeField] ActionConfig[] onExitActions;
        [SerializeField] Condition condition;
        [SerializeField, HideInInspector] Rect rect = new(0, 0, 200, 100);
        [SerializeField, HideInInspector] List <string> children = new();

        public Rect GetRect()
        {
            return rect;
        }

        public string GetText()
        {
            return text;
        }

        public List<string> GetChildren()
        {
            return children;
        }

        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }

        public ActionConfig[] GetOnEnterActions()
        {
            return onEnterActions;
        }

        public ActionConfig[] GetOnExitActions()
        {
            return onExitActions;
        }

        public bool CheckCondition(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return condition.Check(evaluators);
        }

# if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Moved Dialogue Node");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }

        public void SetText(string newText)
        {
            if(newText != text) 
            {
                Undo.RecordObject(this, "Updated Dialogue Text");
                text = newText;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childID)
        {
            Undo.RecordObject(this, "Linked Dialogue");
            children.Add(childID);
            EditorUtility.SetDirty(this);
        }

        public void RemoveChild(string childID)
        {
            Undo.RecordObject(this, "Unlinked Dialogue");
            children.Remove(childID);
            EditorUtility.SetDirty(this);
        }

        public void SetPlayerSpeaking(bool state)
        {
            Undo.RecordObject(this, "Changed Dialogue Speaker");
            isPlayerSpeaking = state;
            EditorUtility.SetDirty(this);
        }
# endif
    }
}
