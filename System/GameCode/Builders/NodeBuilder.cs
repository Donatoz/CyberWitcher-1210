using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech.Behaviours
{
    public static class NodeBuilder
    {
        public static Node Start(BehaviourTree tree)
        {
            return new Node(new StateData(null, null, null)) { Tree = tree };
        }

        public static Node Switch(Node fallbackNode, params (Node, Func<bool>)[] outputNodes)
        {
            return new Node(new CompositeData(fallbackNode, outputNodes));
        }

        public static Node Choice(Node ifTrueNode, Node ifFalseNode, Func<bool> condition)
        {
            return new Node(new CompositeData(ifFalseNode, (ifTrueNode, condition)));
        }

        public static Node Relay()
        {
            return new Node(new RelayData());
        }

        public static Node Action(Action action)
        {
            return new Node(new RelayData(action));
        }
    }
}