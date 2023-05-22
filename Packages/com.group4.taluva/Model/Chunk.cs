using System;
using System.Collections.Generic;

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
        public Cell[] Coords { get; }
        /// <summary>
        /// Gets or sets the rotation of the chunk.
        /// </summary>
        public Rotation Rotation { get; set; }
        [Obsolete("Use Rotation instead")]
        public Rotation rotation
        {
            get => Rotation;
            set => Rotation = value;
        }
        /// <summary>
        /// Gets or sets the level of the chunk.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Initializes a new instance of the Chunk class with the specified level and left/right cells.
        /// </summary>
        /// <param name="l">The level of the chunk.</param>
        /// <param name="left">The left cell of the chunk.</param>
        /// <param name="right">The right cell of the chunk.</param>
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

        
        /// <summary>
        /// Initializes a new instance of the Chunk class from an existing chunk.
        /// </summary>
        /// <param name="c">The existing chunk to copy.</param>
        public Chunk(Chunk c)
        {
            Coords = new Cell[3];
            Coords[0] = c.Coords[0];
            Coords[1] = c.Coords[1];
            c.Coords[1].ParentChunk = c;
            Coords[2] = c.Coords[2];
            c.Coords[2].ParentChunk = c;
            Level = c.Level;
        }

        public override string ToString() => $"{Coords[0].ActualBiome}, {Coords[1].ActualBiome}, {Coords[2].ActualBiome}";

        public static string ListToString(IEnumerable<Chunk> l) => string.Join("\n", l);
    }
}