namespace Taluva.Model
{
    public class Cell
    {
        public Biomes CurrentBiome { get; }
        public Building CurrentBuildings { get; set; }
        public PlayerColor Owner { get; set; }

        public Chunk ParentChunk;

        public Cell(Biomes biome, Chunk c) : this(biome)
        {
            ParentChunk = c;
        }

        public Cell(Cell c) : this(c.CurrentBiome, new(c.ParentChunk))
        {
            CurrentBuildings = c.CurrentBuildings;
            Owner = c.Owner;
        }

        public Cell(Biomes biome)
        {
            CurrentBiome = biome;
            CurrentBuildings = Building.None;
        }

        public bool IsBuildable => !(CurrentBiome is Biomes.None or Biomes.Volcano || CurrentBuildings != Building.None);

        public void Build(Building building) => CurrentBuildings = building;

        public override string ToString() => CurrentBiome.GetChar().ToString();

        public bool ContainsBuilding() => CurrentBuildings != Building.None;
    }
}