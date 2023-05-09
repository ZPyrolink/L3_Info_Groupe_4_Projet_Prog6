using System;

namespace Taluva.Model
{
    public class Cell
    {
        public readonly Biomes ActualBiome;
        public Building ActualBuildings { get; set; }
        public PlayerColor Owner { get; set; }

        public Chunk ParentCunk;

        public Cell(Biomes biome, Chunk c)
        {
            this.ActualBiome = biome;
            this.ActualBuildings = Building.None;
            this.ParentCunk = c;
        }
        public Cell(Cell c)
        {
            this.ActualBiome = c.ActualBiome;
            this.ActualBuildings = c.ActualBuildings;
            this.Owner = c.Owner;
            this.ParentCunk = c.ParentCunk;
        }

        public Cell(Biomes biome)
        {
            this.ActualBiome = biome;
            this.ActualBuildings = Building.None;
        }
        
        public bool IsBuildable =>
            ActualBiome != Biomes.None && ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;

        public void Build(Building building)
        {
            this.ActualBuildings = building;
        }

        public override string ToString() => ActualBiome switch
        {
            Biomes.Desert => "D",
            Biomes.Forest => "F",
            Biomes.Lake => "L",
            Biomes.Mountain => "M",
            Biomes.Plain => "P",
            Biomes.Volcano => "V",
            _ => ""
        };

        public bool ContainsBuilding() => ActualBuildings != Building.None;
    }
}