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

        public string SkyState = "";
        public bool isCustomSkyactive = false;

        public bool MeshTrail_Enabled = false;
        public bool MeshTrail_RandomColours = false;

        public float Mesh_ActiveTime = 1.5f;
        public float Mesh_DestroyDelay = 2.0f;
        public float Mesh_RefreshRate = 0.1f;

        public float Shader_Float_Rate = 1.0f;
        public float Shader_Float_RefreshRate = 0.75f;
        public float Shader_ColorIntensity = 1.0f;

        public float Velocity_Threshold = 1.0f;

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