using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering.HighDefinition;

namespace AliceMod
{
    public class LightController : MonoBehaviour, IColorSetter
    {
        private Coroutine RGBRoutine;
        public IColorSetter ColorSetter;
        public bool lightsFound { get; private set; } = false;
        public bool directLightsFound { get; private set; } = false;

        public List<Light> lights = new List<Light>();
        private Dictionary<Light, HDAdditionalLightData> hdLightdata = new Dictionary<Light, HDAdditionalLightData>();
        private Dictionary<Light, Color> Default_Lights_Colors = new Dictionary<Light, Color>();
        private Dictionary<Light, float> Default_Lights_Intensity = new Dictionary<Light, float>();
        private Dictionary<Light, float> Default_Lights_Range = new Dictionary<Light, float>();

        public Color Lights_Color { get; private set; } = new Color(1.0f, 1.0f, 1.0f);

        private float Lights_lastMultiplier = 1.0f;
        private float Lights_lastDimmer = 1.0f;
        private float Lights_lastVolIntensity = 1.0f;
        private float Lights_lastRange = 8.0f;

        public List<Light> directLights = new List<Light>();
        private Dictionary<Light, HDAdditionalLightData> Direct_hdLightdata = new Dictionary<Light, HDAdditionalLightData>();
        private Dictionary<Light, Color> Default_Direct_Colors = new Dictionary<Light, Color>();
        private Dictionary<Light, float> Default_Direct_Intensity = new Dictionary<Light, float>();

        public Color Direct_Color { get; private set; } = new Color(1.0f, 1.0f, 1.0f);

        private float Direct_lastMultiplier = 1.0f;
        private float Direct_lastDimmer = 1.0f;

        public void Start()
        {
            ColorSetter = this;
        }
        public void Update()
        {
            if (!lightsFound)
                return;

            UpdateLights();

            if (!directLightsFound)
                return;

            UpdateDirectLights();
        }

        public void SetColor(Color color)
        {
            SetLightColor(color, lights);

            if (Main.settings.RGB_Sun_Active)
            {
                SetLightColor(color, directLights);
            }
        }

        private void SetLightColor(Color color, List<Light> lightList)
        {
            foreach (Light light in lightList)
            {
                light.color = color;
            }
        }
        public void StartRGBRoutine(IEnumerator routine)
        {
            if (RGBRoutine == null)
            {
                RGBRoutine = StartCoroutine(routine);
            }
        }
        public void StopRGBRoutine()
        {
            if (RGBRoutine != null)
            {
                StopCoroutine(RGBRoutine);
                RGBRoutine = null;
            }
        }
        /*
        public void StartRGBRoutine()
        {
            if (RGBRoutine == null)
            {
                if (ColorLoop.randomColors)
                {
                    RGBRoutine = StartCoroutine(ColorLoop.RGBRandomColorLoop(this, Main.settings.RGB_Duration));
                }
                else
                {
                    RGBRoutine = StartCoroutine(ColorLoop.RGBColorLoop(this, Main.settings.BG_RGB_Duration));
                }
                Main.Logger.Log($"RGB Color Loop Started - random colors: {ColorLoop.randomColors}");
            }
        }
        public void StopRGBRoutine()
        {
            if (RGBRoutine != null)
            {
                StopCoroutine(RGBRoutine);
                RGBRoutine = null;
                Main.Logger.Log("RGB Color Loop Stopped");
            }
        }
        */
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
            directLightsFound = false;
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
            directLightsFound = CheckLightsFound(directLights);

            GetHDLightData(lightsFound, lights, hdLightdata);
            GetHDLightData(directLightsFound, directLights, Direct_hdLightdata);
            GetDefaultLightValues();
            GetDefaultDirectValues();

            //Main.Logger.Log($"Lights Found: {lightsFound} " + $"Lights on Map: {lights.Count}");
            //Main.Logger.Log($"DirectLights Found: {directLightsFound}" + $"DirectLights on Map: {directLights.Count}");
        }
        private void GetHDLightData(bool lightsFound, List<Light> lights, Dictionary<Light, HDAdditionalLightData> hddata)
        {
            if (!lightsFound)
                return;

            hddata.Clear();

            foreach (Light light in lights)
            {
                if (!hddata.ContainsKey(light))
                {
                    hddata[light] = light.gameObject.GetComponent<HDAdditionalLightData>();
                }
            }
        }
        private void GetDefaultLightValues()
        {
            if (!lightsFound)
                return;

            Default_Lights_Colors.Clear();
            Default_Lights_Intensity.Clear();
            Default_Lights_Range.Clear();

            foreach (Light light in lights)
            {
                if (!Default_Lights_Intensity.ContainsKey(light))
                {
                    Default_Lights_Intensity[light] = light.intensity;
                }
                if (!Default_Lights_Colors.ContainsKey(light))
                {
                    Default_Lights_Colors[light] = light.color;
                }
                if (!Default_Lights_Range.ContainsKey(light))
                {
                    Default_Lights_Range[light] = light.range;
                }
            }
        }
        private void GetDefaultDirectValues()
        {
            if (!directLightsFound)
                return;

            Default_Direct_Colors.Clear();
            Default_Direct_Intensity.Clear();

            foreach (Light light in directLights)
            {
                if (!Default_Direct_Intensity.ContainsKey(light))
                {
                    Default_Direct_Intensity[light] = light.intensity;
                }
                if (!Default_Direct_Colors.ContainsKey(light))
                {
                    Default_Direct_Colors[light] = light.color;
                }
            }
        }
        public void ResetDefaultLightValues()
        {
            Main.settings.ResetLightSettings();

            if (lightsFound)
            {
                foreach (Light light in lights)
                {
                    if (Default_Lights_Intensity.TryGetValue(light, out float originalIntensity))
                    {
                        light.intensity = originalIntensity;
                    }
                    if (Default_Lights_Colors.TryGetValue(light, out Color originalcolor))
                    {
                        light.color = originalcolor;
                    }
                    if (Default_Lights_Range.TryGetValue(light, out float originalrange))
                    {
                        light.range = originalrange;
                    }
                    if (hdLightdata.TryGetValue(light, out HDAdditionalLightData hddata))
                    {
                        hddata.lightDimmer = 1.0f;
                        hddata.volumetricDimmer = 1.0f;
                    }
                }
            }
            if (directLightsFound)
            {
                foreach(Light light in directLights)
                {
                    if (Default_Direct_Intensity.TryGetValue(light, out float originalIntensity))
                    {
                        light.intensity = originalIntensity;
                    }
                    if (Default_Direct_Colors.TryGetValue(light, out Color originalcolor))
                    {
                        light.color = originalcolor;
                    }
                    if (Direct_hdLightdata.TryGetValue(light, out HDAdditionalLightData hddata))
                    {
                        hddata.lightDimmer = 1.0f;
                    }
                }
            }
        }
        private void UpdateLights()
        {
            UpdateLightsColor();
            UpdateIntensity();
            UpdateDimmer();
            UpdateVolumertic();
            UpdateRange();
        }
        private void UpdateDirectLights()
        {
            UpdateDirectLightsColor();
            UpdateDirectIntensity();
            UpdateDirectDimmer();
        }

