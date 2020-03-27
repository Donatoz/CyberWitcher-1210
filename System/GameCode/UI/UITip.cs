using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VovTech.UI
{
    public class UITip : UIEntity
    {
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Title;

        public void PopulateWithInfo(string title, string desc, Color color)
        {
            Title.text = title;
            SetColor(color);
            Description.text = desc;
        }
    }
}