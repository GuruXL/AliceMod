using System;
using System.Collections;
using UnityEngine;

namespace AliceMod
{
    public static class ColorLoop
    {
        public static bool RGB_Lights_Active = false;
        public static bool RGB_Lights_Random = false;
        public static bool RGB_FX_Active = false;
        public static bool RGB_FX_Random = false;

        public static IEnumerator RGBColorLoop(IColorSetter colorSetter, Func<float> getDuration)
        {
            yield return null;

            float t = 0f;
            while (true)
            {
                float r = Mathf.PingPong(t, 1f);
                float g = Mathf.PingPong(t + 0.33f, 1f);
                float b = Mathf.PingPong(t + 0.66f, 1f);

                Color rgbColor = new Color(r, g, b);

                colorSetter?.SetColor(rgbColor);

                //Main.Logger.Log($"Color: {rgbColor}");

                yield return null;
                float speed = Mathf.Max(0.1f, getDuration());
                t += Time.deltaTime / speed;
            }
        }

        public static IEnumerator RGBRandomColorLoop(IColorSetter colorSetter, Func<float> getDuration)
        {
            yield return null;

            Color LastColor = Color.white;

            while (true)
            {
                Color startColor = LastColor;
                Color32 endColor = new Color32(
                    (byte)UnityEngine.Random.Range(0, 256),
                    (byte)UnityEngine.Random.Range(0, 256),
                    (byte)UnityEngine.Random.Range(0, 256),
                    255);
                //Color endColor = new Color(
                    //Mathf.GammaToLinearSpace(UnityEngine.Random.value),
                    //Mathf.GammaToLinearSpace(UnityEngine.Random.value),
                    //Mathf.GammaToLinearSpace(UnityEngine.Random.value),
                    //1.0f);

                float t = 0;
                while (t < 1)
                {
                    float speed = Mathf.Max(0.1f, getDuration());
                    t += Time.deltaTime / speed;
                    Color currentColor = Color.Lerp(startColor, endColor, t);
                    //Color currentColor = Color.LerpUnclamped(startColor, endColor, t);

                    colorSetter?.SetColor(currentColor);

                    yield return null;
                }

                LastColor = endColor;
            }
        }
        
    }
}
