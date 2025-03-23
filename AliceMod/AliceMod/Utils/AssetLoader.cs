using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;
using System;
using GameManagement;
using UnityEngine.VFX;

namespace AliceMod
{
    public static class AssetLoader
    {
        public static AssetBundle bundle;
        public static Cubemap Sky_shanghai;
        public static VisualEffectAsset RGB_Trail_asset;
        public static Shader Mesh_HoloShader;
        public static Material Mesh_HoloMaterial;
        public static Texture2D Mesh_Holotexture;
        public static GameObject FX_SparksPrefab;
        public static string bundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "aliceassets");

        public static bool assetsLoaded { get; private set; } = false;

        public static void LoadBundles()
        {
            // Checks if a type from the Unity assembly has been loaded
            Type unityObjectType = Type.GetType("UnityEngine.Object, UnityEngine");

            if (unityObjectType != null)
            {
                GameStateMachine.Instance.StartCoroutine(LoadAssetBundle());
            }
        }
        private static IEnumerator LoadAssetBundle()
        {
            AssetBundleCreateRequest requestFXbundle = AssetBundle.LoadFromFileAsync(bundlePath);

            yield return new WaitUntil(() => requestFXbundle.isDone);

            bundle = requestFXbundle.assetBundle;

            if (bundle == null)
            {
                assetsLoaded = false;
                yield break;
            }

            Sky_shanghai = bundle.LoadAsset<Cubemap>("shanghai4k");
            RGB_Trail_asset = bundle.LoadAsset<VisualEffectAsset>("RGBTrailV2");
            FX_SparksPrefab = bundle.LoadAsset<GameObject>("SparksV2");
            Mesh_HoloShader = bundle.LoadAsset<Shader>("MeshTrail");
            Mesh_HoloMaterial = bundle.LoadAsset<Material>("MeshTrailv2");
            Mesh_Holotexture = bundle.LoadAsset<Texture2D>("hologramtexture");

            // Check if all assets are loaded
            assetsLoaded = Sky_shanghai != null &&
                           RGB_Trail_asset != null &&
                           FX_SparksPrefab != null &&
                           Mesh_HoloShader != null &&
                           Mesh_HoloMaterial != null &&
                           Mesh_Holotexture != null;

            yield return null;
        }

        public static void UnloadAssetBundle()
        {
            if (bundle != null)
            {
                bundle.Unload(true);
                bundle = null;
                assetsLoaded = false;
            }
        }
        private static void OnDestroy()
        {
            UnloadAssetBundle();
        }
    }
}
