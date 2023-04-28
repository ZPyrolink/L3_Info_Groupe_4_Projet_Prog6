using System.Drawing;

public enum PlayerColor : int
{
    Red = 0xD20F28,
    Green = 0x6C935E,
    Blue = 0x4855B7,
    Yellow = 0xFFD700
}

public static class PlayerColorExt
{
    public static Color getColor(this PlayerColor pc) => Color.FromArgb((int)pc);
}