using System;

namespace Taluva.Model
{
    public class Chunk
    {
        public Cell[] Coords { get; private set; }
        public Rotation rotation;
        public readonly int Level = 1;

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

        public Chunk(Cell left, Cell right) : this(right.ParentCunk.Level + 1, left, right) { }
        

        void RotateChunk()
        {
            throw new NotImplementedException();
        }

        private bool CanRotate()
        {
            throw new NotImplementedException();
        }
    }
}