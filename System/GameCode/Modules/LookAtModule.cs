using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public class LookAtModule : Module
    {
        public Transform Target;
        public float Speed;
        public bool Mouse;
        public bool NegativeZ;

        private Vector3 pos;

        private void Update()
        {
            if(Enabled)
            {
                pos = (Mouse) ? InputManager.Instance.MouseWorldPosition : Target.position;
                Vector3 lookDir = (NegativeZ) ? transform.position - pos : pos - transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation,
                    Quaternion.LookRotation(lookDir), Time.deltaTime * Speed);
            }
        }
    }
}