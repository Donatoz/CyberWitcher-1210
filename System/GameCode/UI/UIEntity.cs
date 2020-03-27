using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

namespace VovTech.UI
{
    public abstract class UIEntity : MonoBehaviour
    {
        public List<Image> ColoredImages;
        protected Action vanishAnimation;

        public void LookAtCamera(float speed)
        {
            if(GetComponent<LookAtModule>() == null)
            {
                LookAtModule lookModule = gameObject.AddComponent<LookAtModule>();
                lookModule.Target = Camera.main.transform;
                lookModule.Speed = 4;
                lookModule.Enabled = true;
                lookModule.NegativeZ = true;
            }
        }

        public void SetColor(Color color)
        {
            foreach(Image img in ColoredImages)
            {
                img.color = color;
            }
        }

        public virtual void Vanish()
        {
            vanishAnimation?.Invoke();
        }
    }
}