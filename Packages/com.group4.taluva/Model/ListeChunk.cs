using System;
using System.Collections.Generic;

namespace Taluva.Model
{
    public class ListeChunk
    {
        public List<Chunk> Chunks = new();

        private int[,] OcurrenceMatrix =
        {
            { 1, 2, 4, 6, 2 },
            { 1, 1, 1, 1, 1 },
            { 4, 1, 1, 2, 2 },
            { 5, 1, 2, 1, 2 },
            { 2, 1, 1, 2, 1 }
        }; //indexes : Forest = 0, Lake = 1, desert = 2, Plain = 3, Mountain = 4

        //Create all chunks and store them in a list
        public void CreateCellSelection()
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
                        Chunk c = new(0, c1, c2);
                        c1.parentCunk = c;
                        c2.parentCunk = c;
                        Chunks.Add(c);
                        reps++;
                    }
                }
            }

        }

        //print one chunk contents
        public void PrintChunkToString(Chunk c)
        {
            Console.WriteLine($"{c.Coords[0].ActualBiome}, {c.Coords[1].ActualBiome}, {c.Coords[2].ActualBiome}");
        }

        public void PrintChunkList(List<Chunk> l)
        {
            foreach (Chunk c in l)
            {
                PrintChunkToString(c);
            }
        }
    }
}