using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech.Behaviours
{
    public class BazookaShootingBehaviour : ShootingBehaviour
    {
        public override Action<Weapon> OnShoot
        {
            get
            {
                return delegate (Weapon weapon)
                {
                    weapon.VisualAttachments.Find((x) => x.Name == "Rocket").Reference.gameObject.SetActive(false);
                    weapon.DelayedInvoke(() =>
                    {
                        weapon.VisualAttachments.Find((x) => x.Name == "Rocket").Reference.gameObject.SetActive(true);
                    }, weapon.ItemStats["ReloadTime"].EffectiveValue);
                };
            }
        }

        public override Action<Weapon> OnStopShooting
        {
            get;
        }
    }
}