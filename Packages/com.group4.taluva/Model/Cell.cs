namespace Taluva.Model
{
    public class Cell
    {
        public Biomes ActualBiome { get; }
        public Building ActualBuildings { get; set; }
        public PlayerColor Owner { get; set; }

        public Chunk ParentChunk;

        public Cell(Biomes biome, Chunk c) : this(biome)
        {
            ParentChunk = c;
        }

        public Cell(Cell c) : this(c.ActualBiome, new(c.ParentChunk))
        {
            ActualBuildings = c.ActualBuildings;
            Owner = c.Owner;
        }

        public Cell(Biomes biome)
        {
            ActualBiome = biome;
            ActualBuildings = Building.None;
        }

        public bool IsBuildable => !(ActualBiome is Biomes.None or Biomes.Volcano || ActualBuildings != Building.None);

        public void Build(Building building) => ActualBuildings = building;

        public override string ToString() => ActualBiome.GetChar().ToString();

        public bool ContainsBuilding() => ActualBuildings != Building.None;
    }
}