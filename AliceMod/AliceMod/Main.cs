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
        public static SkyController skyController;
        public static UIController uiController;
        public static MapChangeManager mapChangeManager;
        public static RGBParticles rgbParticles;
        public static MeshTrail meshTrail;

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
            GUILayout.BeginHorizontal(GUILayout.Width(256));
            {
                settings.BG_RGB_Duration = RGUI.SliderFloat(settings.BG_RGB_Duration, 0.1f, 10.0f, 3.0f, 186, "Background RGB duration");
            }
            GUILayout.EndHorizontal();
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
                    skyController = ScriptManager.AddComponent<SkyController>();
                    uiController = ScriptManager.AddComponent<UIController>();
                    mapChangeManager = ScriptManager.AddComponent<MapChangeManager>();
                    rgbParticles = ScriptManager.AddComponent<RGBParticles>();
                    meshTrail = ScriptManager.AddComponent<MeshTrail>();
                    Object.DontDestroyOnLoad(ScriptManager);
                    AssetLoader.LoadBundles();
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
                    AssetLoader.UnloadAssetBundle();
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
