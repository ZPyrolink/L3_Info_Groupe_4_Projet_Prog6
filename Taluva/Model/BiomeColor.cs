using System.Drawing;

namespace Taluva.Model;

public enum BiomeColor
{
    Plain = unchecked((int)0xFFA8BA26),
    Forest = unchecked((int)0xFF556B2F),
    Lake = unchecked((int)0xFF97D9FF),
    Desert = unchecked((int)0xFFC99965),
    Mountain = unchecked((int)0xFF9D927D),
    Volcano = unchecked((int)0xFF302E2A),
    None = unchecked((int)0xFFFFFFFF)
}

public static class BiomeColorExt
{
    public static Color GetColor(this BiomeColor bc) => Color.FromArgb((int)bc);
}