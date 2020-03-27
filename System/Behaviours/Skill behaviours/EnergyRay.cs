using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace VovTech.Behaviours.Skills
{
    public class EnergyRay : SkillBehaviour
    {
        public override void Initialize()
        {
            Stats["Duration"].AddModifier(4);
            Id = "EnergyRay";
            GameObject energyRay = Resources.Load<GameObject>("VFX/SkillEffects/Complete/EnergyRay");
            GameObject explosion = Resources.Load<GameObject>("VFX/Explosions/DistortExplosionBlue");

            Assert.AreEqual((explosion != null), true, "Failed to load DistortExplosionBlue.prefab");
            Assert.AreEqual((energyRay != null), true, "Failed to load EnergyRay.prefab");

            OnCast.Context = delegate
            {
                GameObject energyRayGo = Instantiate(energyRay, 
                    Caster.transform.position + Caster.transform.forward + new Vector3(0, 0.7f, 0), 
                    Caster.transform.rotation);
                energyRayGo.transform.parent = Caster.transform;
                Caster.DelayedInvoke(() =>
                {
                    RaySpawner spawner = energyRayGo.AddComponent<RaySpawner>();
                    spawner.RayLength = 15;
                    explosion.GetComponentInChildren<DamageZone>().ZoneFraction = Caster.ActorFraction;
                    spawner.ObjectsToSpawn.Add(explosion);
                    spawner.RotateToDirection = true;
                    spawner.Interval = 0.1f;
                    spawner.CustomRay = new Ray(energyRayGo.transform.position, energyRayGo.transform.forward);
                    spawner.UpdateAction = delegate
                    {
                        spawner.CustomRay = new Ray(energyRayGo.transform.position, energyRayGo.transform.forward);
                        Debug.DrawRay(spawner.CustomRay.origin, spawner.CustomRay.direction, Color.red, 0.1f);
                    };
                    spawner.Enabled = true;
                    Caster.DelayedInvoke(() =>
                    {
                        spawner.Enabled = false;
                    }, 2f);
                }, 2f);
            };
            PostInit();
        }
    }
}