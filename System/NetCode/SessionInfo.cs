using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VovTech.Serialization
{
    [Serializable]
    public struct SessionInfo
    {
        public ObjectData[] SessionObjects;
        public string Time;
    }

    [Serializable]
    public class PacketData
    {
        public string DataType;
        public string Prefix;
    }

    #region Packets Data Types

    #region State data
    [Serializable]
    public class ObjectData : PacketData
    {
        public string ReferenceId;
        public string Name;
        public string[] Groups;
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Size;

        public ActorData ToActorData()
        {
            return this as ActorData;
        }

        public CharacterData ToCharacterData()
        {
            return this as CharacterData;
        }

        public ItemData ToItemData()
        {
            return this as ItemData;
        }
    }

    [Serializable]
    public class ActorData : ObjectData
    {
        public StatData[] ActorStats;
        public bool IsDead;
        public string CurrentAnimation;
        public int EquipedWeaponId;
    }

    [Serializable]
    public class CharacterData : ActorData
    {
        public string OwnerName;
        public bool IsController;
        public Vector3 LookingPos;
    }

    [Serializable]
    public class ItemData : ObjectData
    {
        public int Id;
        public ItemData[] Attachments;

        public WeaponData ToWeaponData()
        {
            return this as WeaponData;
        }
    }

    [Serializable]
    public class WeaponData : ItemData
    {
        public StatData[] WeaponStats;
    }
    #endregion

    #region Spawn Data
    [Serializable]
    public class SpawnData : PacketData
    {
        public string PrefabResourcePath;
        public Vector3 Position;
        public Vector3 Rotation;
    }

    [Serializable]
    public class ProjectileSpawnData : SpawnData
    {
        public Vector3 Force;
        public int DataId;
        public float Speed;
        public float Heatlh;
        public string DeathEffectPath;
    }

    #endregion

    [Serializable]
    public class ScriptableData : PacketData
    {
        public string ScriptId;
    }

    #region Other Data
    [Serializable]
    public class StatData
    {
        public string Name;
        public float Value;
    }
    #endregion

    #endregion
}