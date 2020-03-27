using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    [System.Serializable]
    public struct StatAffector
    {
        public string Description;
        public Sprite Icon;
        public string StatToAffect;
        public Color UIColor;
        public float Value;

        public StatAffector(string desc, Sprite icon, string stat, Color color, float val)
        {
            Description = desc;
            Icon = icon;
            StatToAffect = stat;
            UIColor = color;
            Value = val;
        }
    }
}