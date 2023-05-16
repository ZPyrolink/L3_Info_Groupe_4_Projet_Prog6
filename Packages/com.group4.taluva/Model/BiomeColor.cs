using System;

using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
    /// <summary>
    /// [Obsolete] Use the Biome enum instead!
    /// </summary>
    [Obsolete("Use the Biome enum instead!")]
    public enum BiomeColor : uint
    {
        Plain = 0xFFA8BA26,
        Forest = 0xFF556B2F,
        Lake = 0xFF97D9FF,
        Desert = 0xFFC99965,
        Mountain = 0xFF9D927D,
        Volcano = 0xFF302E2A,
        None = 0xFFFFFFFF
    }

    /// <summary>
    /// [Obsolete] Use the BiomeExt instead!
    /// </summary>
    [Obsolete("Use the BiomeExt instead!")]
    public static class BiomeColorExt
    {
        /// <summary>
        /// [Obsolete] Use the GetColor() method from the BiomeExt class instead!
        /// </summary>
        [Obsolete("Use the GetColor() method from the BiomeExt class instead!")]
        public static Color GetColor(this BiomeColor bc) => ColorUtils.From((uint)bc);
    }
}