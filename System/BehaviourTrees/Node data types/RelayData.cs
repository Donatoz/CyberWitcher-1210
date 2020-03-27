using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech.Behaviours
{
    public class RelayData : INodeData
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Node AttachedNode { get; private set; }

        public Action OnEnter;

        public RelayData(Action onEnter = null)
        {
            OnEnter = onEnter;

            AttachedNode.UpdateAction += delegate
            {
                OnEnter?.Invoke();
                AttachedNode.Exit();
            };
        }

        public void Attach(Node node)
        {
            AttachedNode = node;
        }
    }
}