using UnityEngine;

namespace Taluva.Model
{
    public class PointRotation
    {
        public readonly Vector2Int point;
        public readonly bool[] rotations;

        public PointRotation(Vector2Int point)
        {
            this.point = point;
            this.rotations = new bool[6];
            for (int i = 0; i < rotations.Length; i++)
                this.rotations[i] = false;
        }
        
        public PointRotation(Vector2Int point,Rotation rot) : this(point)
        {
            this.rotations[(int)rot] = true;

        }

        public void SetAllTrue()
        {
            for (int i = 0; i < rotations.Length; i++)
                this.rotations[i] = true;
        }

        public void AddRotation(Rotation r)
        {
            rotations[(int)r] = true;
        }
    }
}