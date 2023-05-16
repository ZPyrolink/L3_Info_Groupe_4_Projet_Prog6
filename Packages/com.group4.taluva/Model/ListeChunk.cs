using System;
using System.Collections.Generic;

namespace Taluva.Model
{
    public static class ListeChunk
    {   
        private static readonly List<Chunk> Chunks = new();

        private static readonly int[,] OcurrenceMatrix =
        {
            { 1, 2, 4, 6, 2 },
            { 1, 1, 1, 1, 1 },
            { 4, 1, 1, 2, 2 },
            { 5, 1, 2, 1, 2 },
            { 2, 1, 1, 2, 1 }
        }; //indexes : Forest = 0, Lake = 1, desert = 2, Plain = 3, Mountain = 4

        /// <summary>
        /// Gets the pile of chunks.
        /// </summary>
        public static Pile<Chunk> Pile => new Pile<Chunk>(Chunks.ToArray());

        /// <summary>
        /// Store the chunks in a list.
        /// </summary>
        static ListeChunk()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int reps = 0;
                    while (reps < OcurrenceMatrix[i, j])
                    {
                        Cell c1 = new Cell((Biomes)(i + 1));
                        Cell c2 = new Cell((Biomes)(j + 1));
                        Chunk c = new Chunk(1, c1, c2);
                        Chunks.Add(c);
                        reps++;
                    }
                }
            }
        }

        /// <summary>
        /// Print one chunk.
        /// </summary>
        public static void PrintChunkToString(Chunk c)
        {
            Console.WriteLine($"{c.Coords[0].ActualBiome}, {c.Coords[1].ActualBiome}, {c.Coords[2].ActualBiome}");
        }

        /// <summary>
        /// Print all the chunks.
        /// </summary>
        public static void PrintChunkList(List<Chunk> l)
        {
            foreach (Chunk c in l)
            {
                PrintChunkToString(c);
            }
        }
    }
    
}