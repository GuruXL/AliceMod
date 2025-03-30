using UnityEngine;
using RapidGUI;
using System.Collections;
using System;
using AliceMod;
using static RootMotion.Dynamics.RagdollCreator.CreateJointParams;
using System.Runtime.InteropServices;
using Mono.Cecil;

namespace AliceMod
{
    public class UItab // UI dropdown tabs class
    {
        public bool isClosed;
        public string text;
        public int font;

        public UItab(bool isClosed, string text, int font)
        {
            this.isClosed = isClosed;
            this.text = text;
            this.font = font;
        }
    }

    public class UIController : MonoBehaviour, IColorSetter
    {
        private Coroutine BGColorRoutine;
        private Color BGColor = new Color(0.0f, 0.0f, 0.0f);

        public bool showUI;
        private Rect MainWindowRect = new Rect(20, 20, Screen.width / 5, 20);

        //readonly UItab Test_Tab = new UItab(true, "Test Stuff", 14);
        readonly UItab Lights_Tab = new UItab(true, "Lights", 14);
        readonly UItab Lights_Normal_Tab = new UItab(true, "Light Settings", 13);
        readonly UItab Lights_Direct_Tab = new UItab(true, "Sun Settings", 13);
        readonly UItab Sky_Tab = new UItab(true, "Skys", 14);
        readonly UItab FX_Tab = new UItab(true, "FX", 14);
        readonly UItab FX_GhostTab = new UItab(true, "Ghost Trail", 14);
        readonly UItab FX_GrindSparks = new UItab(true, "Grind Sparks", 14);
        private void Tabs(UItab obj, string color = "#e6ebe8")
        {
            if (GUILayout.Button($"<size={obj.font}><color={color}>" + (obj.isClosed ? "○" : "●") + obj.text + "</color>" + "</size>", "Label"))
            {
                obj.isClosed = !obj.isClosed;
                MainWindowRect.height = 20;
                MainWindowRect.width = Screen.width / 5;
                UIextensions.TabFontSwitch(obj);
            }
        }
        public void StartBGColorRoutine()
        {
            if (BGColorRoutine == null)
            {
                BGColorRoutine = StartCoroutine(ColorLoop.RGBColorLoop(this, () => Main.settings.BG_RGB_Duration));
            }
        }
        public void StopBGColorRoutine()
        {
            if (BGColorRoutine != null)
            {
                StopCoroutine(BGColorRoutine);
                BGColorRoutine = null; // Set to null so it can be restarted again
            }
        }
        private void Start()
        {
            StartCoroutine(WaitForInput());
            //ColorLoop.BG_colorSetter = this;
        }
        private void OnDestroy()
        {
            StopBGColorRoutine();
        }
        private IEnumerator WaitForInput()
        {
            while (!Main.enabled)
            {
                yield return null;
            }

            while (true)
            {
                yield return new WaitForEndOfFrame();
                InputSwitch();
                yield return null;
            }
        }

        private void InputSwitch()
        {
            if ((Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.A))
            {
                ToggleUI();
            }
        }

        private void ToggleUI()
        {
            if (!showUI)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
        private void Open()
        {
            showUI = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            StartBGColorRoutine();
        }

        private void Close()
        {
            showUI = false;
            Cursor.visible = false;
            Main.settings.Save(Main.modEntry);
            StopBGColorRoutine();
        }

        private void OnGUI()
        {
            if (!showUI)
                return;

            GUI.backgroundColor = BGColor;
            MainWindowRect = GUILayout.Window(478462, MainWindowRect, MainWindow, "<b> Alice Mod </b>");
        }

        // Creates the GUI window
        private void MainWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));

