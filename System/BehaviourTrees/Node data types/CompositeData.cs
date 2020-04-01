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
        public string FallbackNodeId;
        public Dictionary<string, Func<bool>> OutputNodes;

        public CompositeData(string fallbackNodeName, params (string, Func<bool>)[] outputNodes)
        {
            OutputNodes = new Dictionary<string, Func<bool>>();
            FallbackNodeId = (fallbackNodeName == string.Empty) ? AttachedNode.GetData().Name : fallbackNodeName;
            foreach((string, Func<bool>) tuple in outputNodes)
            {
                OutputNodes[tuple.Item1] = tuple.Item2;
            }
        }

        public void Attach(Node node)
        {
            AttachedNode = node;
            AttachedNode.UpdateAction += delegate
            {
                foreach (KeyValuePair<string, Func<bool>> pair in OutputNodes)
                {
                    if (pair.Value())
                    {
                        Debug.Log($"Moved to: {pair.Key}");
                        AttachedNode.Tree.Pointer.MoveTo(AttachedNode.Tree.GetNode(pair.Key).GetData().Id);
                        return;
                    }
                }
                Debug.Log($"Reverted to: {FallbackNodeId}");
                AttachedNode.Tree.Pointer.MoveTo(AttachedNode.Tree.GetNode(FallbackNodeId).GetData().Id);
            };
        }
    }
}