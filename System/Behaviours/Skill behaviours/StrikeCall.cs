using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech.Behaviours.Skills
{
    public class StrikeCall : SkillBehaviour
    {
        public override void Initialize()
        {
            OnCast = new SkillAction();
            Type = SkillType.Instant;
            Id = "StrikeCall";
            OnCast.Context = delegate
            {
                GameObject airStrike = Resources.Load<GameObject>("VFX/SkillEffects/Complete/SpaceStrike");
                Vector3[] positions = new Vector3[3]
                {
                    Caster.transform.position + Caster.transform.forward * 6 + new Vector3(0, 0.5f, 0),
                    Caster.transform.position + Caster.transform.forward * 5 + Caster.transform.right * 4  + new Vector3(0, 0.5f, 0),
                    Caster.transform.position + Caster.transform.forward * 5 - Caster.transform.right * 4  + new Vector3(0, 0.5f, 0)
                };
                for(int i = 0; i < 3; i++)
                {
                    EffectsManager.Instance.SpawnEffect(airStrike, positions[i], Quaternion.Euler(-90, 0, 0));
                }
            };
            PostInit();
        }
    }
}