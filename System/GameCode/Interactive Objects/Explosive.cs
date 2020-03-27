using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class Explosive : InteractiveObject, IVurnable
    {
        public float Health;
        public GameObject ExplosionEffect;
        public List<AudioClip> ExplosionSounds;
        [Range(0f, 1f)]
        public float ExplosionVolume;
        public float ExplosionStrength;
        public float ExplosionShakeDuration;

        private Stat healthStat;

        private void Start()
        {
            healthStat = new Stat(Health, 0, Health);
            healthStat.OnValueChange += delegate(Stat health)
            {
                if (health.EffectiveValue <= 0) Explode();
            };
        }

        public void Explode()
        {
            EffectsManager.Instance.SpawnEffect(ExplosionEffect, transform.position, ExplosionEffect.transform.rotation);
            SoundManager.Instance.PlayClipAtPoint(ExplosionSounds.RandomItem(), transform.position, ExplosionVolume);
            MainManager.Instance.MainCameraController.Shake(ExplosionStrength, ExplosionShakeDuration);
            Destroy(gameObject);
        }

        public Stat GetStat(string statName)
        {
            return (statName == "Health") ? healthStat : null;
        }
    }
}