        #region Normal Light Updates
        private void UpdateLightsColor()
        {
            if (!lightsFound) return;

            Color newColor = new Color(Main.settings.Lights_Color_R, Main.settings.Lights_Color_G, Main.settings.Lights_Color_B);

            if (!Mathf.Approximately(Lights_Color.r, newColor.r) ||
                !Mathf.Approximately(Lights_Color.g, newColor.g) ||
                !Mathf.Approximately(Lights_Color.b, newColor.b))
            {
                SetLightColor(newColor, lights);
                Lights_Color = newColor;
            }
        }
        private void UpdateIntensity()
        {
            if (lights.Count <= 0)
                return;

            float Multiplier = Mathf.Max(1, Main.settings.Lights_IntensityMultiplyer);

            if (Mathf.Approximately(Lights_lastMultiplier, Multiplier))
                return;

            foreach (Light light in lights)
            {
                if (Default_Lights_Intensity.TryGetValue(light, out float originalIntensity))
                {
                    light.intensity = originalIntensity * Multiplier;
                }
            }
            Lights_lastMultiplier = Multiplier;
        }
        private void UpdateRange()
        {
            if (hdLightdata.Count <= 0)
                return;

            if (Mathf.Approximately(Lights_lastRange, Main.settings.Lights_Range))
                return;

            foreach (Light light in lights)
            {
                light.range = Main.settings.Lights_Range;
            }
            Lights_lastRange = Main.settings.Lights_Range;
        }
        private void UpdateDimmer()
        {
            if (hdLightdata.Count <= 0)
                return;

            if (Mathf.Approximately(Lights_lastDimmer, Main.settings.Lights_Dimmer))
                return;

            foreach (Light light in lights)
            {
                if (hdLightdata.TryGetValue(light, out HDAdditionalLightData hddata))
                {
                    hddata.lightDimmer = Main.settings.Lights_Dimmer;
                }
            }
            Lights_lastDimmer = Main.settings.Lights_Dimmer;
        }
        private void UpdateVolumertic()
        {
            if (hdLightdata.Count <= 0)
                return;

            if (Mathf.Approximately(Lights_lastVolIntensity, Main.settings.Lights_Volumetric))
                return;

            foreach (Light light in lights)
            {
                if (hdLightdata.TryGetValue(light, out HDAdditionalLightData hddata))
                {
                    hddata.volumetricDimmer = Main.settings.Lights_Volumetric;
                }
            }
            Lights_lastVolIntensity = Main.settings.Lights_Volumetric;
        }
        #endregion

        #region Directional light Updates
        private void UpdateDirectLightsColor()
        {
            if (!directLightsFound) return;

            Color newColor = new Color(Main.settings.Direct_Color_R, Main.settings.Direct_Color_G, Main.settings.Direct_Color_B);

            if (!Mathf.Approximately(Direct_Color.r, newColor.r) ||
                !Mathf.Approximately(Direct_Color.g, newColor.g) ||
                !Mathf.Approximately(Direct_Color.b, newColor.b))
            {
                SetLightColor(newColor, directLights);

                Direct_Color = newColor;
            }
        }
        private void UpdateDirectIntensity()
        {
            if (directLights.Count <= 0)
                return;

            float Multiplier = Mathf.Max(1, Main.settings.Direct_IntensityMultiplyer);

            if (Mathf.Approximately(Direct_lastMultiplier, Multiplier))
                return;

            foreach (Light light in directLights)
            {
                if (Default_Direct_Intensity.TryGetValue(light, out float originalIntensity))
                {
                    light.intensity = originalIntensity * Multiplier;
                }
            }
            Direct_lastMultiplier = Multiplier;
        }
        private void UpdateDirectDimmer()
        {
            if (Direct_hdLightdata.Count <= 0)
                return;

            if (Mathf.Approximately(Direct_lastDimmer, Main.settings.Direct_Dimmer))
                return;

            foreach (Light light in directLights)
            {
                if (Direct_hdLightdata.TryGetValue(light, out HDAdditionalLightData hddata))
                {
                    hddata.lightDimmer = Main.settings.Direct_Dimmer;
                }
            }
            Direct_lastDimmer = Main.settings.Direct_Dimmer;
        }
        #endregion

    }
}
