using System;
using System.Collections.Generic;

namespace Taluva.Model
{
    public static class ListeChunk
    {   
        private static readonly List<Chunk> Chunks = new();

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
        /// Get a new Pile
        /// </summary>
        public static Pile<Chunk> Pile => new(Chunks.ToArray());

        // public static int Count => Chunks.Count;
        
        /// <summary>
        /// Create all chunks and store them in a list
        /// </summary>
        static ListeChunk()
        {
            for (int i = 0; i < OcurrenceMatrix.GetLength(0); i++)
                for (int j = 0; j < OcurrenceMatrix.GetLength(1); j++)
                    for (int reps = 0; reps < OcurrenceMatrix[i, j]; reps++)
                    {
                        Cell c1 = new((Biomes) (i + 1));
                        Cell c2 = new((Biomes) (j + 1));
                        Chunk c = new(1, c1, c2);
                        Chunks.Add(c);
                    }
        }

        /// <summary>
        /// Reset the chunks of a pile
        /// </summary>
        /// <param name="pileChunk">The pile to reset</param>
        public static void ResetChunk(Pile<Chunk> pileChunk)
        {
            Stack<Chunk> stack = new();
            int nb = pileChunk.NbKeeping;
            for(int i = 0; i < nb; i++)
            {
                Chunk chunk = pileChunk._stack.Pop();
                chunk.Coords[1].ActualBuildings = Building.None;
                chunk.Coords[2].ActualBuildings = Building.None;
                chunk.Level = 1;

                stack.Push(chunk);
            }
            pileChunk._stack.Clear();

            for(int i = 0; i < nb; i++)
            {
                Chunk chunk = stack.Pop();
                pileChunk._stack.Push(chunk);
            }
        }

        [Obsolete("Use Chunk.ToString() instead")]
        public static void PrintChunkToString(Chunk c) => Console.WriteLine(c);

        [Obsolete("Use Chunk.ListToString(List<Chunk>) instead")]
        public static void PrintChunkList(List<Chunk> l) => Console.WriteLine(Chunk.ListToString(l));
    }
}