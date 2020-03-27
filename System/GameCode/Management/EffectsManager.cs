using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VovTech.Serialization;

namespace VovTech
{
    public class EffectsManager : MonoBehaviour
    {
        public static EffectsManager Instance => GameObject.Find("GameManagers").GetComponent<EffectsManager>();

        public GameObject SpawnEffect(GameObject effect, Vector3 position, Quaternion rotation)
        {
            GameObject eff = Instantiate(effect, position, rotation);
            if (NetManager.Instance.Connect)
            {
                //Sending packet to the server
                using (Packet packet = new Packet())
                {
                    SpawnData effectSpawnData = new SpawnData();
                    effectSpawnData.DataType = typeof(SpawnData).ToString();
                    effectSpawnData.Position = position;
                    effectSpawnData.Prefix = "PREFIX: EFFECT";
                    effectSpawnData.PrefabResourcePath = effect.name;
                    effectSpawnData.Rotation = rotation.eulerAngles;
                    packet.Write(JsonUtility.ToJson(effectSpawnData, true));
                    NetManager.Instance.SendData(packet);
                }
                //----------------------------
            }
            return eff;
        }
    }
}