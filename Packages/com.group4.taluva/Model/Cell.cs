namespace Taluva.Model
{
    public class Cell
    {
        public readonly Biomes ActualBiome;
        public Building ActualBuildings { get; set; }
        public PlayerColor Owner { get; set; }

        public Chunk parentCunk;

        public Cell(Biomes biome, Chunk c)
        {
            this.ActualBiome = biome;
            this.ActualBuildings = Building.None;
            this.parentCunk = c;
        }

        public Cell(Biomes biome)
        {   
            this.ActualBiome = biome;
            this.ActualBuildings = Building.None;
        }

        /// <summary>
        /// <code>Playable => b;</code> reviens à écrire <code>Playable { get { return b } }</code>
        /// </summary>
        public bool IsBuildable =>
            ActualBiome != Biomes.None && ActualBiome != Biomes.Volcano && ActualBuildings == Building.None;


        public void Build(Building building)
        {
            this.ActualBuildings = building;
        }
        
        public override string ToString()
        {
            switch (ActualBiome)
            {
                case Biomes.Desert:
                    return "D";
                case Biomes.Forest:
                    return "F";
                case Biomes.Lake:
                    return "L";
                case Biomes.Mountain:
                    return "M";
                case Biomes.Plain:
                    return "P";
                case Biomes.Volcano:
                    return "V";
                default:
                    return "";
            }
        }

        public bool HaveBuilding() => this.ActualBuildings != Building.None;




    }
}