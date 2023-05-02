using System.Drawing;

namespace Taluva.Model;

public class Board
{
    private readonly Cell?[,] worldMap;

    public Board()
    {
        worldMap = new Cell[2, 2];
    }

    private void AddCell(Cell c)
    {
        Point p = GetCellCoord(c);
        worldMap[p.X, p.Y] = c;
    }

    private void RemoveCell(Cell c)
    {
        Point p = GetCellCoord(c);
        worldMap[p.X, p.Y] = null;
    }

    public Point[] GetChunkSlots()
    {
        List<Point> chunkSlots = new List<Point>();
        for (int x = 0; x < worldMap.GetLength(0); x++)
        {
            for (int y = 0; y < worldMap.GetLength(1); y++)
            {
                if (worldMap[x, y] != null && worldMap[x, y]!.ActualBiome != Biomes.None)
                {
                    chunkSlots.Add(new Point(x, y));
                }
            }
        }

        return chunkSlots.ToArray();
    }

    public Point[] GetBarrackSlots()
    {
        List<Point> barrackSlots = new List<Point>();
        for (int x = 0; x < worldMap.GetLength(0); x++)
        {
            for (int y = 0; y < worldMap.GetLength(1); y++)
            {
                if (worldMap[x, y] != null && worldMap[x, y]!.ActualBuildings == Building.None)
                {
                    barrackSlots.Add(new Point(x, y));
                }
            }
        }

        return barrackSlots.ToArray();
    }

    public void AddChunk(Chunk c, Player player)
    {
        foreach (Cell tmp in c.Coords)
        {
            Point p = GetCellCoord(tmp);
            AddCell(tmp);
            worldMap[p.X, p.Y]!.Owner = player.ID;

        }
    }

    public void PlaceBuilding(Point coord, Building building, Player player)
    {
        if (worldMap[coord.X, coord.Y] != null && worldMap[coord.X, coord.Y]!.IsBuildable())
        {
            worldMap[coord.X, coord.Y]!.ActualBuildings = building;
            worldMap[coord.X, coord.Y]!.Owner = player.ID;
        }
    }

    public void RemoveChunk(Chunk c)
    {
        throw new NotImplementedException();
    }
    
    public Point[] GetTowerSlots(Player actualPlayer)
    {
        List<Point> towerSlots = new List<Point>();
        for (int x = 0; x < worldMap.GetLength(0); x++)
        {
            for (int y = 0; y < worldMap.GetLength(1); y++)
            {
                if (worldMap[x, y] != null && worldMap[x, y]!.ActualBuildings == Building.Tower &&
                    worldMap[x, y]!.Owner == actualPlayer.ID)
                {
                    towerSlots.Add(new Point(x, y));
                }
            }
        }

        return towerSlots.ToArray();
    }

    public Point[] GetTempleSlots(Player actualPlayer)
    {
        List<Point> templeSlots = new List<Point>();
        for (int x = 0; x < worldMap.GetLength(0); x++)
        {
            for (int y = 0; y < worldMap.GetLength(1); y++)
            {
                if (worldMap[x, y] != null && worldMap[x, y]!.ActualBuildings == Building.Temple &&
                    worldMap[x, y]!.Owner == actualPlayer.ID)
                {
                    templeSlots.Add(new Point(x, y));
                }
            }
        }

        return templeSlots.ToArray();
    }

    public Point[] GetBarracksSlots(Player actualPlayer)
    {
        List<Point> templeSlots = new List<Point>();
        for (int x = 0; x < worldMap.GetLength(0); x++)
        {
            for (int y = 0; y < worldMap.GetLength(1); y++)
            {
                if (worldMap[x, y] != null && worldMap[x, y]!.ActualBuildings == Building.Barrack &&
                    worldMap[x, y]!.Owner == actualPlayer.ID)
                {
                    templeSlots.Add(new Point(x, y));
                }
            }
        }

        return templeSlots.ToArray();
    }

    private Point GetCellCoord(Cell c)
    {
        for (int i = 0; i < worldMap.GetLength(0); i++)
        {
            for (int j = 0; j < worldMap.GetLength(1); j++)
            {
                if (worldMap[i,j] == c)
                    return new(i, j);
            }
        }

        throw new($"Cell {c} not found!");
    }
}