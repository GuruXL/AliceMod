using UnityEngine;
using RapidGUI;

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
    }
}
