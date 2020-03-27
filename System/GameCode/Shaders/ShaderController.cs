using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace VovTech
{
    public class ShaderController : MonoBehaviour
    {
        private Material mat;

        private void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
        }

        public void ChangeFloat(string property, float finalValue, float duration, bool yoyo = false, float yoyoDelay = 0, float yoyoTimeMul = 1)
        {
            float startValue = mat.GetFloat(property);
            Tween tween = mat.DOFloat(finalValue, property, duration);
            if(yoyo)
            {
                tween.OnComplete(() => { mat.DOFloat(startValue, property, duration).SetDelay(yoyoDelay * yoyoTimeMul); });
            }
        }

        public void ChangeColor(string property, Color finalValue, float duration, bool yoyo = false, float yoyoDelay = 0, float yoyoTimeMul = 1)
        {
            Color startValue = mat.GetColor(property);
            Tween tween = mat.DOColor(finalValue, property, duration);
            if (yoyo)
            {
                tween.OnComplete(() => { mat.DOColor(startValue, property, duration).SetDelay(yoyoDelay * yoyoTimeMul); });
            }
        }
    }
}