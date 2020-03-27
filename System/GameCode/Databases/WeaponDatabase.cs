using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class WeaponDatabase : Database
    {
        static WeaponDatabase instance;

        private WeaponDatabase() { }

        public static WeaponDatabase GetInstance()
        {
            if (instance == null)
                instance = new WeaponDatabase();
            return instance;
        }

        public WeaponInfo GetWeapon(int id)
        {
            WeaponInfo[] weapons = GetAllInstances<WeaponInfo>();
            foreach (WeaponInfo w in weapons)
            {
                if (w.Id == id) return w;
            }
            return null;
        }

        public override int GetInstancesAmount()
        {
            return GetAllInstances<WeaponInfo>().Length;
        }
    }
}