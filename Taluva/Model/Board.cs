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

    private void AddCell(Cell c)
    {
        Point p = GetCellCoord(c);
        worldMap.Add(c, p);
    }

    //private void RemoveCell(Cell c)
    //{
    //    Point p = GetCellCoord(c);
    //    worldMap[p.X, p.Y] = null;
    //}

    public PointRotation[] GetChunkSlots()
    {
        if (worldMap.IsEmpty()) {
            Point[] p = new Point[1];
            p[0] = new Point(0, 0);
            return p;
        }

        List<Point> slots = new List<Point>();
        List<PointRotation> chunkSlots = new List<PointRotation>();

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
                    if(worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y - 1)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y - 1)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                    if (worldMap.IsVoid(new Point(pt.X - 1, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.N));
                    if(worldMap.IsVoid(new Point(pt.X + 1, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.S));
                } else {
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y + 1)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y + 1)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                    if (worldMap.IsVoid(new Point(pt.X - 1, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.N));
                    if (worldMap.IsVoid(new Point(pt.X + 1, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.S));
                }

                slots.Remove(pt);
            }

            //Recherche des points qui sont des volcans et qui permettent une position pour ecraser la map
            foreach (Point pt in slots) {
                if (pt.X % 2 == 0) {
                    if (!worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && !worldMap.IsVoid(new Point(pt.X - 1, pt.Y - 1)))
                        if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                            !worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding())
                            chunkSlots.Add(new PointRotation(pt, Rotation.S));
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
            if (!worldMap.IsVoid(new Point(p.X, p.Y)) && worldMap.GetValue(p).Owner == actualPlayer.ID)
            {
                barrackSlots.Add(new Point(p.X,p.Y));
            }
        }

        return barrackSlots.ToArray();
    }

    public Point[] GetTowerSlots(Player actualPlayer)
    {
        List<Point> barrackSlots = new List<Point>();
        foreach(Cell c in worldMap)
        {
            Point tmp = GetCellCoord(c);
            if(!worldMap.IsVoid(new Point(tmp.X,tmp.Y)) && worldMap.GetValue(tmp).ActualBuildings == Building.Tower && worldMap.GetValue(tmp).Owner == actualPlayer.ID )
                barrackSlots.Add(new Point(tmp.X,tmp.Y));
        }

        return barrackSlots.ToArray();
    }

    public Point[] GetTempleSlots(Player actualPlayer)
    {
        List<Point> barrackSlots = new List<Point>();
        foreach(Cell c in worldMap)
        {
            Point tmp = GetCellCoord(c);
            if(!worldMap.IsVoid(new Point(tmp.X,tmp.Y)) && worldMap.GetValue(tmp).ActualBuildings == Building.Temple && worldMap.GetValue(tmp).Owner == actualPlayer.ID )
                barrackSlots.Add(new Point(tmp.X,tmp.Y));
        }

        return barrackSlots.ToArray();
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