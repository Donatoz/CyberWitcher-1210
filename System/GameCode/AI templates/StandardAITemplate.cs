using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class StandardAITemplate : AITemplate
    {
        private NPC attachedNpc;

        public override void Initialize(Actor attachedActor)
        {
            attachedNpc = attachedActor.AsNPC();
        }

        public override void Behave()
        {
            if (attachedNpc.IsDead) return;
            int actorsMask = LayerMask.GetMask("Actors");
            Collider[] touchedColliders = Physics.OverlapSphere(attachedNpc.transform.position, attachedNpc.AggressiveRadius, actorsMask);
            for (int i = 0; i < touchedColliders.Length; i++)
            {
                //Is there any actor in range?
                Actor actor = touchedColliders[i].gameObject.GetComponent<Actor>();
                //If it is - does spoted actor belong to another fraction?
                if (actor != null && actor != attachedNpc && attachedNpc.OrdersSequence.Count == 0 && !actor.IsDead)
                {
                    attachedNpc.Focuse(actor);
                    if (actor.ActorFraction != attachedNpc.ActorFraction)
                    {
                        if (attachedNpc.focusedTarget.ActorFraction == attachedNpc.ActorFraction)
                            attachedNpc.Focuse(actor);
                        if (!attachedNpc.CanSeeActor(actor)) continue;
                        //If this actor can see spoted one, give him order to attack.
                        OrderTarget target = new OrderTarget(null, default);
                        Order order = attachedNpc.AssembleOrder
                            (
                                //No need in targeting.
                                target,
                                delegate
                                {
                                    if (attachedNpc.EquipedWeapon == null)
                                    {
                                            //TODO: Melee combat.
                                            attachedNpc.OrderAssembler.SetActorAsDestination(actor);
                                        if (!attachedNpc.IsBusy)
                                        {
                                            attachedNpc.Animator.SetTrigger("Walk");
                                            attachedNpc.IsBusy = true;
                                        }
                                    }
                                    else
                                    {
                                        if (Vector3.Distance(attachedNpc.transform.position, actor.transform.position) <= attachedNpc.ShootingRange)
                                        {
                                            attachedNpc.Animator.SetTrigger("Aim" + attachedNpc.EquipedWeapon.Settings.HoldingType.ToString());
                                                // Look at player coroutine
                                                attachedNpc.StartCustomCoroutine(() =>
                                            {
                                                if (!attachedNpc.IsDead && attachedNpc.CanSeeActor(actor))
                                                    attachedNpc.LookHorizontalyLerp(actor.transform.position, Time.deltaTime * 6f);
                                            }, -1, 0.03f, () =>
                                            {
                                                return actor.IsDead || actor == null
                                            || Vector3.Distance(attachedNpc.transform.position, actor.transform.position)
                                            > attachedNpc.AggressiveRadius * 2;
                                            });
                                                // Shooting coroutine
                                                attachedNpc.StartCustomCoroutine(() =>
                                            {
                                                if (!attachedNpc.IsDead)
                                                {
                                                    if (attachedNpc.CanSeeActor(actor))
                                                    {
                                                        Debug.Log("Shoot");
                                                        attachedNpc.EquipedWeapon.Shoot(actor.BodyCenter.position);
                                                        attachedNpc.Animator.SetTrigger("Idle1");
                                                        attachedNpc.Animator.SetTrigger("Aim" + attachedNpc.EquipedWeapon.Settings.HoldingType.ToString());
                                                        attachedNpc.Agent.isStopped = true;
                                                    }
                                                    else
                                                    {
                                                        Debug.Log("Search for actor");
                                                        attachedNpc.Animator.SetTrigger("Run");
                                                        attachedNpc.Agent.destination = actor.transform.position;
                                                        attachedNpc.Agent.isStopped = false;
                                                    }
                                                }
                                            }, -1, 0.03f, () =>
                                            {
                                                return actor.IsDead || actor == null
                                            || Vector3.Distance(attachedNpc.transform.position, actor.transform.position)
                                            > attachedNpc.AggressiveRadius * 2;
                                            }, () => {
                                                Debug.Log("Stopped shooting");
                                                attachedNpc.Agent.isStopped = true;
                                                attachedNpc.Animator.SetTrigger("Idle1");
                                            }, 0.05f);
                                        }
                                        else
                                        {
                                            attachedNpc.StartCustomCoroutine(() =>
                                            {
                                                attachedNpc.Agent.isStopped = false;
                                                attachedNpc.Animator.SetTrigger("Run");
                                                attachedNpc.OrderAssembler.SetActorAsDestination(actor);
                                            }, -1, 0.03f,
                                            () =>
                                            {
                                                return Vector3.Distance(attachedNpc.transform.position, actor.transform.position) <= attachedNpc.ShootingRange;
                                            },
                                            () => {
                                                attachedNpc.OrderAssembler.StopAllCoroutines();
                                                attachedNpc.Agent.isStopped = true;
                                            });
                                        }
                                    }
                                },
                                delegate (NPC npc)
                                {
                                    return actor.IsDead || actor == null
                            || Vector3.Distance(attachedNpc.transform.position, actor.transform.position) > attachedNpc.AggressiveRadius * 2;
                                }
                            );
                        order.OnComplete += delegate
                        {
                            Debug.Log("order complete.");
                            attachedNpc.IsBusy = false;
                            attachedNpc.Animator.SetTrigger("Idle" + attachedNpc.EquipedWeapon.Settings.HoldingType.ToString());
                        };
                        attachedNpc.GiveOrder
                        (
                            order,
                            new OrderTarget(null, actor.transform.position)
                        );
                        break;
                    }
                }
                else attachedNpc.Focuse(null);
            }
        }

        public override bool IsTargetValid(Actor actor)
        {
            return false;
        }
    }
}