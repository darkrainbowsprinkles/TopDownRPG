using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(menuName = "RPG/Dialogues/New Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector] List<DialogueNode> nodes = new();
        Vector2 newNodeOffset = new(250, 0);
        Dictionary<string, DialogueNode> nodeLookup = new();

        public IEnumerable<DialogueNode> GetAllNodes()
        {
            return nodes;
        }

        public IEnumerable<DialogueNode> GetRootNodes()
        {
            foreach(DialogueNode node in GetAllNodes())
            {
                bool isChild = false;

                foreach(DialogueNode lookupNode in GetAllNodes())
                {
                    if(lookupNode.GetChildren().Contains(node.name))
                    {
                        isChild = true;
                        break;
                    }
                }

                if(!isChild) 
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode)
        {
            foreach(string childID in parentNode.GetChildren())
            {
                if(nodeLookup.ContainsKey(childID)) 
                {
                    yield return nodeLookup[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach(DialogueNode node in GetAllChildren(currentNode))
            {
                if(node.IsPlayerSpeaking()) 
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode> GetAIChildren(DialogueNode currentNode)
        {
            foreach(DialogueNode node in GetAllChildren(currentNode))
            {
                if(!node.IsPlayerSpeaking()) 
                {
                    yield return node;
                }
            }
        }

# if UNITY_EDITOR
        public void CreateNode(DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }


        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }
# endif

        void OnValidate()
        {
            nodeLookup.Clear();

            foreach(DialogueNode node in GetAllNodes())
            {
                if(node != null)
                {
                    nodeLookup[node.name] = node;
                }
            }
        }

        void Awake()
        {
            OnValidate();
        }

        void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach(DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }

        DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();

            if(parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            return newNode;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
# if UNITY_EDITOR
            if(nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }

            if(AssetDatabase.GetAssetPath(this) != "")
            {
                foreach(DialogueNode node in GetAllNodes())
                {
                    if(AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
# endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { }
    }
}
