using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class ItemAttachment : Attachment, IAttachable<Item>
    {
        public Item AttachedObject { get; private set; }

        protected override void Start()
        {
            base.Start();
            Initialize();
            FixedUpdateContext = () => {
                if (AttachedObject != null)
                    StopCoroutine(rotatingCoroutine);
            };
        }

        public void Attach(Item item)
        {
            AttachmentPosition[] positions = 
                item.AttachmentsContainer.GetComponentsInChildren<AttachmentPosition>();
            foreach(AttachmentPosition p in positions)
            {
                if(p.AttachmentType == Settings.Type && p.AttachedObject == null)
                {
                    transform.position = p.transform.position;
                    transform.rotation = p.transform.rotation;
                    transform.parent = p.transform;
                    p.AttachedObject = this;
                    AttachedObject = item;
                    if (StatsToAffect != null)
                    {
                        foreach (var i in StatsToAffect)
                        {
                            if (item.ItemStats.ContainsKey(i.Key))
                                item.ItemStats[i.Key].AddModifier(i.Value, $"{Settings.Name}Modifier");
                        }
                    }
                    AttachedObject.Attachments.Add(this);
                    break;
                }
            }
        }

        public void DeAttach()
        {
            if(AttachedObject != null)
            {
                foreach (var i in StatsToAffect)
                {
                    if (AttachedObject.ItemStats.ContainsKey(i.Key))
                        AttachedObject.ItemStats[i.Key].RemoveModifier($"{Settings.Name}Modifier");
                }
                AttachedObject = null;
            }
        }
    }
}