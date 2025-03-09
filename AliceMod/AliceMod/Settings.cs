using System;
using UnityEngine;
using UnityModManagerNet;

namespace AliceMod
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings
    {
        //public Color BGColor = new Color(0.0f, 0.0f, 0.0f);

        public float BG_RGB_Duration = 3.0f;
        public float RGB_Duration = 2.0f;

        public float Lights_Brightness = 1.0f;
        public float Lights_Volumetric = 1.0f;
        public float Lights_Range = 8.0f;
        //public float Lights_Area;

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

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}