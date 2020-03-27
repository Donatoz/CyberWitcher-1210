using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VovTech.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class StartAnimation : MonoBehaviour
    {
        [SerializeField]
        private string TriggerName;

        private void Start()
        {
            GetComponent<Animator>().SetTrigger(TriggerName);
        }
    }
}