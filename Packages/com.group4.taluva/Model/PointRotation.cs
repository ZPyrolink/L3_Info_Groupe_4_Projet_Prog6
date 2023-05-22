using System;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Taluva.Model
{
    /// <summary>
    /// Represents a combination of a position and the available rotations for a chunk placement.
    /// </summary>
    public class PointRotation
    {
        /// <summary>
        /// The position of the chunk placement.
        /// </summary>
        public Vector2Int Point { get; }
        [Obsolete("Use Point instead")] public Vector2Int point => Point;
        /// <summary>
        /// An array indicating the available rotations for the chunk placement.
        /// </summary>
        public bool[] Rotations { get; }
        [Obsolete("Use Rotation instead")] public bool[] rotations => Rotations;

        /// <summary>
        /// Creates a new instance of the PointRotation class with the specified position.
        /// </summary>
        /// <param name="point">The position of the chunk placement.</param>
        public PointRotation(Vector2Int point)
        {
            Point = point;
            Rotations = new bool[6];
            for (int i = 0; i < rotations.Length; i++)
                this.rotations[i] = false;
        }

        /// <summary>
        /// Creates a new instance of the PointRotation class with the specified position and rotations.
        /// </summary>
        /// <param name="point">The position of the chunk placement.</param>
        /// <param name="rot">The array indicating the available rotations.</param>
        public PointRotation(Vector2Int point, bool[] rot) : this(point)
        {
            Rotations = rot;
        }
        /// <summary>
        /// Creates a new instance of the PointRotation class with the specified position and rotation.
        /// </summary>
        /// <param name="point">The position of the chunk placement.</param>
        /// <param name="rot">The rotation available for the placement.</param>
        public PointRotation(Vector2Int point, Rotation rot) : this(point)
        {
            rotations[(int) rot] = true;
        }

        public bool RotationEquals(PointRotation rotations) => !this.rotations
            // On ne garde que les rotations qui sont différentes
            .Where((t, i) => t != rotations.rotations[i])
            // On regardes s'il en existe un
            .Any();
        /// <summary>
        /// Checks if there are any available rotations.
        /// </summary>
        /// <returns>True if there are available rotations, false otherwise.</returns>
        public bool HaveRotation() => rotations.Any(b => b);

        /// <summary>
        /// Sets all rotations to true.
        /// </summary>
        public void SetAllTrue()
        {
            for (int i = 0; i < rotations.Length; i++)
                rotations[i] = true;
        }

        /// <summary>
        /// Adds a rotation to the available rotations.
        /// </summary>
        /// <param name="r">The rotation to add.</param>
        public void AddRotation(Rotation r)
        {
            rotations[(int) r] = true;
        }
        /// <summary>
        /// Returns a string representation of the available rotations.
        /// </summary>
        /// <returns>A string representing the available rotations.</returns>
        [Obsolete("Use ToString() instead")]
        public string RotationString() => ToString();

        public override string ToString()
        {
            StringBuilder sb = new();
            for (int i = 0; i < rotations.Length; i++)
                if (rotations[i])
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