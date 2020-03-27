using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    [CreateAssetMenu(fileName ="New Projectile", menuName ="VovTech/Projectile", order =51)]
    public class ProjectileInfo : ScriptableObject
    {
        public string Name;
        public int Id;
        public GameObject Prefab;
        public float Speed;
        public float Heatlh;
        public GameObject DeathEffect;
    }
}