
using System;

using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
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

    [Obsolete("Use the BiomeExt instead!")]
    public static class BiomeColorExt
    {
        public static Color GetColor(this BiomeColor bc) => ColorUtils.From((uint) bc);
    }
}