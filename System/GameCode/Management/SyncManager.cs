using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VovTech
{
    [RequireComponent(typeof(NetManager))]
    public class SyncManager : MonoBehaviour
    {
        public static SyncManager Instance => GameObject.Find("GameManagers").GetComponent<SyncManager>();
        public float TickRate = 60;

        private void Start()
        {
            StartCoroutine(Tick());
        }

        private IEnumerator Tick()
        {
            while(true)
            {
                var syncObjects = FindObjectsOfType<Entity>().OfType<ISynchronizable>().ToArray();
                for (int i = 0; i < syncObjects.Length; i++)
                {
                    syncObjects[i].SendData();
                }
                yield return new WaitForSeconds(1f / TickRate);
            }
        }

    }
}