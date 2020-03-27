using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using VovTech.Serialization;

namespace VovTech
{
    public class NetManager : MonoBehaviour
    {
        public delegate void OnObjectSpawn(string path, SpawnData info);

        public enum Protocol
        {
            TCP,
            UDP
        }

        public enum ServerAddress
        {
            Loopback,
            LocalGleb
        }
        public static NetManager Instance => GameObject.Find("GameManagers").GetComponent<NetManager>();
        /// <summary>
        /// Connect to server?
        /// </summary>
        public bool Connect;
        /// <summary>
        /// Protocol to use.
        /// </summary>
        public Protocol ClientProtocol;
        public ServerAddress Address;
        [HideInInspector]
        /// <summary>
        /// Host IP address.
        /// </summary>
        public string IpAddress;
        /// <summary>
        /// Host port.
        /// </summary>
        public int Port;
        /// <summary>
        /// Connection time-out.
        /// </summary>
        public float TimeOut = 10f;
        /// <summary>
        /// Size of sending and recievning packets (in bytes).
        /// </summary>
        public int RecievePacketSize = 16384;

        #region private members 
        private INetworkModule module;
        private bool sendPackets;
        public int PacketSendLimit = 200;

        private int packetsInSecond = 0;
        public int PacketsSendInSeconds = 0;

        public event OnObjectSpawn OnSpawn;

        [SerializeField]
        private NetSpawner spawner;

        [SerializeField]
        private Character otherPlayer;
        private CharacterData otherPlayerLastData;
        #endregion


        private void Start()
        {
            if (!Connect) return;
            OnSpawn += spawner.SpawnObject;
            if (Address == ServerAddress.LocalGleb)
            {
                IpAddress = "169.254.60.109";
            } else
            {
                IpAddress = "127.0.0.1";
            }
            // Initialize network module (tcp or udp)
            if (ClientProtocol == Protocol.TCP) module = gameObject.AddComponent<TcpModule>();
            else module = gameObject.AddComponent<UdpModule>();
            module.OnPacketRecieved += DecodeJson;
            module.Manager = this;
            module.Connect(IpAddress, Port);
            // Start packet-sending coroutine
            StartCoroutine(UpdatePackets());
            StartCoroutine(SendSelfInformation());
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha0))
            {
                sendPackets = !sendPackets;
            }
        }

        private IEnumerator SendSelfInformation()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f / PacketSendLimit);
                if (module != null && sendPackets)
                {
                    using (Packet packet = new Packet())
                    {
                        string data = MainManager.Instance.LocalPlayer.ControlledCharacter.Serialize();
                        packet.Write(data);
                        module.SendData(packet);
                    }
                }
            }
        }

        private IEnumerator UpdatePackets()
        {
            while(true)
            {
                yield return new WaitForSeconds(1);
                PacketsSendInSeconds = packetsInSecond;
                packetsInSecond = 0;
            }
        }

        public void NetUpdate()
        {
            if (otherPlayer != null && otherPlayerLastData != null)
            {
                otherPlayer.transform.position = Vector3.Lerp(otherPlayer.transform.position, otherPlayerLastData.Position, Time.deltaTime * 15);
                otherPlayer.transform.rotation = Quaternion.Lerp(otherPlayer.transform.rotation, Quaternion.Euler(otherPlayerLastData.Rotation), Time.deltaTime * 40);
                otherPlayer.transform.localScale = otherPlayerLastData.Size;
                otherPlayer.Name = otherPlayerLastData.Name;
                otherPlayer.name = otherPlayerLastData.Name;
                otherPlayer.Animator.SetTrigger(otherPlayerLastData.CurrentAnimation);
                otherPlayer.Look(otherPlayerLastData.LookingPos);
                foreach (StatData stat in otherPlayerLastData.ActorStats)
                {
                    otherPlayer.GetStat(stat.Name).Set(stat.Value);
                }
                if (otherPlayer.EquipedWeapon != null)
                {
                    if (otherPlayer.EquipedWeapon.Settings.Id != otherPlayerLastData.EquipedWeaponId)
                    {
                        Weapon weaponInInventory = otherPlayer.GetWeaponById(otherPlayerLastData.EquipedWeaponId);
                        if (weaponInInventory == null)
                        {
                            Weapon weapon = Instantiate(
                                WeaponDatabase.GetInstance().GetWeapon(otherPlayerLastData.EquipedWeaponId).Prefab,
                                otherPlayer.transform.position,
                                Quaternion.identity).GetComponent<Weapon>();
                        }
                        else
                        {
                            otherPlayer.EquipWeapon(weaponInInventory);
                        }
                    }
                }
                else
                {
                    if (otherPlayerLastData.EquipedWeaponId != -1)
                    {
                        MainManager.Instance.SpawnWeapon(WeaponDatabase.GetInstance().GetWeapon(
                            otherPlayerLastData.EquipedWeaponId).Prefab, otherPlayer.transform.position);
                    }
                }
            }
        }

        public void SendData(params Packet[] packets)
        {
            try
            {
                for (int i = 0; i < packets.Length; i++)
                {
                    module.SendData(packets);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public void DecodeJson(string json)
        {
            //TODO: Strategy Pattern
            try
            {
                PacketData jsonData = JsonUtility.FromJson<PacketData>(json);
                if (jsonData.DataType == typeof(CharacterData).ToString())
                {
                    CharacterData charData = JsonUtility.FromJson<CharacterData>(json);
                    otherPlayerLastData = charData;
                    NetUpdate();
                }
                else
                {
                    if (jsonData.DataType == typeof(ScriptableData).ToString())
                    {
                        ScriptableData scriptData = JsonUtility.FromJson<ScriptableData>(json);
                        ScriptInfo info = ScriptDatabase.GetInstance().GetScript(scriptData.ScriptId);
                        SkillBehaviour skill = ScriptableObject.CreateInstance(info.Script.GetClass()) as SkillBehaviour;
                        skill.Caster = otherPlayer;
                        skill.Initialize();
                        if (skill.OnCast != null)
                        {
                            Debug.Log("Cast");
                            skill.OnCast.RunContext();
                        }                  
                    }
                    else if (jsonData.DataType == typeof(ProjectileSpawnData).ToString())
                    {
                        ProjectileSpawnData projData = JsonUtility.FromJson<ProjectileSpawnData>(json);
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            spawner.SpawnProjectile(projData.PrefabResourcePath, projData);
                        });
                    }
                    else
                    {
                        SpawnData objSpawnData = JsonUtility.FromJson<SpawnData>(json);
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            spawner.SpawnObject(objSpawnData.PrefabResourcePath, objSpawnData);
                        });
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}