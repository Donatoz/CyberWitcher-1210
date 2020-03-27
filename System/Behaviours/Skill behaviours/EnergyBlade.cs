using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Assertions;

namespace VovTech.Behaviours.Skills
{
    public class EnergyBlade : SkillBehaviour
    {
        public override void Initialize()
        {
            Stats["Duration"].AddModifier(3);
            Id = "EnergyBlade";
            GameObject blade = Resources.Load<GameObject>("VFX/SkillEffects/Complete/EnergyBlade");
            GameObject charge = Resources.Load<GameObject>("VFX/Misc/EnergyBladeCharge");
            GameObject absorb = Resources.Load<GameObject>("VFX/Misc/EnergyAbsorb");
            Assert.AreEqual((blade != null), true, "Failed to load EnergyBlade.prefab");
            Assert.AreEqual((charge != null), true, "Failed to load EnergyBladeCharge.prefab");
            Assert.AreEqual((absorb != null), true, "Failed to load EnergyAbsorb.prefab");

            CastCondition = delegate
            {
                return Caster.EquipedWeapon != null && Caster.EquipedWeapon.Settings.DamageType == WeaponDamageType.Melee;
            };

            OnCast.Context = delegate
            {
                Caster.Animator.SetLayerWeight(1, 0);
                Caster.Animator.ReplaceClip("_SANIM", Resources.Load<AnimationClip>("Animations/EnergyBlade_SANIM"));
                Caster.Animator.SetTrigger("Custom");
                EffectsManager.Instance.SpawnEffect(charge, Caster.transform.position, Quaternion.Euler(270, 0, 171));
                GameObject bladeGo = EffectsManager.Instance.SpawnEffect(
                    blade, 
                    Caster.EquipedWeapon.transform.position, 
                    Caster.EquipedWeapon.transform.rotation);
                bladeGo.transform.parent = Caster.EquipedWeapon.transform;
                GameObject absorbGo = Instantiate(absorb, Caster.EquipedWeapon.transform.position, Quaternion.Euler(-90, 0, 0));
                absorbGo.transform.parent = Caster.EquipedWeapon.transform;
                MeshRenderer bladeRenderer = bladeGo.GetComponent<MeshRenderer>();
                bladeRenderer.material.SetFloat("_DissolveAmountAdd", 2);
                Caster.DelayedInvoke(() =>
                {
                    bladeRenderer.material.DOFloat(0, "_DissolveAmountAdd", 0.5f);
                }, 0.3f);
                bladeGo.transform.rotation = bladeGo.transform.parent.transform.rotation;
                bladeGo.transform.position = Caster.EquipedWeapon.transform.position;
                Caster.DelayedInvoke(() =>
                {
                    foreach (ParticleSystem ps in bladeGo.GetComponentsInChildren<ParticleSystem>())
                    {
                        ps.Play();
                    }
                }, 0.4f);
                Caster.DelayedInvoke(() =>
                {
                    bladeGo.GetComponent<MeshRenderer>().material.DOFloat(2, "_DissolveAmountAdd", 0.5f)
                    .OnComplete(() => {
                        foreach (ParticleSystem ps in bladeGo.GetComponentsInChildren<ParticleSystem>())
                        {
                            ps.Stop();
                            ps.transform.parent = null;
                            Caster.DelayedInvoke(() =>
                            {
                                Destroy(ps);
                            }, 4);
                        }
                        Destroy(bladeGo);
                    });
                    Caster.Animator.SetLayerWeight(1, 1);
                    
                }, 1.9f);
            };
            PostInit();
        }
    }
}