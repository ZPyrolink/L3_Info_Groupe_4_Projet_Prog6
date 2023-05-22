using System;
using System.Collections.Generic;
using System.Linq;

using Taluva.Utils;

using UnityEngine;

namespace Taluva.Model
{
    /// <summary>
    /// Represents the game board.
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Gets the world map represented as a dynamic matrix of cells.
        /// </summary>
        public DynamicMatrix<Cell> WorldMap { get; }

        /// <summary>
        /// Initializes a new instance of the Board class.
        /// </summary>
        public Board()
        {
            WorldMap = new DynamicMatrix<Cell>();
        }

        /// <summary>
        /// Initializes a new instance of the Board class from an existing board.
        /// </summary>
        /// <param name="b">The existing board to copy.</param>
        public Board(Board b)
        {
            WorldMap = new DynamicMatrix<Cell>(b.WorldMap);
        }

        /// <summary>
        /// Removes a chunk from the map.
        /// </summary>
        /// <param name="positions">The positions of the cells to remove.</param>
        public void RemoveChunk(Vector2Int[] positions)
        {
            foreach (Vector2Int p in positions)
                WorldMap.Remove(p);
        }

        /// <summary>
        /// Checks if a position contains any building.
        /// </summary>
        /// <param name="pos">The cell position to check.</param>
        /// <returns><c>true</c> if the position is not void and has a non-<see cref="Building.None"/> building, otherwise <c>false</c>.</returns>
        private bool ContainsBuild(Vector2Int pos) => !WorldMap.IsVoid(pos) &&
                                                      WorldMap[pos].ActualBuildings != Building.None;

