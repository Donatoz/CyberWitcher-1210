using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech
{
    public abstract class Module : MonoBehaviour
    {
        public Entity AttachedEntity;
        public bool Enabled;
    }
}