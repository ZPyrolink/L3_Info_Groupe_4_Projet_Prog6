using System.Drawing;

namespace Taluva.Model;

public class Board
{
    private Cell[,] worldMap;

    public Board()
    {
        worldMap = new Cell[2, 2];
    }

    private void AddCell(Point coord)
    {
        throw new NotImplementedException();
    }

    private void RemoveCell(Point coord)
    {
        throw new NotImplementedException();
    }

    public Point[] GetChunkSlots()
    {
        throw new NotImplementedException();
    }

    public Point[] GetTowerSlots()
    {
        throw new NotImplementedException();
    }

    public Point[] GetTempleSlots()
    {
        throw new NotImplementedException();
    }

    public Point[] GetBarrackSlots()
    {
        throw new NotImplementedException();
    }

    public void AddChunk(Chunk coord, Player player)
    {
        throw new NotImplementedException();
    }

    public void PlaceBuilding(Point coord)
    {
        throw new NotImplementedException();
    }

    public void RemoveChunk(Chunk coord)
    {
        throw new NotImplementedException();
    }

    public bool CanRotate(Chunk p)
    {
        throw new NotImplementedException();
    }

    public void RotateChunk(Chunk p)
    {
        throw new NotImplementedException();
    }
}