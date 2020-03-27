using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class ProjectileDatabase : Database
    {
        static ProjectileDatabase instance;

        private ProjectileDatabase() { }

        public static ProjectileDatabase GetInstance()
        {
            if (instance == null)
                instance = new ProjectileDatabase();
            return instance;
        }

        public ProjectileInfo GetProjectile(int id)
        {
            ProjectileInfo[] projeciles = GetAllInstances<ProjectileInfo>();
            foreach (ProjectileInfo c in projeciles)
            {
                if (c.Id == id) return c;
            }
            return null;
        }

        public override int GetInstancesAmount()
        {
            return GetAllInstances<ProjectileInfo>().Length;
        }
    }
}