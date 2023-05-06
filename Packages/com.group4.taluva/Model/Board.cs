using System;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.Client.Differences;
using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
    public class Board
    {
        public readonly DynamicMatrix<Cell> worldMap;

        public Board()
        {
            worldMap = new();
        }

        private void RemoveCell(Cell c)
        {
            Vector2Int p = GetCellCoord(c);
            worldMap.Remove(p);
        }

        public DynamicMatrix<Cell> GetMatrix() => worldMap;
        

        public List<Vector2Int> GetVillage(Vector2Int c)
        {
            //The building at the c position is never a building.none
            List<Vector2Int> villagePositions = new();

            PlayerColor color = worldMap.GetValue(c).Owner;

            List<Vector2Int> cells = new();
            List<Vector2Int> visited = new();

            Vector2Int[] neighbors = GetNeighbors(c);
            foreach (Vector2Int neighbor in neighbors) {
                if (worldMap.IsVoid(neighbor) || worldMap.GetValue(neighbor).ActualBuildings == Building.None
                    || worldMap.GetValue(neighbor).Owner != color)
                    continue;
                cells.Add(neighbor);
            }

            villagePositions.Add(c);
            visited.Add(c);

            while (cells.Count > 0) {
                Vector2Int cellP = cells[0];
                cells.Remove(cellP);
                visited.Add(cellP);
                villagePositions.Add(cellP);
                neighbors = GetNeighbors(cellP);
                foreach (Vector2Int neighbor in neighbors) {
                    if (worldMap.IsVoid(neighbor) || worldMap.GetValue(neighbor).ActualBuildings == Building.None
                        || visited.Contains(neighbor) || worldMap.GetValue(neighbor).Owner != color)
                        continue;
                    cells.Add(neighbor);
                }
            }
            return villagePositions;
        }

        public List<List<Vector2Int>> GetAllVillage(Vector2Int c)
        {
            List<List<Vector2Int>> allVillages = new();

            Vector2Int[] neighbors = GetNeighbors(c);

            List<List<Vector2Int>> villages = new();

            for (int i = 0; i < neighbors.Length; i++) {
                if (!worldMap.IsVoid(neighbors[i]) && worldMap.GetValue(neighbors[i]).ActualBuildings != Building.None)
                    villages.Add(GetVillage(neighbors[i]));
            }
            for (int i = 0; i < villages.Count; i++) {
                for (int j = i; j < villages.Count; j++) {
                    if (villages[i].Contains(villages[j][0])) {
                        villages.RemoveAt(j);
                        j--;
                    }
                }
            }
            return allVillages;
        }

        public Vector2Int[] GetNeighbors(Vector2Int p)
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
            Vector2Int previous = neighbors[5];
            foreach (Vector2Int neighbor in neighbors) {
                possible[i] = worldMap.IsVoid(previous) && worldMap.IsVoid(neighbor) &&
                    (IsConnected(neighbor) || IsConnected(previous) || IsConnected(p));
                previous = neighbor;
                i++;
            }

            return possible;
        }

        public bool IsConnected(Vector2Int p)
        {
            Vector2Int[] neighbors = GetNeighbors(p);
            foreach (Vector2Int neighbor in neighbors) {
                if (!worldMap.IsVoid(neighbor))
                    return true;
            }
            return false;
        }

        public bool PossibleVolcano(Vector2Int left, Vector2Int right, Rotation r, Vector2Int pt)
        {
            int level = worldMap.GetValue(pt).parentCunk.Level;
            if (!worldMap.IsVoid(left) && !worldMap.IsVoid(right))
                if (worldMap.GetValue(left).parentCunk.Level == level && worldMap.GetValue(left).parentCunk.Level == level)
                    if (worldMap.GetValue(pt).parentCunk.rotation != r)
                        if (!worldMap.GetValue(left).HaveBuilding() &&
                            !worldMap.GetValue(right).HaveBuilding()) {
                            return true;
                        } else if (worldMap.GetValue(left).HaveBuilding() && !worldMap.GetValue(right).HaveBuilding() &&
                                   GetVillage(GetCellCoord(worldMap.GetValue(left))).Count > 1) {
                            if (worldMap.GetValue(left).ActualBuildings == Building.Barrack)
                                return true;
                        } else if (!worldMap.GetValue(left).HaveBuilding() && worldMap.GetValue(right).HaveBuilding() &&
                                   GetVillage(GetCellCoord(worldMap.GetValue(right))).Count > 1) {
                            if (worldMap.GetValue(right).ActualBuildings ==
                                Building.Barrack)
                                return true;
                        } else if (worldMap.GetValue(left).HaveBuilding() && worldMap.GetValue(right).HaveBuilding() &&
                                   GetVillage(GetCellCoord(worldMap.GetValue(left))).Count > 2) {
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
                    if (worldMap.IsVoid(neighbor)) {
                        if (!slots.Contains(neighbor))
                            slots.Add(neighbor);
                        Vector2Int[] neighbors2 = GetNeighbors(neighbor);
                        foreach (Vector2Int neighbor2 in neighbors2)
                            if (worldMap.IsVoid(neighbor2))
                                if (!slots.Contains(neighbor2))
                                    slots.Add(neighbor2);
                    }
                }
            }

            List<Vector2Int> pointRemove = new();

            //Recherche des points dans l'eau pouvant placer un chunk dans au moins une position
            foreach (Vector2Int pt in slots) {
                if (!worldMap.IsVoid(pt))
                    if (worldMap.GetValue(pt).ActualBiome == Biomes.Volcano)
                        continue;
                    else {
                        pointRemove.Add(pt);
                        continue;
                    }


                bool[] rotations = GetPossibleRotation(pt);
                PointRotation pr = new(pt);

                for (int i = 0; i < rotations.Length; i++) {
                    if (rotations[i])
                        pr.AddRotation((Rotation)i);
                }
                chunkSlots.Add(pr);
                pointRemove.Add(pt);
            }

            foreach (Vector2Int pr in pointRemove) {
                slots.Remove(pr);
            }

            //Recherche des points qui sont des volcans et qui permettent une position pour ecraser la map
            foreach (Vector2Int pt in slots) {

                Vector2Int[] neighbors = GetNeighbors(pt);
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

                if (pr.HaveRotation())
                    chunkSlots.Add(pr);
            }


            return chunkSlots.ToArray();
        }

        private void AddCell(Chunk c, PointRotation p, Vector2Int left, Vector2Int right)
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
            foreach (PointRotation pr in points) {
                if (!pr.point.Equals(p.point)) {
                    continue;
                }

                for (int i = 0; i < pr.rotations.Length; i++) {
                    if (pr.rotations[i] == p.rotations[i]) {
                        exist = true;
                        break;
                    }
                }

                if (exist) {
                    break;
                }
            }

            if (!exist) {
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


        public void PlaceBuilding(Cell c, Building b, Player player)
        {
            if (c == null || b == null || player ==null)
            {
                return;
            }
            Vector2Int coord = GetCellCoord(c);
            switch (b) {
                case Building.Barrack:
                    Vector2Int[] pointsB = GetBarrackSlots();
                    List<Vector2Int> dispoB = pointsB.ToList();
                    if (dispoB.Contains(coord))
                    {
                        c.Owner = player.ID;
                        c.ActualBuildings = b;
                        player.nbBarrack--;
                    }

                    break;
                case Building.Temple:
                    Vector2Int[] pointsTe = GetTempleSlot(player);
                    List<Vector2Int> dispoTe = pointsTe.ToList();
                    if (dispoTe.Contains(coord))
                    {
                        c.Owner = player.ID;
                        c.ActualBuildings = b;
                        player.nbTemple--;
                    }

                    break;
                case Building.Tower:
                    Vector2Int[] pointsTo = GetTowerSlots(player);
                    List<Vector2Int> dispoTo = pointsTo.ToList();
                    if (dispoTo.Contains(coord)) {
                        c.Owner = player.ID;
                        c.ActualBuildings = b;
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

        public Vector2Int[] GetBarrackSlots()
        {
            List<Vector2Int> barrackSlots = new();
            foreach (Cell c in worldMap) {
                Vector2Int p = GetCellCoord(c);
                if (!worldMap.IsVoid(p) && worldMap.GetValue(p).ActualBuildings == Building.None && worldMap.GetValue(p).ActualBiome != Biomes.Volcano) {
                    barrackSlots.Add(p);
                }
            }

            return barrackSlots.ToArray();
        }

        public Vector2Int[] GetTowerSlots(Player actualPlayer)
        {
            List<Vector2Int> towerSlots = new();
            foreach (Cell c in worldMap) {
                Vector2Int p = GetCellCoord(c);
                if (!worldMap.IsVoid(p) && worldMap.GetValue(p).ActualBuildings == Building.None) {
                    // Cellule de niveau 3 ou plus 
                    if (worldMap.GetValue(p).parentCunk.Level >= 3) {
                        // la cellule est adjacente à une cité du joueur actuel.
                        if (IsAdjacentToCity(p, actualPlayer)) {
                            // aucune autre tour est présente dans cette cité.
                            List<List<Vector2Int>> allvillage = GetAllVillage(p);
                            bool tower = false;
                            foreach (List<Vector2Int> village in allvillage) {
                                if (worldMap.GetValue(village[0]).Owner == actualPlayer.ID)
                                    tower = CityHasTower(village);
                            }
                            if (!tower)
                                towerSlots.Add(p);
                        }
                    }
                }
            }
            return towerSlots.ToArray();
        }

        public bool IsAdjacentToCity(Vector2Int cellCoord, Player actualPlayer)
        {
            List<List<Vector2Int>> allvillage = GetAllVillage(cellCoord);

            if (allvillage.Count > 0)
                foreach (List<Vector2Int> village in allvillage)
                    if (worldMap.GetValue(village[0]).Owner == actualPlayer.ID)
                        return true;

            return false;
        }

        public bool CityHasTower(List<Vector2Int> village)
        {
            foreach (Vector2Int vp in village) {
                if (worldMap.GetValue(vp).ActualBuildings == Building.Tower)
                    return true;
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

            List<List<Vector2Int>> allVillage = GetAllVillage(GetCellCoord(cell));

            foreach (List<Vector2Int> village in allVillage) {
                if (worldMap.GetValue(village[0]).Owner != actualPlayer.ID || CityHasTemple(village) || village.Count < 3)
                    return false;
            }

            return true;
        }

        public bool CityHasTemple(List<Vector2Int> village)
        {
            foreach (Vector2Int vp in village) {
                if (worldMap.GetValue(vp).ActualBuildings == Building.Temple)
                    return true;
            }

            return false;
        }

        public Vector2Int GetCellCoord(Cell c)
        {
            for (int i = worldMap.MinLine; i <= worldMap.MaxLine; i++) {
                if (worldMap.ContainsLine(i)) {
                    for (int j = worldMap.MinColumn(i); j <= worldMap.MaxColumn(i); j++) {
                        if (worldMap.ContainsColumn(i, j) && worldMap.GetValue(new Vector2Int(i, j)) == c) {
                            return new Vector2Int(i, j);
                        }
                    }
                }
            }

            throw new Exception($"Cell {c} not found!");
        }
    }
}