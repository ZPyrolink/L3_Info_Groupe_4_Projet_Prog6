using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
    // ToDo: Move it into Player
    /// <summary>
    /// Represents the available colors for players in the game.
    /// </summary>
    public enum PlayerColor : uint
    {
        /// <summary>
        /// The color red.
        /// </summary>
        Red = 0xFFD20F28,
        /// <summary>
        /// The color green.
        /// </summary>
        Green = 0xFF6C935E,
        /// <summary>
        /// The color blue.
        /// </summary>
        Blue = 0xFF4855B7,
        /// <summary>
        /// The color yellow.
        /// </summary>
        Yellow = 0xFFFFD700,
    }

    /// <summary>
    /// Extension methods for the PlayerColor enum.
    /// </summary>
    public static class PlayerColorExt
    {
        /// <summary>
        /// Gets the corresponding Color for the PlayerColor value.
        /// </summary>
        /// <param name="pc">The PlayerColor value.</param>
        /// <returns>The Color value corresponding to the PlayerColor.</returns>
        public static Color GetColor(this PlayerColor pc) => ColorUtils.From((uint)pc);
    }
}