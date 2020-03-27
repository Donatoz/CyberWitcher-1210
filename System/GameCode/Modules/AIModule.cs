using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class AIModule : Module
    {
        [SerializeField]
        private AITemplate aiTemplate;
        private NPC attachedNpc;

        private void Start()
        {
            attachedNpc = AttachedEntity.AsActor().AsNPC();
        }

        public void SetTemplate(AITemplate aiTemplate)
        {
            this.aiTemplate = aiTemplate;
        }

        private void FixedUpdate()
        {
            if(Enabled && aiTemplate != null)
            {
                aiTemplate.Behave();
            }
        }
    }
}