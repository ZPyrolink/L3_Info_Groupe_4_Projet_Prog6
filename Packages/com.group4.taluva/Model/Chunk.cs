using System;
using System.Collections.Generic;

namespace Taluva.Model
{
    public class Chunk : ICloneable
    {
        public Cell[] Coords { get; }
        public Rotation Rotation { get; set; }
        [Obsolete("Use Rotation instead")]
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

        public Chunk(Chunk c)
        {
            Coords = new Cell[3];
            Coords[0] = new(Biomes.Volcano, this);
            Coords[1] = new Cell(c.Coords[1].ActualBiome);
            Coords[1].Owner = c.Coords[1].Owner;
            Coords[1].ActualBuildings = c.Coords[1].ActualBuildings;
            Coords[1].ParentChunk = this;
            Coords[2] = new Cell(c.Coords[2].ActualBiome);
            Coords[2].Owner = c.Coords[2].Owner;
            Coords[2].ActualBuildings = c.Coords[2].ActualBuildings;
            Coords[2].ParentChunk = this;
            Level = c.Level;
        }

        public Object Clone()
        {
            return new Chunk(this);
        }

        public override string ToString() => $"{Coords[0].ActualBiome}, {Coords[1].ActualBiome}, {Coords[2].ActualBiome}";

        public static string ListToString(IEnumerable<Chunk> l) => string.Join("\n", l);
    }
}