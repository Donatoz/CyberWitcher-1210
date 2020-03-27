using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech
{
    public class Order
    {
        public delegate void OrderState();

        public Predicate<NPC> CompleteCondition;
        public event OrderState OnExecute;
        public event OrderState OnComplete;
        public bool IsExecuted => executed;
        public OrderTarget Target;

        private Action<OrderTarget> orderAction;
        private NPC orderTarget;
        private bool executed;

        public Order(OrderTarget target, Action<OrderTarget> orderAction, NPC orderTarget)
        {
            Target = target;
            this.orderAction = orderAction;
            this.orderTarget = orderTarget;
            executed = false;
        }

        public void Execute()
        {
            executed = true;
            orderAction.Invoke(Target);
            OnExecute?.Invoke();
        }

        public void CheckForComplete()
        {
            if (CompleteCondition != null && CompleteCondition(orderTarget) && executed) OnComplete?.Invoke();
        }

    }

    public struct OrderTarget
    {
        public Entity EntityTarget;
        public Vector3 PointTarget;

        public OrderTarget(Entity entityTarget, Vector3 pointTarget)
        {
            EntityTarget = entityTarget;
            PointTarget = pointTarget;
        }
    }
}