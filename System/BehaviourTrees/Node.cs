using System.Collections.Generic;
using System;
using UnityEngine;

namespace VovTech.Behaviours
{
    /// <summary>
    /// Node object.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Data, which this node holds (node's behaviour part).
        /// </summary>
        private INodeData data;
        /// <summary>
        /// All nodes which are connected to this node.
        /// </summary>
        private List<string> children;
        /// <summary>
        /// Nodes that should be updated in parallel.
        /// </summary>
        private List<Node> parallelNodes;
        /// <summary>
        /// Is node parallel? (if yes, it can't exit itself)
        /// </summary>
        public bool Parallel;
        /// <summary>
        /// Node-parent which this node can move back to.
        /// </summary>
        private Node parent;
        /// <summary>
        /// Tree which this node is attached to.
        /// </summary>
        public BehaviourTree Tree;
        /// <summary>
        /// Property for setting and getting node parent.
        /// </summary>
        public Node Parent
        {
            get
            {
                return (parent != null) ? parent : this;
            }
            set
            {
                if (value != this) parent = value;
            }
        }
        /// <summary>
        /// Delegate which is being invoked on every UpdateNodeState() call.
        /// </summary>
        public Action UpdateAction;

        public Node(INodeData data)
        {
            this.data = data;
            parallelNodes = new List<Node>();
            UpdateAction = delegate 
            {
                foreach(Node node in parallelNodes)
                {
                    node.UpdateNodeState();
                }
            };
            data.Attach(this);
            children = new List<string>();
        }

        /// <summary>
        /// Connect some node to this node.
        /// </summary>
        /// <param name="node">Node to connect</param>
        public void AddChild(string node)
        {
            children.Add(node);
        }

        /// <summary>
        /// Get node data.
        /// </summary>
        /// <returns>Interface for using the node data.</returns>
        public INodeData GetData()
        {
            return data;
        }

        /// <summary>
        /// Prints in the console this node hierachy (parent and children).
        /// </summary>
        public void PrintChildren()
        {
            Debug.Log(data.Name + " children:");
            for(int i = 0; i < children.Count; i++)
            {
                Debug.Log($"    {children[i]}|{Tree.GetNode(children[i]).GetData().Id}|{i}");
            }
            Debug.Log("__________");
        }

        public override string ToString()
        {
            if (Parent != this)
                return $"{Parent.GetData().Name}|{Parent.GetData().Id} -> {GetData().Name}|{GetData().Id}";
            return $"{GetData().Name}|{GetData().Id}";
        }

        /// <summary>
        /// Get child by it's local id.
        /// </summary>
        /// <param name="i">Local id</param>
        /// <returns>Found child or null</returns>
        public string GetChild(int i)
        {
            return children[i];
        }

        /// <summary>
        /// Move to the next (child) node.
        /// </summary>
        /// <param name="localId">Child local id</param>
        public void Exit(int localId = 0)
        {
            Tree.Pointer.MoveNext(localId);
        }

        /// <summary>
        /// Move back to the parent node.
        /// </summary>
        public void Revert()
        {
            Tree.Pointer.MoveTo(parent.GetData().Id);
        }

        /// <summary>
        /// Get global id of the parent.
        /// </summary>
        /// <returns>Parnet node's gloabl id</returns>
        public int GetParentId()
        {
            return parent.GetData().Id;
        }

        /// <summary>
        /// Get children amount of this node.
        /// </summary>
        /// <returns></returns>
        public int GetChildrenAmount()
        {
            return children.Count;
        }

        /// <summary>
        /// Update this node state and resolve node data.
        /// </summary>
        public void UpdateNodeState()
        {
            UpdateAction?.Invoke();
        }

        /// <summary>
        /// Attach other node to this to be updated with this node.
        /// </summary>
        /// <param name="node">Node to attach</param>
        public void AddParallel(Node node)
        {
            parallelNodes.Add(node);
        }

        /// <summary>
        /// Deattach other node from parallel queue.
        /// </summary>
        /// <param name="node">Node to deattach</param>
        public void RemoveParallel(Node node)
        {
            if (parallelNodes.Contains(node)) parallelNodes.Remove(node);
        }
    }

    /// <summary>
    /// Special structure which holds node in order to build trees.
    /// </summary>
    public struct NodeContainer
    {
        public Node AttachedNode;

        public NodeContainer(Node attachedNode)
        {
            AttachedNode = attachedNode;
        }
    }
}