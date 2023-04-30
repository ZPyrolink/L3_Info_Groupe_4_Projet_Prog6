using System;
using System.Collections.Generic;
using System.Drawing;

public class Board
{
    private Cell[,] WorldMap;

    public Board()
    {
        WorldMap = new Cell[2, 2];
    }

    public void AddCell(Point coord, Cell cell)
    {
        WorldMap[coord.X, coord.Y] = cell;
    }

    private void RemoveCell(Point coord)
    {
        WorldMap[coord.X, coord.Y] = null;
    }

    public Point[] GetChunkSlots()
    {
        List<Point> chunkSlots = new List<Point>();
        for (int x = 0; x < WorldMap.GetLength(0); x++)
        {
            for (int y = 0; y < WorldMap.GetLength(1); y++)
            {
                if (WorldMap[x, y] != null && WorldMap[x, y].ActualBiome != Biomes.None)
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
        for (int x = 0; x < WorldMap.GetLength(0); x++)
        {
            for (int y = 0; y < WorldMap.GetLength(1); y++)
            {
                if (WorldMap[x, y] != null && WorldMap[x, y].ActualBuildings == Building.None)
                {
                    barrackSlots.Add(new Point(x, y));
                }
            }
        }
        return barrackSlots.ToArray();
    }

    public void AddChunk(Point coord, Chunk chunk, Player player)
    {
        
    }

    public void PlaceBuilding(Point coord, Building building, Player player)
    {
       
        if (WorldMap[coord.X, coord.Y] != null && WorldMap[coord.X, coord.Y].IsBuildable())
        {
            WorldMap[coord.X, coord.Y].ActualBuildings = building;
            WorldMap[coord.X, coord.Y].Owner = player.ID;
        }
    }

    public void RemoveChunk(Chunk c)
    {
        foreach (Point coord in c.coords)
        {
            RemoveCell(coord);
        }
    }

    public bool CanRotate(Chunk target)
    {
        return true;
    }

    public void RotateChunk(Chunk target)
    {
        
        
    }

    public Point[] GetTowerSlots(Player actualPlayer)
    {
        List<Point> towerSlots = new List<Point>();
        for (int x = 0; x < WorldMap.GetLength(0); x++)
        {
            for (int y = 0; y < WorldMap.GetLength(1); y++)
            {
                if (WorldMap[x, y] != null && WorldMap[x, y].ActualBuildings == Building.Tower && WorldMap[x, y].Owner == actualPlayer.ID)
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
        for (int x = 0; x < WorldMap.GetLength(0); x++)
        {
            for (int y = 0; y < WorldMap.GetLength(1); y++)
            {
                if (WorldMap[x, y] != null && WorldMap[x, y].ActualBuildings == Building.Temple && WorldMap[x, y].Owner == actualPlayer.ID)
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
        for (int x = 0; x < WorldMap.GetLength(0); x++)
        {
            for (int y = 0; y < WorldMap.GetLength(1); y++)
            {
                if (WorldMap[x, y] != null && WorldMap[x, y].ActualBuildings == Building.Barrack && WorldMap[x, y].Owner == actualPlayer.ID)
                {
                    templeSlots.Add(new Point(x, y));
                }
            }
        }
        return templeSlots.ToArray();
    }
}

