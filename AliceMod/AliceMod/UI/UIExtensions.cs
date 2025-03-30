using UnityEngine;
using RapidGUI;
using System.Reflection.Emit;
using RootMotion.Dynamics;

namespace AliceMod
{
    public static class UIextensions
    {
        private const string white = "#e6ebe8";
        private const string LightBlue = "#30e2e6";
        private static string TabColor;

        public static string TabColorSwitch(UItab Tab)
        {
            switch (Tab.isClosed)
            {
                case true:
                    TabColor = white;
                    break;

                case false:
                    TabColor = LightBlue;
                    break;
            }
            return TabColor;
        }
        public static void TabFontSwitch(UItab Tab)
        {
            switch (!Tab.isClosed)
            {
                case true:
                    Tab.font = Tab.font + 2;
                    break;

                case false:
                    Tab.font = Tab.font - 2;
                    break;
            }
        }
        public static Color ButtonColorSwitch(bool toggle)
        {
            if (toggle)
            {
                return Color.cyan;
            }
            else
            {
                return Color.white;
            }
        }
        public static void CenteredLabel(string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label($"<i><b>{label}</b></i>", GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void ColorBox(Color color, float width = 60, float height = 10)
        {
            //Color newColor = new Color(Mathf.GammaToLinearSpace(color.r), Mathf.GammaToLinearSpace(color.g), Mathf.GammaToLinearSpace(color.b));
            Color newColor = new Color(Mathf.LinearToGammaSpace(color.r), Mathf.LinearToGammaSpace(color.g), Mathf.LinearToGammaSpace(color.b));
            Texture2D colorTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            colorTexture.SetPixel(0, 0, newColor);
            colorTexture.Apply();
            GUIStyle colorStyle = new GUIStyle
            {
                normal = { background = colorTexture }
            };
            GUILayout.BeginHorizontal();
            {
                GUI.backgroundColor = newColor;
                GUILayout.Box("", colorStyle, GUILayout.Width(width), GUILayout.Height(height));
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
           
        }
    }
}
