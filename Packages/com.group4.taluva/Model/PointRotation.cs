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

        public PointRotation(Vector2Int point, bool[] rot) : this(point)
        {
            rotations = rot;
        }
        
        public PointRotation(Vector2Int point,Rotation rot) : this(point)
        {
            this.rotations[(int)rot] = true;

        }

        public bool RotationEquals(PointRotation rotations)
        {
            for(int i = 0;i < this.rotations.Length; i++) {
                if (this.rotations[i] != rotations.rotations[i]) 
                    return false;
            }
            return true;
        }

        public bool HaveRotation()
        {
            foreach(bool b in rotations)
                if(b)
                    return true;
            return false;
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

        public string RotationString()
        {
            string s = "";
            for(int i = 0; i < rotations.Length; i++)
                if (rotations[i]) {
                    switch ((Rotation)i) {
                        case Rotation.N:
                            s += "N";
                            break;
                        case Rotation.S:
                            s += "S";
                            break;
                        case Rotation.NE:
                            s += "NE";
                            break;
                        case Rotation.NW:
                            s += "NW";
                            break;
                        case Rotation.SE:
                            s += "SE";
                            break;
                        case Rotation.SW:
                            s += "SW";
                            break;
                    }
                    s += " ";
                }
            return s;    
        }
    }
}