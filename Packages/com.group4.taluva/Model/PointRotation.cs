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
        public readonly Vector2Int point;

        /// <summary>
        /// An array indicating the available rotations for the chunk placement.
        /// </summary>
        public readonly bool[] rotations;

        /// <summary>
        /// Creates a new instance of the PointRotation class with the specified position.
        /// </summary>
        /// <param name="point">The position of the chunk placement.</param>
        public PointRotation(Vector2Int point)
        {
            this.point = point;
            this.rotations = new bool[6];
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
            rotations = rot;
        }

        /// <summary>
        /// Creates a new instance of the PointRotation class with the specified position and rotation.
        /// </summary>
        /// <param name="point">The position of the chunk placement.</param>
        /// <param name="rot">The rotation available for the placement.</param>
        public PointRotation(Vector2Int point, Rotation rot) : this(point)
        {
            this.rotations[(int)rot] = true;
        }

        /// <summary>
        /// Checks if the rotations of this PointRotation are equal to another PointRotation.
        /// </summary>
        /// <param name="rotations">The PointRotation to compare with.</param>
        /// <returns>True if the rotations are equal, false otherwise.</returns>
        public bool RotationEquals(PointRotation rotations)
        {
            for (int i = 0; i < this.rotations.Length; i++)
            {
                if (this.rotations[i] != rotations.rotations[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if there are any available rotations.
        /// </summary>
        /// <returns>True if there are available rotations, false otherwise.</returns>
        public bool HaveRotation()
        {
            foreach (bool b in rotations)
            {
                if (b)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Sets all rotations to true.
        /// </summary>
        public void SetAllTrue()
        {
            for (int i = 0; i < rotations.Length; i++)
                this.rotations[i] = true;
        }

        /// <summary>
        /// Adds a rotation to the available rotations.
        /// </summary>
        /// <param name="r">The rotation to add.</param>
        public void AddRotation(Rotation r)
        {
            rotations[(int)r] = true;
        }

        /// <summary>
        /// Returns a string representation of the available rotations.
        /// </summary>
        /// <returns>A string representing the available rotations.</returns>
        public string RotationString()
        {
            string s = "";
            for (int i = 0; i < rotations.Length; i++)
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