using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech.UI
{
    public class UIDialogueStrategy : UIBuildStrategy
    {
        private GameObject dialogueGo;
        private DialogueTree.DialogueNode node;

        public UIDialogueStrategy(DialogueTree.DialogueNode node)
        {
            dialogueGo = MainManager.Instance.LoadResource("UI/Dialogue") as GameObject;
            this.node = node;
        }

        public override UIEntity Build()
        {
            UIDialogue dialogue = MainManager.Instance.Spawn(dialogueGo, null).GetComponent<UIDialogue>();
            dialogue.PopulateWithInfo(node);
            return dialogue;
        }
    }
}