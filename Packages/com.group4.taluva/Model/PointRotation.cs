using UnityEngine;

namespace Taluva.Model
{
    public class PointRotation
    {
        public readonly Vector2Int point;
        public readonly Rotation rotation;

        public PointRotation(Vector2Int point, Rotation rotation)
        {
            this.point = point;
            this.rotation = rotation;
        }
    }
}