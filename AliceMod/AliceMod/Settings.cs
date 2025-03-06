using System;
using UnityEngine;
using UnityModManagerNet;

namespace AliceMod
{
    [Serializable]
    public class Settings : UnityModManager.ModSettings
    {
        public Color BGColor = new Color(0.0f, 0.0f, 0.0f);

        public float RGB_Duration = 2.0f;

        public float Lights_Intensity;
        public float Lights_Area;

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }
    }
}