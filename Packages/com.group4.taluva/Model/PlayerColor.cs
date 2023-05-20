using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
    // ToDo: Move it into Player
    public enum PlayerColor : uint
    {
        Red = 0xFFD20F28,
        Green = 0xFF6C935E,
        Blue = 0xFF4855B7,
        Yellow = 0xFFFFD700,
    }

    public static class PlayerColorExt
    {
        public static Color GetColor(this PlayerColor pc) => ColorUtils.From((uint) pc);
    }
}