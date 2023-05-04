using System;
using System.Drawing;
namespace Taluva.Model
{
    public class PointRotation
    {
        Point point;
        Rotation rotation;

        public PointRotation (Point point, Rotation rotation)
        {
            this.point = point;
            this.rotation = rotation;
        }
    }
}
