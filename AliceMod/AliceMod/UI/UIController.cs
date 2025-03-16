using UnityEngine;
using RapidGUI;
using System.Collections;
using System;
using AliceMod;
using static RootMotion.Dynamics.RagdollCreator.CreateJointParams;
using System.Runtime.InteropServices;

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
        readonly UItab VFX_Tab = new UItab(true, "VFX", 14);
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
                BGColorRoutine = StartCoroutine(ColorLoop.BG_RGBColorLoop());
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
            ColorLoop.BG_colorSetter = this;
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
            VFXUI();

        }
        public void SetColor(Color color)
        {
            BGColor = color;
        }
        private void MainUI()
        {
            GUILayout.Label("Alice Mod v0.0.1");
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
                        if (RGUI.Button(ColorLoop.isRGBActive, "RGB Lights", GUILayout.Width(128)))
                        {
                            ColorLoop.isRGBActive = !ColorLoop.isRGBActive;

                            if (ColorLoop.isRGBActive)
                            {
                                Main.lightController.StartRGBRoutine();
                            }
                            else
                            {
                                Main.lightController.StopRGBRoutine();
                                Main.lightController.ResetDefaultLightValues();
                            }
                        }
                    }
                    GUILayout.EndHorizontal();   
                    if (ColorLoop.isRGBActive)
                    {
                        GUILayout.BeginVertical("Box");
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (RGUI.Button(ColorLoop.randomColors, "Random Colors", GUILayout.Width(156)))
                                {
                                    ColorLoop.randomColors = !ColorLoop.randomColors;

                                    // restart the loop after button press. loop checks for randomColors bool
                                    Main.lightController.StopRGBRoutine();
                                    Main.lightController.StartRGBRoutine();
                                }
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
                            GUILayout.Space(4);
                            Main.settings.Lights_Dimmer = RGUI.SliderFloat(Main.settings.Lights_Dimmer, 0.0f, 20.0f, 1.0f, 96, "Dimmer");
                            GUILayout.Space(4);
                            Main.settings.Lights_Range = RGUI.SliderFloat(Main.settings.Lights_Range, 0.0f, 20.0f, 8.0f, 96, "Range");
                            GUILayout.Space(4);
                            Main.settings.Lights_Volumetric = RGUI.SliderFloat(Main.settings.Lights_Volumetric, 0.0f, 20.0f, 1.0f, 96, "Volumetrics");

                        }
                        GUILayout.EndVertical();
                        GUILayout.Space(4);
                        GUILayout.Label("<b>Colour</b>");
                        GUILayout.BeginVertical();
                        {
                            Main.settings.Lights_Color_R = RGUI.SliderFloat(Main.settings.Lights_Color_R, 0.0f, 1.0f, 1.0f, 24, "R");
                            GUILayout.Space(2);
                            Main.settings.Lights_Color_G = RGUI.SliderFloat(Main.settings.Lights_Color_G, 0.0f, 1.0f, 1.0f, 24, "G");
                            GUILayout.Space(2);
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
                            GUILayout.Space(4);
                            Main.settings.Direct_Dimmer = RGUI.SliderFloat(Main.settings.Direct_Dimmer, 0.0f, 20.0f, 1.0f, 98, "Sun Dimmer");
                        }
                        GUILayout.EndVertical();
                        GUILayout.Space(4);
                        GUILayout.Label("<b>Colour</b>");
                        GUILayout.BeginVertical();
                        {
                            Main.settings.Direct_Color_R = RGUI.SliderFloat(Main.settings.Direct_Color_R, 0.0f, 1.0f, 1.0f, 24, "R");
                            GUILayout.Space(2);
                            Main.settings.Direct_Color_G = RGUI.SliderFloat(Main.settings.Direct_Color_G, 0.0f, 1.0f, 1.0f, 24, "G");
                            GUILayout.Space(2);
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
            if (RGUI.Button(Main.settings.isCustomSkyactive, "Custom Skys"))
            {
                Main.settings.isCustomSkyactive = !Main.settings.isCustomSkyactive;
            }
            switch (Main.settings.isCustomSkyactive)
            {
                case true:
                    Main.skyController.skyVolumeObj.SetActive(true);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("<b>Skybox Type: </b>");
                    Main.settings.SkyState = RGUI.SelectionPopup(Main.settings.SkyState, Main.skyController.SkyStates);
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
        private void VFXUI()
        {
            Tabs(VFX_Tab, UIextensions.TabColorSwitch(VFX_Tab));
            if (VFX_Tab.isClosed)
                return;

            GUILayout.BeginVertical();
            if (RGUI.Button(Main.settings.MeshTrail_Enabled, "RGB Trail"))
            {
                Main.settings.MeshTrail_Enabled = !Main.settings.MeshTrail_Enabled;

                Main.rgbParticles.ToggleVFXObj(Main.settings.MeshTrail_Enabled);
            }
            if (RGUI.Button(Main.settings.MeshTrail_RandomColours, "Mesh Random Colours"))
            {
                Main.settings.MeshTrail_RandomColours = !Main.settings.MeshTrail_RandomColours;
            }
            GUILayout.EndVertical();
        }
    }
}



