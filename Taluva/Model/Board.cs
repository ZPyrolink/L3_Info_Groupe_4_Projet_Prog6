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

    private void AddCell(Cell c, Point coord)
    {
        Point p = new Point(coord.X, coord.Y);
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
            PointRotation[] pr = new PointRotation[6];
            Point p = new Point(0, 0);
            for (int i = 0; i < 6; i++) {
                pr[i] = new PointRotation(p, (Rotation)i);
            }
            return pr;
        }

        List<Point> slots = new List<Point>();
        List<PointRotation> chunkSlots = new List<PointRotation>();

        foreach (Cell c in worldMap) {
            Point p = GetCellCoord(c);
            if (c.ActualBiome == Biomes.Volcano) {
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

                if (pt.X % 2 == 0) {
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y - 1)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y - 1)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                    if (worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                    if (worldMap.IsVoid(new Point(pt.X - 1, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        chunkSlots.Add(new PointRotation(pt, Rotation.N));
                    if (worldMap.IsVoid(new Point(pt.X + 1, pt.Y - 1)) && worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
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
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.NW)
                            if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X, pt.Y - 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                            } else if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && !worldMap.IsVoid(new Point(pt.X + 1, pt.Y - 1)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.SW)
                            if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X, pt.Y - 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                            } else if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && !worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.NE)
                            if (!worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X, pt.Y + 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                            } else if (!worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X - 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && !worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.SE)
                            if (!worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X, pt.Y + 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                            } else if (!worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X + 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X - 1, pt.Y - 1)) && !worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.N)
                            if (!worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.N));
                            } else if (worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.N));
                            } else if (!worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.N));
                            } else if (worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y - 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X - 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.N));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X + 1, pt.Y - 1)) && !worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.S)
                            if (!worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.S));
                            } else if (worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.S));
                            } else if (!worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.S));
                            } else if (worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y - 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X + 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.S));
                            }
                } else {
                    if (!worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && !worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.NW)
                            if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X, pt.Y - 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                            } else if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X - 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NW));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X, pt.Y - 1)) && !worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.SW)
                            if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X, pt.Y - 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                            } else if (!worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y - 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X + 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SW));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && !worldMap.IsVoid(new Point(pt.X - 1, pt.Y + 1)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.NE)
                            if (!worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X, pt.Y + 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                            } else if (!worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.NE));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X, pt.Y + 1)) && !worldMap.IsVoid(new Point(pt.X + 1, pt.Y + 1)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.SE)
                            if (!worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X, pt.Y + 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                            } else if (!worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                            } else if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X, pt.Y + 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.SE));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X - 1, pt.Y + 1)) && !worldMap.IsVoid(new Point(pt.X - 1, pt.Y)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.N)
                            if (!worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.N));
                            } else if (worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.N));
                            } else if (!worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.N));
                            } else if (worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X - 1, pt.Y)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X - 1, pt.Y + 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X - 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.N));
                            }
                    if (!worldMap.IsVoid(new Point(pt.X + 1, pt.Y + 1)) && !worldMap.IsVoid(new Point(pt.X + 1, pt.Y)))
                        if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.S)
                            if (!worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding()) {
                                chunkSlots.Add(new PointRotation(pt, Rotation.S));
                            } else if (worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).HaveBuilding() &&
                                !worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.S));
                            } else if (!worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).actualVillage.VillageSize() > 1) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.S));
                            } else if (worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).HaveBuilding() &&
                                        worldMap.GetValue(new Point(pt.X + 1, pt.Y)).actualVillage.VillageSize() > 2) {
                                if (worldMap.GetValue(new Point(pt.X + 1, pt.Y + 1)).ActualBuildings == Building.Barrack
                                    && worldMap.GetValue(new Point(pt.X + 1, pt.Y)).ActualBuildings == Building.Barrack)
                                    chunkSlots.Add(new PointRotation(pt, Rotation.S));
                            }
                }
            }
        }

        return chunkSlots.ToArray();
    }


    public void AddChunk(Chunk c, Player player)
    {
        PointRotation[] points = GetChunkSlots();
        List<PointRotation> pointsdispo = points.ToList();
        List<Cell> chunk = new List<Cell>();
        foreach (Cell ce in c.Coords)
            chunk.Add(ce);
        bool ok = true;
        foreach (Cell tmp in chunk)
        {
            Point co = GetCellCoord(tmp);
            if (!pointsdispo.Contains(new PointRotation(co, c.rotation)))
            {
                ok = false;
            }

            if (!ok)
            {
                return;
            }
        }

        foreach (Cell tp in chunk)
        {
            Point coord = GetCellCoord(tp);
            worldMap.Add(tp, coord);

        }


    }


    public void PlaceBuilding(Cell c, Point coord, Building b, Player player)
    {
        switch (b)
        {
            case Building.Barrack:
                Point[] pointsB = GetBarrackSlots(player);
                List<Point> dispoB = pointsB.ToList();
                if (dispoB.Contains(coord))
                {
                    worldMap.Add(c, coord);
                    player.nbBarrack--;
                }

                break;
            case Building.Temple:
                Point[] pointsTe = GetTempleSlot(player);
                List<Point> dispoTe = pointsTe.ToList();
                if (dispoTe.Contains(coord))
                {
                    worldMap.Add(c, coord);
                    player.nbTemple--;
                }
                break;
            case Building.Tower:
                Point[] pointsTo = GetTowerSlots(player);
                List<Point> dispoTo = pointsTo.ToList();
                if (dispoTo.Contains(coord))
                {
                    worldMap.Add(c, coord);
                    player.nbTowers--;
                }
                break;
            default:
                throw new NotImplementedException();


        }


    }
*/
    public void RemoveChunk(Chunk c)
    {
        foreach (Cell cell in c.Coords)
        {
            Point tmp = GetCellCoord(cell);
            worldMap.Remove(tmp);
        }
    }

    public Point[] GetBarrackSlots(Player actualPlayer)
    {
        List<Point> barrackSlots = new List<Point>();
        foreach (Cell c in worldMap)
        {
            Point p = GetCellCoord(c);
            if (!worldMap.IsVoid(new Point(p.X, p.Y)) && worldMap.GetValue(p).ActualBuildings == Building.None &&
                worldMap.GetValue(p).Owner == actualPlayer.ID)
            {
                barrackSlots.Add(new Point(p.X, p.Y));
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
            if (!worldMap.IsVoid(new Point(p.X, p.Y)) && worldMap.GetValue(p).Owner == actualPlayer.ID &&
                worldMap.GetValue(p).ActualBuildings == Building.None)
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
        for (int i = worldMap.MinLine; i <= worldMap.MaxLine; i++)
        {
            if (worldMap.ContainsLine(i))
                for (int j = worldMap.MinColumn(i); j <= worldMap.MaxColumn(i); j++)
                {
                    if (worldMap.ContainsColumn(i, j))
                        return new(i, j);
                }
        }

        throw new($"Cell {c} not found!");
    }
}
    