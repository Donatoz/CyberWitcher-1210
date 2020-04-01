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

        public static Node Switch(string fallbackNodeName, params (string, Func<bool>)[] outputNodes)
        {
            return new Node(new CompositeData(fallbackNodeName, outputNodes));
        }

        public static Node Choice(string ifTrueNodeName, string ifFalseNodeName, Func<bool> condition)
        {
            return new Node(new CompositeData(ifFalseNodeName, (ifTrueNodeName, condition)));
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