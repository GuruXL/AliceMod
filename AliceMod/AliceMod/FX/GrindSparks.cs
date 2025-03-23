using GameManagement;
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
                UpdateVFXState();
            }          
        }

        private void UpdateVFXState()
        {
            if (PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding)
            {
                Sparks_FrontTruck.Play();
            }
            else if (!PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding)
            {
                Sparks_FrontTruck.Stop();
            }
            if (PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding)
            {
                Sparks_BackTruck.Play();
            }
            else if (!PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding)
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

            BackTruckObj = Instantiate(AssetLoader.FX_SparksPrefab);
            BackTruckObj.transform.SetParent(Main.ScriptManager.transform);
            BackTruckObj.AddComponent<ParticleSystemTracker>();

            GetParticleSystems();

            yield return null;
        }

        private void GetParticleSystems()
        {
            Sparks_FrontTruck = FrontTruckObj?.GetComponent<ParticleSystem>();
            Sparks_BackTruck = BackTruckObj?.GetComponent<ParticleSystem>();
        }

        private void UpdateFXPosition()
        {
            if (FrontTruckObj != null && PlayerController.Instance.boardController.triggerManager.frontTruckCollision.isColliding)
            {
                FrontTruckObj.transform.position = PlayerController.Instance.boardController.triggerManager.frontTruckCollision.lastCollision;
                //FrontTruckObj.transform.position = PlayerController.Instance.boardController.frontTruckRigidbody.position;
                //FrontTruckObj.transform.position = CollisionUtil.FrontTruckCollision();
                FrontTruckObj.transform.forward = PlayerController.Instance.GetGrindDirection();
            }
            if (BackTruckObj != null && PlayerController.Instance.boardController.triggerManager.backTruckCollision.isColliding)
            {
                BackTruckObj.transform.position = PlayerController.Instance.boardController.triggerManager.backTruckCollision.lastCollision;
                //BackTruckObj.transform.position = PlayerController.Instance.boardController.backTruckRigidbody.position;
                //BackTruckObj.transform.position = CollisionUtil.BackTruckCollision();
                BackTruckObj.transform.forward = PlayerController.Instance.GetGrindDirection();
            }
        }
    }
}
