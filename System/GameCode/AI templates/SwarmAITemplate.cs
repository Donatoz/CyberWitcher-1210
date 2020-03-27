using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class SwarmAITemplate : AITemplate
    {
        private SwarmNPC attachedNpc;
        private Actor aggroTarget;
        public float AttackRange;

        public override void Initialize(Actor attachedActor)
        {
            attachedNpc = attachedActor as SwarmNPC;
            attachedActor = attachedNpc;
            AttackRange = attachedNpc.GetStat("AttackRange").EffectiveValue;
        }

        public override void Behave()
        {
            if (aggroTarget != null)
            {
                AttackTarget();
                return;
            }
            int actorsMask = LayerMask.GetMask("Actors");
            Collider[] touchedColliders = Physics.OverlapSphere(attachedNpc.transform.position, attachedNpc.AggressiveRadius, actorsMask);
            for (int i = 0; i < touchedColliders.Length; i++)
            {
                //Is there any actor in range?
                Actor actor = touchedColliders[i].gameObject.GetComponent<Actor>();
                //If it is - does spoted actor belong to another fraction?
                if (actor != null && actor != attachedNpc && !actor.IsDead)
                {
                    if (actor.ActorFraction != attachedNpc.ActorFraction)
                    {
                        aggroTarget = actor;
                        break;
                    }
                }
            }
        }

        private void AttackTarget()
        {
            var heading = aggroTarget.transform.position - attachedNpc.transform.position;
            if(heading.sqrMagnitude <= AttackRange * AttackRange)
            {
                attachedNpc.Agent.isStopped = true;
                attachedNpc.Attack(aggroTarget);
                if (aggroTarget.IsDead)
                {
                    aggroTarget = null;
                    Behave();
                }
            }
            else
            {
                attachedNpc.Agent.SetDestination(aggroTarget.transform.position);
                attachedNpc.Agent.isStopped = false;
            }
        }

        public override bool IsTargetValid(Actor actor)
        {
            return true;
        }
    }
}