using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace VovTech
{
    [RequireComponent(typeof(Breakable))]
    public class Window : InteractiveObject, IVurnable
    {
        [Header("Window settings")]
        public float Health = 5;
        public List<string> OnBreakTriggers;
        public List<ParticleSystem> AttachedEffects;
        public List<GameObject> ObjectsToActivate;

        private Breakable breakable => GetComponent<Breakable>();
        private Stat health;
        private bool breaked;

        private void Start()
        {
            health = new Stat(Health, MinValue: 0);
            health.OnValueChange += delegate
            {
                if(!breaked && health.EffectiveValue <= 0)
                {
                    Break();
                }
            };
        }

        public void Break()
        {
            for (int i = 0; i < OnBreakTriggers.Count; i++)
            {
                LevelManager.Instance.ActivateTrigger(OnBreakTriggers[i]);
            }
            foreach(ParticleSystem ps in AttachedEffects)
            {
                ps.Play();
                foreach(ShaderController controller in ps.GetComponentsInChildren<ShaderController>())
                {
                    controller.ChangeFloat("_Cutout", 1, 2, true, 3, 2.5f);
                }
            }
            ObjectsToActivate.ForEach((obj) => { obj.SetActive(true); });
            foreach (Transform t in transform)
            {
                if(t.GetComponent<DOTweenAnimation>() != null) t.DOPlay();
            }
            breakable.BreakFunction();
            breaked = true;
        }

        public Stat GetStat(string statName)
        {
            return (statName == "Health") ? health : null;
        }
    }
}