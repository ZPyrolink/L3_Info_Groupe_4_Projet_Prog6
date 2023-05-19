using System;
using System.Collections.Generic;

using UnityEngine.UIElements;

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

        public static Pile<Chunk> Pile => new(Chunks.ToArray());

        public static int Count => Chunks.Count;

        //Create all chunks and store them in a list
        static ListeChunk()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int reps = 0;
                    while (reps < OcurrenceMatrix[i, j])
                    {
                        Cell c1 = new((Biomes) (i + 1));
                        Cell c2 = new((Biomes) (j + 1));
                        Chunk c = new(1, c1, c2);
                        Chunks.Add(c);
                        reps++;
                    }
                }
            }

        }

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

        //print one chunk contents
        public static void PrintChunkToString(Chunk c)
        {
            Console.WriteLine($"{c.Coords[0].ActualBiome}, {c.Coords[1].ActualBiome}, {c.Coords[2].ActualBiome}");
        }

        public static void PrintChunkList(List<Chunk> l)
        {
            foreach (Chunk c in l)
            {
                PrintChunkToString(c);
            }
        }
    }
}