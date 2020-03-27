using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "VovTech/Weapon", order = 50)]
    public class WeaponInfo : ItemInfo
    {
        [Header("Range")]
        public float Spreading;
        public float ShotInterval;
        public int ClipSize;
        public float ReloadTime;
        public float Stabilization;
        public float Recoil;
        public float MaxRecoil;
        public WeaponDamageType DamageType;
        public WeaponType WeaponType;
        public ProjectileInfo Projectile;
        public GameObject ShootEffect;
        public WeaponHoldingType HoldingType;
        public List<AudioClip> ShootSounds;
        public GameObject Prefab;
        [Range(0f, 1f)]
        public float ShootVolume;
        public List<AudioClip> ReloadSounds;
        [Range(0f, 1f)]
        public float ReloadVolume;
        [Header("Melee")]
        public float HitDamage;
        public float AttackInterval;
    }
}