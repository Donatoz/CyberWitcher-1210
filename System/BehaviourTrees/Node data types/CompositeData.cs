using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech.Behaviours
{
    public class CompositeData : INodeData
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Node AttachedNode { get; private set; }
        public Node FallbackNode;
        public Dictionary<Node, Func<bool>> OutputNodes;

        public CompositeData(Node fallback, params (Node, Func<bool>)[] outputNodes)
        {
            FallbackNode = (fallback == null) ? AttachedNode.Parent : fallback;
            foreach((Node, Func<bool>) tuple in outputNodes)
            {
                OutputNodes[tuple.Item1] = tuple.Item2;
            }

            AttachedNode.UpdateAction += delegate
            {
                foreach (KeyValuePair<Node, Func<bool>> pair in OutputNodes)
                {
                    if (pair.Value())
                    {
                        AttachedNode.Tree.Pointer.MoveTo(pair.Key.GetData().Id);
                        return;
                    }
                }
                AttachedNode.Tree.Pointer.MoveTo(FallbackNode.GetData().Id);
            };
        }

        public void Attach(Node node)
        {
            AttachedNode = node;
        }
    }
}