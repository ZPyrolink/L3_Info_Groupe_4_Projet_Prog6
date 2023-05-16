using System;

namespace Taluva.Model
{
    /// <summary>
    /// Represents the different rotational directions.
    /// </summary>
    public enum Rotation
    {
        N = 0,
        NE = 1,
        SE = 2,
        S = 3,
        SW = 4,
        NW = 5
    }

    /// <summary>
    /// Provides extension methods for the <see cref="Rotation"/> enum.
    /// </summary>
    public static class RotationExt
    {
        /// <summary>
        /// Converts the rotation value to its corresponding degree.
        /// </summary>
        /// <param name="value">The rotation value.</param>
        /// <returns>The degree value.</returns>
        public static float Degree(this Rotation value) =>
            360f / Enum.GetValues(typeof(Rotation)).Length * (int)value;
    }
}