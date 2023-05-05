using UnityEngine;

namespace Utils
{
    public static class UnityStructExt
    {
        public static Vector3 With(this Vector2 v, float? x = null, float? y = null) =>
            new(x ?? v.x, y ?? v.y);
        public static Vector3 With(this Vector3 v, float? x = null, float? y = null, float? z = null) =>
            new(x ?? v.x, y ?? v.y, z ?? v.z);
    }
}