using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
    // ToDo: Move it into Player
    public enum PlayerColor : uint
    {
        Red = 0xFFD20F28,
        Green = 0xFF00FF00,
        Blue = 0xFF2E00FF,
        Yellow = 0xFFFFD700,
    }

    public static class PlayerColorExt
    {
        public static Color GetColor(this PlayerColor pc) => ColorUtils.From((uint) pc);
    }
}