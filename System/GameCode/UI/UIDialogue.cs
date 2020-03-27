using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace VovTech.UI
{
    public class UIDialogue : UIEntity
    {
        public TextMeshProUGUI Name;
        public TextMeshProUGUI NodeText;
        public GameObject OptionsContainer;

        private DialogueTree attachedTree;
        [SerializeField]
        private GameObject optionGo;

        public void Attach(DialogueTree tree)
        {
            attachedTree = tree;
            attachedTree.OnNodeChange += PopulateWithInfo;
        }

        private void OnDestroy()
        {
            attachedTree.OnNodeChange -= PopulateWithInfo;
        }

        public void MoveNext(string nextId)
        {
            attachedTree.MoveNext(nextId);
        }

        public void PopulateWithInfo(DialogueTree.DialogueNode node)
        {
            NodeText.text = "";
            foreach (Transform t in OptionsContainer.transform)
            {
                Destroy(t.gameObject);
            }
            Name.text = node.Title;
            NodeText.DOText(node.Context, 0.8f);
            if (node.Options != null && node.Options.Count > 0)
            {
                foreach (DialogueTree.DialogueOption option in node.Options)
                {
                    UIDialogueOption o = Instantiate(optionGo, OptionsContainer.transform).GetComponent<UIDialogueOption>();
                    o.NextNodeId = option.NodeId;
                    o.Dialogue = this;
                    o.PopulateWithInfo(option.Context);
                }
            }
            else Exit();
        }

        public void Exit()
        {
            GetComponent<Animator>().SetTrigger("Out");
            attachedTree.ResetTree();
            MainManager.Instance.LocalPlayer.ControlledCharacter.MovementModule.Enabled = true;
            Destroy(gameObject, 1);
        }
    }
}