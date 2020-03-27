using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VovTech
{
    public class ActorAttachment : Attachment, IAttachable<Actor>
    {
        public Actor AttachedObject { get; private set; }

        protected override void Start()
        {
            base.Start();
            Initialize();
            FixedUpdateContext = () => {
                if (AttachedObject != null)
                    StopCoroutine(rotatingCoroutine);
            };
            OnPickedUp += Attach;
        }

        public void Attach(Actor target)
        {
            AttachmentPosition[] positions =
                target.AttachmentsContainer.GetComponentsInChildren<AttachmentPosition>();
            foreach (AttachmentPosition p in positions)
            {
                if (p.AttachmentType == Settings.Type && p.AttachedObject == null)
                {
                    transform.position = p.transform.position;
                    transform.rotation = p.transform.rotation;
                    transform.parent = p.transform;
                    p.AttachedObject = this;
                    AttachedObject = target;
                    foreach (Transform t in transform) { t.gameObject.SetActive(true); }
                    GetComponentsInChildren<ParticleSystem>().ForEach((x) => x.gameObject.SetActive(false));
                    GetComponent<MeshRenderer>().materials[1].SetFloat("_OutlineWidth", 0f);
                    if (StatsToAffect != null)
                    {
                        foreach (var i in StatsToAffect)
                        {
                            if (target.GetStat(i.Key) != null)
                                target.GetStat(i.Key).AddModifier(i.Value, $"{Settings.Name}Modifier");
                        }
                    }
                    AttachedObject.Attachments.Add(this);
                    break;
                }
            }
        }

        public void DeAttach()
        {
            if (AttachedObject != null)
            {
                foreach (var i in StatsToAffect)
                {
                    if (AttachedObject.GetStat(i.Key) != null)
                        AttachedObject.GetStat(i.Key).RemoveModifier($"{Settings.Name}Modifier");
                }
                AttachedObject = null;
            }
        }
    }
}