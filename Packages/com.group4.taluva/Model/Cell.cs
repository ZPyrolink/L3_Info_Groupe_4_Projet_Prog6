using System;
using UnityEngine;

namespace Taluva.Model
{
    public class Cell : ICloneable
    {
        public Biomes CurrentBiome { get; }
        public Building CurrentBuildings { get; set; }
        public Player.Color Owner { get; set; }

        public Chunk ParentChunk { get; set; }

        public Vector2Int position { get; set; }

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
        public object Clone()
        {
            return new Cell(this);
        }

        public bool ContainsBuilding() => CurrentBuildings != Building.None;
    }
}