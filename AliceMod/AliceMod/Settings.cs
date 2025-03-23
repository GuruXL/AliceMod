using System;
using UnityEngine;
using UnityModManagerNet;

namespace AliceMod
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings
    {
        public float Lights_Color_R = 1.0f;
        public float Lights_Color_G = 1.0f;
        public float Lights_Color_B = 1.0f;

        public float Direct_Color_R = 1.0f;
        public float Direct_Color_G = 1.0f;
        public float Direct_Color_B = 1.0f;

        public bool RGB_Sun_Active = false;

        public float BG_RGB_Duration = 3.0f;
        public float RGB_Duration = 2.0f;

        public float Lights_IntensityMultiplyer = 1.0f;
        public float Lights_Dimmer = 1.0f;
        public float Lights_Volumetric = 1.0f;
        public float Lights_Range = 8.0f;

        public float Direct_IntensityMultiplyer = 1.0f;
        public float Direct_Dimmer = 1.0f;

        public float Vol_Weight = 1.0f;
        public float Vol_Prio = 10.0f;

        public float Sky_SkyExposure = 8.0f;
        public float Sky_Rotation = 0.0f;
        public float Sky_Exposure = 11.5f;
        public float Sky_IndirectDiffuse = 1.0f;
        public float Sky_IndirectSpecular = 1.0f;
        public float Sky_Temp = 0.0f;
        public float Sky_Tint = 0.0f;
        public string Sky_State = "";
        public bool Sky_CustomSky_Enabled = false;

        public float Mesh_DestroyDelay = 2.0f;
        public float Mesh_RefreshRate = 0.2f;
        public float Shader_Float_FadeDuration = 1.0f;
        public float Shader_ColorIntensity = 0.55f;
        public float Velocity_Threshold = 1.0f;
        public float Shader_BaseColor_R = 0.0f;
        public float Shader_BaseColor_G = 0.80f;
        public float Shader_BaseColor_B = 1.0f;
        public float Shader_DissolveColor_R = 0.85f;
        public float Shader_DissolveColor_G = 0.20f;
        public float Shader_DissolveColor_B = 0.90f;

        public bool FX_Sparks_Enabled = false;

        public void ResetLightSettings()
        {
            Lights_Color_R = 1.0f;
            Lights_Color_G = 1.0f;
            Lights_Color_B = 1.0f;

            Direct_Color_R = 1.0f;
            Direct_Color_G = 1.0f;
            Direct_Color_B = 1.0f;

            Lights_IntensityMultiplyer = 1.0f;
            Lights_Dimmer = 1.0f;
            Lights_Volumetric = 1.0f;
            Lights_Range = 8.0f;

            Direct_IntensityMultiplyer = 1.0f;
            Direct_Dimmer = 1.0f;
        }
        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}