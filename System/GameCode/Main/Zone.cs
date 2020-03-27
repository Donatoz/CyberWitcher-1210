using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech
{
    public class Zone : MonoBehaviour
    {
        public string ZoneId;
        public Action<Entity> OnEnter;
        public Action<Entity> OnExit;
        public Predicate<Entity> RightEntity;
        [BitMask(typeof(EntityType))]
        public EntityType Mask;
        [BitMask(typeof(Fraction))]
        public Fraction FractionMask;
        public List<string> OnEnterTriggers;
        public List<string> OnExitTriggers;
        [Tooltip("How many times zone may activate OnEnter triggers.")]
        public int ZoneLives = 1;
        public List<DelayedAudioClip> OnEnterSounds;
        [Range(0f, 1f)]
        public float OnEnterSoundVolume;

        public List<string> DoorsToOpen;
        public List<string> DoorsToClose;

        [SerializeField]
        private Color gizmosColor = Color.green;

        private void Start()
        {
            OnEnter = delegate (Entity e)
            {
                if (OnEnterSounds.Count > 0)
                    foreach (DelayedAudioClip clip in OnEnterSounds)
                    {
                        Entity.GetById("DummyEntity").DelayedInvoke(() =>
                        {
                            SoundManager.Instance.PlayClipAtPoint(clip.Clip, transform.position, OnEnterSoundVolume);
                        }, clip.Delay);
                    }
            };

            RightEntity = delegate (Entity e)
            {
                return true;
            };
        }

        private void OnTriggerEnter(Collider other)
        {
            Entity e = other.gameObject.GetComponent<Entity>();
            if (e != null)
            {
                if (RightEntity(e) && ZoneLives > 0)
                {
                    OnEnter?.Invoke(e);
                    foreach(string s in OnEnterTriggers)
                    {
                        LevelManager.Instance.ActivateTrigger(s);
                    }
                    ZoneLives--;
                    foreach(string s in DoorsToOpen)
                    {
                        (Entity.GetById(s) as Door).Open();
                    }
                    foreach (string s in DoorsToClose)
                    {
                        try
                        {
                            (Entity.GetById(s) as Door).Open();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Entity e = other.gameObject.GetComponent<Entity>();
            if (e != null)
            {
                if (RightEntity(e))
                {
                    OnExit?.Invoke(e);
                    foreach (string s in OnExitTriggers)
                    {
                        LevelManager.Instance.ActivateTrigger(s);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmosColor;
            Gizmos.DrawCube(transform.position + GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
        }
    }
}