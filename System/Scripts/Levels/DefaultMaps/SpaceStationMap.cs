using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VovTech.Levels
{
    public class SpaceStationMap : Level
    {
        public override void Initialize()
        {
            // Level initialization
            #region PreInit
            InteractiveObject.GetGroup("CanteenMainDoorLight").ForEach(obj =>
            obj.ChangeLightsColor(ColorTemplates.Get.NormalRed, 0.4f).ChangeMaterialColor("_EmissionColor", ColorTemplates.Get.BrightRed, 0.4f));
            TriggerFunctions = new Dictionary<string, System.Action>();
            GameObject testSwarmling = Resources.Load<GameObject>("SpawnableActors/TestSwarmling");
            #endregion

            // Triggers initialization
            #region Canteen

            #region Canteen_Main_Door
            TriggerFunctions["OpenCanteenMainDoor"] = () => {
                (Entity.GetById("CanteenMainDoor") as Door).Open();
                InteractiveObject.GetGroup("CanteenMainDoorLight").ForEach(obj =>
                obj.ChangeLightsColor(ColorTemplates.Get.NormalCyan, 0.4f).ChangeMaterialColor("_EmissionColor", ColorTemplates.Get.BrightCyan, 0.1f));
                Entity.GetById("CanteenEliteSoldier")?.AsActor().AsNPC().OrderAssembler.Walk(new Vector3(-39.899f, 51.155f, -1.971f));
            };
            TriggerFunctions["CloseCanteenMainDoor"] = () => { (Entity.GetById("CanteenMainDoor") as Door).Close(); };
            #endregion

            #region Canteen_Access_Error
            TriggerFunctions["CanteenAccessError"] = () =>
            {
                InteractiveObject.GetGroup("CanteenAccessError").ForEach(obj =>
                obj.ChangeLightsColor(ColorTemplates.Get.NormalRed, 0f).ChangeMaterialColor("_EmissionColor", ColorTemplates.Get.BrightRed, 0f));
                MainManager.Instance.MainCameraController.Shake(0.5f, 1, 1.4f, 10);
                Entity.GetById("DummyEntity").DelayedInvoke(() =>
                {
                    InteractiveObject.GetGroup("CanteenAccessError").ForEach(obj =>
                    obj.ChangeLightsColor(new Color(0, 0, 0, 0), 0f).ChangeMaterialColor("_EmissionColor", Color.black, 0f));
                    SoundManager.Instance.PlayClipAtPoint(Resources.Load<AudioClip>("SFX/energy_off"), default, 0.05f);
                }, 8);
                SoundManager.Instance.PlayClipAtPoint(Resources.Load("SFX/amb_quake") as AudioClip, volume:0.08f);
                SoundManager.Instance.PlayPlaylist(SoundManager.Instance.BattleMusicPlayList);
                Entity.GetById("DummyEntity").StartCustomCoroutine(() =>
                {
                    SwarmNPC npc = MainManager.Instance.SpawnActor(testSwarmling, new Vector3(-39.95f, 51.397f, 14.312f)) as SwarmNPC;
                    npc.Agent.SetDestination(new Vector3(-40, 51, -2));
                }, 20, 0.35f, delay: 8);
                Entity.GetById("DummyEntity").DelayedInvoke(() =>
               {
                   SoundManager.Instance.PlayClip(Resources.Load<AudioClip>("SFX/Announcments/Radio/speech2"), SoundManager.Instance.RadioSource, 0.1f);
               }, 5);
            };
            TriggerFunctions["CanteenReinforcements"] = () =>
            {
                Entity.GetGroup("CanteenReinforcement").ForEach((x) => (x as Actor).AsNPC().OrderAssembler.Walk(new Vector3(-40, 51, -2)));
            };
            #endregion


            #endregion

            #region Corridor_2
            (Entity.GetById("Corridor2Door") as Door).Open();
            TriggerFunctions["Close2CorridorDoor"] = () => {
                (Entity.GetById("Corridor2Door") as Door).Close();
            };

            #endregion

            #region Control_corridor

            TriggerFunctions["ControlCorridorVacuum"] = () =>
            {
                GameObject vacuum = Entity.GetById("ControlCorridorVacuum").gameObject;
                vacuum.SetActive(true);
                vacuum.GetComponentsInChildren<ParticleSystem>().ForEach((x) => x.Play());
            };

            #endregion

            #region Angar 
            TriggerFunctions["AngarSwarm"] = () =>
            {
                Entity.GetById("DummyEntity").StartCustomCoroutine(() =>
                {
                    SwarmNPC npc = MainManager.Instance.SpawnActor(testSwarmling, new Vector3(-21.51f, 45.69f, -34.02f)) as SwarmNPC;
                    npc.Agent.SetDestination(new Vector3(-21.51f, 45.69f, -11.62f));
                }, 70, 0.2f);
                MainManager.Instance.MainCameraController.Shake(0.3f, 1, 1.4f, 4);
                SoundManager.Instance.PlayClipAtPoint(Resources.Load("SFX/amb_quake") as AudioClip, volume: 0.08f);
                InteractiveObject.GetGroup("AngarLight").ForEach(obj =>
                obj.ChangeLightsColor(ColorTemplates.Get.NormalRed, 0f).ChangeMaterialColor("_EmissionColor", ColorTemplates.Get.BrightRed, 0f));
            };
            #endregion

            #region Camera shakes
            TriggerFunctions["WeakShake"] = () =>
            {
                MainManager.Instance.MainCameraController.Shake(0.3f, 3);
            };
            TriggerFunctions["MediumShake"] = () =>
            {
                MainManager.Instance.MainCameraController.Shake(0.7f, 7);
            };
            TriggerFunctions["StrongShake"] = () =>
            {
                MainManager.Instance.MainCameraController.Shake(1.3f, 10);
            };
            #endregion

            // Game start
            #region Game_start
            SoundManager.Instance.PlayClip(Resources.Load<AudioClip>("SFX/Announcments/Radio/speech1"), SoundManager.Instance.RadioSource, 0.1f);
            #endregion
        }
    }
}