using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VovTech.Serialization;
using UnityEditor;
using VovTech.Behaviours;

namespace VovTech
{
    /// <summary>
    /// Controllable character (by player).
    /// </summary>
    public class Character : Actor
    {
        [Header("Character settings")]
        /// <summary>
        /// Player-controller of this character.
        /// </summary>
        public Player Owner;
        /// <summary>
        /// Controller component (for moving and rotating).
        /// </summary>
        public CharacterController Controller;
        /// <summary>
        /// Is character being controlled?
        /// </summary>
        public bool IsControlled => Owner != null;
        [HideInInspector]
        /// <summary>
        /// Direction in which character is looking.
        /// </summary>
        public Vector3 LookDirection;
        /// <summary>
        /// Does this character must move localy or according to the camera position.
        /// </summary>
        public bool LocalMovement;
        /// <summary>
        /// Character's inventory.
        /// </summary>
        public List<Item> Inventory;
        /// <summary>
        /// Animation module for this character.
        /// </summary>
        public AnimationModule AnimationModule;
        /// <summary>
        /// Sequence of weapons (to switch on next/previous weapon).
        /// </summary>
        public NavigationList<Weapon> WeaponSequence;
        /// <summary>
        /// Pivot for holding weapons.
        /// </summary>
        public Transform WeaponPivot;
        /// <summary>
        /// Character class (skills set, base health, energy, etc...).
        /// </summary>
        public ClassInfo CharacterClass;
        /// <summary>
        /// Movement module of this character.
        /// </summary>
        public ControlledMovementModule MovementModule;
        [SerializeField]
        /// <summary>
        /// Debuging stuff.
        /// </summary>
        private float healthModifier = 10;
        [SerializeField]
        private Vector3 headLookingPos;
        /// <summary>
        /// For animations.
        /// </summary>
        private bool readyToShoot;
        /// <summary>
        /// For animations.
        /// </summary>
        private float readyToShootTimer = 0.25f;
        private StaticNPC focusedSpeakable;

        protected override void Start()
        {
            stats["Speed"] = new Stat(3);
            stats["AngularSpeed"] = new Stat(7);
            stats["Health"].AddModifier(healthModifier);
            Inventory = new List<Item>();
            WeaponSequence = new NavigationList<Weapon>();

            LookDirection = transform.forward;
            if (ReferenceId == string.Empty) Init();
            if (Owner.Local)
            {
                MovementModule = gameObject.AddComponent<ControlledMovementModule>();
                MovementModule.AngularSpeedStat = stats["AngularSpeed"];
                MovementModule.GravityStat = stats["Gravity"];
                MovementModule.SpeedStat = stats["Speed"];
                MovementModule.LocalMovement = LocalMovement;
                MovementModule.Controller = Controller;
                MovementModule.Enabled = true;
                MovementModule.AttachedEntity = this;
                MovementModule.SprintFx = transform.Find("Sprint").GetComponent<ParticleSystem>();
            }
            AnimationModule = gameObject.AddComponent<AnimationModule>();
            AnimationModule.AttachedEntity = this;
            AnimationModule.AttachedAnimator = Animator;
            AnimationModule.Enabled = true;
            ReferenceType = EntityType.Character;
            Initialize();
        }

        public override void Initialize()
        {
            serializeContext = delegate (bool pretty)
            {
                //TODO: create info field (optimization)
                CharacterData info = new CharacterData();
                info.Name = Name;
                info.DataType = typeof(CharacterData).ToString();
                info.ReferenceId = ReferenceId;
                info.Position = transform.position.Round(6);
                info.Rotation = transform.rotation.eulerAngles.Round(3);
                info.Size = transform.localScale;
                info.Groups = Groups.ToArray();
                info.CurrentAnimation = CurrentAnimation;
                List<StatData> statsData = new List<StatData>();
                foreach (KeyValuePair<string, Stat> pair in stats)
                {
                    StatData data = new StatData();
                    data.Name = pair.Key;
                    data.Value = pair.Value.EffectiveValue;
                    statsData.Add(data);
                }
                info.IsDead = IsDead;
                info.LookingPos = headLookingPos.Round(4);
                info.OwnerName = Owner.Name;
                info.EquipedWeaponId = (EquipedWeapon != null) ? EquipedWeapon.Settings.Id : -1;
                return JsonUtility.ToJson(info, pretty);
            };
            for(int i = 0; i < CharacterClass.SkillIds.Length; i++)
            {
                ScriptInfo info = ScriptDatabase.GetInstance().GetScript(CharacterClass.SkillIds[i]);
                SkillBehaviour skill = ScriptableObject.CreateInstance(info.Script.GetClass()) as SkillBehaviour;
                AddSkill(skill);
            }
        }

        public Weapon GetWeaponById(int id)
        {
            return (Weapon)Inventory.Find(x => x is Weapon && x.AsWeapon().Settings.Id == id);
        }

        private void Update()
        {
            if(Owner.Local && !IsDead)
                headLookingPos = Vector3.Lerp(headLookingPos, InputManager.Instance.MouseWorldPosition, Time.deltaTime * 14);
        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f);

            bool found = false;
            foreach(Collider col in colliders)
            {
                if(col.GetComponent<StaticNPC>() != null)
                {
                    found = true;
                    focusedSpeakable = col.GetComponent<StaticNPC>();
                    focusedSpeakable.SpeakTip.gameObject.SetActive(true);
                    break;
                }
            }
            if (!found)
            {
                if (focusedSpeakable != null)
                {
                    focusedSpeakable.SpeakTip.gameObject.SetActive(false);
                    focusedSpeakable = null;
                }
            }

