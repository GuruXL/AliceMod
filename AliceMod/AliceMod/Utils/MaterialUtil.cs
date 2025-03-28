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
        public static void SetMaterialColor(Color newColor, Material mat, string shadercolorName, float intensity) 
        {
            Color color = new Color(
              Mathf.GammaToLinearSpace(newColor.r) * intensity,
              Mathf.GammaToLinearSpace(newColor.g) * intensity,
              Mathf.GammaToLinearSpace(newColor.b) * intensity,
              1.0f);
            mat.SetColor(shadercolorName, color);
        }
        public static void SetRandomMaterialColor(Material mat, string shadercolorName, float intensity)
        {
            Color color = GetRandomColor(intensity);
            mat.SetColor(shadercolorName, color);
        }
        private static Color GetRandomColor(float intensity)
        {
            return  new Color(
                   Mathf.GammaToLinearSpace(UnityEngine.Random.value) * intensity,
                   Mathf.GammaToLinearSpace(UnityEngine.Random.value) * intensity,
                   Mathf.GammaToLinearSpace(UnityEngine.Random.value) * intensity,
                   1.0f);
        }
    }
}
