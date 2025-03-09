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

    public class UIcontroller : MonoBehaviour, IColorSetter
    {
        private Coroutine BGColorRoutine;
        private Color BGColor = new Color(0.0f, 0.0f, 0.0f);

        public bool showUI;
        private Rect MainWindowRect = new Rect(20, 20, Screen.width / 6, 20);

        //readonly UItab Test_Tab = new UItab(true, "Test Stuff", 14);
        readonly UItab Lights_Tab = new UItab(true, "Lights", 14);
        readonly UItab Sky_Tab = new UItab(true, "Skys", 14);
        private void Tabs(UItab obj, string color = "#e6ebe8")
        {
            if (GUILayout.Button($"<size={obj.font}><color={color}>" + (obj.isClosed ? "○" : "●") + obj.text + "</color>" + "</size>", "Label"))
            {
                obj.isClosed = !obj.isClosed;
                MainWindowRect.height = 20;
                MainWindowRect.width = Screen.width / 6;
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
                        GUILayout.Label(Main.lightController.lightsFound ? "<b><color=#7CFC00> Lights Found </color></b>" : "<b><color=#b30000> No Lights Found </color></b>", GUILayout.ExpandWidth(true));
                        GUILayout.Label($"For Map - {LevelManager.Instance.currentLevel.name}", GUILayout.ExpandWidth(true));
                        GUILayout.FlexibleSpace();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical();
                    {
                        GUILayout.Label($"Lights: {Main.lightController.lights.Count}", GUILayout.ExpandWidth(true));
                        GUILayout.Label($"Directional Lights: {Main.lightController.directLights.Count}", GUILayout.ExpandWidth(true));
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                GUILayout.Space(8);

                GUILayout.BeginVertical(GUILayout.Width(256));
                {
                    Main.settings.Lights_Brightness = RGUI.SliderFloat(Main.settings.Lights_Brightness, 0.0f, 20.0f, 1.0f, 96, "Brightness");
                    GUILayout.Space(4);
                    Main.settings.Lights_Range = RGUI.SliderFloat(Main.settings.Lights_Range, 0.0f, 20.0f, 8.0f, 96, "Range");
                    GUILayout.Space(4);
                    Main.settings.Lights_Volumetric = RGUI.SliderFloat(Main.settings.Lights_Volumetric, 0.0f, 20.0f, 1.0f, 96, "Volumetric Intensity");
                    
                }
                GUILayout.EndVertical();

                GUILayout.BeginHorizontal();
                {
                    if (RGUI.Button(ColorLoop.isRGBActive, "RGB Lights", GUILayout.Width(156)))
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
                    if (RGUI.Button(ColorLoop.randomColors, "Random Colors", GUILayout.Width(156)))
                    {
                        ColorLoop.randomColors = !ColorLoop.randomColors;

                        if (ColorLoop.isRGBActive)
                        {
                            Main.lightController.StopRGBRoutine();
                            Main.lightController.StartRGBRoutine();
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginVertical(GUILayout.Width(256));
                {
                    if (ColorLoop.isRGBActive)
                    {
                        Main.settings.RGB_Duration = RGUI.SliderFloat(Main.settings.RGB_Duration, 1.0f, 10.0f, 2.0f, 96, "RGB duration");
                    }
                }
                GUILayout.EndVertical();
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
                    GUILayout.Label("<b>SkyBox Type: </b>");
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
    }
}



