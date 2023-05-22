using System;
using UnityEngine;

namespace Taluva.Model
{
    public class Chunk
    {
        public Cell[] Coords { get; private set; }
        public Rotation rotation;
        public int Level = 1;

        public Chunk(int l, Cell left, Cell right)
        {
            this.Coords = new Cell[3];
            this.Coords[0] = new(Biomes.Volcano, this);
            this.Coords[1] = left;
            left.ParentCunk = this;
            this.Coords[2] = right;
            right.ParentCunk = this;
            this.Level = l;

        }
        
        public Chunk(Chunk c)
        {
            this.Coords = new Cell[3];
            this.Coords[0] = c.Coords[0];
            this.Coords[1] = c.Coords[1];
            c.Coords[1].ParentCunk = c;
            this.Coords[2] = c.Coords[2];
            c.Coords[2].ParentCunk = c;
            this.Level = c.Level;
        }
    }
}