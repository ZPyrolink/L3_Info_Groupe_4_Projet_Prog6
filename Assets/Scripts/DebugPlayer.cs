using UnityEngine;

[System.Serializable]
public class DebugPlayer
{
    [SerializeField]
    private string name;

    public string Name => name;

    [SerializeField]
    private Color color;

    public Color Color => color;

    [SerializeField]
    private int[] builds;

    public int[] Builds => builds;

    public DebugPlayer(string name, Color color, int[] builds)
    {
        this.name = name;
        this.color = color;
        this.builds = builds;
    }
}