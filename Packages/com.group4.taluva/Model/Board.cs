using System;
using System.Collections.Generic;
using System.Linq;

using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
    public class Board
    {
        public DynamicMatrix<Cell> WorldMap { get; }

        public Board()
        {
            WorldMap = new();
        }

        private void RemoveCell(Cell c) => WorldMap.Remove(GetCellCoord(c));

        public List<Vector2Int> GetVillage(Vector2Int c)
        {
            PlayerColor color = WorldMap.GetValue(c).Owner;

            bool TestNeighbor(Vector2Int neighbor) =>
                !WorldMap.IsVoid(neighbor) && WorldMap.GetValue(neighbor).ActualBuildings != Building.None &&
                WorldMap.GetValue(neighbor).Owner == color;

            Vector2Int[] neighbors = GetNeighbors(c);
            List<Vector2Int> cells = neighbors.Where(TestNeighbor).ToList();

            //The building at the c position is never a building.none
            List<Vector2Int> villagePositions = new() { c };
            List<Vector2Int> visited = new() { c };

            while (cells.Count > 0)
            {
                Vector2Int cellP = cells[0];
                cells.Remove(cellP);
                visited.Add(cellP);
                villagePositions.Add(cellP);
                neighbors = GetNeighbors(cellP);
                cells.AddRange(neighbors.Where(neighbor =>
                    TestNeighbor(neighbor) && !visited.Contains(neighbor)));
            }

            return villagePositions;
        }

        public List<List<Vector2Int>> GetAllVillage(Vector2Int c)
        {
            Vector2Int[] neighbors = GetNeighbors(c);

            List<List<Vector2Int>> villages = neighbors
                .Where(neighbor =>
                    !WorldMap.IsVoid(neighbor) && WorldMap.GetValue(neighbor).ActualBuildings != Building.None)
                .Select(GetVillage).ToList();

            for (int i = 0; i < villages.Count - 1; i++)
                for (int j = i + 1; j < villages.Count; j++)
                    if (villages[i].Contains(villages[j][0]))
                    {
                        villages.RemoveAt(j);
                        j--;
                    }

            return villages;
        }

        public static Vector2Int[] GetNeighbors(Vector2Int p)
        {
            int offset = 0;
            if (p.x % 2 == 0)
                offset = -1;

            // Neighbors visited clockwise
            Vector2Int[] neighborsCoord =
            {
                new(p.x - 1, p.y + 1 + offset),
                new(p.x, p.y + 1),
                new(p.x + 1, p.y + 1 + offset),
                new(p.x + 1, p.y + offset),
                new(p.x, p.y - 1),
                new(p.x - 1, p.y + offset)
            };

            return neighborsCoord;
        }

        public bool[] GetPossibleRotation(Vector2Int p)
        {
            Vector2Int[] neighbors = GetNeighbors(p);
            int i = 0;
            Vector2Int previous = neighbors[5];
            bool[] possible = new bool[6];
            foreach (Vector2Int neighbor in neighbors)
            {
                possible[i] = WorldMap.IsVoid(previous) && WorldMap.IsVoid(neighbor) &&
                              (IsConnected(neighbor) || IsConnected(previous) || IsConnected(p));
                previous = neighbor;
                i++;
            }

            return possible;
        }

        public bool IsConnected(Vector2Int p) => GetNeighbors(p).Any(neighbor => !WorldMap.IsVoid(neighbor));

        public bool PossibleVolcano(Vector2Int left, Vector2Int right, Rotation r, Vector2Int pt)
        {
            int level = WorldMap.GetValue(pt).ParentCunk.Level;

            if (WorldMap.IsVoid(left) || WorldMap.IsVoid(right))
                return false;

            Cell leftCell = WorldMap.GetValue(left);

            if (leftCell.ParentCunk.Level != level || leftCell.ParentCunk.Level != level)
                return false;

            if (WorldMap.GetValue(pt).ParentCunk.rotation == r)
                return false;

            Cell rightCell = WorldMap.GetValue(right);

            if (!leftCell.ContainsBuilding() && !rightCell.ContainsBuilding())
                return true;

            if (leftCell.ContainsBuilding() && !rightCell.ContainsBuilding() && GetVillage(GetCellCoord(leftCell)).Count > 1)
                return leftCell.ActualBuildings == Building.Barrack;

            if (!leftCell.ContainsBuilding() && rightCell.ContainsBuilding() && GetVillage(GetCellCoord(rightCell)).Count > 1)
                return rightCell.ActualBuildings == Building.Barrack;

            if (leftCell.ContainsBuilding() && rightCell.ContainsBuilding() && GetVillage(GetCellCoord(leftCell)).Count > 2)
                return leftCell.ActualBuildings == Building.Barrack && rightCell.ActualBuildings == Building.Barrack;

            return false;
        }

        public PointRotation[] GetChunkSlots()
        {
            if (WorldMap.Empty)
            {
                PointRotation[] pr = new PointRotation[1];
                pr[0] = new(new(0, 0));
                pr[0].SetAllTrue();
                return pr;
            }

            List<Vector2Int> slots = new();
            List<PointRotation> chunkSlots = new();

            foreach (Cell c in WorldMap)
            {
                Vector2Int p = GetCellCoord(c);
                if (c.ActualBiome == Biomes.Volcano)
                    slots.Add(p);

                Vector2Int[] neighbors = GetNeighbors(p);

                foreach (Vector2Int neighbor in neighbors.Where(n => WorldMap.IsVoid(n)))
                {
                    if (!slots.Contains(neighbor))
                        slots.Add(neighbor);

                    slots.AddRange(GetNeighbors(neighbor)
                        .Where(n => WorldMap.IsVoid(n) && !slots.Contains(n)));
                }
            }

            List<Vector2Int> pointRemove = new();

            //Recherche des points dans l'eau pouvant placer un chunk dans au moins une position
            foreach (Vector2Int pt in slots)
            {
                if (!WorldMap.IsVoid(pt))
                {
                    if (WorldMap.GetValue(pt).ActualBiome == Biomes.Volcano)
                        continue;
                    
                    pointRemove.Add(pt);
                    continue;
                }


                bool[] rotations = GetPossibleRotation(pt);
                PointRotation pr = new(pt);

                for (int i = 0; i < rotations.Length; i++)
                    if (rotations[i])
                        pr.AddRotation((Rotation) i);

                chunkSlots.Add(pr);
                pointRemove.Add(pt);
            }

            foreach (Vector2Int pr in pointRemove) 
                slots.Remove(pr);

            //Recherche des points qui sont des volcans et qui permettent une position pour ecraser la map
            foreach (Vector2Int pt in slots)
            {
                Vector2Int[] neighbors = GetNeighbors(pt);
                PointRotation pr = new(pt);

                Rotation[] rotations = (Rotation[]) Enum.GetValues(typeof(Rotation));

                foreach (Rotation r in rotations)
                {
                    int right = (int) r, left = right - 1;
                    if (left < 0)
                        left = rotations.Length + left;
                    
                    
                    if (PossibleVolcano(neighbors[left], neighbors[right], r, pt))
                        pr.AddRotation(r);
                }

                if (pr.HaveRotation())
                    chunkSlots.Add(pr);
            }


            return chunkSlots.ToArray();
        }

        private void AddCell(Chunk c, PointRotation p, Vector2Int left, Vector2Int right)
        {
            WorldMap.Add(c.Coords[0], p.point);
            WorldMap.Add(c.Coords[1], left);
            WorldMap.Add(c.Coords[2], right);
        }

        public void AddChunk(Chunk c, Player _, PointRotation p, Rotation r)
        {
            if (!GetChunkSlots()
                    .Where(pr => pr.point.Equals(p.point))
                    .Any(pr => pr.rotations.Where((t, i) => t == p.rotations[i]).Any()))
            {
                return;
            }

            Vector2Int[] neighbors = GetNeighbors(p.point);

            if (p.rotations[(int) Rotation.N] && r == Rotation.N)
                AddCell(c, p, neighbors[0], neighbors[5]);
            else if (p.rotations[(int)Rotation.S] && r == Rotation.S)
                AddCell(c, p, neighbors[3], neighbors[2]);
            else if (p.rotations[(int) Rotation.NE] && r == Rotation.NE)
                AddCell(c, p, neighbors[1], neighbors[0]);
            else if (p.rotations[(int) Rotation.SE] && r == Rotation.SE)
                AddCell(c, p, neighbors[2], neighbors[1]);
            else if (p.rotations[(int) Rotation.SW] && r == Rotation.SW)
                AddCell(c, p, neighbors[4], neighbors[3]);
            else if (p.rotations[(int) Rotation.NW] && r == Rotation.NW)
                AddCell(c, p, neighbors[5], neighbors[4]);

            c.rotation = r;
        }

        public void PlaceBuilding(Cell c, Building b, Player player)
        {
            if (c == null || player == null)
                return;

            Vector2Int coord = GetCellCoord(c);

            void SetC()
            {
                c.Owner = player.ID;
                c.Build(b);
            }

            switch (b)
            {
                case Building.Barrack when GetBarrackSlots().Contains(coord):
                    SetC();
                    player.nbBarrack--;
                    break;
                case Building.Temple when GetTempleSlots(player).Contains(coord):
                    SetC();
                    player.nbTemple--;
                    break;
                case Building.Tower when GetTowerSlots(player).Contains(coord):
                    SetC();
                    player.nbTowers--;
                    break;
                default:
                    return;
            }
        }

        public void RemoveChunk(Chunk c)
        {
            foreach (Cell cell in c.Coords)
                WorldMap.Remove(GetCellCoord(cell));
        }

        public Vector2Int[] GetBarrackSlots() => WorldMap
            .Select(GetCellCoord)
            .Where(p => !WorldMap.IsVoid(p) && WorldMap.GetValue(p).ActualBuildings == Building.None &&
                        WorldMap.GetValue(p).ActualBiome != Biomes.Volcano)
            .ToArray();

        public Vector2Int[] GetTowerSlots(Player actualPlayer) => WorldMap
            .Select(GetCellCoord)
            .Where(p => !WorldMap.IsVoid(p) && WorldMap.GetValue(p).ActualBuildings == Building.None && WorldMap.GetValue(p).IsBuildable)
            .Where(p => WorldMap.GetValue(p).ParentCunk.Level >= 3)
            .Where(p => IsAdjacentToCity(p, actualPlayer))
            .Where(p => !GetAllVillage(p)
                .Where(village => WorldMap.GetValue(village[0]).Owner == actualPlayer.ID)
                .Any(CityHasTower)).ToArray();

        public bool IsAdjacentToCity(Vector2Int cellCoord, Player actualPlayer)
        {
            List<List<Vector2Int>> allvillage = GetAllVillage(cellCoord);
            return allvillage.Count > 0 &&
                   allvillage.Any(village => WorldMap.GetValue(village[0]).Owner == actualPlayer.ID);
        }

        public bool CityHasTower(List<Vector2Int> village) =>
            village.Any(vp => WorldMap.GetValue(vp).ActualBuildings == Building.Tower);

        public Vector2Int[] GetTempleSlots(Player actualPlayer) => WorldMap
            .Where(cell => CanBuildTemple(cell, actualPlayer))
            .Select(GetCellCoord)
            .ToArray();

        public bool CanBuildTemple(Cell cell, Player actualPlayer) =>
            cell.ActualBuildings == Building.None && cell.IsBuildable && IsAdjacentToCity(GetCellCoord(cell), actualPlayer) && 
            GetAllVillage(GetCellCoord(cell))
                .All(village =>
                    WorldMap.GetValue(village[0]).Owner == actualPlayer.ID && !CityHasTemple(village) &&
                    village.Count >= 3);

        public bool CityHasTemple(List<Vector2Int> village) =>
            village.Any(vp => WorldMap.GetValue(vp).ActualBuildings == Building.Temple);

        public Vector2Int GetCellCoord(Cell c)
        {
            for (int i = WorldMap.MinLine; i <= WorldMap.MaxLine; i++)
            {
                if (!WorldMap.ContainsLine(i))
                    continue;

                for (int j = WorldMap.MinColumn(i); j <= WorldMap.MaxColumn(i); j++)
                    if (WorldMap.ContainsColumn(i, j) && WorldMap.GetValue(new(i, j)) == c)
                        return new(i, j);
            }

            throw new($"Cell {c} not found!");
        }
    }
}