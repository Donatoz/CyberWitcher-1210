using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech {
    public class MainMenuButton : MonoBehaviour
    {
        public enum ButtonType
        {
            Play,
            Options,
            Authors,
            Exit
        }

        public float MaximumDissolve;
        public float MinimumDissolve;
        public float Speed = 12f;
        public string SceneName;
        public List<SpriteRenderer> SpritesToFade;
        public ButtonType Type;

        private Material dissolveMaterial;
        private float currentDissolve;
        private float spriteFade;

        private void Start()
        {
            currentDissolve = MinimumDissolve;
            spriteFade = 1;
            dissolveMaterial = GetComponent<MeshRenderer>().materials[1];
        }

        void Update()
        {
            dissolveMaterial.SetFloat("_MaskAppearProgress",
                Mathf.Lerp(dissolveMaterial.GetFloat("_MaskAppearProgress"), currentDissolve, Time.deltaTime * Speed));
            foreach (SpriteRenderer sr in SpritesToFade)
            {
                sr.color = Color.Lerp(sr.color, sr.color.Alpha(spriteFade), Time.deltaTime * Speed);
            }
        }

        private void OnMouseEnter()
        {
            currentDissolve = MaximumDissolve;
            spriteFade = 0;
        }

        private void OnMouseExit()
        {
            currentDissolve = MinimumDissolve;
            spriteFade = 1;
        }

        public void OnMouseDown()
        {
            if (SceneName != string.Empty)
            {
                Camera.main.GetComponent<MainMenu>().OnButtonPressed.Invoke(Type);
                Camera.main.GetComponent<MainMenu>().LoadScene(SceneName);
            };
        }
    }
}