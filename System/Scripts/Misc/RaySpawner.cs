using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech
{
    public class RaySpawner : Module
    {
        public float RayLength;
        public List<GameObject> ObjectsToSpawn = new List<GameObject>();
        public float Interval;
        public Vector3 RayOrigin;
        public Vector3 RayDirection;
        public bool RotateToDirection;
        public Action UpdateAction;
        public Ray CustomRay;

        private float currentInterval;

        private void Start()
        {
            currentInterval = 0;
        }

        private void FixedUpdate()
        {
            currentInterval = Mathf.Clamp(currentInterval - Time.fixedDeltaTime, 0, Interval);
        }

        private void Update()
        {
            if(ObjectsToSpawn != null && Enabled)
            {
                RaycastHit hit;
                Ray ray = (CustomRay.Equals(default)) ? new Ray(RayOrigin, RayDirection) : CustomRay;
                if(Physics.Raycast(ray, out hit, RayLength))
                {
                    if(currentInterval == 0)
                    {
                        foreach (GameObject go in ObjectsToSpawn)
                        {
                            Quaternion rot = (RotateToDirection) ? Quaternion.LookRotation(RayDirection) : Quaternion.identity;
                            Instantiate(go, hit.point, rot);
                        }
                        currentInterval = Interval;
                    }
                }
                UpdateAction?.Invoke();
            }
        }
    }
}