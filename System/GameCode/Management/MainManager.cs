using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using com.ootii.Cameras;

using Random = UnityEngine.Random;

namespace VovTech
{
    public class MainManager : MonoBehaviour
    {
        public static MainManager Instance => GameObject.Find("GameManagers").GetComponent<MainManager>();

        public Player LocalPlayer;
        public CameraController MainCameraController;

        [SerializeField]
        private List<Entity> sceneEntities;
        [SerializeField]
        private bool debug;
        [SerializeField]
        private List<GameObject> debugObjects;
        [SerializeField]
        private int idSeed;

        private void Awake()
        {
            sceneEntities = new List<Entity>();
        }

        public void GiveId(out string refId)
        {
            int seconds = Mathf.RoundToInt(Time.time);
            string id = seconds.ToString("X") + sceneEntities.Count.ToString("X") 
                + Random.Range(0, Random.Range(2, 1 + Mathf.Clamp(idSeed, 2, 100)) 
                    + Mathf.Pow(sceneEntities.Count, Random.Range(1, Random.Range(2, idSeed))));
            id = id.Replace(',', Convert.ToChar(Random.Range(65, 90)));
            refId = id;
        }

        public void AddEntity(Entity e)
        {
            sceneEntities.Add(e);
        }

        public GameObject Spawn(GameObject go, Vector3 pos, Quaternion rot)
        {
            return Instantiate(go, pos, rot);
        }

        public UnityEngine.Object LoadResource(string path)
        {
            return Resources.Load<UnityEngine.Object>(path);
        }

        public GameObject Spawn(GameObject go, GameObject parent)
        {
            return (parent != null) ? Instantiate(go, parent.transform) : Instantiate(go);
        }
            
        public Actor SpawnActor(GameObject actorPrefab, Vector3 position)
        {
            Actor actor = Instantiate(actorPrefab, position, Quaternion.identity).GetComponent<Actor>();
            actor.Initialize();
            return actor;
        }

        public Weapon SpawnWeapon(GameObject weaponPrefab, Vector3 position)
        {
            if (weaponPrefab.GetComponent<Weapon>() == null) return null;
            Weapon weapon = Instantiate(weaponPrefab, position, Quaternion.identity).GetComponent<Weapon>();
            weapon.Initialize();
            return weapon;
        }

        public List<Entity> GetSceneEntities()
        {
            return sceneEntities;
        }
    }
}