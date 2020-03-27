using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public abstract class Attachment : Item
    {
        public AttachmentInfo Settings;
        public Dictionary<string, float> StatsToAffect { get; protected set; }
        protected System.Action FixedUpdateContext;

        protected virtual void FixedUpdate()
        {
            FixedUpdateContext?.Invoke();
        }

        public override void Initialize()
        {
            if (Settings == null) return;
            StatsToAffect = new Dictionary<string, float>();
            foreach (AttachmentInfo.StatAffectInfo info in Settings.StatsToAffect)
            {
                StatsToAffect[info.StatName] = info.AffectValue;
            }
        }
    }
}