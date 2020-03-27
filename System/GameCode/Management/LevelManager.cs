using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VovTech.Levels;

namespace VovTech
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance => GameObject.Find("GameManagers").GetComponent<LevelManager>();

        public MonoScript LoadedLevelScript;
        public Level LoadedLevel;

        private void Start()
        {
            if (LoadedLevelScript != null) Invoke("LoadLevel", 0.4f);
        }

        private void LoadLevel()
        {
            LoadedLevel = ScriptableObject.CreateInstance(LoadedLevelScript.GetClass()) as Level;
            LoadedLevel.Initialize();
        }

        public void ActivateTrigger(string trigger)
        {
            if(LoadedLevel.TriggerFunctions.ContainsKey(trigger))
            {
                LoadedLevel.TriggerFunctions[trigger].Invoke();
            }
        }
    }
}