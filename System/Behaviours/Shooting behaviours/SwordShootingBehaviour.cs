using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech.Behaviours
{
    public class SwordShootingBehaviour : ShootingBehaviour
    {
        private NavigationList<string> animationsList = new NavigationList<string>()
        {
            "SwordSlash1_MANIM", "SwordSlash2_MANIM", "SwordSlash3_MANIM"
        };

        private NavigationList<GameObject> effects = new NavigationList<GameObject>()
        {

        };

        private GameObject slash;
        private GameObject prick;

        private void OnEnable()
        {
            slash = Resources.Load<GameObject>("VFX/Melee/HorizontalSlash");
            prick = Resources.Load<GameObject>("VFX/Melee/Prick");
        }

        public override Action<Weapon> OnShoot
        {
            get
            {
                return delegate (Weapon weapon)
                {
                    WeaponOwner.Animator.ReplaceClip("_MANIM", Resources.Load<AnimationClip>($"Animations/{animationsList.MoveNext}"));
                    WeaponOwner.Animator.SetTrigger("Melee");
                    GameObject drive = WeaponOwner.transform.Find("Drive").gameObject;
                    Quaternion ownerRot = WeaponOwner.transform.rotation;
                    if (!WeaponOwner.GetComponent<ControlledMovementModule>().Sprinting)
                        ((MeleeWeapon)weapon).BusyTime += 0.4f;
                    else
                    {
                        ((MeleeWeapon)weapon).BusyTime += 0.1f;
                        WeaponOwner.GetComponent<ControlledMovementModule>().DriveTimer += 0.4f;
                    }
                    weapon.Owner.DelayedInvoke(() =>
                    {
                        GameObject toSpawn;
                        Quaternion rot;
                        Vector3 pos;
                        bool parent = false;
                        switch(animationsList.Current)
                        {
                            case "SwordSlash1_MANIM":
                                toSpawn = slash;
                                pos = WeaponOwner.transform.position + new Vector3(0, 1, 0);
                                rot = Quaternion.Euler(Vector3.zero);
                                parent = true;
                                break;
                            case "SwordSlash2_MANIM":
                                toSpawn = slash;
                                pos = WeaponOwner.transform.position + new Vector3(0, 1, 0);
                                rot = Quaternion.Euler(3, -7, -134);
                                parent = true;
                                break;
                            default:
                                toSpawn = prick;
                                pos = WeaponOwner.transform.position 
                                    + new Vector3(0, 0.6f, 0) 
                                    + WeaponOwner.transform.forward * 0.7f
                                    + WeaponOwner.transform.right * 0.6f;
                                rot = ownerRot;
                                break;
                        }
                        GameObject go;
                        if (parent)
                        {
                            go = Instantiate(toSpawn, weapon.Owner.transform);
                            go.transform.position = pos;
                            go.transform.localRotation = rot;
                        } else
                        {
                            go = Instantiate(toSpawn, pos, rot);
                        }
                        
                    }, 0.14f);
                };
            }
        }

        public override Action<Weapon> OnStopShooting
        {
            get;
        }
    }
}