using System.Drawing;

namespace Taluva.Model;

public class Cell
{
    public Biomes ActualBiome { get; }
    public Building ActualBuildings { get; set; }
    public PlayerColor Owner { get; set; }

    public Village? actualVillage;

    public Cell(Biomes biome, int level)
    {
        this.ActualBiome = biome;
        this.ActualBuildings = Building.None;
    }

    /// <summary>
    /// <code>Playable => b;</code> reviens à écrire <code>Playable { get { return b } }</code>
    /// </summary>
    public bool IsPlayable =>
        ActualBiome != Biomes.None && ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;
      
    public bool IsBuildable => ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;

    public void Build(Building building)
    {
        this.ActualBuildings = building;
        this.actualVillage = new();
    }
    

    public bool HaveBuilding() => !(this.ActualBuildings == Building.None);
    
    
    
    
}