using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VovTech.UI;

namespace VovTech
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance => GameObject.Find("GameManagers").GetComponent<UIManager>();

        public Canvas MainCanvas;
        public Canvas CameraCanvas;

        [SerializeField]
        private TextMeshProUGUI fpsCounter;
        [SerializeField]
        private GameObject logPrefab;
        [SerializeField]
        private Transform logsRoot;
        public Color ErrorColor;
        public Color SuccessColor;
        public Color DefaultMessageColor;

        private void Update()
        {
            fpsCounter.text = "FPS:" + Mathf.Round(1.0f / Time.deltaTime).ToString();
        }

        public void Log(string message, Color color)
        {
            TextMeshProUGUI mes = Instantiate(logPrefab, logsRoot).GetComponent<TextMeshProUGUI>();
            mes.text = message;
            mes.color = color;
        }

        /// <summary>
        /// Build UI Element on CameraCanvas.
        /// </summary>
        /// <param name="strategy">Building strategy</param>
        /// <param name="pos">Element position</param>
        public GameObject BuildUIElement(UIBuildStrategy strategy, Vector3 pos)
        {
            GameObject elementGo = strategy.Build().gameObject;
            elementGo.transform.position = pos;
            return elementGo;
        }

        public void CastSkill(int id)
        {
            MainManager.Instance.LocalPlayer.ControlledCharacter.CastSkill(id);
        }
    }
}