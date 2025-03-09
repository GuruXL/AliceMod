using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering.HighDefinition;

namespace AliceMod
{
    public class LightController : MonoBehaviour, IColorSetter
    {
        private Coroutine RGBRoutine;
        public bool lightsFound { get; private set; } = false;
        //public bool directLightsFound { get; private set; } = false;

        public List<Light> lights = new List<Light>();
        public List<HDAdditionalLightData> hdLightdata = new List<HDAdditionalLightData>();
        public List<Color> default_LightColors = new List<Color>();
        public List<float> default_Range = new List<float>();

        public List<Light> directLights = new List<Light>();

        private float lastBrightness = 1.0f;
        private float lastVolIntensity = 1.0f;
        private float lastRange = 8.0f;

        public void Start()
        {
            SetUpInterfaces();
            GetLights();
        }
        private void SetUpInterfaces()
        {
            ColorLoop.colorSetter = this;
        }
        public void Update()
        {
            if (!lightsFound)
                return;

            UpdateBrightness();
            UpdateVolIntensity();
            UpdateRange();
        }

        public void SetColor(Color color)
        {
            SetLightColor(color, lights);
            //SetLightColor(color, directLights);
            //SetDirectLightColor(color);
        }

        private void SetLightColor(Color color, List<Light> lightList)
        {
            foreach (Light light in lightList)
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
        private bool CheckLightsFound(List<Light> lights)
        {
            if (lights != null && lights.Count >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public void GetLights()
        {
            lightsFound = false;
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

            lightsFound = CheckLightsFound(lights);

            GetHDLightData();
            GetDefaultLightValues();

            Main.Logger.Log($"Lights Found: {lightsFound} " + $"Lights on Map: {lights.Count}");
            //Main.Logger.Log($"DirectLights Found: {directLightsFound}" + $"DirectLights on Map: {directLights.Count}");
        }
        private void GetHDLightData()
        {
            if (!lightsFound)
                return;

            hdLightdata.Clear();

            foreach (Light light in lights)
            {
                hdLightdata.Add(light.gameObject.GetComponent<HDAdditionalLightData>());
            }

            Main.Logger.Log($"hdLightdata:{hdLightdata.Count}");
        }
        private void GetDefaultLightValues()
        {
            if (!lightsFound)
                return;

            default_LightColors.Clear();
            default_Range.Clear();

            foreach (Light Light in lights)
            {
                default_LightColors.Add(Light.color);
                default_Range.Add(Light.range);
            }

            Main.Logger.Log($"default_LightColors:{default_LightColors.Count}");
        }
        public void ResetDefaultLightValues()
        {
            if (!lightsFound)
                return;

            Main.settings.Lights_Brightness = 1.0f;
            Main.settings.Lights_Volumetric = 1.0f;
            Main.settings.Lights_Range = 8.0f;

            if (default_LightColors.Count > 0)
            {
                for (int i = 0; i < lights.Count; i++)
                {
                    lights[i].color = default_LightColors[i];
                }
            }
            if (hdLightdata.Count > 0)
            {
                for (int i = 0; i < hdLightdata.Count; i++)
                {
                    hdLightdata[i].range = default_Range[i];
                    hdLightdata[i].lightDimmer = 1.0f;
                    hdLightdata[i].volumetricDimmer = 1.0f;
                }
            }
        }

        private void UpdateBrightness()
        {
            if (hdLightdata.Count <= 0)
                return;

            if (lastBrightness != Main.settings.Lights_Brightness)
            {
                foreach (HDAdditionalLightData lightdata in hdLightdata)
                {
                    lightdata.lightDimmer = Main.settings.Lights_Brightness;
                }
                lastBrightness = Main.settings.Lights_Brightness;
            }
        }
        private void UpdateVolIntensity()
        {
            if (hdLightdata.Count <= 0)
                return;

            if (lastVolIntensity != Main.settings.Lights_Volumetric)
            {
                foreach (HDAdditionalLightData lightdata in hdLightdata)
                {
                    lightdata.volumetricDimmer = Main.settings.Lights_Volumetric;
                }
                lastVolIntensity = Main.settings.Lights_Volumetric;
            }
        }
        private void UpdateRange()
        {
            if (hdLightdata.Count <= 0)
                return;

            if (lastRange != Main.settings.Lights_Range)
            {
                foreach (HDAdditionalLightData lightdata in hdLightdata)
                {
                    lightdata.range = Main.settings.Lights_Range;
                }
                lastRange = Main.settings.Lights_Range;
            }
        }
    }
}
