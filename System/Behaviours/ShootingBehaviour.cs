using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech.Behaviours
{
    public abstract class ShootingBehaviour : ScriptableObject
    {
        public Actor WeaponOwner;
        public abstract Action<Weapon> OnShoot { get; }
        public abstract Action<Weapon> OnStopShooting { get; }
    }
}