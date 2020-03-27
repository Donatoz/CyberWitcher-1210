using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech.Behaviours
{
    /// <summary>
    /// Noda data which holds the state information.
    /// </summary>
    [Serializable]
    public class StateData : INodeData
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public Action OnStateEnter;
        public Action OnStateExit;
        public Action OnState;
        public Dictionary<Func<bool>, int> ExitConditions;
        public Node AttachedNode { get; private set; }

        private bool entered;

        public StateData(Action OnStateEnter, Action OnStateExit,
            Action OnState, params (Func<bool>, int)[] exitConditions)
        {
            this.OnStateEnter = OnStateEnter;
            this.OnStateExit = OnStateExit;
            this.OnState = OnState;
            ExitConditions = new Dictionary<Func<bool>, int>();
            if (exitConditions.Length > 0)
            {
                foreach ((Func<bool>, int) tuple in exitConditions)
                {
                    ExitConditions[tuple.Item1] = tuple.Item2;
                }
            }
            else
                ExitConditions[delegate { return true; }] = 0;
        }

        public void Attach(Node node)
        {
            AttachedNode = node;
            AttachedNode.UpdateAction += delegate ()
            {
                if (!entered)
                {
                    OnStateEnter?.Invoke();
                    entered = true;
                }
                OnState?.Invoke();
                foreach (KeyValuePair<Func<bool>, int> pair in ExitConditions)
                {
                    if (pair.Key() == true)
                    {
                        entered = false;
                        if (AttachedNode.GetChildrenAmount() > 0)
                            AttachedNode.Exit(pair.Value);
                        else
                        {
                            AttachedNode.Tree.ReachedEnd = true;
                            break;
                        }
                        Debug.Log($"{Name} is exited and moved to {AttachedNode.GetChild(pair.Value).GetData().Name}");
                        break;
                    }
                }
            };
        }
    }
}