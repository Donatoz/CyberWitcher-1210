using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VovTech.Behaviours
{
    /// <summary>
    /// Base for all behaviour trees.
    /// </summary>
    public abstract class BehaviourTree : MonoBehaviour
    {
        /// <summary>
        /// Object that points on currently active behaviour tree node.
        /// </summary>
        public sealed class TreePointer
        {
            public Node CurrentNode { get; private set; }
            private BehaviourTree tree;

            /// <summary>
            /// Create new tree pointer.
            /// </summary>
            /// <param name="startNode">Node which from pointer should start</param>
            public TreePointer(Node startNode, BehaviourTree attachedTree)
            {
                CurrentNode = startNode;
                tree = attachedTree;
            }

            /// <summary>
            /// Move the pointer to the next node (use it by subscribing to node events).
            /// </summary>
            /// <param name="localId"></param>
            public void MoveNext(int localId)
            {
                
                Node next = CurrentNode.GetChild(localId);
                if (next != null)
                    CurrentNode = next;
            }

            /// <summary>
            /// Move back to parent node (not recommended).
            /// </summary>
            /// <param name="globalId">Global id of the parent node.</param>
            public void MoveTo(int globalId)
            {
                Node node = tree.GetNode(globalId);
                if (node != null)
                    CurrentNode = node;
            }
        }
        /// <summary>
        /// All nodes in the tree.
        /// </summary>
        protected List<Node> nodes = new List<Node>();
        /// <summary>
        /// Tree owner with AI component.
        /// </summary>
        public Entity Owner;
        /// <summary>
        /// Local initialization.
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Add nodes to the tree (it is only possible through the code at this momment).
        /// </summary>
        /// <returns>Is tree built successfully?</returns>
        public abstract bool Build();
        /// <summary>
        /// Is the tree reached it's end?
        /// </summary>
        public bool ReachedEnd;
        /// <summary>
        /// Tree pointer.
        /// </summary>
        public TreePointer Pointer;

        protected virtual void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Add new node to the tree.
        /// </summary>
        /// <param name="node">Node to add</param>
        /// <param name="parent">Node to which the new node will be connected (if null - node will be connected to the start node)</param>
        /// <param name="customId">Custom node name</param>
        public void AddNode(Node node, Node parent, string customId = "")
        {
            if (parent != null)
            {
                parent.AddChild(node);
                node.Parent = parent;
            }
            else if (nodes.Count > 0)
            {
                nodes[0].AddChild(node);
                node.Parent = nodes[0];
            }

            node.GetData().Name = (customId != string.Empty) ? customId : (nodes.Count + 1).ToString();
            node.GetData().Id = nodes.Count;
            node.Tree = this;
            nodes.Add(node);
        }

        public void ConnectNodes(Node from, Node to)
        {
            if(nodes.Contains(from) && nodes.Contains(to))
                from.AddChild(to);
        }

        protected IEnumerator UpdateNodes()
        {
            while (!ReachedEnd)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                if (nodes.Count == 0) break;
                Pointer.CurrentNode.UpdateNodeState();
            }
            Debug.Log("Tree reached the end");
        }

        /// <summary>
        /// Get node by its name.
        /// </summary>
        /// <param name="id">Node name</param>
        /// <returns>Found node or null</returns>
        public Node GetNode(string name)
        {
            return nodes.Find(x => x.GetData().Name == name);
        }

        /// <summary>
        /// Get node by its global id.
        /// </summary>
        /// <param name="id">Node global id</param>
        /// <returns>Found node or null</returns>
        public Node GetNode(int globalId)
        {
            return nodes.Find(x => x.GetData().Id == globalId);
        }
    }
}