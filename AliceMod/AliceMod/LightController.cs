using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace AliceMod
{
    public interface IColorSetter
    {
        void SetColor(Color color);
    }

    public class LightController : MonoBehaviour, IColorSetter
    {

        //public Light[] lights;
        public List<Light> lights = new List<Light>();
        public List<Light> directLights = new List<Light>();

        public List<Color> Default_ColorList = new List<Color>();
        public List<float> Default_IntensityList = new List<float>();

        private float cachedIntensity;
        private float cachedArea;

        private Coroutine RGBRoutine;

        public void Start()
        {
            ColorLoop.colorSetter = this;
        }

        public void Update()
        {
            /*
            if (lights != null)
            {
                if (cachedIntensity != Main.settings.Lights_Intensity)
                {
                    UpdateSettings();
                    cachedIntensity = Main.settings.Lights_Intensity;
                }

                if (cachedArea != Main.settings.Lights_Area)
                {
                    UpdateSettings();
                    cachedArea = Main.settings.Lights_Area;
                }
            }
            */
        }

        public void SetColor(Color color)
        {
            SetLightColor(color);
            //SetDirectLightColor(color);
        }

        private void SetLightColor(Color color)
        {
            foreach (Light light in lights)
            {
                light.color = color;
            }
        }
        private void SetDirectLightColor(Color color)
        {
            foreach (Light light in directLights)
            {
                light.color = color;
            }
        }
        public void StartRGBRoutine()
        {
            if (RGBRoutine == null)
            {
                if (ColorLoop.randomColors)
                {
                    RGBRoutine = StartCoroutine(ColorLoop.RGBRandomColorLoop());
                }
                else
                {
                    RGBRoutine = StartCoroutine(ColorLoop.RGBColorLoop());
                }
                Main.Logger.Log($"RGB Color Loop Started - random colors: {ColorLoop.randomColors}");
            }
        }
        public void StopRGBRoutine()
        {
            if (RGBRoutine != null)
            {
                StopCoroutine(RGBRoutine);
                RGBRoutine = null; // Set to null so it can be restarted again
                Main.Logger.Log("RGB Color Loop Stopped");
            }
        }
       
        public void GetLights()
        {
            lights.Clear();
            directLights.Clear();

            Light[] lightsList = FindObjectsOfType(typeof(Light)) as Light[];

            foreach(Light light in lightsList)
            {
                if (light.type == LightType.Directional)
                {
                    directLights.Add(light);
                }
                else
                {
                    lights.Add(light);
                }
            }

            GetDefaultLightValues();

            Main.Logger.Log("Lights on Map: " + lights.Count);
        }

        public void GetDefaultLightValues()
        {
            if (lights == null || lights.Count <= 0)
                return;

            Default_ColorList.Clear();
            Default_IntensityList.Clear();

            foreach (Light LightObj in lights)
            {
                Default_ColorList.Add(LightObj.color);
                Default_IntensityList.Add(LightObj.intensity);
            }

            Main.Logger.Log($"Default_ColorList:{Default_ColorList.Count}" + $"Default_IntensityList:{Default_IntensityList.Count}");
        }
        public void ResetDefaultLightValues()
        {
            if (lights == null || lights.Count <= 0 || Default_ColorList.Count <= 0 || Default_IntensityList.Count <= 0)
                return;

            for (int i = 0; i < lights.Count; i++)
            {
                lights[i].color = Default_ColorList[i];
                lights[i].intensity = Default_IntensityList[i];
            }
        }
        void UpdateSettings()
        {
            if (lights == null || lights.Count <= 0)
                return;

            foreach (Light LightObj in lights)
            {
                if (LightObj.type != LightType.Directional)
                {
                    LightObj.intensity = Main.settings.Lights_Intensity * 10;
                    LightObj.spotAngle = Main.settings.Lights_Area;
                }
            }
        }
    }
}
