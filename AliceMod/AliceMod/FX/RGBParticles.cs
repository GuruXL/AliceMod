using GameManagement;
using ReplayEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;
using MapEditor;
using Rewired;

namespace AliceMod
{
    public class RGBParticles : MonoBehaviour
    {
        private GameObject RGBTrailObj;
        private VisualEffect RGBTrail;
        //private VisualEffectAsset RGBTrailAsset;
        public bool isVFXPlaying { get; private set; } = false;

        private void Start()
        {
            CreateVFX();
        }

        private void Update()
        {
            UpdateVFXPosition();
        }
        private void FixedUpdate()
        {
            UpdateVFXState();
        }
        private void UpdateVFXPosition()
        {
            if (RGBTrailObj == null)
                return;

            if (Main.meshTrail.MeshTrail_Enabled)
            {
                RGBTrailObj.transform.position = PositionUtil.PlayerTransform().position;
            }
        }
        private void UpdateVFXState()
        {
            if (RGBTrail == null)
                return;

            if (Main.meshTrail.MeshTrail_Enabled)
            {
                if (PositionUtil.IsPlayerMoving())
                {
                    if (!isVFXPlaying)
                    {
                        PlayVFX();
                    }
                }
                else
                {
                    if (isVFXPlaying)
                    {
                        StopVFX();
                    }
                }
            }
        }
        public void CreateVFX()
        {
            RGBTrailObj = new GameObject("RGBTrail");
            RGBTrailObj.transform.SetParent(Main.ScriptManager.transform);
            RGBTrailObj.SetActive(false);

            if (RGBTrailObj == null) return;

            RGBTrail = RGBTrailObj.AddComponent<VisualEffect>();
            RGBTrail.initialEventName = "OnStop";

            if (RGBTrail == null) return;

            StartCoroutine(ApplyAsset());
        }
        private IEnumerator ApplyAsset()
        {
            yield return new WaitUntil(() => AssetLoader.assetsLoaded);
            RGBTrail.visualEffectAsset = AssetLoader.RGB_Trail_asset;
            yield return null;
        }

        public void ToggleVFXObj(bool enabled)
        {
            if (RGBTrailObj == null) return;

            RGBTrailObj.SetActive(enabled);

            if (!enabled)
            {
                StopVFX();
            }
        }
        public void PlayVFX()
        {
            RGBTrail.Play();
            isVFXPlaying = true;
        }
        public void StopVFX()
        {
            RGBTrail.Stop();
            isVFXPlaying = false;
        }
    }
}
