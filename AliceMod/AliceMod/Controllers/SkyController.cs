using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace AliceMod
{
    public class SkyController : MonoBehaviour
    {
        public GameObject skyVolumeObj;
        private Volume skyVolume;
        private VolumeProfile skyProfile;
        private VisualEnvironment visualEnvironment;
        public HDRISky hdriSky;
        private Exposure exposure;
        private IndirectLightingController indirectLight;
        private WhiteBalance whiteBalance;

        private float last_Weight = 1.0f;
        private float last_prio = 10.0f;
        private float last_SkyExposure = 8.0f;
        private float last_Rotation = 0.0f;
        private float last_Exposure = 11.5f;
        private float last_IndirectDiffuse = 1.0f;
        private float last_IndirectSpecular = 1.0f;
        private float last_Temp = 0.0f;
        private float last_Tint = 0.0f;

        private const string none = "";
        private const string shanghai_sky = "shanghai";

        public string[] SkyStates = new string[] {
            shanghai_sky,
            none,
        };

        private void Start()
        {
            CreateSkyVolume();
        }

        private void Update()
        {
            UpdateSettings();
            UpdateSkyState();
        }

        #region Create SkyVolume
        private void CreateSkyVolume()
        {
            skyVolumeObj = new GameObject("SkyVolume");
            skyVolumeObj.transform.SetParent(Main.ScriptManager.transform);
            skyVolumeObj.SetActive(false);

            if (skyVolumeObj == null)
                return;

            AddVolume();

            if (skyVolume == null)
                return;

            AddVolumeProfile();

            if (skyProfile == null)
                return;

            AddVisualEnvironment();
            AddHDRISky();
            AddExposure();
            AddIndirectLight();
            AddWhiteBalance();
        }
        private void AddVolume()
        {
            skyVolume = skyVolumeObj.AddComponent<Volume>();
        }
        private void AddVolumeProfile()
        {
            skyProfile = new VolumeProfile();
            skyVolume.profile = skyProfile;
        }
        private void AddVisualEnvironment()
        {
            visualEnvironment = skyProfile.Add<VisualEnvironment>();
            visualEnvironment.skyType.Override((int)SkyType.HDRI);
            visualEnvironment.skyAmbientMode.Override(SkyAmbientMode.Dynamic);
        }
        private void AddHDRISky()
        {
            hdriSky = skyProfile.Add<HDRISky>();
            //hdriSky.hdriSky.Override(AssetLoader.Sky_shanghai); // set this to Default CubeMap
            hdriSky.exposure.Override(Main.settings.Sky_SkyExposure);
            hdriSky.rotation.Override(Main.settings.Sky_Rotation);
            StartCoroutine(ApplyDefaultCubeMap());
        }
        private IEnumerator ApplyDefaultCubeMap()
        {
            yield return new WaitUntil(() => AssetLoader.assetsLoaded);
            hdriSky.hdriSky.Override(AssetLoader.Sky_shanghai); // set this to Default CubeMap
            yield return null;
        }
        private void AddExposure()
        {
            exposure = skyProfile.Add<Exposure>();
            exposure.mode.overrideState = true;
            exposure.mode.Override(ExposureMode.Fixed);
            exposure.fixedExposure.overrideState = true;
            exposure.fixedExposure.Override(Main.settings.Sky_Exposure);
        }
        private void AddIndirectLight()
        {
            indirectLight = skyProfile.Add<IndirectLightingController>();
            indirectLight.SetAllOverridesTo(true);
        }
        private void AddWhiteBalance()
        {
            whiteBalance = skyProfile.Add<WhiteBalance>();
            whiteBalance.SetAllOverridesTo(true);
            whiteBalance.temperature.Override(Main.settings.Sky_Temp);
            whiteBalance.temperature.Override(Main.settings.Sky_Tint);
        }
        #endregion

        private void UpdateSettings()
        {
            if (skyVolumeObj == null)
                return;

            UpdateVolume();
            UpdateHDRI();
            UpdateExposure();
            UpdateIndirectLight();
            UpdateWhiteBalance();
        }
        private void UpdateSkyState()
        {
            if(hdriSky == null)
                return;

            switch (Main.settings.SkyState)
            {
                case shanghai_sky:
                    hdriSky.hdriSky.Override(AssetLoader.Sky_shanghai);
                    break;
                case none:
                    break;
                default:
                    Main.Logger.Log("failed to update skybox");
                    break;
            }
        }

        #region Sky Updates
        private void UpdateVolume()
        {
            if (skyVolume == null)
                return;

            if (last_Weight != Main.settings.Vol_Weight)
            {
                skyVolume.weight = Main.settings.Vol_Weight;
                last_Weight = Main.settings.Vol_Weight;
            }
            if (last_prio != Main.settings.Vol_Prio)
            {
                skyVolume.priority = Main.settings.Vol_Prio;
                last_prio = Main.settings.Vol_Prio;
            }
        }
        private void UpdateHDRI()
        {
            if (hdriSky == null)
                return;

            if (last_SkyExposure != Main.settings.Sky_SkyExposure)
            {
                hdriSky.exposure.Override(Main.settings.Sky_SkyExposure);
                last_SkyExposure = Main.settings.Sky_SkyExposure;
            }
            if (last_Rotation != Main.settings.Sky_Rotation)
            {
                hdriSky.rotation.Override(Main.settings.Sky_Rotation);
                last_Rotation = Main.settings.Sky_Rotation;
            }
        }
        private void UpdateExposure()
        {
            if (exposure == null)
                return;

            if (last_Exposure != Main.settings.Sky_Exposure)
            {
                exposure.fixedExposure.Override(Main.settings.Sky_Exposure);
                last_Exposure = Main.settings.Sky_Exposure;
            }
        }
        private void UpdateIndirectLight()
        {
            if (indirectLight == null)
                return;

            if (last_IndirectDiffuse != Main.settings.Sky_IndirectDiffuse)
            {
                indirectLight.indirectDiffuseIntensity.Override(Main.settings.Sky_IndirectDiffuse);
                last_IndirectDiffuse = Main.settings.Sky_IndirectDiffuse;
            }
            if (last_IndirectSpecular != Main.settings.Sky_IndirectSpecular)
            {
                indirectLight.indirectSpecularIntensity.Override(Main.settings.Sky_IndirectSpecular);
                last_IndirectSpecular = Main.settings.Sky_IndirectSpecular;
            }
        }
        private void UpdateWhiteBalance()
        {
            if (whiteBalance == null)
                return;

            if (last_Temp != Main.settings.Sky_Temp)
            {
                whiteBalance.temperature.Override(Main.settings.Sky_Temp);
                last_Temp = Main.settings.Sky_Temp;
            }
            if (last_Tint != Main.settings.Sky_Tint)
            {
                whiteBalance.tint.Override(Main.settings.Sky_Tint);
                last_Tint = Main.settings.Sky_Tint;
            }
        }
        #endregion
    }
}
