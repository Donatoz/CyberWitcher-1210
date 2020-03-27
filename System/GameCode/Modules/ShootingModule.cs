using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VovTech.Serialization;

namespace VovTech
{
    public class ShootingModule : Module
    {
        public Transform ProjectileSpawn;
        public ProjectileInfo LoadedProjectile;
        public GameObject ShotEffect;
        public WeaponInfo Settings;

        private float baseSpreading;
        private float currentSpreading;
        private float currentShotInterval = 0f;
        private int currentClipSize = 0;
        

        public void Shoot(Vector3 target = default)
        {
            if (currentShotInterval > 0) return;
            AttachedEntity.AsItem().Owner.ReadyTimer += Settings.ShotInterval + 0.2f;
            Quaternion startRotation = Quaternion.LookRotation(InputManager.Instance.MouseWorldPosition - ProjectileSpawn.position);
            if (target != default)
            {
                startRotation = Quaternion.LookRotation(target - ProjectileSpawn.position);
            }
            Projectile proj = Instantiate(LoadedProjectile.Prefab, ProjectileSpawn.position,
                startRotation).GetComponent<Projectile>();
            proj.Direction = proj.transform.forward;
            proj.Creator = AttachedEntity;
            proj.Settings = LoadedProjectile;
            proj.Initialize();

            float spreadRandomX = Random.Range(-currentSpreading, currentSpreading);
            float spreadRandomY = Random.Range(-currentSpreading, currentSpreading);
            float spreadRandomZ = Random.Range(-currentSpreading, currentSpreading);
            proj.Direction += new Vector3(spreadRandomX, spreadRandomY, spreadRandomZ);
            proj.GetComponent<Rigidbody>().AddForce(proj.Direction * proj.ProjectileStats["Speed"].EffectiveValue);
            EffectsManager.Instance.SpawnEffect(ShotEffect, ProjectileSpawn.position, Quaternion.identity);
            if(Settings.ShootSounds.Count > 0)
            {
                SoundManager.Instance.PlayClipAtPoint(Settings.ShootSounds.RandomItem(), transform.position, Settings.ShootVolume);
            }

            if (NetManager.Instance.Connect)
            {
                //Sending packet to the server
                using (Packet packet = new Packet())
                {
                    ProjectileSpawnData data = new ProjectileSpawnData();
                    data.DataType = typeof(ProjectileSpawnData).ToString();
                    data.Prefix = "PREFIX: PROJECTILE";
                    data.Force = proj.Direction * proj.ProjectileStats["Speed"].EffectiveValue;
                    data.DataId = LoadedProjectile.Id;
                    data.PrefabResourcePath = LoadedProjectile.Prefab.name;
                    data.Position = ProjectileSpawn.position;
                    data.Rotation = startRotation.eulerAngles;
                    data.DeathEffectPath = LoadedProjectile.DeathEffect.name;
                    data.Speed = LoadedProjectile.Speed;
                    data.Heatlh = LoadedProjectile.Heatlh;
                    packet.Write(JsonUtility.ToJson(data, true));
                    NetManager.Instance.SendData(packet);
                }
                //-------------------
            }

            ((Weapon)AttachedEntity).OnShoot?.Invoke();
            currentShotInterval = Settings.ShotInterval;
            // Recoil grow (spreading grows)
            currentSpreading = 
                Mathf.Clamp(currentSpreading + Settings.Recoil, Settings.Spreading, Settings.Spreading + Settings.MaxRecoil);
            currentClipSize--;
            if(currentClipSize == 0)
            {
                currentShotInterval += Settings.ReloadTime - Settings.ShotInterval;
                currentClipSize = Settings.ClipSize;
                if (Settings.ReloadSounds.Count > 0)
                {
                    StartCoroutine(PlayReloadSounds());
                }
            }
        }

        private IEnumerator PlayReloadSounds()
        {
            for (int i = 0; i < Settings.ReloadSounds.Count; i++)
            {
                SoundManager.Instance.PlayClipAtPoint(Settings.ReloadSounds[i], transform.position, Settings.ShootVolume);
                yield return new WaitForSeconds(Settings.ReloadSounds[i].length);
            }
        }

        public void Initialize(WeaponInfo info)
        {
            Settings = info;
            currentClipSize = Settings.ClipSize;
            baseSpreading = info.Spreading;
            currentSpreading = Settings.Spreading;
        }

        private void FixedUpdate()
        {
            currentShotInterval = Mathf.Clamp(currentShotInterval - Time.fixedDeltaTime, 0, int.MaxValue);
            //Stabilization (spreading decreases)
            currentSpreading = Mathf.Clamp(currentSpreading - Settings.Stabilization, baseSpreading, int.MaxValue);
        }
    }
}