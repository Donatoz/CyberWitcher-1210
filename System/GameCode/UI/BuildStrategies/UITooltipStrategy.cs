using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech.UI
{
    public class UITooltipStrategy : UIBuildStrategy
    {
        private GameObject tipGo;
        private string description;
        private Color color;
        private string title;

        public UITooltipStrategy(string title, string description, Color color)
        {
            tipGo = MainManager.Instance.LoadResource("UI/Tip") as GameObject;
            this.description = description;
            this.color = color;
            this.title = title;
        }

        public override UIEntity Build()
        {
            UITip tooltip = MainManager.Instance.Spawn(
                tipGo, null
                ).GetComponent<UITip>();
            tooltip.PopulateWithInfo(title, description, color);
            return tooltip;
        }
    }
}