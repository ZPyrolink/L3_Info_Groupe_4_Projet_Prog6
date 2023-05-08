using UnityEngine;

namespace Taluva.Utils
{
    public static class ColorUtils
    {
        private const int _ALPHA_SHIFT = 24;
        private const int _RED_SHIFT = 16;
        private const int _GREEN_SHIFT = 8;
        private const int _BLUE_SHIFT = 0;
        private static float ByteToFloat(byte b) => Mathf.InverseLerp(byte.MinValue, byte.MaxValue, b);
        private static byte FloatToByte(float b) => (byte) Mathf.Lerp(byte.MinValue, byte.MaxValue, b);

        public static Color From(int c) => new()
        {
            a = ByteToFloat((byte) (c >> _ALPHA_SHIFT & 0xFF)),
            r = ByteToFloat((byte) (c >> _RED_SHIFT & 0xFF)),
            g = ByteToFloat((byte) (c >> _GREEN_SHIFT & 0xFF)),
            b = ByteToFloat((byte) (c >> _BLUE_SHIFT & 0xFF))
        };

        public static Color From(uint c) => From((int) c);

        public static int ToInt(this Color c) => FloatToByte(c.a) << _ALPHA_SHIFT | FloatToByte(c.r) << _RED_SHIFT |
                                                 FloatToByte(c.g) << _GREEN_SHIFT | FloatToByte(c.b) << _BLUE_SHIFT;
    }
}