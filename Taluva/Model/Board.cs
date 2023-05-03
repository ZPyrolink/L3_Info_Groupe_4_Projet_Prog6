using System.Drawing;

namespace Taluva.Model;

public class Board
{
    private readonly Cell?[,] worldMap;

    public Board()
    {
        worldMap = new Cell[2, 2];
    }

    public Cell? this[Point coord]
    {
        set => worldMap[coord.X, coord.Y] = value;
    }

    [Obsolete("Use the indexer this[Point coord] instead")]
    public void AddCell(Point coord, Cell cell)
    {
        worldMap[coord.X, coord.Y] = cell;
    }

    [Obsolete("Use the indexer this[Point coord] instead")]
    private void RemoveCell(Point coord)
    {
        worldMap[coord.X, coord.Y] = null;
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
                if (worldMap[x, y] != null && worldMap[x, y].ActualBuildings == Building.None)
                {
                    barrackSlots.Add(new Point(x, y));
                }
            }
        }

        return barrackSlots.ToArray();
    }

    public void AddChunk(Chunk c, Player player)
    {
        throw new NotImplementedException();
    }

    public void PlaceBuilding(Point coord, Building building, Player player)
    {
        if (worldMap[coord.X, coord.Y] != null && worldMap[coord.X, coord.Y].IsBuildable())
        {
            worldMap[coord.X, coord.Y].ActualBuildings = building;
            worldMap[coord.X, coord.Y].Owner = player.ID;
        }
    }

    public void RemoveChunk(Chunk c)
    {
        foreach (Point coord in c.Coords)
        {
            RemoveCell(coord);
        }
    }

    public bool CanRotate(Chunk target)
    {
        throw new NotImplementedException();
    }

    public void RotateChunk(Chunk target)
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
                if (worldMap[x, y] != null && worldMap[x, y].ActualBuildings == Building.Tower &&
                    worldMap[x, y].Owner == actualPlayer.ID)
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
                if (worldMap[x, y] != null && worldMap[x, y].ActualBuildings == Building.Temple &&
                    worldMap[x, y].Owner == actualPlayer.ID)
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
                if (worldMap[x, y] != null && worldMap[x, y].ActualBuildings == Building.Barrack &&
                    worldMap[x, y].Owner == actualPlayer.ID)
                {
                    templeSlots.Add(new Point(x, y));
                }
            }
        }

        return templeSlots.ToArray();
    }
}