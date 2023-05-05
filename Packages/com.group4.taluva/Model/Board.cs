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

        public DynamicMatrix<Cell> GetMatrix()
        {
            return worldMap;
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
            if (p.x % 2 == 0) {
                offset = -1;
            }

            // Neighbors visited clockwise
            neighborsCoord[0] = new(p.x - 1, p.y + 1 + offset);
            neighborsCoord[1] = new(p.x, p.y + 1);
            neighborsCoord[2] = new(p.x + 1, p.y + 1 + offset);
            neighborsCoord[3] = new(p.x + 1, p.y + offset);
            neighborsCoord[4] = new(p.x, p.y - 1);
            neighborsCoord[5] = new(p.x - 1, p.y + offset);

            return neighborsCoord;
        }

        public bool[] GetPossibleRotation(Vector2Int p)
        {
            bool[] possible = new bool[6];
            Vector2Int[] neighbors = GetNeighbors(p);
            int i = 0;
            bool previous = worldMap.IsVoid(neighbors[5]);
            foreach (Vector2Int neighbor in neighbors) {
                possible[i] = previous && (previous = worldMap.IsVoid(neighbors[i]));
                i++;
            }

            return possible;
        }

        public bool PossibleVolcano(Vector2Int left, Vector2Int right, Rotation r, Vector2Int pt)
        {
            if (!worldMap.IsVoid(left) && !worldMap.IsVoid(right))
                if (worldMap.GetValue(pt).parentCunk.rotation != r)
                    if (!worldMap.GetValue(left).HaveBuilding() &&
                        !worldMap.GetValue(right).HaveBuilding()) {
                        return true;
                    } else if (worldMap.GetValue(left).HaveBuilding() && !worldMap.GetValue(right).HaveBuilding() &&
                               worldMap.GetValue(left).actualVillage.VillageSize() > 1) {
                        if (worldMap.GetValue(left).ActualBuildings == Building.Barrack)
                            return true;
                    } else if (!worldMap.GetValue(left).HaveBuilding() && worldMap.GetValue(right).HaveBuilding() &&
                               worldMap.GetValue(right).actualVillage.VillageSize() > 1) {
                        if (worldMap.GetValue(right).ActualBuildings ==
                            Building.Barrack)
                            return true;
                    } else if (worldMap.GetValue(left).HaveBuilding() && worldMap.GetValue(right).HaveBuilding() &&
                               worldMap.GetValue(right).actualVillage.VillageSize() > 2) {
                        if (worldMap.GetValue(left).ActualBuildings == Building.Barrack &&
                            worldMap.GetValue(right).ActualBuildings == Building.Barrack)
                            return true;
                    }
            return false;
        }

        public PointRotation[] GetChunkSlots()
        {
            if (worldMap.IsEmpty()) {
                PointRotation[] pr = new PointRotation[1];
                pr[0] = new(new(0, 0));
                pr[0].SetAllTrue();
                return pr;
            }

            List<Vector2Int> slots = new();
            List<PointRotation> chunkSlots = new();

            foreach (Cell c in worldMap) {
                Vector2Int p = GetCellCoord(c);
                if (c.ActualBiome == Biomes.Volcano) {
                    slots.Add(p);
                }

                Vector2Int[] neighbors = GetNeighbors(p);

                foreach (Vector2Int neighbor in neighbors) {
                    if (worldMap.IsVoid(neighbor))
                        slots.Add(neighbor);
                }

                slots = slots.Distinct().ToList();


                //Recherche des points dans l'eau pouvant placer un chunk dans au moins une position
                foreach (Vector2Int pt in slots) {
                    if (worldMap.GetValue(pt).ActualBiome == Biomes.Volcano)
                        continue;

                    bool[] rotations = GetPossibleRotation(pt);
                    PointRotation pr = new(pt);

                    for (int i = 0; i < rotations.Length; i++) {
                        if (rotations[i])
                            pr.AddRotation((Rotation)i);
                    }
                    chunkSlots.Add(pr);
                    slots.Remove(pt);
                }

                //Recherche des points qui sont des volcans et qui permettent une position pour ecraser la map
                foreach (Vector2Int pt in slots) {

                    neighbors = GetNeighbors(pt);
                    PointRotation pr = new(pt);

                    if (PossibleVolcano(neighbors[0], neighbors[1], Rotation.NE, pt))
                        pr.AddRotation(Rotation.NE);
                    if (PossibleVolcano(neighbors[1], neighbors[2], Rotation.SE, pt))
                        pr.AddRotation(Rotation.SE);
                    if (PossibleVolcano(neighbors[2], neighbors[3], Rotation.S, pt))
                        pr.AddRotation(Rotation.S);
                    if (PossibleVolcano(neighbors[3], neighbors[4], Rotation.SW, pt))
                        pr.AddRotation(Rotation.SW);
                    if (PossibleVolcano(neighbors[4], neighbors[5], Rotation.NW, pt))
                        pr.AddRotation(Rotation.NW);
                    if (PossibleVolcano(neighbors[5], neighbors[0], Rotation.N, pt))
                        pr.AddRotation(Rotation.N);

                    chunkSlots.Add(pr);
                }
            }

            return chunkSlots.ToArray();
        }

        public void AddCell(Chunk c, PointRotation p, Vector2Int left, Vector2Int right)
        {

            worldMap.Add(c.Coords[1], left);
            worldMap.Add(c.Coords[2], right);
            worldMap.Add(c.Coords[0], p.point);
        }

        public void AddChunk(Chunk c, Player player, PointRotation p)
        {
            PointRotation[] points = GetChunkSlots();
            List<PointRotation> pointsdispo = points.ToList();
            bool exist = false;
            foreach (PointRotation pr in points)
            {
                if (!pr.point.Equals(p.point))
                {
                    continue;
                }

                for (int i = 0; i < pr.rotations.Length; i++)
                {
                    if (pr.rotations[i] == p.rotations[i])
                    {
                        exist = true;
                        break;
                    }
                }

                if (exist)
                {
                    break;
                }
            }

            if (!exist)
            {
                return;
            }

            Vector2Int[] neighbors = GetNeighbors(p.point);

            if (p.rotations[(int)Rotation.N]) {
                AddCell(c, p, neighbors[0], neighbors[5]);
            } else if (p.rotations[(int)Rotation.S]) {
                AddCell(c, p, neighbors[3], neighbors[2]);
            } else if (p.rotations[(int)Rotation.NE]) {
                AddCell(c, p, neighbors[1], neighbors[0]);
            } else if (p.rotations[(int)Rotation.SE]) {
                AddCell(c, p, neighbors[2], neighbors[1]);
            } else if (p.rotations[(int)Rotation.SW]) {
                AddCell(c, p, neighbors[4], neighbors[3]);
            } else if (p.rotations[(int)Rotation.NW]) {
                AddCell(c, p, neighbors[5], neighbors[4]);
            }
        }


        public void PlaceBuilding(Cell c, Vector2Int coord, Building b, Player player)
        {
            switch (b) {
                case Building.Barrack:
                    Vector2Int[] pointsB = GetBarrackSlots(player);
                    List<Vector2Int> dispoB = pointsB.ToList();
                    if (dispoB.Contains(coord)) {
                        worldMap.Add(c, coord);
                        player.nbBarrack--;
                    }

                    break;
                case Building.Temple:
                    Vector2Int[] pointsTe = GetTempleSlot(player);
                    List<Vector2Int> dispoTe = pointsTe.ToList();
                    if (dispoTe.Contains(coord)) {
                        worldMap.Add(c, coord);
                        player.nbTemple--;
                    }

                    break;
                case Building.Tower:
                    Vector2Int[] pointsTo = GetTowerSlots(player);
                    List<Vector2Int> dispoTo = pointsTo.ToList();
                    if (dispoTo.Contains(coord)) {
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
            foreach (Cell cell in c.Coords) {
                worldMap.Remove(GetCellCoord(cell));
            }
        }

        public Vector2Int[] GetBarrackSlots(Player actualPlayer)
        {
            List<Vector2Int> barrackSlots = new();
            foreach (Cell c in worldMap) {
                Vector2Int p = GetCellCoord(c);
                if (!worldMap.IsVoid(new(p.x, p.y)) && worldMap.GetValue(p).ActualBuildings == Building.None &&
                    worldMap.GetValue(p).Owner == actualPlayer.ID) {
                    barrackSlots.Add(new(p.x, p.y));
                }
            }

            return barrackSlots.ToArray();
        }

        public Vector2Int[] GetTowerSlots(Player actualPlayer)
        {
            List<Vector2Int> towerSlots = new();
            foreach (Cell c in worldMap) {
                Vector2Int p = GetCellCoord(c);
                if (!worldMap.IsVoid(new(p.x, p.y)) && worldMap.GetValue(p).Owner == actualPlayer.ID &&
                    worldMap.GetValue(p).ActualBuildings == Building.None) {
                    // Cellule de niveau 3 ou plus 
                    if (worldMap.GetValue(p).parentCunk.Level >= 3) {
                        // la cellule est adjacente à une cité du joueur actuel.
                        if (IsAdjacentToCity(p, actualPlayer)) {
                            // aucune autre tour est présente dans cette cité.
                            if (!CityHasTower(p, actualPlayer)) {
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
            if (cell != null && cell.actualVillage != null) {
                foreach (Cell neighbor in cell.actualVillage.neighbors) {
                    if (neighbor != null && neighbor.Owner == actualPlayer.ID &&
                        neighbor.ActualBuildings != Building.None) {
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
            if (cell != null && cell.actualVillage != null) {
                foreach (Cell neighbor in cell.actualVillage.neighbors) {
                    if (neighbor != null && neighbor.Owner == actualPlayer.ID &&
                        neighbor.ActualBuildings == Building.Tower) {
                        return true;
                    }
                }
            }

            return false;
        }

        public Vector2Int[] GetTempleSlot(Player actualPlayer)
        {
            List<Vector2Int> templeSlots = new();
            foreach (Cell cell in worldMap) {
                if (CanBuildTemple(cell, actualPlayer)) {
                    templeSlots.Add(GetCellCoord(cell));
                }
            }

            return templeSlots.ToArray();
        }

        public bool CanBuildTemple(Cell cell, Player actualPlayer)
        {
            if (cell.ActualBuildings != Building.None) {
                return false;
            }

            Vector2Int[] pts = GetAdjacentPositions(GetCellCoord(cell));
            bool v = false;
            Vector2Int pv = new();
            foreach (Vector2Int p in pts) {
                if (worldMap.IsVoid(p) || worldMap.GetValue(p).actualVillage == null)
                    continue;
                else {
                    v = true;
                    pv = p;
                    break;
                }
            }

            if (!v) {
                return false;
            }

            if (worldMap.GetValue(pv).actualVillage.VillageSize() < 3
                && worldMap.GetValue(pv).Owner != actualPlayer.ID &&
                !CityHasTemple(worldMap.GetValue(pv).actualVillage))
                return false;

            return true;
        }

        public bool CityHasTemple(Village village)
        {
            List<Cell> cells = new();
            List<Cell> visited = new();
            foreach (Cell? c in village.neighbors) {
                if (c == null || !c.HaveBuilding())
                    continue;
                if (c.ActualBuildings == Building.Temple)
                    return true;
                cells.Add(c);
            }

            visited.Add(village.currentCell);

            while (cells.Count != 0) {
                Cell cell = cells[0];
                cells.Remove(cell);
                visited.Add(cell);
                foreach (Cell? c in cell.actualVillage.neighbors) {
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
}