using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech.UI
{
    public class DialogueTree : MonoBehaviour
    {
        public delegate void StateChange(DialogueNode newNode);
        [System.Serializable]
        public struct DialogueNode
        {
            public string Id;
            public string Title;
            public string Context;
            public Color NodeColor;
            public List<DialogueOption> Options;
            public string DialogueTrigger;
        }

        [System.Serializable]
        public struct DialogueOption
        {
            public string Context;
            public string NodeId;
        }

        public DialogueNode StartNode;
        public List<DialogueNode> Nodes = new List<DialogueNode>();
        public DialogueNode Current { get { return currentNode; } }
        public event StateChange OnNodeChange;

        private DialogueNode currentNode;

        private void Start()
        {
            currentNode = StartNode;
        }

        public bool MoveNext(string nextId)
        {
            DialogueNode nextNode = Nodes.Find(x => x.Id == nextId);
            if (!nextNode.Equals(default))
            {
                currentNode = nextNode;
                OnNodeChange?.Invoke(currentNode);
                return true;
            }
            return false;
        }

        public void ResetTree()
        {
            currentNode = StartNode;   
        }
    }
}