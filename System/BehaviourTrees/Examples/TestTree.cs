using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

namespace VovTech.Behaviours
{
    public class TestTree : BehaviourTree
    {
        private NPC npcOwner => Owner as NPC;

        public override bool Build()
        {
            /* Creating new node with state data.
             * This node will set the destination which attached NPC will move to.
             * The pointer will exit this node when the NPC will be in the destination.
             */

            //Preparing data

            Node start = NodeBuilder.Start(this);
            AddNode(start, null, "Start");

            //Defining action which will be invoked when the pointer will enter our node
            Action onEnter = delegate
            {
                npcOwner.Agent.SetDestination(GameObject.Find("Waypoint").transform.position);
            };

            //Defining condition which will move pointer to the next node
            Func<bool> condition = delegate
            {
                return npcOwner.Agent.InDestination();
            };
            
            //Creating data object
            StateData data = new StateData(onEnter, null, null, (condition, 0));

            //Creating node
            Node movingNode = new Node(data);
            //Adding node and connect to the start node
            AddNode(movingNode, null, "Moving node");

            /* Let's create other node, which will print the message "Test debug" when the pointer reaches this node.
             * Pointer will exit this node automatically.
             */

            //Debug log node data
            StateData debugLogData = new StateData(delegate { Debug.Log("Test debug"); }, null, null);
            //Debug log node itself
            Node debugLogNode = new Node(debugLogData);
            //Adding node and connect to the (earlier created) moving node
            AddNode(debugLogNode, movingNode, "Debug node");
            
            //Let's create a node which will set the destination as second waypoint

            //Create node object (similar to movingNode)
            Node moveBackNode = new Node(
                new StateData(
                    delegate
                    {
                        npcOwner.Agent.SetDestination(GameObject.Find("Waypoint2").transform.position);
                    },
                    null,
                    null,
                    (delegate { return npcOwner.Agent.InDestination(); }, 0)
                )
            );

            //Add new node to the tree
            AddNode(moveBackNode, debugLogNode, "Move back node");

            //Connect new node to the start node, whereby our tree becomes endless.
            ConnectNodes(moveBackNode, movingNode);

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