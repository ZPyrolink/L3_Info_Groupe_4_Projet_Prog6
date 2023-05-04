namespace Taluva.Model;

public class ListeCell
{
    public List<Chunk> ListeChunk = new List<Chunk>();
    private int[,] OcurrenceMatrix =
    {
        {1, 2, 4, 6, 2},
        {1, 1, 1, 1, 1},
        {4, 1, 1, 2, 2},
        {5, 1, 2, 1, 2},
        {2, 1, 1, 2, 1}
    };       //indexes : Forest = 0, Lake = 1, desert = 2, Plain = 3, Mountain = 4

    //Create all chunks and store them in a list
    public void CreateCellSelection()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int reps = 0;
                while (reps < OcurrenceMatrix[i,j])
                {
                    ListeChunk.Add(new Chunk(0, new Cell((Biomes)(i + 1)), new Cell((Biomes)(j + 1))));
                    reps++;
                }
            }
        }
        
    }
    public void PrintChunkToString(List<Chunk> l)
    {
        String res;
        foreach (Chunk c in l)
        {
            res = $"{c.Coords[0].ActualBiome}, {c.Coords[1].ActualBiome}, {c.Coords[2].ActualBiome}";
            Console.WriteLine(res);
        }
    }
}