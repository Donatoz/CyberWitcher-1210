using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    /// <summary>
    /// Any pickable item in the game.
    /// </summary>
    public abstract class Item : Entity
    {
        public delegate void PickedUpDelegate(Actor picker);
        /// <summary>
        /// Item stats.
        /// </summary>
        public Dictionary<string, Stat> ItemStats;
        /// <summary>
        /// Invoked when item is being picked up.
        /// </summary>
        public event PickedUpDelegate OnPickedUp;
        [Header("Item settings")]
        /// <summary>
        /// Container for item attachments.
        /// </summary>
        public Transform AttachmentsContainer;
        /// <summary>
        /// Attachments to attach in Start() function.
        /// </summary>
        public List<ItemAttachment> StartAttachments;
        /// <summary>
        /// All attachments.
        /// </summary>
        public List<IAttachable<Item>> Attachments;
        /// <summary>
        /// Rotating coroutine (when this item has no owner).
        /// </summary>
        public Coroutine rotatingCoroutine { get; protected set; }
        /// <summary>
        /// Owner of the item.
        /// </summary>
        public Actor Owner;
        [SerializeField]
        protected AudioClip pickUpSound;
        [SerializeField]
        [Range(0f, 1f)]
        protected float pickUpSoundVolume;

        private void Awake()
        {
            ItemStats = new Dictionary<string, Stat>();
            Init();
        }

        protected virtual void Start()
        {
            Attachments = new List<IAttachable<Item>>();
            rotatingCoroutine = StartCustomCoroutine(
                () => { transform.RotateAround(transform.position, transform.up, 20f * Time.deltaTime); },
                -1, 0.03f);
            if(StartAttachments != null)
            {
                foreach(ItemAttachment att in StartAttachments)
                {
                    att.Attach(this);
                }
            }
            OnPickedUp += delegate
            {
                if(pickUpSound != null)
                    SoundManager.Instance.PlayClipAtPoint(pickUpSound, transform.position, 0.1f);
            };
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.GetComponent<Actor>() != null)
                OnPickedUp?.Invoke(other.gameObject.GetComponent<Actor>());
        }
    }
}