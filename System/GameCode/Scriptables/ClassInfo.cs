using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    [CreateAssetMenu(fileName ="New Character Class", menuName ="VovTech/Character Class", order =60)]
    public class ClassInfo : ScriptableObject
    {
        public string Name;
        public float BaseHealth;
        public float BaseEnergy;
        public string[] SkillIds;
    }
}