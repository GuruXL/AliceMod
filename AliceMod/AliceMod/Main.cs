using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using RapidGUI;
using System;
using Object = UnityEngine.Object;
using System.Runtime;

namespace AliceMod
{
    internal static class Main
    {
        public static bool enabled;
        public static Harmony harmonyInstance;
        public static string modId = "ReplayTestMod";
        public static UnityModManager.ModEntry modEntry;
        public static Settings settings;
        public static GameObject ScriptManager;
        public static LightController lightController;

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
                modEntry.OnGUI = OnGUI;
                modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(OnSaveGUI);
                modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(OnToggle);
                modEntry.OnUnload = new Func<UnityModManager.ModEntry, bool>(Unload);
                Main.modEntry = modEntry;
                Logger.Log(nameof(Load));
            }
            catch (Exception ex)
            {
                Logger.Error($"Error Loading {modEntry}: {ex.Message}");
                return false;
            }

            return true;
        }
        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Get Lights", RGUIStyle.button, GUILayout.Width(182)))
            {
                lightController.GetLights();
            }
            //GUILayout.BeginVertical();
            //settings.Lights_Intensity = RGUI.SliderFloat(settings.Lights_Intensity, 0f, 200000f, 4000f, 90, "Light Intensity");
            //settings.Lights_Area = RGUI.SliderFloat(settings.Lights_Area, 0f, 200f, 80f, 90, "Light Area");
            //GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (RGUI.Button(ColorLoop.isRGBActive, "RGB Lights", GUILayout.Width(156)))
            {
                ColorLoop.isRGBActive = !ColorLoop.isRGBActive;

                if (ColorLoop.isRGBActive)
                {
                    lightController.StartRGBRoutine();
                }
                else
                {
                    lightController.StopRGBRoutine();
                    lightController.ResetDefaultLightValues();
                }
            }
            if (RGUI.Button(ColorLoop.randomColors, "Random Colors", GUILayout.Width(156)))
            {
                ColorLoop.randomColors = !ColorLoop.randomColors;

                if (ColorLoop.isRGBActive)
                {
                    lightController.StopRGBRoutine();
                    lightController.StartRGBRoutine();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(256));
            if (ColorLoop.isRGBActive)
            {
                settings.RGB_Duration = RGUI.SliderFloat(settings.RGB_Duration, 1.0f, 10.0f, 2.0f, 96, "RGB duration");
            }
            GUILayout.EndVertical();
        }
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (enabled == value)
                return true;

            enabled = value;

            if (enabled)
            {
                try
                {
                    harmonyInstance = new Harmony(modEntry.Info.Id);
                    harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());       
                    ScriptManager = new GameObject("AliceMod");
                    lightController = ScriptManager.AddComponent<LightController>();
                    Object.DontDestroyOnLoad(ScriptManager);
                    //AssetLoader.LoadBundles();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error during {modEntry} initialization: {ex.Message}");
                    enabled = false;
                    return false;
                }
            }
            else
            {
                Unload(modEntry);
            }

            return true;
        }

        public static bool Unload(UnityModManager.ModEntry modEntry)
        {
            try
            {
                harmonyInstance?.UnpatchAll(harmonyInstance.Id);

                if (ScriptManager != null)
                {
                    //AssetLoader.UnloadAssetBundle();
                    Object.Destroy(ScriptManager);
                    ScriptManager = null;
                }

                Logger.Log(nameof(Unload));
            }
            catch (Exception ex)
            {
                Logger.Error($"Error during {modEntry} unload: {ex.Message}");
                return false;
            }

            return true;
        }

        public static UnityModManager.ModEntry.ModLogger Logger => modEntry.Logger;
    }
}
