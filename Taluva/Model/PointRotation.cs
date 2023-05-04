using System;
using System.Drawing;
namespace Taluva.Model
{
    public class PointRotation
    {
        public readonly Point point;
        public readonly Rotation rotation;

        public PointRotation (Point point, Rotation rotation)
        {
            this.point = point;
            this.rotation = rotation;
        }
    }
}
