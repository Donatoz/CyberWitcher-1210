using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VovTech
{
    [CreateAssetMenu(fileName ="New script info", menuName ="VovTech/ScriptInfo", order =57 )]
    public class ScriptInfo : ScriptableObject
    {
        public string Id;
        public MonoScript Script;
    }
}