        /// <summary>
        /// Finds all the positions of a village.
        /// </summary>
        /// <param name="c">The starting cell of the village.</param>
        /// <returns>Returns all the positions of the village.</returns>
        public List<Vector2Int> GetVillage(Vector2Int c)
        {
            PlayerColor color = WorldMap[c].Owner;

            bool TestNeighbor(Vector2Int neighbor) =>
                ContainsBuild(neighbor) && WorldMap[neighbor].Owner == color;

            Vector2Int[] neighbors = GetNeighbors(c);
            List<Vector2Int> cells = neighbors.Where(TestNeighbor).ToList();

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

        /// <summary>
        /// Finds all the villages around a position.
        /// </summary>
        /// <param name="c">A cell position.</param>
        /// <returns>Returns all the villages around the cell position.</returns>
        public List<List<Vector2Int>> GetAllVillage(Vector2Int c)
        {
            Vector2Int[] neighbors = GetNeighbors(c);

            List<List<Vector2Int>> villages = neighbors.Where(ContainsBuild).Select(GetVillage).ToList();

            for (int i = 0; i < villages.Count - 1; i++)
                for (int j = i + 1; j < villages.Count; j++)
                    if (villages[i].Contains(villages[j][0]))
                    {
                        villages.RemoveAt(j);
                        j--;
                    }

            return villages;
        }

        /// <summary>
        /// Gets the coordinates of the chunks based on the position and rotation.
        /// </summary>
        /// <param name="c">The position of the chunk.</param>
        /// <param name="r">The rotation of the chunk.</param>
        /// <returns>Returns the coordinates of the cells in the chunk.</returns>
        public Vector2Int[] GetChunksCoords(Vector2Int c, Rotation r)
        {
            Vector2Int[] cells = new Vector2Int[3];
            cells[0] = c;

            Vector2Int[] neighbors = GetNeighbors(c);

            switch (r)
            {
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
        /// Gets the neighbors of a position.
        /// </summary>
        /// <param name="p">The position of the cell.</param>
        /// <returns>Returns the neighboring positions in a clockwise order.</returns>
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
        /// Gets the possible rotations for a chunk at a given position.
        /// </summary>
        /// <param name="p">The position to check.</param>
        /// <returns>Returns a boolean array indicating the possible rotations.</returns>
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

        /// <summary>
        /// Checks if a position is connected to the island.
        /// </summary>
        /// <param name="p">The position to check.</param>
        /// <returns>Returns <c>true</c> if the position has a neighboring cell, otherwise <c>false</c>.</returns>
        public bool IsConnected(Vector2Int p) => GetNeighbors(p).Any(neighbor => !WorldMap.IsVoid(neighbor));

        /// <summary>
        /// Checks if a chunk can be placed on a volcano.
        /// </summary>
        /// <param name="left">The left position of the chunk.</param>
        /// <param name="right">The right position of the chunk.</param>
        /// <param name="r">The rotation of the chunk.</param>
        /// <param name="pt">The position of the volcano.</param>
        /// <returns>Returns <c>true</c> if the chunk can be placed at the given position and rotation, otherwise <c>false</c>.</returns>
        public bool PossibleVolcano(Vector2Int left, Vector2Int right, Rotation r, Vector2Int pt)
        {
            int level = WorldMap[pt].ParentChunk.Level;

            if (WorldMap.IsVoid(left) || WorldMap.IsVoid(right))
                return false;

            Cell leftCell = WorldMap[left];
            Cell rightCell = WorldMap[right];

            if (leftCell.ParentChunk.Level != level || rightCell.ParentChunk.Level != level)
                return false;

            if (WorldMap[pt].ParentChunk.rotation == r)
                return false;


            if (!leftCell.ContainsBuilding() && !rightCell.ContainsBuilding())
                return true;

            if (leftCell.ContainsBuilding() && !rightCell.ContainsBuilding() &&
                GetVillage(left).Count > 1)
                return leftCell.ActualBuildings == Building.Barrack;

            if (!leftCell.ContainsBuilding() && rightCell.ContainsBuilding() &&
                GetVillage(right).Count > 1)
                return rightCell.ActualBuildings == Building.Barrack;

            if (leftCell.ContainsBuilding() && rightCell.ContainsBuilding())
                if ((leftCell.Owner == rightCell.Owner && GetVillage(left).Count > 2) ||
                    leftCell.Owner != rightCell.Owner && GetVillage(left).Count >= 2 && GetVillage(right).Count >= 2)
                    return leftCell.ActualBuildings == Building.Barrack && rightCell.ActualBuildings == Building.Barrack;


            return false;
        }

        /// <summary>
        /// Finds all the slots where a chunk can be placed.
        /// </summary>
        /// <returns>Returns the different positions allowed for a chunk.</returns>
        public PointRotation[] GetChunkSlots()
        {
            if (WorldMap.Empty)
            {
                PointRotation[] pr = new PointRotation[1];
                pr[0] = new PointRotation(new(0, 0));
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

            foreach (Vector2Int pt in slots)
            {
                if (!WorldMap.IsVoid(pt))
                {
                    if (WorldMap[pt].ActualBiome == Biomes.Volcano)
                        continue;

                    pointRemove.Add(pt);
                    continue;
                }


                bool[] rotations = GetPossibleRotation(pt);
                PointRotation pr = new PointRotation(pt);

                for (int i = 0; i < rotations.Length; i++)
                    if (rotations[i])
                        pr.AddRotation((Rotation)i);

                if (pr.HaveRotation())
                    chunkSlots.Add(pr);

                pointRemove.Add(pt);
            }

            foreach (Vector2Int pr in pointRemove)
                slots.Remove(pr);

            foreach (Vector2Int pt in slots)
            {
                Vector2Int[] neighbors = GetNeighbors(pt);
                PointRotation pr = new PointRotation(pt);

                Rotation[] rotations = (Rotation[])Enum.GetValues(typeof(Rotation));

                foreach (Rotation r in rotations)
                {
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
        /// Adds a cell to the world map.
        /// </summary>
        /// <param name="c">The chunk to be placed.</param>
        /// <param name="p">The chosen position.</param>
        /// <param name="left">The left position of the cell.</param>
        /// <param name="right">The right position of the cell.</param>
        private void AddCell(Chunk c, PointRotation p, Vector2Int left, Vector2Int right)
        {
            WorldMap.Add(c.Coords[0], p.point);
            WorldMap.Add(c.Coords[1], left);
            WorldMap.Add(c.Coords[2], right);
        }

        /// <summary>
        /// Adds a chunk to the world map.
        /// </summary>
        /// <param name="c">The chunk to be placed.</param>
        /// <param name="player">The current player.</param>
        /// <param name="p">The chosen position from the GetChunkSlots method.</param>
        /// <param name="r">The chosen rotation.</param>
        /// <returns>Returns <c>true</c> if the chunk has been placed, otherwise <c>false</c>.</returns>
        public bool AddChunk(Chunk c, Player player, PointRotation p, Rotation r)
        {
            if (!GetChunkSlots()
                    .Where(pr => pr.point.Equals(p.point))
                    .Any(pr => pr.rotations.Where((t, i) => t == p.rotations[i]).Any()))
            {
                return false;
            }

            if (!WorldMap.IsVoid(p.point))
                c.Level = WorldMap[p.point].ParentChunk.Level + 1;

            player.LastChunk = c;

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
        /// Places a building on the world map.
        /// </summary>
        /// <param name="c">The cell where the building will be placed.</param>
        /// <param name="b">The building to be placed.</param>
        /// <param name="player">The current player.</param>
        /// <returns>Returns <c>true</c> if the building has been placed, otherwise <c>false</c>.</returns>
        public bool PlaceBuilding(Cell c, Building b, Player player)
        {
            if (c == null || player == null)
                return false;

            Vector2Int coord = GetCellCoord(c);

            void SetC(Cell cell)
            {
                cell.Owner = player.ID;
                cell.Build(b);
            }

            Vector2Int[] tmp = GetBarrackSlots(player);
            List<Vector2Int> tmp2 = FindBiomesAroundVillage(GetCellCoord(c), player);

            if (b == Building.Barrack)
            {
                int nbBarrack = 0;
                foreach (Vector2Int v in tmp2)
                {
                    nbBarrack += WorldMap[v].ParentChunk.Level;
                }
                if (nbBarrack > player.NbBarrack)
                    return false;
            }

            switch (b)
            {
                case Building.Barrack when tmp2.All(t => tmp.Contains(t)):
                    foreach (Vector2Int v in tmp2)
                    {
                        SetC(WorldMap[v]);
                        player.NbBarrack -= WorldMap[v].ParentChunk.Level;
                    }
                    break;
                case Building.Temple when GetTempleSlots(player).Contains(coord):
                    SetC(c);
                    player.NbTemple--;
                    break;
                case Building.Tower when GetTowerSlots(player).Contains(coord):
                    SetC(c);
                    player.NbTowers--;
                    break;
                default:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Finds the biomes around a village.
        /// </summary>
        /// <param name="cell">The cell of the village.</param>
        /// <param name="player">The player.</param>
        /// <returns>Returns a list of positions representing the biomes around the village.</returns>
        public List<Vector2Int> FindBiomesAroundVillage(Vector2Int cell, Player player)
        {
            List<List<Vector2Int>> allVillages = GetAllVillage(cell);
            List<Vector2Int> sameBiome = new() { cell };
            Biomes biomes = WorldMap[cell].ActualBiome;

            sameBiome.AddRange(allVillages
                .Where(villages => WorldMap[villages[0]].Owner == player.ID)
                .SelectMany(villages => villages, (_, c) => GetNeighbors(c))
                .SelectMany(
                    neighbors => neighbors
                        .Where(n => !WorldMap.IsVoid(n) && WorldMap[n].ActualBuildings == Building.None),
                    (_, neighbor) => neighbor)
                .Where(n => WorldMap[n].ActualBiome == biomes && !sameBiome.Contains(n)));

            return sameBiome;
        }

        /// <summary>
        /// Finds the slots for the barrack.
        /// </summary>
        /// <returns>Returns the possible positions for the barrack.</returns>
        public Vector2Int[] GetBarrackSlots(Player player) => WorldMap
            .Select(GetCellCoord)
            .Where(p => !WorldMap.IsVoid(p) && (WorldMap[p].ParentChunk.Level == 1 || IsAdjacentToCity(p, player))
                        && WorldMap[p].ActualBuildings == Building.None &&
                        WorldMap[p].ActualBiome != Biomes.Volcano && WorldMap[p].ParentChunk.Level <= player.NbBarrack)
            .ToArray();

        /// <summary>
        /// Finds the slots for the tower.
        /// </summary>
        /// <param name="actualPlayer">The current player.</param>
        /// <returns>Returns the possible positions for a tower for the current player.</returns>
        public Vector2Int[] GetTowerSlots(Player actualPlayer) => WorldMap
            .Select(GetCellCoord)
            .Where(p => !WorldMap.IsVoid(p) && WorldMap[p].ActualBuildings == Building.None && WorldMap[p].IsBuildable)
            .Where(p => WorldMap[p].ParentChunk.Level >= 3)
            .Where(p => IsAdjacentToCity(p, actualPlayer))
            .Where(p => !GetAllVillage(p)
                .Where(village => WorldMap[village[0]].Owner == actualPlayer.ID)
                .Any(CityHasTower))
            .ToArray();

        /// <summary>
        /// Checks if a position is adjacent to a city.
        /// </summary>
        /// <param name="cellCoord">The cell position.</param>
        /// <param name="actualPlayer">The current player.</param>
        /// <returns>Returns <c>true</c> if the position is adjacent to a city of the current player, otherwise <c>false</c>.</returns>
        public bool IsAdjacentToCity(Vector2Int cellCoord, Player actualPlayer)
        {
            List<List<Vector2Int>> allvillage = GetAllVillage(cellCoord);
            return allvillage.Count > 0 &&
                   allvillage.Any(village => WorldMap[village[0]].Owner == actualPlayer.ID);
        }

        /// <summary>
        /// Checks if a city has a tower.
        /// </summary>
        /// <param name="village">The position of the different buildings of the city.</param>
        /// <returns>Returns <c>true</c> if the city has a tower, otherwise <c>false</c>.</returns>
        public bool CityHasTower(List<Vector2Int> village) =>
            village.Any(vp => WorldMap[vp].ActualBuildings == Building.Tower);

        /// <summary>
        /// Finds the slots for the temple.
        /// </summary>
        /// <param name="actualPlayer">The current player.</param>
        /// <returns>Returns the possible positions for a temple for the current player.</returns>
        public Vector2Int[] GetTempleSlots(Player actualPlayer) => WorldMap
            .Where(cell => CanBuildTemple(cell, actualPlayer))
            .Select(GetCellCoord)
            .ToArray();

        /// <summary>
        /// Checks if a temple can be built on a cell.
        /// </summary>
        /// <param name="cell">The cell where the temple will be built.</param>
        /// <param name="actualPlayer">The current player.</param>
        /// <returns>Returns <c>true</c> if a temple can be built on the cell for the current player, otherwise <c>false</c>.</returns>
        public bool CanBuildTemple(Cell cell, Player actualPlayer)
        {
            if (!cell.IsBuildable)
                return false;

            List<List<Vector2Int>> allVillages = GetAllVillage(GetCellCoord(cell));

            if (allVillages.Count == 0) return false;

            bool build = false;

            foreach (List<Vector2Int> village in allVillages
                         .Where(village => WorldMap[village[0]].Owner == actualPlayer.ID))
            {
                if (CityHasTemple(village))
                    return false;

                if (village.Count >= 3)
                    build = true;
            }

            return build;
        }

        /// <summary>
        /// Checks if a city has a temple.
        /// </summary>
        /// <param name="village">The position of the different buildings of the city.</param>
        /// <returns>Returns <c>true</c> if the city has a temple, otherwise <c>false</c>.</returns>
        public bool CityHasTemple(List<Vector2Int> village) =>
            village.Any(vp => WorldMap[vp].ActualBuildings == Building.Temple);

        /// <summary>
        /// Finds the position of a cell.
        /// </summary>
        /// <param name="c">The cell whose position we want.</param>
        /// <returns>Returns the position (x, y) of the cell.</returns>
        public Vector2Int GetCellCoord(Cell c)
        {
            for (int i = WorldMap.MinLine; i <= WorldMap.MaxLine; i++)
            {
                if (!WorldMap.ContainsLine(i))
                    continue;

                for (int j = WorldMap.MinColumn(i); j <= WorldMap.MaxColumn(i); j++)
                    if (WorldMap.ContainsColumn(i, j) && WorldMap[new(i, j)] == c)
                        return new(i, j);
            }

            throw new Exception($"Cell {c} not found!");
        }
        /// <summary>
        /// Set The level of the chunk.
        /// </summary>
        /// <param name="pr">The position of the chunk.</param>
        public void SetChunkLevel(PointRotation pr)
        {
            WorldMap[pr.point].ParentChunk.Level++;
        }
    }
}
