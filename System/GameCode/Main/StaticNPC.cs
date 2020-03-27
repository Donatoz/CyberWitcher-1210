using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VovTech.UI;

namespace VovTech
{
    public class StaticNPC : Actor
    {
        public UIPressButtonTip SpeakTip;
        [SerializeField]
        private DialogueTree dialogue;
        [SerializeField]
        private GameObject dialogueGo;
        

        private void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            dialogue = GetComponent<DialogueTree>();
        }

        public void StartDialogue(Character player)
        {
            if(dialogue != null && dialogueGo == null)
            {
                UIDialogue dialogueGo = UIManager.Instance.BuildUIElement(
                    new UIDialogueStrategy(dialogue.Current),
                    player.transform.position
                        + player.transform.forward 
                        + player.transform.right
                        + new Vector3(0, 1, 0)).GetComponent<UIDialogue>();
                player.MovementModule.Enabled = false;
                dialogueGo.Attach(dialogue);
                dialogueGo.LookAtCamera(7);
                dialogueGo.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }
        }
    }
}