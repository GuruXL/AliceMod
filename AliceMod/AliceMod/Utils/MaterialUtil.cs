using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AliceMod
{
    public static class MaterialUtil
    {
        public static IEnumerator AnimateMaterialFloat(Material mat, string shaderFloatVar, float goal, float duration)
        {
            float startValue = mat.GetFloat(shaderFloatVar);
            float timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / duration;
                float newValue = Mathf.Lerp(startValue, goal, t);
                mat.SetFloat(shaderFloatVar, newValue);

                yield return null;
            }
            mat.SetFloat(shaderFloatVar, goal);
        }
        public static void SetRandomMaterialColor(Material mat, string shadercolorName, float intensity)
        {
            Color color = GetRandomColor(intensity);
            mat.SetColor(shadercolorName, color);
        }
        private static Color GetRandomColor(float intensity)
        {
            return new Color(
                UnityEngine.Random.value * intensity,
                UnityEngine.Random.value * intensity,
                UnityEngine.Random.value * intensity
            );
        }
    }
}
