using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using VovTech.Serialization;

namespace VovTech {
    public enum SkillType
    {
        Channel,
        Instant,
        Passive
    }

    public class SkillAction
    {
        public Action OnComplete;
        public Action Context;

        public SkillAction RunContext()
        {
            Context?.Invoke();
            OnComplete?.Invoke();
            return this;
        }
    }

    /// <summary>
    /// Custom behaviours for skills.
    /// </summary>
    public abstract class SkillBehaviour : ScriptableObject
    {
        /// <summary>
        /// Skill-casting actor.
        /// </summary>
        public Actor Caster;
        /// <summary>
        /// Entity-target of this skill.
        /// </summary>
        public Entity Target;
        /// <summary>
        /// Invokes when caster submited this skill casting.
        /// </summary>
        public SkillAction OnPrepare = new SkillAction();
        /// <summary>
        /// Invokes when OnPrepare action ended invoking.
        /// </summary>
        public SkillAction OnCast = new SkillAction();
        /// <summary>
        /// Invokes on cast end (when OnCast action ended invoking).
        /// </summary>
        public SkillAction OnRelease = new SkillAction();
        /// <summary>
        /// Conditions to cast.
        /// </summary>
        public Func<bool> CastCondition;
        /// <summary>
        /// Skill type.
        /// </summary>
        public SkillType Type;
        public bool Scripted;
        /// <summary>
        /// Unique id for this skill.
        /// </summary>
        public string Id;
        /// <summary>
        /// Skill runtime stats.
        /// </summary>
        public Dictionary<string, Stat> Stats = new Dictionary<string, Stat>() {
            ["EnergyConsumption"] = new Stat(1, MinValue: 0),
            ["Range"] = new Stat(1, MinValue: 0.01f),
            ["Cooldown"] = new Stat(1, MinValue: 0.01f),
            ["Duration"] = new Stat(0.1f, MinValue: 0)
        };

        public virtual void SendData()
        {
            using(Packet packet = new Packet())
            {
                ScriptableData skillData = new ScriptableData();
                skillData.DataType = typeof(ScriptableData).ToString();
                skillData.Prefix = "PREFIX: SKILL";
                skillData.ScriptId = Id;
                packet.Write(JsonUtility.ToJson(skillData));
                NetManager.Instance.SendData(packet);
            }
        }

        /// <summary>
        /// Initialize all actions and skill stats.
        /// </summary>
        public abstract void Initialize();

        public virtual void PostInit()
        {
            if (OnPrepare != null)
            {
                OnPrepare.OnComplete += delegate
                {
                    OnCast?.RunContext();
                };
            }
            if (OnCast != null)
            {
                {
                    OnRelease?.RunContext();
                };
            }
        }
    }
}