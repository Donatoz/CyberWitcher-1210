using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VovTech.Serialization;
using System;

namespace VovTech
{
    public class NetSpawner : MonoBehaviour
    {
        /// <summary>
        /// Object resouce, used to spawn some object when recieved object to spawn it.
        /// </summary>
        [System.Serializable]
        public struct ObjectResource
        {
            /// <summary>
            /// Object to spawn prefab.
            /// </summary>
            public GameObject Prefab;
            /// <summary>
            /// Custom path to find this object using packet spawn data.
            /// </summary>
            public string Path;
        }

        public List<ObjectResource> SpawnBuffer;


        public void SpawnObject(string path, SpawnData data)
        {
            ObjectResource obj = FindResource(path);
            if (!obj.Equals(default))
            {
                Instantiate(obj.Prefab, data.Position, Quaternion.Euler(data.Rotation));
            }
        }
        
        public void SpawnProjectile(string path, ProjectileSpawnData data)
        {
            ObjectResource projectile = FindResource(path);
            if (!projectile.Equals(default))
            {
                Projectile proj = Instantiate(projectile.Prefab, data.Position,
                Quaternion.Euler(data.Rotation)).GetComponent<Projectile>();
                proj.Direction = proj.transform.forward;
                proj.Settings = ProjectileDatabase.GetInstance().GetProjectile(data.DataId);
                proj.Initialize();
                proj.GetComponent<Rigidbody>().AddForce(proj.Direction * proj.ProjectileStats["Speed"].EffectiveValue);
                Debug.Log("Spawned bullet");
            }
        }

        private ObjectResource FindResource(string path)
        {
            return SpawnBuffer.Find(x => x.Path == path);
        }
    }
}