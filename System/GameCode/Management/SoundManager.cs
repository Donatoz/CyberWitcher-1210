using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace VovTech
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance => GameObject.Find("GameManagers").GetComponent<SoundManager>();
        public AudioSource MusicSource;
        public AudioSource RadioSource;
        [Range(0f, 1f)]
        public float MusicVolume;
        public List<AudioClip> BattleMusicPlayList;
        public bool KeepPlaylist = true;

        public void PlayClip(AudioClip musicClip, AudioSource source, float volume = 1f)
        {
            source.DOFade(0, 0.4f).OnComplete(() =>
            {
                source.clip = musicClip;
                source.DOFade(volume, 0.5f);
                source.Play();
            });
        }

        public void PlayPlaylist(List<AudioClip> playList)
        {
            AudioClip clip = MusicSource.clip;
            MusicSource.DOFade(0, 0.4f).OnComplete(() =>
            {
                MusicSource.clip = playList.RandomItem();
                while(MusicSource.clip == clip && BattleMusicPlayList.Count > 1)
                {
                    MusicSource.clip = playList.RandomItem();
                }
                MusicSource.DOFade(MusicVolume, 0.5f);
                MusicSource.Play();
                if(KeepPlaylist)
                    Entity.GetById("DummyEntity").DelayedInvoke(() => { Instance.PlayPlaylist(playList); }, MusicSource.clip.length - 1);
            });
        }

        public void PlayClipAtPoint(AudioClip clip, Vector3 position = default, float volume = 1)
        {
            position = (position == default) ? MusicSource.transform.position : position;
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }
    } 
}