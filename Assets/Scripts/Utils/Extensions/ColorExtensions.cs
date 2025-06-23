using UnityEngine;

namespace ToB
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        public static Color32 WithAlpha(this Color32 color, byte alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}
