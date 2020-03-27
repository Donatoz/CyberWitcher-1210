using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace VovTech.UI
{
    public class UIDialogueOption : UIEntity
    {
        public UIDialogue Dialogue;
        public TextMeshProUGUI Context;
        public string NextNodeId;

        public void PopulateWithInfo(string context)
        {
            Context.text = context;
        }

        public void MoveNext()
        {
            if (NextNodeId == "None")
            {
                Dialogue.Exit();
                return;
            }
            Dialogue.MoveNext(NextNodeId);
        }
    }
}