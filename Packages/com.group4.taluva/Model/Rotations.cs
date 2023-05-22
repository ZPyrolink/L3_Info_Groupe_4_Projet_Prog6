using static Taluva.Model.Rotation;

namespace Taluva.Model
{
    /// <summary>
    /// Represents the different rotational directions.
    /// </summary>
    public enum Rotation
    {
        /// <summary>
        /// North direction (0 degrees).
        /// </summary>
        N = 0,
        /// <summary>
        /// Northeast direction (1/6th of a circle clockwise).
        /// </summary>
        NE = 1,
        /// <summary>
        /// Southeast direction (2/6th of a circle clockwise).
        /// </summary>
        SE = 2,
        /// <summary>
        /// South direction (3/6th of a circle clockwise).
        /// </summary>
        S = 3,
        /// <summary>
        /// Southwest direction (4/6th of a circle clockwise).
        /// </summary>
        SW = 4,
        /// <summary>
        /// Northwest direction (5/6th of a circle clockwise).
        /// </summary>
        NW = 5
    }

    /// <summary>
    /// Provides extension methods for the <see cref="Rotation"/> enum.
    /// </summary>
    public static class RotationExt
    {
        /// <summary>
        /// Converts the specified degree value to its corresponding <see cref="Rotation"/>.
        /// </summary>
        /// <param name="degree">The degree value.</param>
        /// <returns>The <see cref="Rotation"/> value.</returns>
        public static Rotation Of(float degree) => degree switch
        {
            270 => SW,
            330 => NW,
            30 => N,
            90 => NE,
            150 => SE,
            210 => S,

            _ => throw new()
        };

        /// <summary>
        /// Converts the rotation value to its corresponding degree value.
        /// </summary>
        /// <param name="rot">The rotation value.</param>
        /// <returns>The degree value.</returns>
        public static float YDegree(this Rotation rot) => rot switch
        {
            SW => 270,
            NW => 330,
            N => 30,
            NE => 90,
            SE => 150,
            S => 210,

            _ => throw new()
        };
    }
}
