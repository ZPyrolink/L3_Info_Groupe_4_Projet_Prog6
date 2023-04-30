namespace Taluva.Model;

public class Cell
{
    private Biomes actualBiome;
    public Building ActualBuildings { get; private set; }
    private int level;
    private PlayerColor owner;
    //Texture BinomeVisual;
    private Village actualVillage;

    public Cell(Biomes biome, int level)
    {
        this.actualBiome = biome;
        this.level = level;
        this.actualVillage = new Village();
    }

    public bool IsPlayable()
    {
        throw new NotImplementedException();
    }

    public bool IsBuildable()
    {
        if (actualBiome == Biomes.Volcano || ActualBuildings != Building.None)
            return false;
        return true;
    }
}