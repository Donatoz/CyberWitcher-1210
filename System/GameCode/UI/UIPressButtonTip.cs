using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace VovTech.UI
{
    public class UIPressButtonTip : UIEntity
    {
        public TextMeshProUGUI ButtonText;
        public TextMeshProUGUI AfterButtonText;
        public KeyCode Key;

        public void PopulateWithInfo(string buttonValue, string afterText, KeyCode key)
        {
            ButtonText.text = buttonValue;
            AfterButtonText.text = afterText;
            Key = key;
        }

        private void Start()
        {
            StartCoroutine(WaitForInput());
        }

        private IEnumerator WaitForInput()
        {
            yield return new WaitWhile(() => Input.GetButtonDown(Key.ToString()));
            transform.DOScale(0, 0.4f).SetEase(Ease.InCubic).OnComplete(() => { Destroy(gameObject); });
        }
    }
}