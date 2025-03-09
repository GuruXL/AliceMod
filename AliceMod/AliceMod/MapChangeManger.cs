using System.Collections;
using UnityEngine;
using Rewired;

namespace AliceMod
{
    public class MapChangeManager : MonoBehaviour
    {
        private void Awake()
        {
            LevelManager.Instance.OnLevelChanged += HandleLevelChanged;
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
    }
}