// All code below belongs to BOB[A]H Technologies.
// BOB[A]H Technologies 2020. All rights reserved.
//-----------------------------------------------
//Entity class.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using VovTech.Serialization;

namespace VovTech
{
    /// <summary>
    /// Main class representing all custom game instances.
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        /// <summary>
        /// Delegate for all events connected to entity state change.
        /// </summary>
        public delegate void StateChange();
        [Header("Entity settings")]
        /// <summary>
        /// Entity name.
        /// </summary>
        public string Name;
        /// <summary>
        /// Unique id which can be useful when we try to get some entity by known id.
        /// </summary>
        public string ReferenceId;
        /// <summary>
        /// Invokes when entity is fully initialized.
        /// </summary>
        public event StateChange OnInit;
        /// <summary>
        /// Invokes when entity is being deleted from scene(destroyed).
        /// </summary>
        public event StateChange OnVanish;
        /// <summary>
        /// All custom coroutines.
        /// </summary>
        public List<Coroutine> OngoingCoroutines;
        public List<string> Groups;
        public EntityType ReferenceType { get; protected set; }

        protected Func<bool, string> serializeContext;

        /// <summary>
        /// Destroy the entity. (Dont ever use Destroy() on entities).
        /// </summary>
        public virtual void Vanish()
        {
            OnVanish?.Invoke();
            MainManager.Instance.GetSceneEntities().Remove(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// Local initialization.
        /// </summary>
        protected virtual void Init()
        {
            if(ReferenceId == string.Empty)
                MainManager.Instance.GiveId(out ReferenceId);
            if(!MainManager.Instance.GetSceneEntities().Contains(this))
                MainManager.Instance.AddEntity(this);
            if (Name == string.Empty) Name = $"Entity:{ReferenceId}";
        }

        public static Entity GetById(string refId)
        {
            return MainManager.Instance.GetSceneEntities().Find(x => x.ReferenceId == refId);
        }

        public Coroutine StartCustomCoroutine(Action context, int invokesAmount, float interval, Func<bool> stopCondition = null, Action stopAction = null, float delay = 0, bool deltaTime = false)
        {
            if (OngoingCoroutines == null) OngoingCoroutines = new List<Coroutine>();
            Coroutine cor = null;
            OngoingCoroutines.Add(cor);
            cor = StartCoroutine(CustomCoroutine(() =>
            {
                OngoingCoroutines.Remove(OngoingCoroutines.Find(x => x == cor));
            }, 
            context, invokesAmount, interval, stopCondition, stopAction, delay, deltaTime));
            return cor;
        }

        public IEnumerator CustomCoroutine(Action clear, Action context, int invokesAmount, float interval, Func<bool> stopCondition = null, Action stopAction = null, float delay = 0, bool deltaTime = false)
        {
            yield return new WaitForSeconds(delay);
            if (invokesAmount == -1)
            {
                while (true)
                {
                    try
                    {
                        context.Invoke();
                        if (stopCondition != null && stopCondition())
                        {
                            stopAction?.Invoke();
                            break;
                        }
                    }
                    catch
                    {
                        break;
                    }
                    yield return (deltaTime) ? new WaitForSeconds(Time.deltaTime) : new WaitForSeconds(interval);
                }
            }
            else
            {
                for (int i = 0; i < invokesAmount; i++)
                {
                    try
                    {
                        context.Invoke();
                        if (stopCondition != null && stopCondition.Invoke())
                        {
                            stopAction?.Invoke();
                            break;
                        }
                    }
                    catch
                    {
                        break;
                    }
                    yield return (deltaTime) ? new WaitForSeconds(Time.deltaTime) : new WaitForSeconds(interval);
                }
            }
            clear.Invoke();
        }

        public Coroutine DelayedInvoke(Action method, float delay)
        {
            if (OngoingCoroutines == null) OngoingCoroutines = new List<Coroutine>();
            Coroutine cor = null;
            OngoingCoroutines.Add(cor);
            cor = StartCoroutine(DelayedInvokeRoutine(method, delay));
            return cor;
        }

        private IEnumerator DelayedInvokeRoutine(Action method, float delay)
        {
            yield return new WaitForSeconds(delay);
            method.Invoke();
        }

        /// <summary>
        /// Initializtion (and Reinitialization) from outside.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Serializes entity for data saving/sending to server.
        /// </summary>
        /// <param name="pretty">JSON-string pretty print</param>
        /// <returns>JSON-string</returns>
        public string Serialize(bool pretty = true)
        {
            return serializeContext(pretty);
        }

        public static Entity[] GetGroup(string groupId)
        {
            List<Entity> foundObjects = new List<Entity>();
            foreach (Entity obj in MainManager.Instance.GetSceneEntities())
            {
                if (obj.Groups.Contains(groupId))
                {
                    foundObjects.Add(obj);
                }
            }
            return foundObjects.ToArray();
        }
    }
}