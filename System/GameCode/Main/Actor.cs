// All code below belongs to BOB[A]H Technologies.
// BOB[A]H Technologies 2020. All rights reserved.
//-----------------------------------------------
//Actor class.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VovTech.Serialization;
using System.Linq;
using UnityEditor;
using System;

namespace VovTech
{
    /// <summary>
    /// Any acting entity (NPC, Player, etc...).
    /// </summary>
    public abstract class Actor : Entity, IVurnable
    {
        /// <summary>
        /// Actor's stats.
        /// </summary>
        protected Dictionary<string, Stat> stats;
        /// <summary>
        /// Invokes when actor dies.
        /// </summary>
        public event StateChange OnDeath;
        [Header("Actor settings")]
        /// <summary>
        /// Actor's fraction.
        /// </summary>
        public Fraction ActorFraction;
        /// <summary>
        /// Animator component.
        /// </summary>
        public Animator Animator => GetComponent<Animator>();
        /// <summary>
        /// Is actor dead? (health <= 0)
        /// </summary>
        public bool IsDead;
        /// <summary>
        /// NPC settings.
        /// </summary>
        public ActorInfo Settings;
        /// <summary>
        /// Center of actor's gameobject.
        /// </summary>
        public Transform BodyCenter;
        /// <summary>
        /// Actor's head position.
        /// </summary>
        public Transform HeadPosition;
        /// <summary>
        /// For animations.
        /// </summary>
        public bool Ready;
        public float ReadyTimer = 0;
        /// <summary>
        /// Weapon.
        /// </summary>
        public Weapon EquipedWeapon;
        /// <summary>
        /// Position where this actor was hitted last time.
        /// </summary>
        public Vector3 LastHitPosition;
        public List<IAttachable<Actor>> Attachments;
        public Transform AttachmentsContainer;
        [ColorUsage(true, true)]
        public Color DissolveColor;
        /// <summary>
        /// Current animation trigger name.
        /// </summary>
        [HideInInspector]
        public string CurrentAnimation;
        /// <summary>
        /// Renderer of the Actor's model. (to dissolve)
        /// </summary>
        [SerializeField]
        private SkinnedMeshRenderer modelRenderer;
        /// <summary>
        /// Ragdoll colliders.(to avoid fake hits)
        /// </summary>
        [SerializeField]
        protected Collider[] ragdollColliders;
        /// <summary>
        /// Skills set.
        /// </summary>
        public List<MonoScript> SkillSet;
        /// <summary>
        /// Skills list (initialized from SkillSet).
        /// </summary>
        protected Dictionary<int, SkillBehaviour> skills;
        protected float skillGlobalCooldown = 0;

        [SerializeField]
        private AudioClip footStepSound;
        [SerializeField]
        private float footStepSoundVolume;
        public bool debug;

        private void Awake()
        {
            stats = new Dictionary<string, Stat>();
            skills = new Dictionary<int, SkillBehaviour>();
            stats["Health"] = new Stat(10);
            stats["Gravity"] = new Stat(-9.81f);
            Attachments = new List<IAttachable<Actor>>();
            if (Settings != null)
            {
                foreach (ActorInfo.StatInfo info in Settings.Stats)
                {
                    stats[info.Name] = new Stat(info.BaseValue, info.MaxValue, info.MinValue);
                }

                if (Settings.DeathSounds.Count > 0)
                {
                    OnDeath += delegate
                    {
                        SoundManager.Instance.PlayClipAtPoint(
                            Settings.DeathSounds.RandomItem(), transform.position, Settings.DeathSoundVolume);
                    };
                }
                if (Settings.HitSounds.Count > 0)
                {
                    stats["Health"].OnValueChange += delegate
                    {
                        SoundManager.Instance.PlayClipAtPoint(
                            Settings.HitSounds.RandomItem(), transform.position, Settings.HitSoundVolume
                            );
                    };
                }
            }
            stats["Health"].OnValueChange += delegate (Stat health)
            {
                if (health.EffectiveValue <= 0) Kill();
            };
            ReferenceType = EntityType.Actor;
            serializeContext = delegate(bool pretty)
            {
                ActorData info = new ActorData();
                info.Name = Name;
                info.ReferenceId = ReferenceId;
                info.Position = transform.position;
                info.Rotation = transform.rotation.eulerAngles;
                info.Size = transform.localScale;
                info.Groups = Groups.ToArray();
                info.CurrentAnimation = CurrentAnimation;
                List<StatData> statsData = new List<StatData>();
                foreach(KeyValuePair<string, Stat> pair in stats)
                {
                    StatData data = new StatData();
                    data.Name = pair.Key;
                    data.Value = pair.Value.EffectiveValue;
                    statsData.Add(data);
                }
                info.IsDead = IsDead;
                return JsonUtility.ToJson(info, pretty);
            };
        }

