using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech.Levels
{
    public abstract class Level : ScriptableObject
    {
        public string Name;
        public Dictionary<string, Action> TriggerFunctions;
        public Dictionary<string, int> LevelVariables;

        public abstract void Initialize();
    }
}