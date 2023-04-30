using System.Drawing;

public enum PlayerColor
{
    Red = unchecked((int) 0xFFD20F28),
    Green = unchecked((int) 0xFF6C935E),
    Blue = unchecked((int) 0xFF4855B7),
    Yellow = unchecked((int) 0xFFFFD700)
}

public static class PlayerColorExt
{
    public static Color GetColor(this PlayerColor pc) => Color.FromArgb((int) pc);
}