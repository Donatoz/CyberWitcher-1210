using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech.UI
{
    public class UIPressButtonStrategy : UIBuildStrategy
    {
        private GameObject buildGo;
        private string buttonValue;
        private string afterText;
        private KeyCode key;

        public UIPressButtonStrategy(string buttonValue, string afterText, KeyCode key)
        {
            buildGo = MainManager.Instance.LoadResource("UI/PressTooltip") as GameObject;
            this.buttonValue = buttonValue;
            this.afterText = afterText;
            this.key = key;
        }

        public override UIEntity Build()
        {
            UIPressButtonTip topGo = MainManager.Instance.Spawn(buildGo, null).GetComponent<UIPressButtonTip>();
            topGo.PopulateWithInfo(buttonValue, afterText, key);
            return topGo;
        }
    }
}