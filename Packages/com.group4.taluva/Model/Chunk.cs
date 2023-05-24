using System;
using System.Collections.Generic;

namespace Taluva.Model
{
    public class Chunk : ICloneable
    {
        public Cell[] Coords { get; }
        public Rotation Rotation { get; set; }
        [Obsolete("Use Rotation instead", true)]
        public Rotation rotation
        {
            get => Rotation;
            set => Rotation = value;
        }
        public int Level { get; set; }

        public Chunk(int l, Cell left, Cell right)
        {
            Coords = new Cell[3];
            Coords[0] = new(Biomes.Volcano, this);
            Coords[1] = left;
            left.ParentChunk = this;
            Coords[2] = right;
            right.ParentChunk = this;
            Level = l;
        }

        public Chunk(Chunk c, bool building = true)
        {
            Coords = new Cell[3];
            Coords[0] = new(Biomes.Volcano, this);
            Coords[1] = new(c.Coords[1].CurrentBiome) { ParentChunk = this };
            Coords[2] = new(c.Coords[2].CurrentBiome) { ParentChunk = this };
            if (building)
            {
                Coords[1].Owner = c.Coords[1].Owner;
                Coords[1].CurrentBuildings = c.Coords[1].CurrentBuildings;
                Coords[2].Owner = c.Coords[2].Owner;
                Coords[2].CurrentBuildings = c.Coords[2].CurrentBuildings;
            }

            Level = c.Level;
        }

        public object Clone()
        {
            return new Chunk(this);
        }

        public override string ToString() =>
            $"{Coords[0].CurrentBiome}, {Coords[1].CurrentBiome}, {Coords[2].CurrentBiome}";

        public static string ListToString(IEnumerable<Chunk> l) => string.Join("\n", l);
    }
}