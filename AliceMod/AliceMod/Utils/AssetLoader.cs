using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;
using System;
using GameManagement;

namespace AliceMod
{
    public static class AssetLoader
    {
        public static AssetBundle bundle;
        public static Cubemap Sky_shanghai;
        public static string bundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "aliceskys");

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

            while (!requestFXbundle.isDone)
            {
                yield return null;
            }

            bundle = requestFXbundle.assetBundle;

            if (bundle == null)
            {
                assetsLoaded = false;
                yield break;
            }

            Sky_shanghai = bundle.LoadAsset<Cubemap>("shanghai4k");

            if (Sky_shanghai != null)
            {
                assetsLoaded = true;
            }
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
