using System.Drawing;

namespace Taluva.Model;

public class Cell
{
    public Biomes ActualBiome { get; }
    public Building ActualBuildings { get; set; }
    private int level;
    public PlayerColor Owner { get; set; }
    private Village actualVillage;
    public Point Coord { get; private set; }

    public Cell(Biomes biome, int level, Point coord)
    {
        this.ActualBiome = biome;
        this.level = level;
        this.Coord = coord;
        this.actualVillage = new Village();
        this.ActualBuildings = Building.None;
    }

    /// <summary>
    /// <code>Playable => b;</code> reviens à écrire <code>Playable { get { return b } }</code>
    /// </summary>
    public bool Playable =>
        ActualBiome != Biomes.None && ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;

    [Obsolete($"Use the {nameof(Playable)} property instead")]
    public bool IsPlayable()
    {
        return ActualBiome != Biomes.None && ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;
    }

    public bool Buildable => ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;

    [Obsolete($"Use the {nameof(Buildable)} property instead")]
    public bool IsBuildable()
    {
        return ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;
    }
}