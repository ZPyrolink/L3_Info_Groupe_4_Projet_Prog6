using System;
using UnityEngine;

namespace Taluva.Model
{
    /// <summary>
    /// Represents a chunk in the game board.
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// Gets the array of cells in the chunk.
        /// </summary>
        public Cell[] Coords { get; private set; }
        /// <summary>
        /// Gets or sets the rotation of the chunk.
        /// </summary>
        public Rotation rotation;
        /// <summary>
        /// Gets or sets the level of the chunk.
        /// </summary>
        public int Level = 1;

        /// <summary>
        /// Initializes a new instance of the Chunk class with the specified level and left/right cells.
        /// </summary>
        /// <param name="l">The level of the chunk.</param>
        /// <param name="left">The left cell of the chunk.</param>
        /// <param name="right">The right cell of the chunk.</param>
        public Chunk(int l, Cell left, Cell right)
        {
            this.Coords = new Cell[3];
            this.Coords[0] = new(Biomes.Volcano, this);
            this.Coords[1] = left;
            left.ParentChunk = this;
            this.Coords[2] = right;
            right.ParentChunk = this;
            this.Level = l;

        }
        
        /// <summary>
        /// Initializes a new instance of the Chunk class from an existing chunk.
        /// </summary>
        /// <param name="c">The existing chunk to copy.</param>
        public Chunk(Chunk c)
        {
            this.Coords = new Cell[3];
            this.Coords[0] = c.Coords[0];
            this.Coords[1] = c.Coords[1];
            c.Coords[1].ParentChunk = this;
            this.Coords[2] = c.Coords[2];
            c.Coords[2].ParentChunk = this;
            this.Level = c.Level;
        }
        

        /// <summary>
        /// Rotates the chunk.
        /// </summary>
        void RotateChunk()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the chunk can be rotated.
        /// </summary>
        /// <returns>True if the chunk can be rotated, false otherwise.</returns>
        private bool CanRotate()
        {
            throw new NotImplementedException();
        }
        
        
    }
}