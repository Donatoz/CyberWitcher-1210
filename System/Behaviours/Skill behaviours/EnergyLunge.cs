using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace VovTech.Behaviours.Skills
{
    public class EnergyLunge : SkillBehaviour
    {
        public override void Initialize()
        {
            Stats["Duration"].AddModifier(2.4f);
            Id = "EnergyLunge";
            GameObject charge = Resources.Load<GameObject>("VFX/Misc/EnergyLungeCharge");
            GameObject crack = Resources.Load<GameObject>("VFX/SkillEffects/Complete/EnergyCrack");
            Assert.AreEqual((charge != null), true, "Failed to load EnergyLungeCharge.prefab");
            Assert.AreEqual((crack != null), true, "Failed to load EnergyCrack.prefab");

            CastCondition = delegate
            {
                return Caster.EquipedWeapon != null && Caster.EquipedWeapon.Settings.DamageType == WeaponDamageType.Melee;
            };

            OnCast.Context = delegate
            {
                Caster.Animator.SetLayerWeight(1, 0);
                Caster.Animator.ReplaceClip("_SANIM", Resources.Load<AnimationClip>("Animations/EnergyLunge_SANIM"));
                Caster.Animator.SetTrigger("Custom");
                GameObject chargeGo = Instantiate(charge, Caster.EquipedWeapon.transform.position, Quaternion.identity);
                chargeGo.transform.parent = Caster.EquipedWeapon.transform;
                Caster.DelayedInvoke(() =>
                {
                    MainManager.Instance.MainCameraController.Shake(0.5f, 2);
                    Instantiate(crack, Caster.EquipedWeapon.transform.position + Caster.transform.right * 0.5f, Caster.transform.rotation);
                }, 1.35f);
                Caster.DelayedInvoke(() =>
                {
                    Caster.Animator.SetLayerWeight(1, 1);
                }, 1.75f);
            };
            PostInit();
        }
    }
}