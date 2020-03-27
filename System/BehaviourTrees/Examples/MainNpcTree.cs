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
            #region Start Group

            Node start = NodeBuilder.Start(this);
            AddNode(start, null, "Start");

            #endregion


            #region Nodes Initialize

            Node shoot = null;
            Node idle = null;

            #endregion


            #region Idle state

            Func<bool> idleSeesActorCondition = delegate
            {
                return npcOwner.focusedTarget != null;
            };
            Func<bool> idleSeesEnemyCondition = delegate
            {
                return npcOwner.focusedTarget.ActorFraction != npcOwner.ActorFraction;
            };

            Node idleSeesActorSwitch = NodeBuilder.Switch(idle, (shoot, idleSeesEnemyCondition));
            StateData idleStateData = new StateData(null, null, null, (idleSeesActorCondition, 0));

            idle = new Node(idleStateData);
            AddNode(idle, start, "Idle State");
            AddNode(idleSeesActorSwitch, idle, "Idle State -> Sees actor switch");

            #endregion


            #region Shoot State

            StateData shootStateData = new StateData(null, null, null);
            shoot = new Node(shootStateData);
            AddNode(shoot, idleSeesActorSwitch, "Shoot State");

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