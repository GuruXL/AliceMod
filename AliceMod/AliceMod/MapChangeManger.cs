using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;
using UnityEngine.Rendering.HighDefinition;

namespace AliceMod
{
    public class MapChangeManager : MonoBehaviour
    {
        private void Awake()
        {
            LevelManager.Instance.OnLevelChanged += HandleLevelChanged;
        }
        private void Start()
        {
            StartCoroutine(GetFirstSceneLights());
        }
        private void OnDestroy()
        {
            LevelManager.Instance.OnLevelChanged -= HandleLevelChanged;
        }
        private void HandleLevelChanged(LevelInfo levelInfo)
        {
            Main.lightController.GetLights();

            if (ColorLoop.isRGBActive)
            {
                Main.lightController.StopRGBRoutine();
            }
        }
        private IEnumerator GetFirstSceneLights()
        {
            yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
            Main.lightController.GetLights();
            yield return null;
        }
    }
}