using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

namespace VovTech {
    public sealed class MainMenu : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI pressAnyButton;
        [SerializeField]
        private Image blur;
        [SerializeField]
        private Image bg;
        [SerializeField]
        private GameObject spinner;
        [SerializeField]
        private TextMeshProUGUI loadingLevel;
        [SerializeField]
        private Image glow;

        public Action<MainMenuButton.ButtonType> OnButtonPressed;
    
        private void Start()
        {
            OnButtonPressed = delegate (MainMenuButton.ButtonType button)
            {
                switch(button)
                {
                    case MainMenuButton.ButtonType.Exit:
                        Application.Quit();
                        break;
                }
            };
            StartCoroutine(EnterMenu());
        }

        private IEnumerator EnterMenu()
        {
            yield return new WaitForSeconds(4);
            while (true)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                if (Input.anyKeyDown)
                {
                    DOTween.Play("Enter");
                    pressAnyButton.DOFade(0, 1);
                    pressAnyButton.DOScale(0.1f, 1);
                    break;
                }
            }
        }

        public void LoadScene(string sceneId)
        {
            StartCoroutine(AsyncSceneLoad(sceneId));
            blur.material.DOFloat(50, "_BumpAmt", 3);
            blur.material.DOFloat(0.7f, "_Size", 3);
            bg.DOFade(0.8f, 1);
            foreach (Transform t in spinner.transform)
            {
                t.GetComponent<Image>().DOFade(1, 1);
            }
            loadingLevel.DOFade(1, 1);
            glow.material.DOColor(new Color((float)73 / (float)255,
                (float)54 / (float)255,
                (float)191 / (float)255,
                (float)23 / (float)255), "_MainColor", 1);
        }

        private void OnDestroy()
        {
            blur.material.SetFloat("_BumpAmt", 0);
            blur.material.SetFloat("_Size", 0);
        }

        private IEnumerator AsyncSceneLoad(string sceneId)
        {
            yield return new WaitForSeconds(4);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneId);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}