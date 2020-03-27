using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace VovTech
{
    public class SwarmNPC : Actor
    {
        public AIModule Behavior;
        public float AggressiveRadius;
        public NavMeshAgent Agent;

        [SerializeField]
        private float currentInterval = 0;

        private void FixedUpdate()
        {
            currentInterval = Mathf.Clamp(currentInterval - Time.fixedDeltaTime, stats["AttackInterval"].MinValue, stats["AttackInterval"].MaxValue);
            if (Agent.velocity.sqrMagnitude > 0.4f)
                Animator.SetTrigger("Run");
            else
                Animator.SetTrigger("Idle");
        }

        protected override void Start()
        {
            base.Start();
            Initialize();
            ReferenceType = EntityType.SawrmNPC;
        }

        public override void Initialize()
        {
            Init();
            if(GetComponent<AIModule>() == null)
                Behavior = gameObject.AddComponent<AIModule>();
            AITemplate template = new SwarmAITemplate();
            template.Initialize(this);
            Behavior.SetTemplate(template);
            Behavior.Enabled = true;
            Agent = GetComponent<NavMeshAgent>();
        }

        public void Attack(Actor target)
        {
            if (currentInterval == stats["AttackInterval"].MinValue)
            {
                target.GetStat("Health").AddModifier(-stats["Damage"].EffectiveValue);
                currentInterval = stats["AttackInterval"].EffectiveValue;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, AggressiveRadius);
        }

        public override void Kill()
        {
            base.Kill();
            Agent.isStopped = true;
            Behavior.Enabled = false;
        }
    }
}