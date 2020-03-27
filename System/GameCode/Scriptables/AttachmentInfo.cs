using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    [CreateAssetMenu(fileName = "New Attachment", menuName = "VovTech/Attachment", order = 55)]
    public class AttachmentInfo : ItemInfo
    {
        [System.Serializable]
        public struct StatAffectInfo
        {
            public string StatName;
            public float AffectValue;
        }
        public AttachmentType Type;
        public List<StatAffectInfo> StatsToAffect;
    }
}