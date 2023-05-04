using System.Collections;
using System.Drawing;
using Taluva.Utils;

namespace Taluva.Model;

public class Board
{
    private readonly DynamicMatrix<Cell> worldMap;

    public Board()
    {
        worldMap = new DynamicMatrix<Cell>();
    }

    private void AddCell(Cell c , Point coord)
    {
        Point p = new Point(coord.X,coord.Y);
        worldMap.Add(c, p);
    }

    //private void RemoveCell(Cell c)
    //{
    //    Point p = GetCellCoord(c);
    //    worldMap[p.X, p.Y] = null;
    //}

    public Point[] GetChunkSlots()
    {
        if (worldMap.IsEmpty()) {
            Point[] p = new Point[1];
            p[0] = new Point(0, 0);
            return p;
        }

        List<Point> slots = new List<Point>();
        List<Point> chunkSlots = new List<Point>();

        foreach (Cell c in worldMap) {
            Point p = GetCellCoord(c);
            if(c.ActualBiome == Biomes.Volcano) {
                slots.Add(p);
            }

            if (worldMap.IsVoid(new Point(p.X, p.Y - 1))) {
                slots.Add(new Point(p.X, p.Y - 1));
                continue;
            } else if (worldMap.IsVoid(new Point(p.X, p.Y + 1))) {
                slots.Add(new Point(p.X, p.Y + 1));
                continue;
            }

            if (p.X % 2 == 0) {
                if (worldMap.IsVoid(new Point(p.X - 1, p.Y - 1))) {
                    slots.Add(new Point(p.X - 1, p.Y - 1));
                } else if (worldMap.IsVoid(new Point(p.X - 1, p.Y))) {
                    slots.Add(new Point(p.X - 1, p.Y));
                } else if (worldMap.IsVoid(new Point(p.X + 1, p.Y))) {
                    slots.Add(new Point(p.X + 1, p.Y));
                } else if (worldMap.IsVoid(new Point(p.X + 1, p.Y - 1))) {
                    slots.Add(new Point(p.X + 1, p.Y - 1));
                }
            } else {
                if (worldMap.IsVoid(new Point(p.X - 1, p.Y + 1))) {
                    slots.Add(new Point(p.X - 1, p.Y + 1));
                } else if (worldMap.IsVoid(new Point(p.X - 1, p.Y))) {
                    slots.Add(new Point(p.X - 1, p.Y));
                } else if (worldMap.IsVoid(new Point(p.X + 1, p.Y))) {
                    slots.Add(new Point(p.X + 1, p.Y));
                } else if (worldMap.IsVoid(new Point(p.X + 1, p.Y + 1))) {
                    slots.Add(new Point(p.X + 1, p.Y + 1));
                }
            }
            slots = slots.Distinct().ToList();


            //Recherche des points dans l'eau pouvant placer un chunk dans au moins une position
            foreach (Point pt in slots) {
                if (worldMap.GetValue(pt).ActualBiome == Biomes.Volcano)
                    continue;
                
                if(pt.X % 2 == 0) {
                    if(worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && (worldMap.IsVoid(new Point(pt.X - 1, pt.Y - 1)) 
                        || worldMap.IsVoid(new Point(pt.X + 1, pt.Y - 1))))
                        chunkSlots.Add(pt);
                    else if(worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && (worldMap.IsVoid(new Point(pt.X - 1, pt.Y))
                        || worldMap.IsVoid(new Point(pt.X + 1, pt.Y))))
                        chunkSlots.Add(pt);
                    else if(worldMap.IsVoid(new Point(pt.X - 1, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        chunkSlots.Add(pt);
                    else if(worldMap.IsVoid(new Point(pt.X + 1, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        chunkSlots.Add(pt);
                } else {
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && (worldMap.IsVoid(new Point(pt.X - 1, pt.Y))
                        || worldMap.IsVoid(new Point(pt.X + 1, pt.Y))))
                        chunkSlots.Add(pt);
                    else if (worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && (worldMap.IsVoid(new Point(pt.X - 1, pt.Y + 1))
                        || worldMap.IsVoid(new Point(pt.X + 1, pt.Y + 1))))
                        chunkSlots.Add(pt);
                    else if (worldMap.IsVoid(new Point(pt.X - 1, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        chunkSlots.Add(pt);
                    else if (worldMap.IsVoid(new Point(pt.X + 1, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        chunkSlots.Add(pt);
                }

                slots.Remove(pt);
            }

            //Recherche des points qui sont des volcans et qui permettent au moins une position pour ecraser la map
            foreach (Point pt in slots) {
                if (pt.X % 2 == 0) {
                    if (!worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && !worldMap.IsVoid(new Point(pt.X - 1, pt.Y - 1)))
                        if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                            !worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding())
                            chunkSlots.Add(pt);
                        else if(worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() && 
                            !worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding() &&
                            worldMap.GetValue(new Point(pt.X, pt.Y - 1)).actualVillage.VillageSize() > 1)
                            if(worldMap.GetValue(new Point(pt.X, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                chunkSlots.Add(pt);
                            //Implementer la recherche de village autour afin de savoir si c'est un building isolé
                }
            }
        }
    

        return chunkSlots.ToArray();
    }


    public void AddChunk(Chunk c, Player player)
    {
        Point[] points = GetChunkSlots();
        List<Point> pointsdispo = points.ToList();
        List<Cell> chunk = new List<Cell>();
        foreach(Cell ce in c.Coords)
            chunk.Add(ce);
        bool ok = true;
        foreach(Cell tmp in chunk)
        {
            Point co = GetCellCoord(tmp);
            if(!pointsdispo.Contains(co))
            {
                ok = false;
            }
            if(!ok)
            {
                return ;
            }
        }
        foreach(Cell tp in chunk)
        {
            Point coord = GetCellCoord(tp);
            worldMap.Add(tp , coord);
            
        }

        
    }


    /*public void PlaceBuilding(Cell coord, Building b, Player player)
    {
        //Faire des verifications puis utiliser la fonction build de cell
        Point p = GetCellCoord(coord);
        switch (b) {
            case Building.Barrack:
                if (worldMap[p.X, p.Y] != null && worldMap[p.X, p.Y]!.IsBuildable()) {
                    worldMap[p.X, p.Y]!.ActualBuildings = b;
                    worldMap[p.X, p.Y]!.Owner = player.ID;
                }
                break;

        }

    }
*/
    public void RemoveChunk(Chunk c)
    {
        throw new NotImplementedException();
    }

    public Point[] GetBarrackSlots(Player actualPlayer )
    {
        List<Point> barrackSlots = new List<Point>();
        foreach(Cell c in worldMap)
        {
            Point p = GetCellCoord(c);
            if (!worldMap.IsVoid(new Point(p.X, p.Y)) && worldMap.GetValue(p).ActualBuildings == Building.None &&  worldMap.GetValue(p).Owner == actualPlayer.ID)
            {
                barrackSlots.Add(new Point(p.X,p.Y));
            }
        }

        return barrackSlots.ToArray();
    }

    public Point[] GetTowerSlots(Player actualPlayer)
    {
        List<Point> towerSlots = new List<Point>();
        foreach (Cell c in worldMap)
        {
            Point p = GetCellCoord(c);
            if (!worldMap.IsVoid(new Point(p.X, p.Y)) && worldMap.GetValue(p).Owner == actualPlayer.ID && worldMap.GetValue(p).ActualBuildings == Building.None)
            {
                // Cellule de niveau 3 ou plus 
                if (worldMap.GetValue(p).parentCunk.Level >= 3)
                {
                    // la cellule est adjacente à une cité du joueur actuel.
                    if (IsAdjacentToCity(p, actualPlayer))
                    {
                        // aucune autre tour est présente dans cette cité.
                        if (!CityHasTower(p, actualPlayer))
                        {
                            towerSlots.Add(new Point(p.X, p.Y));
                        }
                    }
                }
            }
        }

        return towerSlots.ToArray();
    }
    public bool IsAdjacentToCity(Point cellCoord, Player actualPlayer)
    {
        Cell cell = worldMap.GetValue(cellCoord);
        if (cell != null && cell.actualVillage != null)
        {
            foreach (Cell neighbor in cell.actualVillage.neighbors)
            {
                if (neighbor != null && neighbor.Owner == actualPlayer.ID && neighbor.ActualBuildings != Building.None)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    public Point[] GetAdjacentPositions(Point cellp)
    {
        List<Point> adjacentPositions = new List<Point>();

        int yOffset = cellp.X % 2 == 0 ? -1 : 1;

        adjacentPositions.Add(new Point(cellp.X - 1, cellp.Y));
        adjacentPositions.Add(new Point(cellp.X - 1, cellp.Y + yOffset));
        adjacentPositions.Add(new Point(cellp.X, cellp.Y - 1));
        adjacentPositions.Add(new Point(cellp.X, cellp.Y + 1));
        adjacentPositions.Add(new Point(cellp.X + 1, cellp.Y + yOffset));
        adjacentPositions.Add(new Point(cellp.X + 1, cellp.Y));

        return adjacentPositions.ToArray();
    }
    
    public bool CityHasTower(Point cellCoord, Player actualPlayer)
    {
        Cell cell = worldMap.GetValue(cellCoord);
        if (cell != null && cell.actualVillage != null)
        {
            foreach (Cell neighbor in cell.actualVillage.neighbors)
            {
                if (neighbor != null && neighbor.Owner == actualPlayer.ID && neighbor.ActualBuildings == Building.Tower)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Point[] GetTempleSlot(Player actualPlayer)
    {
        List<Point> templeSlots = new List<Point>();
        foreach (Cell cell in worldMap)
        {
            if (CanBuildTemple(cell, actualPlayer))
            {
                Point p = GetCellCoord(cell);
                templeSlots.Add(p);
            }
        }
        return templeSlots.ToArray();
    }

    public bool CanBuildTemple(Cell cell, Player actualPlayer)
    {
        if (cell.Owner != actualPlayer.ID || cell.ActualBuildings == Building.Temple)
        {
            return false;
        }

        Village village = cell.actualVillage;
        if (village == null || cell.parentCunk.Level < 3 || VillageHasTemple(village))
        {
            return false;
        }

        return IsAdjacentToCity(GetCellCoord(cell), actualPlayer);
    }

    public bool VillageHasTemple(Village village)
    {
        foreach (Cell cell in village.neighbors)
        {
            if (cell.ActualBuildings == Building.Temple)
            {
                return true;
            }
        }
        return false;
    }
    private Point GetCellCoord(Cell c)
    {
        for (int i = worldMap.MinLine; i <= worldMap.MaxLine; i++) {
            if (worldMap.ContainsLine(i))
                for (int j = worldMap.MinColumn(i); j <= worldMap.MaxColumn(i); j++) {
                    if (worldMap.ContainsColumn(i, j))
                        return new(i, j);
                }
        }

        throw new($"Cell {c} not found!");
    }


}