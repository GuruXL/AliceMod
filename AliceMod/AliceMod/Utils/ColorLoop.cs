using System;
using System.Collections;
using UnityEngine;

namespace AliceMod
{
    public static class ColorLoop
    {
        public static IColorSetter colorSetter;
        public static IColorSetter BG_colorSetter;

        public static bool isRGBActive = false;
        public static bool randomColors = false;

        public static IEnumerator BG_RGBColorLoop()
        {
            yield return null;

            float t = 0f;
            while (true)
            {
                float r = Mathf.PingPong(t, 1f);
                float g = Mathf.PingPong(t + 0.33f, 1f);
                float b = Mathf.PingPong(t + 0.66f, 1f);

                Color rgbColor = new Color(r, g, b);

                BG_colorSetter?.SetColor(rgbColor);

                //Main.Logger.Log($"Color: {rgbColor}");

                yield return null;
                float duration = Mathf.Max(0.1f, Main.settings.BG_RGB_Duration);
                t += Time.deltaTime / duration;
            }
        }
        public static IEnumerator RGBColorLoop()
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
                float duration = Mathf.Max(0.1f, Main.settings.RGB_Duration);
                t += Time.deltaTime / duration;
            }
        }

        public static IEnumerator RGBRandomColorLoop()
        {
            yield return null;

            Color LastColor = Color.white;

            while (true)
            {
                Color startColor = LastColor;
                var endColor = new Color32(
                    (byte)UnityEngine.Random.Range(0, 256),
                    (byte)UnityEngine.Random.Range(0, 256),
                    (byte)UnityEngine.Random.Range(0, 256),
                    255
                );

                float t = 0;

                // transition from startColor to endColor
                while (t < 1)
                {
                    float duration = Mathf.Max(0.1f, Main.settings.RGB_Duration);
                    t += Time.deltaTime / duration;
                    Color currentColor = Color.Lerp(startColor, endColor, t);

                    colorSetter?.SetColor(currentColor);

                    yield return null;
                }

                LastColor = endColor;
            }
        }
        
    }
}
