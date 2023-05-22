using System.Collections.Generic;
using Taluva.Utils;
using UnityEngine;

namespace Taluva.Model
{
    /// <summary>
    /// Represents the different biomes in the game.
    /// </summary>
    public enum Biomes
    {
        None,
        Forest,
        Lake,
        Desert,
        Plain,
        Mountain,
        Volcano
    }

    /// <summary>
    /// Extension class for the Biomes enum providing color information for each biome.
    /// </summary>
    public static class BiomeExt
    {
        private static readonly Dictionary<Biomes, Color> Colors = new Dictionary<Biomes, Color>()
        {
            [Biomes.Plain] = ColorUtils.From(0xFFA8BA26),
            [Biomes.Forest] = ColorUtils.From(0xFF556B2F),
            [Biomes.Lake] = ColorUtils.From(0xFF97D9FF),
            [Biomes.Desert] = ColorUtils.From(0xFFC99965),
            [Biomes.Mountain] = ColorUtils.From(0xFF9D927D),
            [Biomes.Volcano] = ColorUtils.From(0xFF302E2A),
            [Biomes.None] = ColorUtils.From(0xFFFFFFFF)
        };

        /// <summary>
        /// Gets the color associated with a specific biome.
        /// </summary>
        /// <param name="bc">The biome.</param>
        /// <returns>The color associated with the biome.</returns>
        public static Color GetColor(this Biomes bc) => Colors[bc];

        /// <summary>
        /// Gets the character representation associated with a specific biome.
        /// </summary>
        /// <param name="b">The biome.</param>
        /// <returns>The character representation associated with the biome.</returns>
        public static char GetChar(this Biomes b) => b switch
        {
            Biomes.Desert => 'D',
            Biomes.Forest => 'F',
            Biomes.Lake => 'L',
            Biomes.Mountain => 'M',
            Biomes.Plain => 'P',
            Biomes.Volcano => 'V',
            _ => '\0'
        };
    }
}