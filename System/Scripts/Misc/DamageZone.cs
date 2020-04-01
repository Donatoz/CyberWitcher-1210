using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    [RequireComponent(typeof(SphereCollider))]
    public class DamageZone : MonoBehaviour
    {
        public float LifeTime;
        public float Damage;
        public Fraction ZoneFraction;
        public float Radius;
        public float Delay;

        private List<Actor> affected = new List<Actor>();

        private void Start()
        {
            Invoke("Vanish", LifeTime + Delay);
        }

        private void Update()
        {
            GetComponent<SphereCollider>().radius = Radius;
        }

        private void FixedUpdate()
        {
            Delay = Mathf.Clamp(Delay - Time.fixedDeltaTime, 0, Delay);
        }

        private void Vanish()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Delay > 0) return;
            Actor actor = other.transform.root.GetComponent<Actor>();
            if(actor != null && actor.ActorFraction != ZoneFraction && !affected.Contains(actor))
            {
                actor.GetStat("Health").AddModifier(Damage);
                affected.Add(actor);
            }
        }
    }
}