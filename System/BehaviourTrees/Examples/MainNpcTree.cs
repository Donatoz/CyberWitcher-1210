// All code below belongs to BOB[A]H Technologies.
//-----------------------------------------------
//Main Behaviour tree for NPC(bots).
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech.Behaviours
{
    public class MainNpcTree : BehaviourTree
    {
        private NPC npcOwner => Owner as NPC;

        public override bool Build()
        {
            InterruptCondition = delegate
            {
                return npcOwner.IsDead;
            };
            #region Start Group

            Node start = NodeBuilder.Start(this);
            AddNode(start, null, "Start");

            #endregion

            #region Nodes Initialize

            Node shoot = null;
            Node idle = null;
            Node searchForActor = null;

            //TODO: Rewrite nodes code using NodeContainers (in order to get rid of strings)
            NodeContainer shootContainer = new NodeContainer(shoot);
            NodeContainer idleContainer = new NodeContainer(idle);
            NodeContainer searchForActorContainer = new NodeContainer(searchForActor);

            #endregion

            #region Idle state

            //TODO: Remove this unneccesary condition
            Func<bool> seesActorCondition = delegate
            {
                return npcOwner.focusedTarget != null &&
                npcOwner.CanSeeActor(npcOwner.focusedTarget);
            };
            Func<bool> seesEnemyCondition = delegate
            {
                return npcOwner.focusedTarget != null
                && npcOwner.focusedTarget.ActorFraction != npcOwner.ActorFraction
                && !npcOwner.focusedTarget.IsDead;
            };

            StateData idleStateData = new StateData
            (
                null, 
                null, 
                delegate { npcOwner.Animator.SetTrigger("Idle" + npcOwner.EquipedWeapon.Settings.HoldingType.ToString()); }, 
                (seesActorCondition, 0)
            );
            idle = new Node(idleStateData);
            idleContainer.AttachedNode = idle;
            Node idleSeesActorSwitch = NodeBuilder.Switch("Idle State", ("Shoot State", seesEnemyCondition));
            AddNode(idle, start, "Idle State");
            AddNode(idleSeesActorSwitch, idle, "Idle State: Sees actor switch");

            #endregion

            #region Shoot State

            Func<bool> enemyIsDead = delegate
            {
                return npcOwner.focusedTarget != null && npcOwner.focusedTarget.IsDead;
            };

            Action shootingAction = delegate
            {
                npcOwner.ShootInActor(npcOwner.focusedTarget);
            };

            StateData shootStateData = new StateData
            (
                null,
                null,
                delegate { Debug.Log("Shoot on sate"); shootingAction.Invoke(); }, //On update
                (enemyIsDead, 0),
                (()=> { return true; }, 1)
            );
            shoot = new Node(shootStateData);
            shootContainer.AttachedNode = shoot;
            AddNode(shoot, idleSeesActorSwitch, "Shoot State");
            // Idle is now child of shooting state with localId = 0
            ConnectNodes("Shoot State", "Idle State");

            Action searchForActorContext = delegate
            {
                // Interruption
                if(seesActorCondition() && seesEnemyCondition())
                {
                    searchForActor.Exit();
                }
            };

            Func<bool> noSearchResultCondition = delegate
            {
                return npcOwner.Agent.InDestination();
            };

            StateData searchForActorData = new StateData
            (
                () => { npcOwner.Agent.SetDestination(npcOwner.TargetLastSeenPosition); }, 
                null,
                searchForActorContext, 
                (noSearchResultCondition, 1)
            );

            searchForActor = new Node(searchForActorData);
            searchForActorContainer.AttachedNode = searchForActor;

            // Shooting choice-node with localId = 1
            Node shootingChoiceNode = NodeBuilder.Choice
            (
                "Shoot State",
                "Shoot State: Search State", 
                () => { return npcOwner.focusedTarget != null && !npcOwner.focusedTarget.IsDead && npcOwner.CanSeeActor(npcOwner.focusedTarget); }
            );
            AddNode(shootingChoiceNode, shoot, "Shoot State: Choice");
            AddNode(searchForActor, shoot, "Shoot State: Search State");
            // shoot is now child of searchForActor with localId = 0
            ConnectNodes("Shoot State: Search State", "Shoot State");
            // idle is now child of searchForActor with localId = 1
            ConnectNodes("Shoot State: Search State", "Idle State");

            #endregion

            return true;
        }

        public override void Initialize()
        {
            Build();
            Pointer = new TreePointer(nodes[0], this);
            StartCoroutine(UpdateNodes());
        }
    }
}