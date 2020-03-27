using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VovTech.Behaviours;

namespace VovTech
{
    public class MeleeWeapon : Weapon
    {
        [Header("Melee weapon settings")]
        public List<string> AttackAnimationsNameList;
        public float BusyTime = 0;

        protected override void Start()
        {
            Attachments = new List<IAttachable<Item>>();
            rotatingCoroutine = StartCustomCoroutine(
                () => { transform.RotateAround(transform.position, transform.up, 20f * Time.deltaTime); },
                -1, 0.03f);
            if (StartAttachments != null)
            {
                foreach (ItemAttachment att in StartAttachments)
                {
                    att.Attach(this);
                }
            }
            OnPickedUp += delegate
            {
                if (pickUpSound != null)
                    SoundManager.Instance.PlayClipAtPoint(pickUpSound, transform.position, 0.1f);
            };
            OnPickedUp += delegate {
                MeshRenderer mr = GetComponent<MeshRenderer>();
                if (mr != null && mr.materials.Length > 1)
                    mr.materials[1].SetFloat("_OutlineWidth", 0f);
            };
            Initialize();
        }

        private void FixedUpdate()
        {
            BusyTime = Mathf.Clamp(BusyTime - Time.fixedDeltaTime, 0, int.MaxValue);
        }

        public override void Initialize()
        {
            ItemStats = new Dictionary<string, Stat>();
            ItemStats["HitDamage"] = new Stat(Settings.HitDamage);
            ItemStats["AttackInterval"] = new Stat(Settings.AttackInterval);
            if (customShootingBehaviour != null)
            {
                Behaviour = ScriptableObject.CreateInstance(customShootingBehaviour.GetClass()) as ShootingBehaviour;
                OnShoot = delegate
                {
                    Behaviour.OnShoot.Invoke(this);
                };
            }
        }

        public override void Shoot(Vector3 target = default)
        {
            if (BusyTime > 0) return;
            if (Behaviour == null)
            {
                if (AttackAnimationsNameList.Count > 0)
                {
                    string animName = AttackAnimationsNameList.RandomItem();
                    Debug.Log(animName);
                    Owner.Animator.SetTrigger(animName);
                }
            } else
            {
                Behaviour.OnShoot.Invoke(this);
            }
        }
    }
}