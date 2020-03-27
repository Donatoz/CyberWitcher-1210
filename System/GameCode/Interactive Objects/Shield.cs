using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class Shield : InteractiveObject, IVurnable
    {
        public float Health;
        public bool ClampDamageToOne;
        public float DeathTimer;
        public GameObject ShieldObject;

        private Stat health;

        private void Start()
        {
            health = new Stat(Health, Health, 0);
            if(ClampDamageToOne)
            {

            }
            health.OnValueChange += delegate
            {
                if (health.EffectiveValue <= 0)
                {
                    Die();
                }
            };
        }

        private void Die()
        {
            ShieldObject.GetComponent<Animator>().SetTrigger("Out");
            Destroy(ShieldObject, DeathTimer);
        }

        public Stat GetStat(string statName)
        {
            return (statName == "Health") ? health : null;
        }
    }
}