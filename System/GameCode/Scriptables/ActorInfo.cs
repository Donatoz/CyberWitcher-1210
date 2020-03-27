using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName ="New Actor", menuName ="VovTech/Actor", order =53)]
public class ActorInfo : ScriptableObject
{
    [System.Serializable]
    public struct StatInfo
    {
        public string Name;
        public float BaseValue;
        public float MinValue;
        public float MaxValue;
    }

    public string Name;
    public int Id;
    public List<StatInfo> Stats;
    public List<AudioClip> HitSounds;
    [Range(0f, 1f)]
    public float HitSoundVolume;
    public List<AudioClip> DeathSounds;
    [Range(0f, 1f)]
    public float DeathSoundVolume;
}
