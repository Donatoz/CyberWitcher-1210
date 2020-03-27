using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VovTech.UI
{
    public class UIWeaponPrefix : UIEntity
    {
        public TextMeshProUGUI Description;
        public Image Glow;
        public Image Icon;

        public void PopulateWithInfo(StatAffector affector)
        {
            Description.text = affector.Description;
            Description.color = affector.UIColor;
            Glow.color = affector.UIColor;
            Icon.sprite = affector.Icon;
            Icon.color = affector.UIColor;
        }
    }
}