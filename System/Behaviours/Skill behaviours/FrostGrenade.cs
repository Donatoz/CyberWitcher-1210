using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace VovTech.Behaviours.Skills
{
    public class FrostGrenade : SkillBehaviour
    {
        public override void Initialize()
        {
            GameObject grenade = Resources.Load<GameObject>("VFX/SkillEffects/Complete/FrostGrenade");
            Vector3 mousePos = InputManager.Instance.MouseWorldPosition;
            GameObject go = Instantiate(grenade, Caster.transform.position, Quaternion.identity);
        }
    }
}