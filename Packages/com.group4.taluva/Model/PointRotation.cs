using System;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Taluva.Model
{
    public class PointRotation
    {
        public Vector2Int Point { get; }
        [Obsolete("Use Point instead", true)]
        public Vector2Int point => Point;
        public bool[] Rotations { get; }
        [Obsolete("Use Rotation instead", true)]
        public bool[] rotations => Rotations;

        public PointRotation(Vector2Int point)
        {
            Point = point;
            Rotations = new bool[6];
            for (int i = 0; i < Rotations.Length; i++)
                this.Rotations[i] = false;
        }

        public PointRotation(Vector2Int point, bool[] rot) : this(point)
        {
            Rotations = rot;
        }

        public PointRotation(Vector2Int point, Rotation rot) : this(point)
        {
            Rotations[(int) rot] = true;
        }

        public bool RotationEquals(PointRotation rotations) => !this.Rotations
            // On ne garde que les rotations qui sont différentes
            .Where((t, i) => t != rotations.Rotations[i])
            // On regardes s'il en existe un
            .Any();

        public bool HaveRotation() => Rotations.Any(b => b);

        public void SetAllTrue()
        {
            for (int i = 0; i < Rotations.Length; i++)
                Rotations[i] = true;
        }

        public void AddRotation(Rotation r)
        {
            Rotations[(int) r] = true;
        }

        [Obsolete("Use ToString() instead", true)]
        public string RotationString() => ToString();

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < Rotations.Length; i++)
                if (Rotations[i])
                    sb.Append((Rotation) i switch
                    {
                        Rotation.N => sb.Append("N"),
                        Rotation.S => sb.Append("S"),
                        Rotation.NE => sb.Append("NE"),
                        Rotation.NW => sb.Append("NW"),
                        Rotation.SE => sb.Append("SE"),
                        Rotation.SW => sb.Append("SW")
                    }).Append(' ');

            return sb.ToString();
        }
    }
}