            MainUI();
            LightsUI();
            SkyUI();
            FXUI();
        }
        public void SetColor(Color color) // IcolorSetter
        {
            BGColor = color;
        }
        private void MainUI()
        {
            GUILayout.Label($"Alice Mod {Main.version}");
        }
        private void LightsUI()
        {
            Tabs(Lights_Tab, UIextensions.TabColorSwitch(Lights_Tab));
            if (Lights_Tab.isClosed)
                return;

            GUILayout.BeginVertical("Box");
            {
                GUILayout.BeginVertical("Box");
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("Get Lights", RGUIStyle.button, GUILayout.Width(128)))
                        {
                            Main.lightController.GetLights();
                        }
                        GUILayout.Space(4);
                        if (GUILayout.Button("Reset Lights", RGUIStyle.button, GUILayout.Width(128)))
                        {
                            Main.lightController.ResetDefaultLightValues();
                        }
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label($"Map - {LevelManager.Instance.currentLevel.name}", GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(Main.lightController.lightsFound ? "<b><color=#7CFC00> Found </color></b>" : "<b><color=#b30000> Not Found </color></b>", GUILayout.ExpandWidth(true));
                        GUILayout.Label($"Lights: {Main.lightController.lights.Count}", GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(Main.lightController.directLightsFound ? "<b><color=#7CFC00> Found </color></b>" : "<b><color=#b30000> Not Found </color></b>", GUILayout.ExpandWidth(true));
                        GUILayout.Label($"Directional Lights: {Main.lightController.directLights.Count}", GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Space(8);
                    GUILayout.BeginHorizontal();
                    {
                        if (RGUI.Button(ColorLoop.RGB_Lights_Active, "RGB Lights"))
                        {
                            ColorLoop.RGB_Lights_Active = !ColorLoop.RGB_Lights_Active;

                            if (ColorLoop.RGB_Lights_Active)
                            {
                                Main.lightController.StopRGBRoutine();
                                Main.lightController.StartRGBRoutine(ColorLoop.RGBColorLoop(Main.lightController.ColorSetter, () => Main.settings.RGB_Duration));
                                ColorLoop.RGB_Lights_Random = false;
                            }
                            else
                            {
                                Main.lightController.StopRGBRoutine();
                                Main.lightController.ResetDefaultLightValues();
                            }
                        }
                        if (RGUI.Button(ColorLoop.RGB_Lights_Random, "RGB Random Colors"))
                        {
                            ColorLoop.RGB_Lights_Random = !ColorLoop.RGB_Lights_Random;

                            if (ColorLoop.RGB_Lights_Random)
                            {
                                Main.lightController.StopRGBRoutine();
                                Main.lightController.StartRGBRoutine(ColorLoop.RGBRandomColorLoop(Main.lightController.ColorSetter, () => Main.settings.RGB_Duration));
                                ColorLoop.RGB_Lights_Active = false;
                            }
                            else
                            {
                                Main.lightController.StopRGBRoutine();
                                Main.lightController.ResetDefaultLightValues();
                            }
                        }
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();
                    if (ColorLoop.RGB_Lights_Active || ColorLoop.RGB_Lights_Random)
                    {
                        GUILayout.BeginVertical("Box");
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (RGUI.Button(Main.settings.RGB_Sun_Active, "RGB Sun", GUILayout.Width(128)))
                                {
                                    Main.settings.RGB_Sun_Active = !Main.settings.RGB_Sun_Active;
                                }
                            }
                            GUILayout.EndHorizontal();
                            GUILayout.BeginVertical();
                            {
                                Main.settings.RGB_Duration = RGUI.SliderFloat(Main.settings.RGB_Duration, 0.1f, 10.0f, 2.0f, 96, "RGB duration");
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndVertical();
                    }
                }
                GUILayout.EndVertical();

                Tabs(Lights_Normal_Tab, UIextensions.TabColorSwitch(Lights_Normal_Tab));
                if (!Lights_Normal_Tab.isClosed)
                {
                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.BeginVertical();
                        {
                            Main.settings.Lights_IntensityMultiplyer = RGUI.SliderFloat(Main.settings.Lights_IntensityMultiplyer, 1.0f, 100.0f, 1.0f, 128, "Intensity Multiplier");
                            Main.settings.Lights_Dimmer = RGUI.SliderFloat(Main.settings.Lights_Dimmer, 0.0f, 20.0f, 1.0f, 96, "Dimmer");
                            Main.settings.Lights_Range = RGUI.SliderFloat(Main.settings.Lights_Range, 0.0f, 20.0f, 8.0f, 96, "Range");
                            Main.settings.Lights_Volumetric = RGUI.SliderFloat(Main.settings.Lights_Volumetric, 0.0f, 20.0f, 1.0f, 96, "Volumetrics");

                        }
                        GUILayout.EndVertical();
                        GUILayout.Label("<b>Colour</b>");
                        Color color = new Color(Main.settings.Lights_Color_R, Main.settings.Lights_Color_G, Main.settings.Lights_Color_B);
                        UIextensions.ColorBox(color, 60, 10);
                        GUILayout.BeginVertical();
                        {
                            Main.settings.Lights_Color_R = RGUI.SliderFloat(Main.settings.Lights_Color_R, 0.0f, 1.0f, 1.0f, 24, "R");
                            Main.settings.Lights_Color_G = RGUI.SliderFloat(Main.settings.Lights_Color_G, 0.0f, 1.0f, 1.0f, 24, "G");
                            Main.settings.Lights_Color_B = RGUI.SliderFloat(Main.settings.Lights_Color_B, 0.0f, 1.0f, 1.0f, 24, "B");
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }

                Tabs(Lights_Direct_Tab, UIextensions.TabColorSwitch(Lights_Direct_Tab));
                if (!Lights_Direct_Tab.isClosed)
                {
                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.BeginVertical();
                        {
                            Main.settings.Direct_IntensityMultiplyer = RGUI.SliderFloat(Main.settings.Direct_IntensityMultiplyer, 1.0f, 100.0f, 1.0f, 128, "Sun Multiplier");
                            Main.settings.Direct_Dimmer = RGUI.SliderFloat(Main.settings.Direct_Dimmer, 0.0f, 20.0f, 1.0f, 98, "Sun Dimmer");
                        }
                        GUILayout.EndVertical();
                        GUILayout.Label("<b>Colour</b>");
                        Color color = new Color(Main.settings.Direct_Color_R, Main.settings.Direct_Color_G, Main.settings.Direct_Color_B);
                        UIextensions.ColorBox(color, 60, 10);
                        GUILayout.BeginVertical();
                        {
                            Main.settings.Direct_Color_R = RGUI.SliderFloat(Main.settings.Direct_Color_R, 0.0f, 1.0f, 1.0f, 24, "R");
                            Main.settings.Direct_Color_G = RGUI.SliderFloat(Main.settings.Direct_Color_G, 0.0f, 1.0f, 1.0f, 24, "G");
                            Main.settings.Direct_Color_B = RGUI.SliderFloat(Main.settings.Direct_Color_B, 0.0f, 1.0f, 1.0f, 24, "B");
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
        }
        private void SkyUI()
        {
            Tabs(Sky_Tab, UIextensions.TabColorSwitch(Sky_Tab));
            if (Sky_Tab.isClosed)
                return;

            GUILayout.BeginVertical("Box");
            if (RGUI.Button(Main.settings.Sky_CustomSky_Enabled, "Custom Skys"))
            {
                Main.settings.Sky_CustomSky_Enabled = !Main.settings.Sky_CustomSky_Enabled;
            }
            switch (Main.settings.Sky_CustomSky_Enabled)
            {
                case true:
                    Main.skyController.skyVolumeObj.SetActive(true);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("<b>Skybox Type: </b>");
                    Main.settings.Sky_State = RGUI.SelectionPopup(Main.settings.Sky_State, Main.skyController.SkyStates);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical("Box");
                    Main.settings.Vol_Weight = RGUI.SliderFloat(Main.settings.Vol_Weight, 0.0f, 1.0f, 1.0f, 96, "Weight");
                    Main.settings.Vol_Prio = RGUI.SliderFloat(Main.settings.Vol_Prio, 0.0f, 9999.0f, 10.0f, 96, "Priority");
                    GUILayout.EndVertical();

                    GUILayout.BeginVertical("Box");
                    Main.settings.Sky_SkyExposure = RGUI.SliderFloat(Main.settings.Sky_SkyExposure, -10, 20, 8.0f, 96, "Sky Exposure");
                    Main.settings.Sky_Rotation = RGUI.SliderFloat(Main.settings.Sky_Rotation, 0.0f, 360.0f, 0.0f, 96, "Sky Rotation");
                    Main.settings.Sky_Exposure = RGUI.SliderFloat(Main.settings.Sky_Exposure, -10, 20, 11.5f, 96, "Exposure");
                    Main.settings.Sky_IndirectDiffuse = RGUI.SliderFloat(Main.settings.Sky_IndirectDiffuse, -1.0f, 4.0f, 1.0f, 96, "Indirect Diffuse");
                    Main.settings.Sky_IndirectSpecular = RGUI.SliderFloat(Main.settings.Sky_IndirectSpecular, -1.0f, 4.0f, 1.0f, 100, "Indirect Specular");
                    Main.settings.Sky_Temp = RGUI.SliderFloat(Main.settings.Sky_Temp, -20.0f, 20.0f, -12.0f, 96, "Temperature");
                    Main.settings.Sky_Tint = RGUI.SliderFloat(Main.settings.Sky_Tint, -20.0f, 20.0f, 0.0f, 96, "Tint");
                    GUILayout.EndVertical();
                    break;
                case false:
                    Main.skyController.skyVolumeObj.SetActive(false);
                    break;
            }
            GUILayout.EndVertical();
        }
        private void FXUI()
        {
            Tabs(FX_Tab, UIextensions.TabColorSwitch(FX_Tab));
            if (FX_Tab.isClosed)
                return;
            GUILayout.BeginVertical("Box");
            {
                Tabs(FX_GhostTab, UIextensions.TabColorSwitch(FX_GhostTab));
                if (!FX_GhostTab.isClosed)
                {
                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (RGUI.Button(Main.meshTrail.MeshTrail_Enabled, "Ghost Trail"))
                            {
                                Main.meshTrail.MeshTrail_Enabled = !Main.meshTrail.MeshTrail_Enabled;

                                Main.rgbParticles.ToggleVFXObj(Main.meshTrail.MeshTrail_Enabled);
                            }
                            if (RGUI.Button(Main.meshTrail.MeshTrail_RandomColours, "Random Ghost Colours"))
                            {
                                Main.meshTrail.MeshTrail_RandomColours = !Main.meshTrail.MeshTrail_RandomColours;
                            }
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();

                        GUILayout.BeginVertical("Box");
                        {
                            Main.settings.Mesh_DestroyDelay = RGUI.SliderFloat(Main.settings.Mesh_DestroyDelay, 0.0f, 4.0f, 2.0f, 120, "Destroy Delay");
                            Main.settings.Mesh_RefreshRate = RGUI.SliderFloat(Main.settings.Mesh_RefreshRate, 0.0f, 1.0f, 0.2f, 120, "Refresh Rate");
                            Main.settings.Shader_Float_FadeDuration = RGUI.SliderFloat(Main.settings.Shader_Float_FadeDuration, 0.0f, 4.0f, 1.0f, 120, "Fade Duration");
                            Main.settings.Velocity_Threshold = RGUI.SliderFloat(Main.settings.Velocity_Threshold, 0.0f, 5.0f, 1.0f, 120, "Velocity Threshold");
                            Main.settings.Shader_ColorIntensity = RGUI.SliderFloat(Main.settings.Shader_ColorIntensity, 0.0f, 4.0f, 1.0f, 120, "Colour Intensity");
                        }
                        GUILayout.EndVertical();
                        GUILayout.Label("<b>Base Colour</b>");
                        Color basecolor = new Color(Main.settings.Shader_BaseColor_R, Main.settings.Shader_BaseColor_G, Main.settings.Shader_BaseColor_B);
                        UIextensions.ColorBox(basecolor, 60, 10);
                        GUILayout.BeginVertical("Box");
                        {
                            Main.settings.Shader_BaseColor_R = RGUI.SliderFloat(Main.settings.Shader_BaseColor_R, 0.0f, 1.0f, 0.0f, 24, "R");
                            Main.settings.Shader_BaseColor_G = RGUI.SliderFloat(Main.settings.Shader_BaseColor_G, 0.0f, 1.0f, 0.80f, 24, "G");
                            Main.settings.Shader_BaseColor_B = RGUI.SliderFloat(Main.settings.Shader_BaseColor_B, 0.0f, 1.0f, 1.0f, 24, "B");
                        }
                        GUILayout.EndVertical();
                        GUILayout.Label("<b>Dissolve Colour</b>");
                        Color dissolvecolor = new Color(Main.settings.Shader_DissolveColor_R, Main.settings.Shader_DissolveColor_G, Main.settings.Shader_DissolveColor_B);
                        UIextensions.ColorBox(dissolvecolor, 60, 10);
                        GUILayout.BeginVertical("Box");
                        {
                            Main.settings.Shader_DissolveColor_R = RGUI.SliderFloat(Main.settings.Shader_DissolveColor_R, 0.0f, 1.0f, 0.85f, 24, "R");
                            Main.settings.Shader_DissolveColor_G = RGUI.SliderFloat(Main.settings.Shader_DissolveColor_G, 0.0f, 1.0f, 0.20f, 24, "G");
                            Main.settings.Shader_DissolveColor_B = RGUI.SliderFloat(Main.settings.Shader_DissolveColor_B, 0.0f, 1.0f, 0.90f, 24, "B");
                        }
                        GUILayout.EndVertical();
                    }
                    GUILayout.EndVertical();
                }

                Tabs(FX_GrindSparks, UIextensions.TabColorSwitch(FX_GrindSparks));
                if (!FX_GrindSparks.isClosed)
                {
                    GUILayout.BeginVertical("Box");
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (RGUI.Button(Main.settings.FX_Sparks_Enabled, "Grind Sparks"))
                            {
                                Main.settings.FX_Sparks_Enabled = !Main.settings.FX_Sparks_Enabled;

                                Main.grindSparks.ToggleSparkObjects(Main.settings.FX_Sparks_Enabled);
                            }
                            if (RGUI.Button(Main.settings.FX_Sparks_SyncReplaySpeed, "Sync Replay Speed"))
                            {
                                Main.settings.FX_Sparks_SyncReplaySpeed = !Main.settings.FX_Sparks_SyncReplaySpeed;
                            }
                            GUILayout.FlexibleSpace();
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Label("Spawn Position");
                            Main.settings.FX_Sparks_SpawnPoint = RGUI.SelectionPopup(Main.settings.FX_Sparks_SpawnPoint, Main.grindSparks.SpawnPoints);
                        }
                        GUILayout.EndVertical();
                        if (Main.settings.FX_Sparks_Enabled)
                        {
                            GUILayout.BeginVertical("Box");
                            {
                                GUILayout.BeginHorizontal();
                                {
                                    if (RGUI.Button(ColorLoop.RGB_FX_Active, "RGB Loop"))
                                    {
                                        ColorLoop.RGB_FX_Active = !ColorLoop.RGB_FX_Active;

                                        if (Main.settings.FX_Sparks_SetCustomColor)
                                        {
                                            Main.settings.FX_Sparks_SetCustomColor = false;
                                        }
                                        if (ColorLoop.RGB_FX_Active)
                                        {
                                            Main.grindSparks.StopRGBRoutine();
                                            Main.grindSparks.StartRGBRoutine(ColorLoop.RGBColorLoop(Main.grindSparks.ColorSetter, () => Main.settings.FX_Sparks_RGBDuration));
                                            ColorLoop.RGB_FX_Random = false;
                                        }
                                        else
                                        {
                                            Main.grindSparks.StopRGBRoutine();
                                        }

                                    }
                                    if (RGUI.Button(ColorLoop.RGB_FX_Random, "RGB Random Loop"))
                                    {
                                        ColorLoop.RGB_FX_Random = !ColorLoop.RGB_FX_Random;

                                        if (Main.settings.FX_Sparks_SetCustomColor)
                                        {
                                            Main.settings.FX_Sparks_SetCustomColor = false;
                                        }
                                        if (ColorLoop.RGB_FX_Random)
                                        {
                                            Main.grindSparks.StopRGBRoutine();
                                            Main.grindSparks.StartRGBRoutine(ColorLoop.RGBRandomColorLoop(Main.grindSparks.ColorSetter, () => Main.settings.FX_Sparks_RGBDuration));
                                            ColorLoop.RGB_FX_Active = false;
                                        }
                                        else
                                        {
                                            Main.grindSparks.StopRGBRoutine();
                                        }
                                    }
                                    GUILayout.FlexibleSpace();
                                }
                                GUILayout.EndHorizontal();
                                GUILayout.BeginVertical();
                                {
                                    Main.settings.FX_Sparks_RGBDuration = RGUI.SliderFloat(Main.settings.FX_Sparks_RGBDuration, 0.1f, 5.0f, 0.5f, 96, "RGB duration");
                                    Main.settings.FX_Sparks_EmissionMultiplier = RGUI.SliderFloat(Main.settings.FX_Sparks_EmissionMultiplier, 1.0f, 10.0f, 1.0f, 128, "Emission Multiplier");
                                    Main.settings.FX_Sparks_SpawnRate = RGUI.SliderFloat(Main.settings.FX_Sparks_SpawnRate, 10.0f, 500.0f, 200.0f, 128, "Spawn Rate");
                                }
                                GUILayout.EndVertical();
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.BeginVertical("Box");
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (RGUI.Button(Main.settings.FX_Sparks_SetCustomColor, "Custom Color"))
                                {
                                    Main.settings.FX_Sparks_SetCustomColor = !Main.settings.FX_Sparks_SetCustomColor;

                                    if (Main.settings.FX_Sparks_SetCustomColor)
                                    {
                                        if (ColorLoop.RGB_FX_Active || ColorLoop.RGB_FX_Random)
                                        {
                                            Main.grindSparks.StopRGBRoutine();
                                            ColorLoop.RGB_FX_Active = false;
                                            ColorLoop.RGB_FX_Random = false;
                                        }
                                        Main.grindSparks.SetMatColorToWhite();
                                        Main.grindSparks.Sparks_Color = Color.white;
                                    }
                                    else
                                    {
                                        Main.grindSparks.ResetAllMatsToDefault();
                                    }
                                }
                                GUILayout.FlexibleSpace();
                            }  
                            GUILayout.EndHorizontal();
                            if (Main.settings.FX_Sparks_SetCustomColor)
                            {
                                GUILayout.Label("<b>Colour</b>");
                                Color color = new Color(Main.settings.FX_Sparks_Color_R, Main.settings.FX_Sparks_Color_G, Main.settings.FX_Sparks_Color_B);
                                UIextensions.ColorBox(color, 60, 10);
                                GUILayout.BeginVertical();
                                {
                                    Main.settings.FX_Sparks_Color_R = RGUI.SliderFloat(Main.settings.FX_Sparks_Color_R, 0.0f, 1.0f, 1.0f, 24, "R");
                                    Main.settings.FX_Sparks_Color_G = RGUI.SliderFloat(Main.settings.FX_Sparks_Color_G, 0.0f, 1.0f, 1.0f, 24, "G");
                                    Main.settings.FX_Sparks_Color_B = RGUI.SliderFloat(Main.settings.FX_Sparks_Color_B, 0.0f, 1.0f, 1.0f, 24, "B");
                                }
                                GUILayout.EndVertical();
                            }            
                        }
                        GUILayout.EndVertical();
                        
                    }
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
        }
    }
}



