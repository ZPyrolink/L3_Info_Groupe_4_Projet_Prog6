using PlasticGui.WorkspaceWindow.QueryViews;
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

        /// <summary>
        /// Remove the cell from the map
        /// </summary>
        /// <param name="c">The cell to remove</param>
        private void RemoveCell(Cell c) => WorldMap.Remove(GetCellCoord(c));

        /// <summary>
        /// Remove the chunk from the map.
        /// Be careful! This only remove the chunk and don't add the possible under cells
        /// </summary>
        /// <param name="c">The chunk to remove</param>
        public void RemoveChunk(Chunk c)
        {
            foreach (Cell cell in c.Coords)
                RemoveCell(cell);
        }

        /// <summary>
        /// Check if the position contains any build
        /// </summary>
        /// <param name="pos">A cell position</param>
        /// <returns>
        ///     <see langword="true"/> if the position is not a void and his build is not <see cref="Building.None"/>
        /// </returns>
        private bool ContainsBuild(Vector2Int pos) => !WorldMap.IsVoid(pos) &&
                                                      WorldMap[pos].ActualBuildings != Building.None;

        /// <summary>
        /// Find all the position of the village.
        /// Be careful! The position given has a building on it.
        /// </summary>
        /// <param name="c">The begining of the village</param>
        /// <returns>Return all the position of the village.</returns>
        public List<Vector2Int> GetVillage(Vector2Int c)
        {
            PlayerColor color = WorldMap[c].Owner;

            bool TestNeighbor(Vector2Int neighbor) =>
                ContainsBuild(neighbor) && WorldMap[neighbor].Owner == color;

            Vector2Int[] neighbors = GetNeighbors(c);
            List<Vector2Int> cells = neighbors.Where(TestNeighbor).ToList();

            //The building at the c position is never a building.none
            List<Vector2Int> villagePositions = new() { c };
            List<Vector2Int> visited = new() { c };

            while (cells.Count > 0) {
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

        /// <summary>
        /// Find all the village around a position.
        /// </summary>
        /// <param name="c">A cell position</param>
        /// <returns>Return all the village around a cell position</returns>
        public List<List<Vector2Int>> GetAllVillage(Vector2Int c)
        {
            Vector2Int[] neighbors = GetNeighbors(c);

            List<List<Vector2Int>> villages = neighbors.Where(ContainsBuild).Select(GetVillage).ToList();

            for (int i = 0; i < villages.Count - 1; i++)
                for (int j = i + 1; j < villages.Count; j++)
                    if (villages[i].Contains(villages[j][0])) {
                        villages.RemoveAt(j);
                        j--;
                    }

            return villages;
        }

        public Vector2Int[] GetChunksCoords(Vector2Int c, Rotation r)
        {
            Vector2Int[] cells = new Vector2Int[3];
            cells[0] = c;

            Vector2Int[] neighbors = GetNeighbors(c);

            switch(r){
                case Rotation.N:
                    cells[1] = neighbors[0];
                    cells[2] = neighbors[5];
                    break;
                case Rotation.NE:
                    cells[1] = neighbors[1];
                    cells[2] = neighbors[0];
                    break;
                case Rotation.SE:
                    cells[1] = neighbors[2];
                    cells[2] = neighbors[1];
                    break;
                case Rotation.S:
                    cells[1] = neighbors[3];
                    cells[2] = neighbors[2];
                    break;
                case Rotation.SW:
                    cells[1] = neighbors[4];
                    cells[2] = neighbors[3];
                    break;
                case Rotation.NW:
                    cells[1] = neighbors[5];
                    cells[2] = neighbors[4];
                    break;
            }

            return cells;
        }

        /// <summary>
        /// Find all the neighbors.
        /// The neighbors will always return clockwise.
        /// See the next schema!
        ///  5 0
        /// 4 c 1
        ///  3 2
        /// </summary>
        /// <param name="p"></param>
        /// <returns>Return the different neighbors of a position</returns>
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

        /// <summary>
        /// Check the possible rotation for a chunk at the p position
        /// </summary>
        /// <param name="p">Position to check</param>
        /// <returns>Return a boolean array with the different rotation</returns>
        public bool[] GetPossibleRotation(Vector2Int p)
        {
            Vector2Int[] neighbors = GetNeighbors(p);
            int i = 0;
            Vector2Int previous = neighbors[5];
            bool[] possible = new bool[6];
            foreach (Vector2Int neighbor in neighbors) {
                possible[i] = WorldMap.IsVoid(previous) && WorldMap.IsVoid(neighbor) &&
                              (IsConnected(neighbor) || IsConnected(previous) || IsConnected(p));
                previous = neighbor;
                i++;
            }

            return possible;
        }

        /// <summary>
        /// Check if a position is connected to the island
        /// </summary>
        /// <param name="p">Position to check</param>
        /// <returns>Return if the point have a neighbors with a cell</returns>
        public bool IsConnected(Vector2Int p) => GetNeighbors(p).Any(neighbor => !WorldMap.IsVoid(neighbor));

        /// <summary>
        /// Check if the chunk can be placed on a volcano
        /// </summary>
        /// <param name="left">Left position of the chunk</param>
        /// <param name="right">Right position of the chunk</param>
        /// <param name="r">Rotation of the chunk</param>
        /// <param name="pt">Position of the volcano</param>
        /// <returns>Return if we can put a chunk at the pt position with r rotation</returns>
        public bool PossibleVolcano(Vector2Int left, Vector2Int right, Rotation r, Vector2Int pt)
        {
            int level = WorldMap[pt].ParentCunk.Level;

            if (WorldMap.IsVoid(left) || WorldMap.IsVoid(right))
                return false;

            Cell leftCell = WorldMap[left];
            Cell rightCell = WorldMap[right];

            if (leftCell.ParentCunk.Level != level || rightCell.ParentCunk.Level != level)
                return false;

            if (WorldMap[pt].ParentCunk.rotation == r)
                return false;

            

            if (!leftCell.ContainsBuilding() && !rightCell.ContainsBuilding())
                return true;
            if (leftCell.ContainsBuilding() && !rightCell.ContainsBuilding() &&
                GetVillage(left).Count > 1) {
                return leftCell.ActualBuildings == Building.Barrack;
            }
                

            if (!leftCell.ContainsBuilding() && rightCell.ContainsBuilding() &&
                GetVillage(right).Count > 1)
                return rightCell.ActualBuildings == Building.Barrack;

            if (leftCell.ContainsBuilding() && rightCell.ContainsBuilding() &&
                GetVillage(left).Count > 2)
                return leftCell.ActualBuildings == Building.Barrack && rightCell.ActualBuildings == Building.Barrack;

            return false;
        }

        /// <summary>
        /// Find all the slots where a chunk can be placed
        /// </summary>
        /// <returns>Return the different position allowed for a chunk</returns>
        public PointRotation[] GetChunkSlots()
        {
            if (WorldMap.Empty) {
                PointRotation[] pr = new PointRotation[1];
                pr[0] = new(new(0, 0));
                pr[0].SetAllTrue();
                return pr;
            }

            List<Vector2Int> slots = new();
            List<PointRotation> chunkSlots = new();

            foreach (Cell c in WorldMap) {
                Vector2Int p = GetCellCoord(c);
                if (c.ActualBiome == Biomes.Volcano) {
                    slots.Add(p);
                }
                    

                Vector2Int[] neighbors = GetNeighbors(p);

                foreach (Vector2Int neighbor in neighbors.Where(n => WorldMap.IsVoid(n))) {
                    if (!slots.Contains(neighbor))
                        slots.Add(neighbor);

                    slots.AddRange(GetNeighbors(neighbor)
                        .Where(n => WorldMap.IsVoid(n) && !slots.Contains(n)));
                }
            }

            List<Vector2Int> pointRemove = new();

            //Recherche des points dans l'eau pouvant placer un chunk dans au moins une position
            foreach (Vector2Int pt in slots) {
                if (!WorldMap.IsVoid(pt)) {
                    if (WorldMap[pt].ActualBiome == Biomes.Volcano)
                        continue;

                    

                    pointRemove.Add(pt);
                    continue;
                }


                bool[] rotations = GetPossibleRotation(pt);
                PointRotation pr = new(pt);

                for (int i = 0; i < rotations.Length; i++)
                    if (rotations[i])
                        pr.AddRotation((Rotation)i);

                if(pr.HaveRotation())
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

                Rotation[] rotations = (Rotation[])Enum.GetValues(typeof(Rotation));

                foreach (Rotation r in rotations) {
                    int right = (int)r, left = right - 1;
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

        /// <summary>
        /// Add the cell to the worldmap
        /// </summary>
        /// <param name="c">Chunk to placed</param>
        /// <param name="p">Position chosen</param>
        /// <param name="left">Left position of the cell</param>
        /// <param name="right">Right position of the cell</param>
        private void AddCell(Chunk c, PointRotation p, Vector2Int left, Vector2Int right)
        {
            WorldMap.Add(c.Coords[0], p.point);
            WorldMap.Add(c.Coords[1], left);
            WorldMap.Add(c.Coords[2], right);
        }

        /// <summary>
        /// Add the chunk to the worldmap
        /// </summary>
        /// <param name="c">Chunk to placed</param>
        /// <param name="player">Actual player</param>
        /// <param name="p">Position chosen in the GetChunkSlots</param>
        /// <param name="r">Rotation chosen</param>
        public bool AddChunk(Chunk c, Player player, PointRotation p, Rotation r)
        {
            if (!GetChunkSlots()
                    .Where(pr => pr.point.Equals(p.point))
                    .Any(pr => pr.rotations.Where((t, i) => t == p.rotations[i]).Any())) {
                return false;
            }

            player.lastChunk = c;

            Vector2Int[] neighbors = GetNeighbors(p.point);

            if (p.rotations[(int)Rotation.N] && r == Rotation.N)
                AddCell(c, p, neighbors[0], neighbors[5]);
            else if (p.rotations[(int)Rotation.S] && r == Rotation.S)
                AddCell(c, p, neighbors[3], neighbors[2]);
            else if (p.rotations[(int)Rotation.NE] && r == Rotation.NE)
                AddCell(c, p, neighbors[1], neighbors[0]);
            else if (p.rotations[(int)Rotation.SE] && r == Rotation.SE)
                AddCell(c, p, neighbors[2], neighbors[1]);
            else if (p.rotations[(int)Rotation.SW] && r == Rotation.SW)
                AddCell(c, p, neighbors[4], neighbors[3]);
            else if (p.rotations[(int)Rotation.NW] && r == Rotation.NW)
                AddCell(c, p, neighbors[5], neighbors[4]);

            c.rotation = r;
            return true;
        }

        /// <summary>
        /// Place the building on the worldmap
        /// </summary>
        /// <param name="c">Cell where the building will be places</param>
        /// <param name="b">Building to placed</param>
        /// <param name="player">Actual player</param>
        public bool PlaceBuilding(Cell c, Building b, Player player)
        {
            if (c == null || player == null) {
                Debug.Log("Oh no!");
                return false;
            }
                

            Vector2Int coord = GetCellCoord(c);

            void SetC()
            {
                c.Owner = player.ID;
                c.Build(b);
            }

            Vector2Int[] tmp = GetBarrackSlots();
            List<Vector2Int> tmp2 = FindBiomesAroundVillage(GetCellCoord(c), player);

            switch (b)
            {
                case Building.Barrack when tmp2.All(t => tmp.Contains(t)):
                    foreach (Vector2Int v in tmp2)
                    {
                        SetC();
                        player.nbBarrack -= c.ParentCunk.Level;
                    }
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
                    return false;
            }

            return true;
        }

        public List<Vector2Int> FindBiomesAroundVillage(Vector2Int cell, Player player)
        {
            List<List<Vector2Int>> allVillages = GetAllVillage(cell);
            List<Vector2Int> sameBiome = new() { cell };
            Biomes biomes = WorldMap[cell].ActualBiome;

            foreach (List<Vector2Int> villages in allVillages) {
                if (WorldMap[villages[0]].Owner != player.ID)
                    continue;
                foreach (Vector2Int c in villages) {
                    Vector2Int[] neighbors = GetNeighbors(c);
                    foreach (Vector2Int neighbor in neighbors) {
                        if (WorldMap.IsVoid(neighbor)) continue;
                        if (WorldMap[neighbor].ActualBuildings != Building.None) continue;

                        if (WorldMap[neighbor].ActualBiome == biomes)
                            if (!sameBiome.Contains(neighbor))
                                sameBiome.Add(neighbor);
                    }
                }
            }
            return sameBiome;
        }

        /// <summary>
        /// Find all the slots for the barrack
        /// </summary>
        /// <returns>Return the possible possition for the barrack</returns>
        public Vector2Int[] GetBarrackSlots() => WorldMap
            .Select(GetCellCoord)
            .Where(p => !WorldMap.IsVoid(p) && WorldMap[p].ActualBuildings == Building.None &&
                        WorldMap[p].ActualBiome != Biomes.Volcano)
            .ToArray();

        /// <summary>
        /// Find all the slots for the tower
        /// </summary>
        /// <param name="actualPlayer">Actual player</param>
        /// <returns>Return the possible position for a tower for the player actualPlayer</returns>
        public Vector2Int[] GetTowerSlots(Player actualPlayer) => WorldMap
            .Select(GetCellCoord)
            .Where(p => !WorldMap.IsVoid(p) && WorldMap[p].ActualBuildings == Building.None && WorldMap[p].IsBuildable)
            .Where(p => WorldMap[p].ParentCunk.Level >= 3)
            .Where(p => IsAdjacentToCity(p, actualPlayer))
            .Where(p => !GetAllVillage(p)
                .Where(village => WorldMap[village[0]].Owner == actualPlayer.ID)
                .Any(CityHasTower)).ToArray();

        /// <summary>
        /// Check if a position is adjacent to a city
        /// </summary>
        /// <param name="cellCoord">Cell position</param>
        /// <param name="actualPlayer">Actual Player</param>
        /// <returns>Return if the position cellCoord is adjacent to a city of the player actualPlayer</returns>
        public bool IsAdjacentToCity(Vector2Int cellCoord, Player actualPlayer)
        {
            List<List<Vector2Int>> allvillage = GetAllVillage(cellCoord);
            return allvillage.Count > 0 &&
                   allvillage.Any(village => WorldMap[village[0]].Owner == actualPlayer.ID);
        }

        /// <summary>
        /// Check if a city has a tower
        /// </summary>
        /// <param name="village">Position of the different building of the village</param>
        /// <returns>Return if the village has a tower</returns>
        public bool CityHasTower(List<Vector2Int> village) =>
            village.Any(vp => WorldMap[vp].ActualBuildings == Building.Tower);

        /// <summary>
        /// Find all the slots for the temple
        /// </summary>
        /// <param name="actualPlayer">Actual Player</param>
        /// <returns>Return the different position for a temple for the player actualPlayer</returns>
        public Vector2Int[] GetTempleSlots(Player actualPlayer) => WorldMap
            .Where(cell => CanBuildTemple(cell, actualPlayer))
            .Select(GetCellCoord)
            .ToArray();

        /// <summary>
        /// Check if we can build a temple on a cell
        /// </summary>
        /// <param name="cell">Cell where we want to build a temple</param>
        /// <param name="actualPlayer">Actual Player</param>
        /// <returns>Return if we can build a temple on the cell cell for the player actualPlayer</returns>
        public bool CanBuildTemple(Cell cell, Player actualPlayer) =>
            cell.ActualBuildings == Building.None && cell.IsBuildable &&
            IsAdjacentToCity(GetCellCoord(cell), actualPlayer) &&
            GetAllVillage(GetCellCoord(cell))
                .All(village =>
                    WorldMap[village[0]].Owner == actualPlayer.ID && !CityHasTemple(village) &&
                    village.Count >= 3);

        /// <summary>
        /// Check if a city has a temple
        /// </summary>
        /// <param name="village">Position of the different building of the village</param>
        /// <returns>Return if the city has a temple</returns>
        public bool CityHasTemple(List<Vector2Int> village) =>
            village.Any(vp => WorldMap[vp].ActualBuildings == Building.Temple);

        /// <summary>
        /// Find the position of a cell.
        /// </summary>
        /// <param name="c">Cell whose position we want</param>
        /// <returns>Return the position (x,y) of the cell</returns>
        public Vector2Int GetCellCoord(Cell c)
        {
            for (int i = WorldMap.MinLine; i <= WorldMap.MaxLine; i++) {
                if (!WorldMap.ContainsLine(i))
                    continue;

                for (int j = WorldMap.MinColumn(i); j <= WorldMap.MaxColumn(i); j++)
                    if (WorldMap.ContainsColumn(i, j) && WorldMap[new(i, j)] == c)
                        return new(i, j);
            }

            throw new($"Cell {c} not found!");
        }

        public void SetChunkLevel(PointRotation pr)
        {
            WorldMap[pr.point].ParentCunk.Level++;
        }
    }
}