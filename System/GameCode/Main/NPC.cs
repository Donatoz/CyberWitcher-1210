// All code below belongs to BOB[A]H Technologies.
//-----------------------------------------------
//NPC class.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEditor;

namespace VovTech
{
    /// <summary>
    /// Non playable character (BOT) with specific AI.
    /// </summary>
    public class NPC : Actor
    {
        [Header("NPC settings")]
        public NavMeshAgent Agent;
        /// <summary>
        /// Radius for spotting the enemy.
        /// </summary>
        public float AggressiveRadius;
        /// <summary>
        /// Orders module.
        /// </summary>
        public OrderModule OrderAssembler;
        /// <summary>
        /// NPC AI component.
        /// </summary>
        public AIModule Behavior;
        /// <summary>
        /// AI behavior template.
        /// </summary>
        public MonoScript BehaviorTemplate;
        /// <summary>
        /// Refreshable sequence of orders.
        /// </summary>
        public Queue<Order> OrdersSequence;
        /// <summary>
        /// Maximum shooting range.
        /// </summary>
        public float ShootingRange;
        /// <summary>
        /// Pivot for holding a weapon.
        /// </summary>
        public Transform WeaponPivot;
        /// <summary>
        /// Spot which from rays (for spotting the enemy) will cast.
        /// </summary>
        public Transform ViewSpot;
        /// <summary>
        /// Is character busy (shooting, chasing, etc...)
        /// </summary>
        public bool IsBusy;
        /// <summary>
        /// Weapon which will be given to the NPC in the Start() function.
        /// </summary>
        public Weapon StartWeapon;

        [SerializeField]
        public Actor focusedTarget { get; private set; }

        protected override void Start()
        {
            base.Start();
            OrderAssembler = gameObject.AddComponent<OrderModule>();
            Behavior = gameObject.AddComponent<AIModule>();
            OrdersSequence = new Queue<Order>();
            Agent = GetComponent<NavMeshAgent>();
            OrderAssembler.Initialize(this);
            if (Settings != null) {
                if (stats.ContainsKey("Speed"))
                {
                    Agent.speed = stats["Speed"].EffectiveValue;
                    Agent.angularSpeed = stats["Speed"].EffectiveValue * 240;
                }
                if (stats.ContainsKey("ShootingRange")) ShootingRange = stats["ShootingRange"].EffectiveValue;
            }
            Initialize();
            if (StartWeapon != null)
            {
                EquipWeapon(StartWeapon);
            }
            ReferenceType = EntityType.NPC;
        }

        private void Update()
        {
            Order[] orders = OrdersSequence.ToArray();
            for (int i = orders.Length - 1; i >= 0; i--)
            {
                orders[i].CheckForComplete();
            }
        }

        public override void Initialize()
        {
            Init();
            AITemplate template;
            if (BehaviorTemplate != null)
                template = ScriptableObject.CreateInstance(BehaviorTemplate.GetClass()) as AITemplate;
            else
                template = StandardAITemplate.CreateInstance<StandardAITemplate>();
            template.Initialize(this);
            Behavior.SetTemplate(template);
            Behavior.Enabled = true;
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (weapon.Owner != null && weapon.Owner != this) return;
            if (EquipedWeapon != null)
                Destroy(EquipedWeapon);

            weapon.gameObject.SetActive(true);
            weapon.GetComponent<Collider>().enabled = false;
            weapon.transform.rotation = WeaponPivot.rotation;
            EquipedWeapon = weapon;
            weapon.transform.position = WeaponPivot.position;
            weapon.transform.parent = WeaponPivot;
            weapon.ClearEffects();
            weapon.Owner = this;
            Animator.SetTrigger("Idle" + EquipedWeapon.Settings.HoldingType.ToString());
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawSphere(transform.position, AggressiveRadius);
        }

        public void RefreshOrders()
        {
            if (OrdersSequence.Count > 0 && !OrdersSequence.Peek().IsExecuted) OrdersSequence.Peek().Execute();
        }

        public void Focuse(Actor actor)
        {
            focusedTarget = actor;
        }

        public bool CanSeeActor(Actor actor)
        {
            //Check for obstacles
            RaycastHit hit;
            int mask = ~LayerMask.GetMask("Zones", "Projectiles", "Bones", "Weapons");
            if (Physics.Raycast(ViewSpot.position,
                actor.BodyCenter.position - HeadPosition.position,
                out hit,
                AggressiveRadius,
                mask))
            {
                Debug.DrawRay(HeadPosition.position, actor.BodyCenter.position - HeadPosition.position);
                if (hit.collider.gameObject.transform.root.gameObject != actor.gameObject)
                {
                    if(debug)
                        Debug.Log("Sees: " + hit.collider.gameObject.name);
                    return false;
                }
            }

            //Is spoted actor in the sight of this actor?
            float angle = Vector3.Angle(transform.forward, actor.transform.position - transform.position);
            if (Mathf.Abs(angle) > 90)
            {
                //If not, does spoted actor is behind this actor? If not - this actor can't see spoted actor.
                if (Vector3.Distance(transform.position, actor.transform.position) > 2) return false;
            }
            return true;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (focusedTarget != null)
            {
                Animator.SetLookAtWeight(0.8f);
                Animator.SetLookAtPosition(focusedTarget.HeadPosition.position);
            }
            if(EquipedWeapon != null && EquipedWeapon.SecondHandPivot != null)
            {
                Animator.SetIKPosition(AvatarIKGoal.LeftHand, EquipedWeapon.SecondHandPivot.position);
                Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            }
        }

        public override void Kill()
        {
            base.Kill();
            Agent.isStopped = true;
        }

        public void GiveOrder(Order order, OrderTarget target)
        {
            OrdersSequence.Enqueue(order);
            if (OrdersSequence.Count == 1)
                order.Execute();
        }

        public Order AssembleOrder(OrderTarget target, Action<OrderTarget> orderAction, Predicate<NPC> completeCondition)
        {
            Order order = new Order
            (
                target,
                orderAction,
                this
            );
            order.CompleteCondition = completeCondition;
            order.OnComplete += delegate { Debug.Log("Complete"); };
            order.OnComplete += delegate { OrdersSequence.Dequeue(); };
            order.OnComplete += delegate { RefreshOrders(); };
            return order;
        }
    }
}