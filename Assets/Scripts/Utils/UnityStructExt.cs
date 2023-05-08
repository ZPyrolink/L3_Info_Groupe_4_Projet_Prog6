using UnityEngine;

namespace Utils
{
    public static class UnityStructExt
    {
        public static Vector2 With(this Vector2 v, float? x = null, float? y = null) =>
            new(x ?? v.x, y ?? v.y);

        public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null) =>
            new(x ?? v.x, y ?? v.y, z ?? v.z);

        public static Color With(this Color c, float? r = null, float? g = null, float? b = null, float? a = null) =>
            new(r ?? c.r, g ?? c.g, b ?? c.b, a ?? c.a);
    }
}