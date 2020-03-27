using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace VovTech
{
    public class Projectile : Entity, IVurnable
    {
        public delegate void ProjectileState(Projectile proj);
        public ProjectileInfo Settings;
        public Dictionary<string, Stat> ProjectileStats;
        public Vector3 Direction;
        public Entity Creator;
        public int FallbackDataId = 2;

        public event ProjectileState OnLive;

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            if(GetComponent<Rigidbody>().velocity.magnitude < 10)
            {
                GetComponent<Rigidbody>().AddForce(Direction);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            IVurnable vurnable = collision.gameObject.GetComponent<IVurnable>();
            if(vurnable == null)
            {
                Destroy(GetComponent<Rigidbody>());
                Die();
            }
            else
            {
                float otherHealth = vurnable.GetStat("Health").EffectiveValue;
                vurnable.GetStat("Health").AddModifier(-ProjectileStats["Health"].EffectiveValue);
                ProjectileStats["Health"].AddModifier(-otherHealth);
                if(vurnable is Actor)
                {
                    (vurnable as Actor).LastHitPosition = collision.contacts[0].point;
                }
            }
        }

        private void Die()
        {
            if (Settings.DeathEffect != null)
                EffectsManager.Instance.SpawnEffect(Settings.DeathEffect, transform.position, Quaternion.Euler(Vector3.zero));
            Vanish();
        }

        public override void Initialize()
        {
            if (Settings == null)
            {
                Settings = ProjectileDatabase.GetInstance().GetProjectile(FallbackDataId);
            }
            ProjectileStats = new Dictionary<string, Stat>();
            ProjectileStats["Speed"] = new Stat(Settings.Speed);
            ProjectileStats["Health"] = new Stat(Settings.Heatlh);
            ProjectileStats["Health"].OnValueChange += delegate(Stat stat)
            {
                if (stat.EffectiveValue <= 0) Die();
            };
            OnLive?.Invoke(this);
        }

        public Stat GetStat(string statName)
        {
            try
            {
                return ProjectileStats[statName];
            }
            catch
            {
                return null;
            }
        }
    }
}