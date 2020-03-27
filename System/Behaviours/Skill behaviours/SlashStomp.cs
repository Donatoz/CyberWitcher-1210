using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech.Behaviours.Skills
{
    public class SlashStomp : SkillBehaviour
    {
        public override void Initialize()
        {
            Stats["Duration"].AddModifier(2f);
            Id = "SlashStomp";
            GameObject jumpFx = Resources.Load<GameObject>("VFX/Misc/JumpFx1");
            GameObject onMoveFx = Resources.Load<GameObject>("VFX/OnMove/LightningMove");
            GameObject lightningStrike = Resources.Load<GameObject>("VFX/SkillEffects/Complete/LightningSmash");

            CastCondition = delegate
            {
                return Caster.EquipedWeapon != null && Caster.EquipedWeapon.Settings.DamageType == WeaponDamageType.Melee;
            };

            OnCast.Context = delegate
            {
                Vector3 casterPos = Caster.transform.position;
                Caster.Animator.SetLayerWeight(1, 0);
                Caster.Animator.ReplaceClip("_SANIM", Resources.Load<AnimationClip>("Animations/StompSlash_SANIM"));
                Caster.Animator.SetTrigger("Custom");
                GameObject onMoveFxGo = Instantiate(onMoveFx, Caster.BodyCenter.position, Quaternion.identity);
                onMoveFxGo.transform.parent = Caster.transform;
                Caster.DelayedInvoke(() =>
                {
                    Destroy(onMoveFxGo);
                }, 2);
                Caster.DelayedInvoke(() =>
                {
                    Instantiate(jumpFx, casterPos, Quaternion.Euler(-90,0,0));
                }, 0.3f);
                Caster.DelayedInvoke(() =>
                {
                    GameObject lightning = Instantiate(lightningStrike, casterPos, Quaternion.Euler(-90, 0, 0));
                    lightning.transform.Find("Damage").GetComponent<DamageZone>().ZoneFraction = Caster.ActorFraction;
                }, 1.1f);
                Caster.DelayedInvoke(() =>
                {
                    Caster.Animator.SetLayerWeight(1, 1);
                }, Stats["Duration"].EffectiveValue);
            };
            PostInit();
        }
    }
}