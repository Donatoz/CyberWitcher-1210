using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.AI;

namespace VovTech
{
    public class Door : InteractiveObject
    {
        [Serializable]
        public struct DoorPart
        {
            public GameObject DoorObject;
            public Vector3 ClosedPosition;
            public Vector3 OpenedPosition;
            public Vector3 ClosedRotation;
            public Vector3 OpenedRotation;
        }

        public List<DoorPart> DoorParts;
        public float OpenDuration;
        public float CloseDuration;
        public bool LocalMove;
        public Ease OpenEase;
        public Ease CloseEase;

        public void Open()
        {
            foreach(DoorPart part in DoorParts)
            {
                if (LocalMove)
                {
                    part.DoorObject.transform.DOLocalMove(part.OpenedPosition, OpenDuration).SetEase(OpenEase);
                    part.DoorObject.transform.DOLocalRotate(part.OpenedRotation, OpenDuration).SetEase(OpenEase);
                }
                else
                {
                    part.DoorObject.transform.DOMove(part.OpenedPosition, OpenDuration).SetEase(OpenEase);
                    part.DoorObject.transform.DORotate(part.OpenedRotation, OpenDuration).SetEase(OpenEase);
                }
            }
            DelayedInvoke(() => {
            GetComponentsInChildren<NavMeshObstacle>().ForEach((x) => x.gameObject.SetActive(false) );
            }, OpenDuration * 0.2f);
        }

        public void Close()
        {
            foreach (DoorPart part in DoorParts)
            {
                if (LocalMove)
                {
                    part.DoorObject.transform.DOLocalMove(part.ClosedPosition, CloseDuration).SetEase(CloseEase);
                    part.DoorObject.transform.DOLocalRotate(part.ClosedRotation, CloseDuration).SetEase(OpenEase);
                }
                else
                {
                    part.DoorObject.transform.DOMove(part.ClosedPosition, CloseDuration).SetEase(CloseEase);
                    part.DoorObject.transform.DORotate(part.ClosedRotation, CloseDuration).SetEase(OpenEase);
                }
            }
            DelayedInvoke(() => {
                GetComponentsInChildren<NavMeshObstacle>().ForEach((x) => x.gameObject.SetActive(true));
            }, CloseDuration * 0.2f);
        }
    }
}