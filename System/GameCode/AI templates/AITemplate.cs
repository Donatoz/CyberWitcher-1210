using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    [System.Serializable]
    public abstract class AITemplate : ScriptableObject
    {
        public abstract void Behave();
        public abstract bool IsTargetValid(Actor actor);

        protected Actor attachedActor;

        public virtual void Initialize(Actor attachedActor)
        {
            this.attachedActor = attachedActor;
        }
    }
}