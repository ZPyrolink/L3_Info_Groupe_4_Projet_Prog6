using System.Collections.Generic;

using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
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

    public static class BiomeExt
    {
        private static readonly Dictionary<Biomes, Color> Colors = new()
        {
            [Biomes.Plain] = ColorUtils.From(0xFFA8BA26),
            [Biomes.Forest] = ColorUtils.From(0xFF556B2F),
            [Biomes.Lake] = ColorUtils.From(0xFF97D9FF),
            [Biomes.Desert] = ColorUtils.From(0xFFC99965),
            [Biomes.Mountain] = ColorUtils.From(0xFF9D927D),
            [Biomes.Volcano] = ColorUtils.From(0xFF302E2A),
            [Biomes.None] = ColorUtils.From(0xFFFFFFFF)
        };

        public static Color GetColor(this Biomes bc) => Colors[bc];
    }
}