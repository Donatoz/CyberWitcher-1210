using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class AnimationModule : Module
    {
        public Animator AttachedAnimator;
        public Actor AttachedActor => AttachedEntity as Actor;
        public List<IKBonesAnimationInfo> IKAnimationsSet;
        private IKBonesAnimationInfo currentIKClip;

        private bool animateLocalCharacter;

        private void Start()
        {
            animateLocalCharacter = AttachedActor is Character && ((Character)AttachedActor).Owner.Local;
        }

        private void FixedUpdate()
        {
            if(Enabled && animateLocalCharacter)
            {
                Character character = AttachedActor as Character;
                if (character.EquipedWeapon != null && character.Ready)
                    SetAnimationTrigger("Aim" + character.EquipedWeapon.Settings.HoldingType.ToString());
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                    SetAnimationTrigger("Run");
                else
                {
                    if (!character.Ready && character.EquipedWeapon != null)
                    {
                        SetAnimationTrigger("Idle" + character.EquipedWeapon.Settings.HoldingType.ToString());
                    }
                    else
                    {
                        SetAnimationTrigger("Idle1");
                    }
                }
            }
        }

        public void SetAnimationTrigger(string name)
        {
            AttachedAnimator.SetTrigger(name);
            AttachedActor.CurrentAnimation = name;
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if(Enabled && currentIKClip != null)
            {
                foreach(BonePosition pos in currentIKClip.Positions)
                {
                    AttachedAnimator.SetIKPosition(pos.Bone, pos.DestinationTransform.position);
                    AttachedAnimator.SetIKRotation(pos.Bone, pos.DestinationTransform.rotation);
                }
            }
        }

        public void SetClip(string name)
        {
            IKBonesAnimationInfo info = IKAnimationsSet.Find(x => x.AnimationTriggerName == name);
            currentIKClip = info != null ? info : null;
        }
    }

    [System.Serializable]
    public class IKBonesAnimationInfo
    {
        public string AnimationTriggerName;
        public List<BonePosition> Positions;
    }

    [System.Serializable]
    public struct BonePosition
    {
        public AvatarIKGoal Bone;
        public Transform DestinationTransform;
    }
}