using UnityEngine;

namespace Utils
{
    public static class ColorUtils
    {
        private static float ByteToFloat(byte b) => Mathf.InverseLerp(byte.MinValue, byte.MaxValue, b);
        private static byte GetLsb(int value, int offset) => (byte) (value >> (offset * 8) & byte.MaxValue);
        public static Color FromInt(int c) => new()
        {
            b = ByteToFloat(GetLsb(c, 0)),
            g = ByteToFloat(GetLsb(c, 1)),
            r = ByteToFloat(GetLsb(c, 2)),
            a = ByteToFloat(GetLsb(c, 3))
        };

        public static Color FromInt(uint c) => FromInt((int) c);
    }
}