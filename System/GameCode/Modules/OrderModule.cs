using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech
{
    public class OrderModule : Module
    {
        private NPC attachedNpc;
        public bool debug;

        private void FixedUpdate()
        {
            if (debug) Debug.Log(attachedNpc.Agent.velocity.sqrMagnitude <= 0f);
            if (attachedNpc.Agent.velocity.sqrMagnitude <= 0f)
            {
                attachedNpc.Animator.SetTrigger("Idle1");
            } else
            {
                attachedNpc.Animator.SetTrigger("Run");
            }
        }

        public void Initialize(Actor actorToAttach)
        {
            attachedNpc = actorToAttach.AsNPC();
        }

        public void Walk(Vector3 destination)
        {
            attachedNpc.Agent.SetDestination(destination);
        }

        public Coroutine SetActorAsDestination(Actor actor)
        {
            attachedNpc.Animator.SetTrigger("Run");
            return attachedNpc.StartCustomCoroutine(
                () =>
                {
                    attachedNpc.Agent.SetDestination(actor.transform.position);
                },
                -1,
                0.03f
            );
        }


    }
}