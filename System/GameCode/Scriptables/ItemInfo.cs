using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public abstract class ItemInfo : ScriptableObject
    {
        public string Name;
        public int Id;
        public float Weight;
        public float Price;
    }
}