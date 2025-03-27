using GameManagement;
using ReplayEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AliceMod
{
    public class GrindSparks : MonoBehaviour
    {
        GameObject FrontTruckObj;
        GameObject BackTruckObj;
        ParticleSystem Sparks_FrontTruck;
        ParticleSystem Sparks_BackTruck;

        List<ParticleSystem> PS_List = new List<ParticleSystem>();
        List<ParticleSystem.MainModule> PS_MainModules = new List<ParticleSystem.MainModule>();

        private float last_Timescale = 1.0f;

        private void Start()
        {
            StartCoroutine(GetPrefab());

            //PlayerController.Instance.boardController.triggerManager.grindContactPoint // maybe position to spawn sparks?
            //PlayerController.Instance.boardController.triggerManager.grindContactSplinePosition // maybe position to spawn sparks?
            //PlayerController.Instance.GetGrindContactPosition();
            //PlayerController.Instance.GetGrindDirection();
            //PlayerController.Instance.IsCurrentGrindMetal();
            //PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding
            //PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding
        }

        private void Update()
        {
            if (!Main.settings.FX_Sparks_Enabled) return;

            if (GameStateMachine.Instance.CurrentState is PlayState)
            {
                UpdateFXPosition();
                UpdateFXState();

                if (Main.settings.FX_Sparks_SyncReplaySpeed)
                {
                    UpdateFXPlaybackSpeed(1.0f);
                }
                
            }
            else if (GameStateMachine.Instance.CurrentState is ReplayState)
            {
                if (Main.settings.FX_Sparks_SyncReplaySpeed)
                {
                    UpdateFXPlaybackSpeed(ReplayEditorController.Instance.playbackTimeScale);
                }
            }
        }
        private void UpdateFXPlaybackSpeed(float speed)
        {
            if (PS_MainModules == null || PS_MainModules.Count <= 0 
                && Mathf.Approximately(last_Timescale, speed))
                return;

            for (int i = 0; i < PS_MainModules.Count; i++)
            {
                ParticleSystem.MainModule main = PS_MainModules[i];

                if (!Mathf.Approximately(main.simulationSpeed, speed))
                {
                    main.simulationSpeed = Mathf.Abs(speed);
                }
            }
            last_Timescale = speed;
        }
        private void UpdateFXState()
        {
            if (Sparks_FrontTruck == null || Sparks_BackTruck == null)
                return;

            if (PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding && !Sparks_FrontTruck.isEmitting)
            {
                Sparks_FrontTruck.Play();
            }
            else if (!PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding && Sparks_FrontTruck.isEmitting)
            {
                Sparks_FrontTruck.Stop();
            }
            if (PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding && !Sparks_BackTruck.isEmitting)
            {
                Sparks_BackTruck.Play();
            }
            else if (!PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding && Sparks_BackTruck.isEmitting)
            {
                Sparks_BackTruck.Stop();
            }

        }
        private IEnumerator GetPrefab()
        {
            yield return new WaitUntil(() => AssetLoader.assetsLoaded);

            FrontTruckObj = Instantiate(AssetLoader.FX_SparksPrefab);
            FrontTruckObj.transform.SetParent(Main.ScriptManager.transform);
            FrontTruckObj.AddComponent<ParticleSystemTracker>();
            FrontTruckObj.SetActive(Main.settings.FX_Sparks_Enabled);

            BackTruckObj = Instantiate(AssetLoader.FX_SparksPrefab);
            BackTruckObj.transform.SetParent(Main.ScriptManager.transform);
            BackTruckObj.AddComponent<ParticleSystemTracker>();
            BackTruckObj.SetActive(Main.settings.FX_Sparks_Enabled);

            GetParticleSystems();

            yield return null;
        }

        private void GetParticleSystems()
        {
            Sparks_FrontTruck = FrontTruckObj?.GetComponent<ParticleSystem>();
            Sparks_BackTruck = BackTruckObj?.GetComponent<ParticleSystem>();
            PopulatePSList();
            GetPSMainModules();
        }
        private void PopulatePSList()
        {
            PS_List.Clear();
            //PS_List.Add(Sparks_FrontTruck);
            foreach (ParticleSystem ps in FrontTruckObj.GetComponentsInChildren<ParticleSystem>())
            {
                PS_List.Add(ps);
            }
            //PS_List.Add(Sparks_BackTruck);
            foreach (ParticleSystem ps in BackTruckObj.GetComponentsInChildren<ParticleSystem>())
            {
                PS_List.Add(ps);
            }
        }
        private void GetPSMainModules()
        {
            if (PS_List.Count > 0)

            foreach (ParticleSystem ps in PS_List)
            {
                PS_MainModules.Add(ps.main);
            }
        }
        private void UpdateFXPosition()
        {
            if (FrontTruckObj != null && PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding)
            {
                //FrontTruckObj.transform.position = PlayerController.Instance.boardController.triggerManager.grindContactPoint;
                //FrontTruckObj.transform.position = PlayerController.Instance.GetGrindContactPosition();
                //FrontTruckObj.transform.position = PlayerController.Instance.boardController.triggerManager.frontTruckCollision.lastCollision;
                //FrontTruckObj.transform.position = PlayerController.Instance.characterCustomizer.TruckBaseParents[1].transform.position;
                //FrontTruckObj.transform.position = PlayerController.Instance.boardController.triggerManager.grindContactSplinePosition.position;

                Vector3 frontTruckPos = PlayerController.Instance.boardController.frontTruckRigidbody.position + new Vector3 (0, -0.03f, 0);
                FrontTruckObj.transform.position = frontTruckPos;

                FrontTruckObj.transform.forward = PlayerController.Instance.GetGrindDirection();
            }
            if (BackTruckObj != null && PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding)
            {
                //BackTruckObj.transform.position = PlayerController.Instance.boardController.triggerManager.backTruckCollision.lastCollision;
                //BackTruckObj..transform.position = PlayerController.Instance.boardController.triggerManager.grindContactPoint;
                //BackTruckObj.transform.position = PlayerController.Instance.GetGrindContactPosition();
                //BackTruckObj.transform.position = PlayerController.Instance.characterCustomizer.TruckBaseParents[0].transform.position;
                //BackTruckObj.transform.position = PlayerController.Instance.boardController.triggerManager.grindContactSplinePosition.position;

                Vector3 backTruckPos = PlayerController.Instance.boardController.backTruckRigidbody.position + new Vector3(0, -0.03f, 0);
                BackTruckObj.transform.position = backTruckPos;

                BackTruckObj.transform.forward = PlayerController.Instance.GetGrindDirection();
            }
        }

        public void ToggleSparkObjects(bool enabled)
        {
            FrontTruckObj?.SetActive(enabled);
            BackTruckObj?.SetActive(enabled);
        }
    }
}
