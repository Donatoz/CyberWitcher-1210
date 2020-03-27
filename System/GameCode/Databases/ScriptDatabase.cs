using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class ScriptDatabase : Database
    {
        static ScriptDatabase instance;

        private ScriptDatabase() { }

        public static ScriptDatabase GetInstance()
        {
            if (instance == null)
                instance = new ScriptDatabase();
            return instance;
        }

        public ScriptInfo GetScript(string id)
        {
            ScriptInfo[] scripts = GetAllInstances<ScriptInfo>();
            foreach (ScriptInfo s in scripts)
            {
                if (s.Id == id) return s;
            }
            return null;
        }

        public override int GetInstancesAmount()
        {
            return GetAllInstances<ScriptInfo>().Length;
        }
    }
}