        protected virtual void FixedUpdate()
        {
            skillGlobalCooldown = Mathf.Clamp(skillGlobalCooldown - Time.fixedDeltaTime, 0, Int32.MaxValue);
        }

        protected virtual void Start()
        {
            ragdollColliders = GetComponentsInChildren<Collider>();
            foreach(Collider col in ragdollColliders)
            {
                if(col != GetComponent<Collider>())
                    col.isTrigger = true;
            }
            if(modelRenderer != null)
                modelRenderer.material.SetColor("_DissolveColor", DissolveColor);
            for(int i = 0; i < SkillSet.Count; i++)
            {
                SkillBehaviour skill = ScriptableObject.CreateInstance(SkillSet[i].GetClass()) as SkillBehaviour;
                AddSkill(skill);
            }
        }

        public void AddSkill(SkillBehaviour skill)
        {
            if(!skills.ContainsValue(skill))
            {
                skill.Caster = this;
                skill.Initialize();
                skills[skills.Count + 1] = skill;
            }
        }

        public virtual bool CastSkill(int id)
        {
            if (!skills.ContainsKey(id) || skillGlobalCooldown > 0) return false;
            SkillBehaviour behaviour = skills[id];
            if (behaviour.CastCondition != null && !behaviour.CastCondition()) return false;
            skillGlobalCooldown = behaviour.Stats["Duration"].EffectiveValue;
            if (behaviour.OnPrepare != null)
            {
                behaviour.OnPrepare.RunContext();
            }
            else if (behaviour.OnCast != null)
            {
                behaviour.OnCast.RunContext();
            }
            behaviour.SendData();
            return true;
        }

        /// <summary>
        /// Kill this actor.
        /// </summary>
        public virtual void Kill()
        {
            OnDeath?.Invoke();
            IsDead = true;
            GetComponent<Collider>().enabled = false;
            Animator.StopPlayback();
            Animator.enabled = false;
            Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
            foreach (Collider col in ragdollColliders)
            {
                col.isTrigger = false;
            }
            foreach (Rigidbody rb in bodies)
            {
                rb.isKinematic = false;
            }
            foreach (Rigidbody rb in bodies)
            {
                rb.AddForce((BodyCenter.position - LastHitPosition) * 100);
            }
            DelayedInvoke(() => { modelRenderer?.material.DOFloat(2, "_DissolveAmount", 3).SetEase(Ease.OutSine); }, 2);
            DelayedInvoke(Vanish, 6);
        }

        public void DisableAllModules()
        {
            foreach(Transform t in transform)
            {
                Module m = t.GetComponent<Module>();
                if (m != null) m.Enabled = false;
            }
        }

        public void LookHorizontalyLerp(Vector3 pos, float speed)
        {
            Quaternion lookRot = Quaternion.LookRotation(pos - transform.position);
            transform.rotation = new Quaternion(0, lookRot.y, 0, lookRot.w);
        }


        public Stat GetStat(string name)
        {
            try
            {
                return stats[name];
            }
            catch
            {
                return null;
            }
        }

        public void PlayFootStepSound()
        {
            SoundManager.Instance.PlayClipAtPoint(footStepSound, transform.position, footStepSoundVolume);
        }
    }
}