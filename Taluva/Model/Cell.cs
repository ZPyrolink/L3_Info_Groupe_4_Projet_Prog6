using System;
using System.Drawing;

public class Cell
{
    public Biomes ActualBiome;
    public Building ActualBuildings;
    int Level;
    public PlayerColor Owner { get;  set; }
    Village ActualVillage;
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
        if (ActualBiome == Biomes.Volcano || ActualBuildings != Building.None)
            return false;
        return true;
    }
    
}