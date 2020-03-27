using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace VovTech
{
    public class InteractiveObject : Entity
    {
        public bool Dragable;
        public InteractiveObject ChangeLightsColor(Color color, float changeDuration)
        {
            Light[] lights = GetComponentsInChildren<Light>();
            foreach(Light light in lights)
            {
                light.DOColor(color, changeDuration);
            }
            return this;
        }

        public InteractiveObject ChangeMaterialColor(string colorId, Color color, float changeDuration)
        {
            Material[] materials = GetComponent<MeshRenderer>().materials;
            foreach(Material mat in materials)
            {
                mat.DOColor(color, colorId, changeDuration);
            }
            return this;
        }

        public void AddPhysics()
        {
            gameObject.AddComponent<Rigidbody>();
        }

        public void RemovePhysics()
        {
            if(GetComponent<Rigidbody>() != null) Destroy(gameObject.GetComponent<Rigidbody>());
        }

        private void Start()
        {
            Init();
        }

        public override void Initialize()
        {
            
        }

        public static InteractiveObject[] GetGroup(string groupId)
        {
            List<InteractiveObject> foundObjects = new List<InteractiveObject>();
            foreach(Entity obj in MainManager.Instance.GetSceneEntities())
            {
                if (obj is InteractiveObject && (obj as InteractiveObject).Groups.Contains(groupId))
                {
                    foundObjects.Add(obj as InteractiveObject);
                }
            }
            return foundObjects.ToArray();
        }
    }
}