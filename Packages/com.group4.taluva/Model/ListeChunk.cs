using System;
using System.Collections.Generic;

namespace Taluva.Model
{
    public static class ListeChunk
    {   
        private static readonly List<Chunk> Chunks = new List<Chunk>();

        /// <summary>
        /// Indexes: <see cref="Biomes"/> - 1
        /// </summary>
        private static readonly int[,] OcurrenceMatrix =
        {
            { 1, 2, 4, 6, 2 },
            { 1, 1, 1, 1, 1 },
            { 4, 1, 1, 2, 2 },
            { 5, 1, 2, 1, 2 },
            { 2, 1, 1, 2, 1 }
        };

        /// <summary>
        /// Get a new Pile containing the chunks.
        /// </summary>
        public static Pile<Chunk> Pile => new Pile<Chunk>(Chunks.ToArray());

        // public static int Count => Chunks.Count;
        
        /// <summary>
        /// Creates all the chunks and stores them in a list.
        /// </summary>
        static ListeChunk()
        {
            for (int i = 0; i < OcurrenceMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < OcurrenceMatrix.GetLength(1); j++)
                {
                    for (int reps = 0; reps < OcurrenceMatrix[i, j]; reps++)
                    {
                        Cell c1 = new Cell((Biomes)(i + 1));
                        Cell c2 = new Cell((Biomes)(j + 1));
                        Chunk c = new Chunk(1, c1, c2);
                        Chunks.Add(c);
                    }
                }
            }
        }

        /// <summary>
        /// Resets the chunks of a pile.
        /// </summary>
        /// <param name="pileChunk">The pile to reset.</param>
        public static void ResetChunk(Pile<Chunk> pileChunk)
        {
            Stack<Chunk> stack = new Stack<Chunk>();
            int nb = pileChunk.NbKeeping;

            for(int i = 0; i < nb; i++)
            {
                Chunk chunk = pileChunk.Content.Pop();
                chunk.Coords[1].ActualBuildings = Building.None;
                chunk.Coords[2].ActualBuildings = Building.None;
                chunk.Level = 1;

                stack.Push(chunk);
            }

            pileChunk.Content.Clear();

            for(int i = 0; i < nb; i++)
            {
                Chunk chunk = stack.Pop();
                pileChunk.Content.Push(chunk);
            }
        }

        /// <summary>
        /// Prints the string representation of a chunk.
        /// </summary>
        /// <param name="c">The chunk to print.</param>
        [Obsolete("Use Chunk.ToString() instead")]
        public static void PrintChunkToString(Chunk c) => Console.WriteLine(c);

        /// <summary>
        /// Prints the string representation of a list of chunks.
        /// </summary>
        /// <param name="l">The list of chunks to print.</param>
        [Obsolete("Use Chunk.ListToString(List<Chunk>) instead")]
        public static void PrintChunkList(List<Chunk> l) => Console.WriteLine(Chunk.ListToString(l));
    }
}
