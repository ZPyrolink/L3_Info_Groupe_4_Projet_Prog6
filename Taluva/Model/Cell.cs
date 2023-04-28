using System;
using System.Drawing;

public class Cell
{
    Biomes ActualBiome;
    public Building ActualBuildings;
    int Level;
    PlayerColor Owner;
    //Texture BinomeVisual;
    Village ActualVillage;

    public Cell(Biomes biome, int level)
    {
        this.ActualBiome = biome;
        this.Level = level;
        this.ActualVillage = new Village();
    }

    public bool IsPlayable()
    {

    }

    public bool IsBuildable()
    {
        if (ActualBiome == Biomes.Volcano || ActualBuildings != Building.None)
            return false;
        return true;
    }
}