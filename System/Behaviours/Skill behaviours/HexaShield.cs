using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace VovTech
{
    public class HexaShield : SkillBehaviour
    {
        public override void Initialize()
        {
            GameObject shield = Resources.Load<GameObject>("VFX/SkillEffects/HexaShield");

            Assert.AreEqual((shield != null), true, "Failed to load HexaShield.prefab");

            OnCast.Context = delegate
            {
                GameObject shieldGo = Instantiate(shield, Caster.transform.position, Quaternion.identity);
                Animator shieldAnimator = shieldGo.GetComponent<Animator>();
                Coroutine cor = Caster.StartCustomCoroutine(() =>
                {
                    if (shieldGo != null)
                    {
                        shieldGo.transform.position = Caster.transform.position;
                    }
                }, -1, Time.deltaTime, deltaTime: true);
                Caster.DelayedInvoke(() => { Caster.StopCoroutine(cor); Destroy(shieldGo.transform.Find("Collision").gameObject); }, 7);
                Caster.DelayedInvoke(() =>
                {
                    shieldAnimator.SetTrigger("Out");
                }, 5);
            };
            PostInit();
        }
    }
}