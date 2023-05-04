using System.Collections.Generic;
using System.Linq;

using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
    public class Board
    {
        private readonly DynamicMatrix<Cell> worldMap;

        public Board()
        {
            worldMap = new();
        }

        private void AddCell(Cell c, Vector2Int coord)
        {
            worldMap.Add(c, new(coord.x, coord.y));
        }

        private void RemoveCell(Cell c)
        {
            Vector2Int p = GetCellCoord(c);
            worldMap.Remove(p);
        }

        private Vector2Int[] GetNeighbors(Vector2Int p)
        {
            Vector2Int[] neighborsCoord = new Vector2Int[6];
            int offset = 0;
            if (p.x % 2 == 0)
            {
                offset = -1;
            }

            // Neighbors visited clockwise
            neighborsCoord[(offset + 6) % 6] = new(p.x - 1, p.y + 1 + offset);
            neighborsCoord[offset + 1] = new(p.x, p.y + 1);
            neighborsCoord[offset + 2] = new(p.x - 1, p.y + 1 + offset);
            neighborsCoord[offset + 3] = new(p.x - 1, p.y + offset);
            neighborsCoord[offset + 4] = new(p.x, p.y - 1);
            neighborsCoord[offset + 5] = new(p.x - 1, p.y + 1 + offset);

            return neighborsCoord;
        }

        public bool[] GetPossibleRotation(Vector2Int p)
        {
            bool[] possible = new bool[6];
            Vector2Int[] neighbors = GetNeighbors(p);
            int i = 0;
            bool previous = worldMap.IsVoid(neighbors[5]);
            foreach (Vector2Int neighbor in neighbors)
            {
                possible[i] = previous && (previous = worldMap.IsVoid(neighbors[i]));
                i++;
            }

            return possible;
        }
        //private void RemoveCell(Cell c)
        //{
        //    Point p = GetCellCoord(c);
        //    worldMap[p.X, p.Y] = null;
        //}

        public PointRotation[] GetChunkSlots()
        {
            if (worldMap.IsEmpty())
            {
                PointRotation[] pr = new PointRotation[6];
                Vector2Int p = new(0, 0);
                for (int i = 0; i < 6; i++)
                {
                    pr[i] = new(p, (Rotation) i);
                }

                return pr;
            }

            List<Vector2Int> slots = new();
            List<PointRotation> chunkSlots = new();

            foreach (Cell c in worldMap)
            {
                Vector2Int p = GetCellCoord(c);
                if (c.ActualBiome == Biomes.Volcano)
                {
                    slots.Add(p);
                }

                if (worldMap.IsVoid(new(p.x, p.y - 1)))
                {
                    slots.Add(new(p.x, p.y - 1));
                    continue;
                }
                else if (worldMap.IsVoid(new(p.x, p.y + 1)))
                {
                    slots.Add(new(p.x, p.y + 1));
                    continue;
                }

                if (p.x % 2 == 0)
                {
                    if (worldMap.IsVoid(new(p.x - 1, p.y - 1)))
                    {
                        slots.Add(new(p.x - 1, p.y - 1));
                    }
                    else if (worldMap.IsVoid(new(p.x - 1, p.y)))
                    {
                        slots.Add(new(p.x - 1, p.y));
                    }
                    else if (worldMap.IsVoid(new(p.x + 1, p.y)))
                    {
                        slots.Add(new(p.x + 1, p.y));
                    }
                    else if (worldMap.IsVoid(new(p.x + 1, p.y - 1)))
                    {
                        slots.Add(new(p.x + 1, p.y - 1));
                    }
                }
                else
                {
                    if (worldMap.IsVoid(new(p.x - 1, p.y + 1)))
                    {
                        slots.Add(new(p.x - 1, p.y + 1));
                    }
                    else if (worldMap.IsVoid(new(p.x - 1, p.y)))
                    {
                        slots.Add(new(p.x - 1, p.y));
                    }
                    else if (worldMap.IsVoid(new(p.x + 1, p.y)))
                    {
                        slots.Add(new(p.x + 1, p.y));
                    }
                    else if (worldMap.IsVoid(new(p.x + 1, p.y + 1)))
                    {
                        slots.Add(new(p.x + 1, p.y + 1));
                    }
                }

                slots = slots.Distinct().ToList();


                //Recherche des points dans l'eau pouvant placer un chunk dans au moins une position
                foreach (Vector2Int pt in slots)
                {
                    if (worldMap.GetValue(pt).ActualBiome == Biomes.Volcano)
                        continue;

                    if (pt.x % 2 == 0)
                    {
                        if (worldMap.IsVoid(new(pt.x, pt.y - 1)) &&
                            worldMap.IsVoid(new(pt.x - 1, pt.y - 1)))
                            chunkSlots.Add(new(pt, Rotation.NW));
                        if (worldMap.IsVoid(new(pt.x, pt.y - 1)) &&
                            worldMap.IsVoid(new(pt.x + 1, pt.y - 1)))
                            chunkSlots.Add(new(pt, Rotation.SW));
                        if (worldMap.IsVoid(new(pt.x, pt.y + 1)) && worldMap.IsVoid(new(pt.x - 1, pt.y)))
                            chunkSlots.Add(new(pt, Rotation.NE));
                        if (worldMap.IsVoid(new(pt.x, pt.y + 1)) && worldMap.IsVoid(new(pt.x + 1, pt.y)))
                            chunkSlots.Add(new(pt, Rotation.SE));
                        if (worldMap.IsVoid(new(pt.x - 1, pt.y - 1)) &&
                            worldMap.IsVoid(new(pt.x - 1, pt.y)))
                            chunkSlots.Add(new(pt, Rotation.N));
                        if (worldMap.IsVoid(new(pt.x + 1, pt.y - 1)) &&
                            worldMap.IsVoid(new(pt.x + 1, pt.y)))
                            chunkSlots.Add(new(pt, Rotation.S));
                    }
                    else
                    {
                        if (worldMap.IsVoid(new(pt.x, pt.y - 1)) && worldMap.IsVoid(new(pt.x - 1, pt.y)))
                            chunkSlots.Add(new(pt, Rotation.NW));
                        if (worldMap.IsVoid(new(pt.x, pt.y - 1)) && worldMap.IsVoid(new(pt.x + 1, pt.y)))
                            chunkSlots.Add(new(pt, Rotation.SW));
                        if (worldMap.IsVoid(new(pt.x, pt.y + 1)) &&
                            worldMap.IsVoid(new(pt.x - 1, pt.y + 1)))
                            chunkSlots.Add(new(pt, Rotation.NE));
                        if (worldMap.IsVoid(new(pt.x, pt.y + 1)) &&
                            worldMap.IsVoid(new(pt.x + 1, pt.y + 1)))
                            chunkSlots.Add(new(pt, Rotation.SE));
                        if (worldMap.IsVoid(new(pt.x - 1, pt.y + 1)) &&
                            worldMap.IsVoid(new(pt.x - 1, pt.y)))
                            chunkSlots.Add(new(pt, Rotation.N));
                        if (worldMap.IsVoid(new(pt.x + 1, pt.y + 1)) &&
                            worldMap.IsVoid(new(pt.x + 1, pt.y)))
                            chunkSlots.Add(new(pt, Rotation.S));
                    }

                    slots.Remove(pt);
                }

                //Recherche des points qui sont des volcans et qui permettent une position pour ecraser la map
                foreach (Vector2Int pt in slots)
                {
                    if (pt.x % 2 == 0)
                    {
                        if (!worldMap.IsVoid(new(pt.x, pt.y - 1)) &&
                            !worldMap.IsVoid(new(pt.x - 1, pt.y - 1)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.NW)
                                if (!worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x - 1, pt.y - 1)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.NW));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x - 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x, pt.y - 1)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NW));
                                }
                                else if (!worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y - 1)).actualVillage.VillageSize() >
                                         1)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NW));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y - 1)).actualVillage.VillageSize() >
                                         2)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y - 1)).ActualBuildings == Building.Barrack
                                        && worldMap.GetValue(new(pt.x - 1, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NW));
                                }

                        if (!worldMap.IsVoid(new(pt.x, pt.y - 1)) &&
                            !worldMap.IsVoid(new(pt.x + 1, pt.y - 1)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.SW)
                                if (!worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x + 1, pt.y - 1)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.SW));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x + 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x, pt.y - 1)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SW));
                                }
                                else if (!worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y - 1)).actualVillage.VillageSize() >
                                         1)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SW));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y - 1)).actualVillage.VillageSize() >
                                         2)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y - 1)).ActualBuildings == Building.Barrack
                                        && worldMap.GetValue(new(pt.x + 1, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SW));
                                }

                        if (!worldMap.IsVoid(new(pt.x, pt.y + 1)) && !worldMap.IsVoid(new(pt.x - 1, pt.y)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.NE)
                                if (!worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.NE));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x, pt.y + 1)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NE));
                                }
                                else if (!worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NE));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).actualVillage.VillageSize() > 2)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y + 1)).ActualBuildings == Building.Barrack
                                        && worldMap.GetValue(new(pt.x - 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NE));
                                }

                        if (!worldMap.IsVoid(new(pt.x, pt.y + 1)) && !worldMap.IsVoid(new(pt.x + 1, pt.y)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.SE)
                                if (!worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.SE));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x, pt.y + 1)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SE));
                                }
                                else if (!worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SE));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).actualVillage.VillageSize() > 2)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y + 1)).ActualBuildings == Building.Barrack
                                        && worldMap.GetValue(new(pt.x + 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SE));
                                }

                        if (!worldMap.IsVoid(new(pt.x - 1, pt.y - 1)) &&
                            !worldMap.IsVoid(new(pt.x - 1, pt.y)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.N)
                                if (!worldMap.GetValue(new(pt.x - 1, pt.y - 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.N));
                                }
                                else if (worldMap.GetValue(new(pt.x - 1, pt.y - 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y - 1)).actualVillage.VillageSize() >
                                         1)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.N));
                                }
                                else if (!worldMap.GetValue(new(pt.x - 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.N));
                                }
                                else if (worldMap.GetValue(new(pt.x - 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).actualVillage.VillageSize() > 2)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack
                                        && worldMap.GetValue(new(pt.x - 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.N));
                                }

                        if (!worldMap.IsVoid(new(pt.x + 1, pt.y - 1)) &&
                            !worldMap.IsVoid(new(pt.x + 1, pt.y)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.S)
                                if (!worldMap.GetValue(new(pt.x + 1, pt.y - 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.S));
                                }
                                else if (worldMap.GetValue(new(pt.x + 1, pt.y - 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y - 1)).actualVillage.VillageSize() >
                                         1)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.S));
                                }
                                else if (!worldMap.GetValue(new(pt.x + 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.S));
                                }
                                else if (worldMap.GetValue(new(pt.x + 1, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).actualVillage.VillageSize() > 2)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack
                                        && worldMap.GetValue(new(pt.x + 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.S));
                                }
                    }
                    else
                    {
                        if (!worldMap.IsVoid(new(pt.x, pt.y - 1)) && !worldMap.IsVoid(new(pt.x - 1, pt.y)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.NW)
                                if (!worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.NW));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x, pt.y - 1)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NW));
                                }
                                else if (!worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NW));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).actualVillage.VillageSize() > 2)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y - 1)).ActualBuildings == Building.Barrack
                                        && worldMap.GetValue(new(pt.x - 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NW));
                                }

                        if (!worldMap.IsVoid(new(pt.x, pt.y - 1)) && !worldMap.IsVoid(new(pt.x + 1, pt.y)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.SW)
                                if (!worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.SW));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x, pt.y - 1)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y - 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SW));
                                }
                                else if (!worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SW));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y - 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).actualVillage.VillageSize() > 2)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y - 1)).ActualBuildings == Building.Barrack
                                        && worldMap.GetValue(new(pt.x + 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SW));
                                }

                        if (!worldMap.IsVoid(new(pt.x, pt.y + 1)) &&
                            !worldMap.IsVoid(new(pt.x - 1, pt.y + 1)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.NE)
                                if (!worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x - 1, pt.y + 1)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.NE));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x - 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x, pt.y + 1)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NE));
                                }
                                else if (!worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y + 1)).actualVillage.VillageSize() >
                                         1)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NE));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y + 1)).actualVillage.VillageSize() >
                                         2)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y + 1)).ActualBuildings == Building.Barrack
                                        && worldMap.GetValue(new(pt.x - 1, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.NE));
                                }

                        if (!worldMap.IsVoid(new(pt.x, pt.y + 1)) &&
                            !worldMap.IsVoid(new(pt.x + 1, pt.y + 1)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.SE)
                                if (!worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x + 1, pt.y + 1)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.SE));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x + 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x, pt.y + 1)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SE));
                                }
                                else if (!worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y + 1)).actualVillage.VillageSize() >
                                         1)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SE));
                                }
                                else if (worldMap.GetValue(new(pt.x, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y + 1)).actualVillage.VillageSize() >
                                         2)
                                {
                                    if (worldMap.GetValue(new(pt.x, pt.y + 1)).ActualBuildings == Building.Barrack
                                        && worldMap.GetValue(new(pt.x + 1, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.SE));
                                }

                        if (!worldMap.IsVoid(new(pt.x - 1, pt.y + 1)) &&
                            !worldMap.IsVoid(new(pt.x - 1, pt.y)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.N)
                                if (!worldMap.GetValue(new(pt.x - 1, pt.y + 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.N));
                                }
                                else if (worldMap.GetValue(new(pt.x - 1, pt.y + 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y + 1)).actualVillage.VillageSize() >
                                         1)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.N));
                                }
                                else if (!worldMap.GetValue(new(pt.x - 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.N));
                                }
                                else if (worldMap.GetValue(new(pt.x - 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x - 1, pt.y)).actualVillage.VillageSize() > 2)
                                {
                                    if (worldMap.GetValue(new(pt.x - 1, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack
                                        && worldMap.GetValue(new(pt.x - 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.N));
                                }

                        if (!worldMap.IsVoid(new(pt.x + 1, pt.y + 1)) &&
                            !worldMap.IsVoid(new(pt.x + 1, pt.y)))
                            if (worldMap.GetValue(pt).parentCunk.rotation != Rotation.S)
                                if (!worldMap.GetValue(new(pt.x + 1, pt.y + 1)).HaveBuilding() &&
                                    !worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding())
                                {
                                    chunkSlots.Add(new(pt, Rotation.S));
                                }
                                else if (worldMap.GetValue(new(pt.x + 1, pt.y + 1)).HaveBuilding() &&
                                         !worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y + 1)).actualVillage.VillageSize() >
                                         1)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.S));
                                }
                                else if (!worldMap.GetValue(new(pt.x + 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).actualVillage.VillageSize() > 1)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.S));
                                }
                                else if (worldMap.GetValue(new(pt.x + 1, pt.y + 1)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).HaveBuilding() &&
                                         worldMap.GetValue(new(pt.x + 1, pt.y)).actualVillage.VillageSize() > 2)
                                {
                                    if (worldMap.GetValue(new(pt.x + 1, pt.y + 1)).ActualBuildings ==
                                        Building.Barrack
                                        && worldMap.GetValue(new(pt.x + 1, pt.y)).ActualBuildings ==
                                        Building.Barrack)
                                        chunkSlots.Add(new(pt, Rotation.S));
                                }
                    }
                }
            }

            return chunkSlots.ToArray();
        }


        public void AddChunk(Chunk c, Player player, PointRotation p)
        {
            PointRotation[] points = GetChunkSlots();
            List<PointRotation> pointsdispo = points.ToList();
            if (pointsdispo.Contains(p))
                return;
            if (p.point.x % 2 == 0)
            {
                if (p.rotation == Rotation.N)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x - 1, p.point.y));
                    worldMap.Add(c.Coords[2], new(p.point.x - 1, p.point.y - 1));
                }
                else if (p.rotation == Rotation.S)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x + 1, p.point.y - 1));
                    worldMap.Add(c.Coords[2], new(p.point.x + 1, p.point.y));
                }
                else if (p.rotation == Rotation.NE)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x, p.point.y + 1));
                    worldMap.Add(c.Coords[2], new(p.point.x - 1, p.point.y));
                }
                else if (p.rotation == Rotation.SE)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x + 1, p.point.y));
                    worldMap.Add(c.Coords[2], new(p.point.x, p.point.y + 1));
                }
                else if (p.rotation == Rotation.SW)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x, p.point.y - 1));
                    worldMap.Add(c.Coords[2], new(p.point.x + 1, p.point.y - 1));
                }
                else if (p.rotation == Rotation.NW)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x - 1, p.point.y - 1));
                    worldMap.Add(c.Coords[2], new(p.point.x, p.point.y - 1));
                }

                worldMap.Add(c.Coords[0], p.point);
            }
            else
            {
                if (p.rotation == Rotation.N)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x - 1, p.point.y + 1));
                    worldMap.Add(c.Coords[2], new(p.point.x - 1, p.point.y));
                }
                else if (p.rotation == Rotation.S)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x + 1, p.point.y));
                    worldMap.Add(c.Coords[2], new(p.point.x + 1, p.point.y + 1));
                }
                else if (p.rotation == Rotation.NE)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x, p.point.y + 1));
                    worldMap.Add(c.Coords[2], new(p.point.x - 1, p.point.y + 1));
                }
                else if (p.rotation == Rotation.SE)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x + 1, p.point.y + 1));
                    worldMap.Add(c.Coords[2], new(p.point.x, p.point.y + 1));
                }
                else if (p.rotation == Rotation.SW)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x, p.point.y - 1));
                    worldMap.Add(c.Coords[2], new(p.point.x + 1, p.point.y));
                }
                else if (p.rotation == Rotation.NW)
                {
                    worldMap.Add(c.Coords[1], new(p.point.x - 1, p.point.y));
                    worldMap.Add(c.Coords[2], new(p.point.x, p.point.y - 1));
                }

                worldMap.Add(c.Coords[0], p.point);
            }
        }


        public void PlaceBuilding(Cell c, Vector2Int coord, Building b, Player player)
        {
            switch (b)
            {
                case Building.Barrack:
                    Vector2Int[] pointsB = GetBarrackSlots(player);
                    List<Vector2Int> dispoB = pointsB.ToList();
                    if (dispoB.Contains(coord))
                    {
                        worldMap.Add(c, coord);
                        player.nbBarrack--;
                    }

                    break;
                case Building.Temple:
                    Vector2Int[] pointsTe = GetTempleSlot(player);
                    List<Vector2Int> dispoTe = pointsTe.ToList();
                    if (dispoTe.Contains(coord))
                    {
                        worldMap.Add(c, coord);
                        player.nbTemple--;
                    }

                    break;
                case Building.Tower:
                    Vector2Int[] pointsTo = GetTowerSlots(player);
                    List<Vector2Int> dispoTo = pointsTo.ToList();
                    if (dispoTo.Contains(coord))
                    {
                        worldMap.Add(c, coord);
                        player.nbTowers--;
                    }

                    break;
                default:
                    return;
            }
        }

        public void RemoveChunk(Chunk c)
        {
            foreach (Cell cell in c.Coords)
            {
                worldMap.Remove(GetCellCoord(cell));
            }
        }

        public Vector2Int[] GetBarrackSlots(Player actualPlayer)
        {
            List<Vector2Int> barrackSlots = new();
            foreach (Cell c in worldMap)
            {
                Vector2Int p = GetCellCoord(c);
                if (!worldMap.IsVoid(new(p.x, p.y)) && worldMap.GetValue(p).ActualBuildings == Building.None &&
                    worldMap.GetValue(p).Owner == actualPlayer.ID)
                {
                    barrackSlots.Add(new(p.x, p.y));
                }
            }

            return barrackSlots.ToArray();
        }

        public Vector2Int[] GetTowerSlots(Player actualPlayer)
        {
            List<Vector2Int> towerSlots = new();
            foreach (Cell c in worldMap)
            {
                Vector2Int p = GetCellCoord(c);
                if (!worldMap.IsVoid(new(p.x, p.y)) && worldMap.GetValue(p).Owner == actualPlayer.ID &&
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
                                towerSlots.Add(new(p.x, p.y));
                            }
                        }
                    }
                }
            }

            return towerSlots.ToArray();
        }

        public bool IsAdjacentToCity(Vector2Int cellCoord, Player actualPlayer)
        {
            Cell cell = worldMap.GetValue(cellCoord);
            if (cell != null && cell.actualVillage != null)
            {
                foreach (Cell neighbor in cell.actualVillage.neighbors)
                {
                    if (neighbor != null && neighbor.Owner == actualPlayer.ID &&
                        neighbor.ActualBuildings != Building.None)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Vector2Int[] GetAdjacentPositions(Vector2Int cellp)
        {
            List<Vector2Int> adjacentPositions = new();

            int yOffset = cellp.x % 2 == 0 ? -1 : 1;

            adjacentPositions.Add(new(cellp.x - 1, cellp.y));
            adjacentPositions.Add(new(cellp.x - 1, cellp.y + yOffset));
            adjacentPositions.Add(new(cellp.x, cellp.y - 1));
            adjacentPositions.Add(new(cellp.x, cellp.y + 1));
            adjacentPositions.Add(new(cellp.x + 1, cellp.y + yOffset));
            adjacentPositions.Add(new(cellp.x + 1, cellp.y));

            return adjacentPositions.ToArray();
        }

        public bool CityHasTower(Vector2Int cellCoord, Player actualPlayer)
        {
            Cell cell = worldMap.GetValue(cellCoord);
            if (cell != null && cell.actualVillage != null)
            {
                foreach (Cell neighbor in cell.actualVillage.neighbors)
                {
                    if (neighbor != null && neighbor.Owner == actualPlayer.ID &&
                        neighbor.ActualBuildings == Building.Tower)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Vector2Int[] GetTempleSlot(Player actualPlayer)
        {
            List<Vector2Int> templeSlots = new();
            foreach (Cell cell in worldMap)
            {
                if (CanBuildTemple(cell, actualPlayer))
                {
                    templeSlots.Add(GetCellCoord(cell));
                }
            }

            return templeSlots.ToArray();
        }

        public bool CanBuildTemple(Cell cell, Player actualPlayer)
        {
            if (cell.ActualBuildings != Building.None)
            {
                return false;
            }

            Vector2Int[] pts = GetAdjacentPositions(GetCellCoord(cell));
            bool v = false;
            Vector2Int pv = new();
            foreach (Vector2Int p in pts)
            {
                if (worldMap.IsVoid(p) || worldMap.GetValue(p).actualVillage == null)
                    continue;
                else
                {
                    v = true;
                    pv = p;
                    break;
                }
            }

            if (!v)
            {
                return false;
            }

            if (worldMap.GetValue(pv).actualVillage.VillageSize() < 3
                && worldMap.GetValue(pv).Owner != actualPlayer.ID &&
                !VillageHasTemple(worldMap.GetValue(pv).actualVillage))
                return false;

            return true;
        }

        public bool VillageHasTemple(Village village)
        {
            List<Cell> cells = new();
            List<Cell> visited = new();
            foreach (Cell? c in village.neighbors)
            {
                if (c == null || !c.HaveBuilding())
                    continue;
                if (c.ActualBuildings == Building.Temple)
                    return true;
                cells.Add(c);
            }

            visited.Add(village.currentCell);

            while (cells.Count != 0)
            {
                Cell cell = cells[0];
                cells.Remove(cell);
                visited.Add(cell);
                foreach (Cell? c in cell.actualVillage.neighbors)
                {
                    if (c == null || !c.HaveBuilding() || visited.Contains(c))
                        continue;
                    cells.Add(c);
                }

                if (cell.ActualBuildings == Building.Temple)
                    return true;
            }

            return false;
        }

        private Vector2Int GetCellCoord(Cell c)
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
}