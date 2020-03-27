using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using VovTech.Behaviours;
using VovTech.UI;

namespace VovTech
{
    public class Weapon : Item
    {
        [Serializable]
        public struct VisualAttachment
        {
            public string Name;
            public GameObject Reference;
        }

        [Header("Weapon settings")]
        public WeaponInfo Settings;
        public Transform ProjectilesSpawn;
        public Transform SecondHandPivot;
        public Transform SecondHandPivotShooting;
        public List<VisualAttachment> VisualAttachments;
        public Action OnShoot;
        public ShootingBehaviour Behaviour;
        public List<StatAffector> Prefixes;
        public bool UIEnabled;

        [SerializeField]
        private bool debug;
        private ShootingModule shootingModule;
        private UIWeaponInfo uiInfo;
        [SerializeField]
        protected MonoScript customShootingBehaviour;
        protected Action<Character> OnPickUpAction;

        protected virtual void Awake()
        {
            ItemStats = new Dictionary<string, Stat>();
            if(Prefixes == null)
                Prefixes = new List<StatAffector>();
            OnPickUpAction = delegate (Character character)
            {
                transform.Find("ShineFx")?.gameObject.SetActive(false);
                GetComponent<Collider>().enabled = false;
                transform.position = GameObject.Find("WeaponsSpot").transform.position;
                gameObject.SetActive(false);
                if(rotatingCoroutine != null) StopCoroutine(rotatingCoroutine);
                character.RecieveItem(this);
                Owner = character;
            };
            Init();
        }

        protected override void Start()
        {
            base.Start();
            OnPickedUp += delegate {
                MeshRenderer mr = GetComponent<MeshRenderer>();
                if(mr != null)
                    mr.materials[1].SetFloat("_OutlineWidth", 0f);
            };
            Initialize();
        }

        private void FixedUpdate()
        {
            if(Owner != null && rotatingCoroutine != null)
            {
                StopCoroutine(rotatingCoroutine);
                rotatingCoroutine = null;
            }
            if (UIEnabled)
            {
                if (Vector3.Distance(
                    MainManager.Instance.LocalPlayer.ControlledCharacter.transform.position,
                    transform.position) < 4)
                {
                    if (uiInfo == null)
                    {
                        uiInfo = Instantiate(
                            Resources.Load<GameObject>("UI/WeaponInfo"),
                            UIManager.Instance.CameraCanvas.transform).GetComponent<UIWeaponInfo>();
                        uiInfo.Attach(gameObject);
                        uiInfo.PopulateWithInfo(this);
                    }
                }
                else
                {
                    if (uiInfo != null) uiInfo.Vanish();
                }
            } else
            {
                if (uiInfo != null) uiInfo.Vanish();
            }
        }

        public override void Initialize()
        {
            ItemStats["Spreading"] = new Stat(Settings.Spreading);
            ItemStats["ShotInterval"] = new Stat(Settings.ShotInterval);
            ItemStats["Weight"] = new Stat(Settings.Weight);
            ItemStats["Price"] = new Stat(Settings.Price);
            ItemStats["ClipSize"] = new Stat(Settings.ClipSize);
            ItemStats["ReloadTime"] = new Stat(Settings.ReloadTime);
            Name = Settings.Name;
            shootingModule = gameObject.AddComponent<ShootingModule>();
            shootingModule.LoadedProjectile = Settings.Projectile;
            shootingModule.ProjectileSpawn = ProjectilesSpawn;
            shootingModule.ShotEffect = Settings.ShootEffect;
            shootingModule.AttachedEntity = this;
            shootingModule.Initialize(Settings);
            if (customShootingBehaviour != null)
            {
                Behaviour = ScriptableObject.CreateInstance(customShootingBehaviour.GetClass()) as ShootingBehaviour;
                OnShoot = delegate
                {
                    Behaviour.OnShoot.Invoke(this);
                };
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            Debug.Log("Picked up weapon");
            Character character = other.GetComponent<Character>();
            if (character != null && Owner == null)
            {
                OnPickUpAction?.Invoke(character);
                UIEnabled = false;
            }
        }

        public void ClearEffects()
        {
            StopAllCoroutines();
            MeshRenderer thisMr = GetComponent<MeshRenderer>();
            if(thisMr != null)
                thisMr.materials[1].SetFloat("_OutlineWidth", 0);
            transform.Find("ShineFx")?.gameObject.SetActive(false);
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer mr in renderers)
            {
                if(mr.materials.Length > 1)
                    mr.materials[1].SetFloat("_OutlineWidth", 0);
            }
        }

        public void RestoreEffects()
        {
            if(GetComponent<MeshRenderer>() != null)
                GetComponent<MeshRenderer>().materials[1].SetFloat("_OutlineWidth", 0.01f);
            transform.Find("ShineFx").gameObject.SetActive(true);
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in renderers)
            {
                mr.materials[1].SetFloat("_OutlineWidth", 0.01f);
            }
        }

        public virtual void Shoot(Vector3 target = default)
        {
            shootingModule.Shoot(target);
        }
    }
}