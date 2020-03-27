using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class ColorTemplates : MonoBehaviour
    {
        public static ColorTemplates Get => GameObject.Find("Templates").GetComponent<ColorTemplates>();

        [ColorUsage(true, true)]
        public Color BrightRed;
        [ColorUsage(true, true)]
        public Color NormalRed;
        [ColorUsage(true, true)]
        public Color BrightCyan;
        [ColorUsage(true, true)]
        public Color NormalCyan;
        [ColorUsage(true, true)]
        public Color BrightGreen;
    }
}