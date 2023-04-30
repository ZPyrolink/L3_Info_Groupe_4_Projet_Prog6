using System.Drawing;

namespace Taluva.Model;

public class Cell
{
    public Biomes ActualBiome;
    public Building ActualBuildings;
    private int Level;
    public PlayerColor Owner { get;  set; }
    private Village ActualVillage;
    public Point coord { get; private set; }

    public Cell(Biomes biome, int level, Point coord)
    {
        this.ActualBiome = biome;
        this.Level = level;
        this.coord = coord;
        this.ActualVillage = new Village();
        this.ActualBuildings = Building.None;
    }

    public bool IsPlayable()
    {
        return ActualBiome != Biomes.None && ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;
    }

    public bool IsBuildable()
    {
        return ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;
    }
}