            if(Input.GetKeyDown(KeyCode.J) && focusedSpeakable != null)
            {
                focusedSpeakable.StartDialogue(this);
                focusedSpeakable.SpeakTip.gameObject.SetActive(false);
            }

            ReadyTimer = Mathf.Clamp(ReadyTimer - Time.fixedDeltaTime, 0, 3);
            Ready = ReadyTimer > 0;
        }

        public override bool CastSkill(int id)
        {
            if (!base.CastSkill(id)) return false;
            Controller.enabled = false;
            DelayedInvoke(() =>
            {
                Controller.enabled = true;
            }, skills[id].Stats["Duration"].EffectiveValue);
            return true;
        }

        public void Look(Vector3 pos)
        {
            headLookingPos = pos;
        }

        public void RecieveItem(Item item)
        {
            if(!Inventory.Contains(item))
                Inventory.Add(item);
            if (item is Weapon) EquipWeapon(item as Weapon);
        }
        /// <summary>
        /// Equip weapon.
        /// </summary>
        /// <param name="weapon">Weapon to equip.</param>
        public void EquipWeapon(Weapon weapon)
        {
            if(EquipedWeapon != null)
            {
                EquipedWeapon.gameObject.SetActive(false);
                EquipedWeapon.transform.parent = null;
                EquipedWeapon.gameObject.transform.position = GameObject.Find("WeaponsSpot").transform.position;
            }
            if(weapon.Behaviour != null)
            {
                weapon.Behaviour.WeaponOwner = this;
            }
            weapon.gameObject.SetActive(true);
            weapon.ClearEffects();
            EquipedWeapon = weapon;
            weapon.transform.position = WeaponPivot.position;
            weapon.transform.parent = WeaponPivot;
            weapon.transform.rotation = WeaponPivot.transform.rotation;
            AnimationModule.SetAnimationTrigger("Idle" + EquipedWeapon.Settings.HoldingType.ToString());
            if(!WeaponSequence.Contains(weapon))
                WeaponSequence.Add(weapon);
        }

        /// <summary>
        /// Make this character shoot in some direction.
        /// </summary>
        /// <param name="target">If default - target is mouse position.</param>
        public void Shoot(Vector3 target = default)
        {
            if (!readyToShoot)
            {
                if (EquipedWeapon.Settings.HoldingType != WeaponHoldingType.Sword)
                {
                    StartCustomCoroutine(() =>
                   {
                       AnimationModule.SetAnimationTrigger("Aim" + EquipedWeapon.Settings.HoldingType.ToString());
                       readyToShootTimer -= 0.03f;
                   }, -1, 0.03f, () => { return readyToShootTimer <= 0; }, () =>
                   {
                       readyToShoot = true;
                       headLookingPos = InputManager.Instance.MouseWorldPosition;
                       EquipedWeapon.Shoot();
                       readyToShootTimer = 0.25f;
                   }
                    );
                }
                else
                {
                    EquipedWeapon.Shoot();
                    readyToShootTimer = EquipedWeapon.Settings.AttackInterval;
                }
            }
            else
            {
                AnimationModule.SetAnimationTrigger("Aim" + EquipedWeapon.Settings.HoldingType.ToString());
                headLookingPos = InputManager.Instance.MouseWorldPosition;
                EquipedWeapon.Shoot();
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (!AnimationModule.Enabled) return;
            Animator.SetLookAtWeight(1);
            Animator.SetLookAtPosition(headLookingPos);
            if (EquipedWeapon != null)
            {
                if (EquipedWeapon.SecondHandPivot != null)
                {
                    Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    if (!Ready)
                    {
                        Animator.SetIKPosition(AvatarIKGoal.LeftHand, EquipedWeapon.SecondHandPivot.position);
                        Animator.SetBoneLocalRotation(HumanBodyBones.LeftHand, EquipedWeapon.SecondHandPivot.rotation);
                    }
                    else
                    {
                        Animator.SetIKPosition(AvatarIKGoal.LeftHand, EquipedWeapon.SecondHandPivotShooting.position);
                        Animator.SetBoneLocalRotation(HumanBodyBones.LeftHand, EquipedWeapon.SecondHandPivotShooting.rotation);
                    }
                }
                Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                Animator.SetBoneLocalRotation(HumanBodyBones.RightHand, Quaternion.LookRotation(InputManager.Instance.MouseWorldPosition - EquipedWeapon.transform.position));
            }
        }

        public override void Kill()
        {
            IsDead = true;
            Animator.enabled = false;
            GetComponents<CapsuleCollider>().ForEach((col) => col.enabled = false);
            Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in bodies)
            {
                rb.isKinematic = false;
            }
            foreach (Collider col in ragdollColliders)
            {
                col.isTrigger = false;
            }
            Invoke("Respawn", 5);
        }

        private void Respawn()
        {
            Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in bodies)
            {
                rb.isKinematic = true;
            }
            IsDead = false;
            stats["Health"].Clear();
            GetComponents<CapsuleCollider>().ForEach((col) => col.enabled = true);
            Animator.enabled = true;
            foreach (Collider col in ragdollColliders)
            {
                if(col.GetComponent<CharacterController>() == null)
                    col.isTrigger = true;
            }
        }

        protected override void Init()
        {
            base.Init();
        }
    }
}