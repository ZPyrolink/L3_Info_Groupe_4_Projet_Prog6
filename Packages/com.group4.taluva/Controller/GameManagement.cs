using System;

using Taluva.Model;
using Taluva.Utils;
using Taluva.Model.AI;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;
using Codice.Client.Common.FsNodeReaders;
using UnityEngine.UIElements;
using System.IO;
using System.Text;
using System.Globalization;
using System.Linq;
using Codice.Client.GameUI.Explorer;

namespace Taluva.Controller
{
    public class GameManagment
    {
        private readonly string savePath = Directory.GetCurrentDirectory() + "/Save/";

        private Player[] players;
        public Player actualPlayer;
        private AI ActualAi => (AI)actualPlayer;
        public int NbPlayers { get; set; }
        public int actualTurn { get; private set; }
        public int maxTurn;
        public Board gameBoard;
        private TurnPhase actualPhase = TurnPhase.SelectCells;
        private Historic<Coup> historic;
        public Pile<Chunk> pile = ListeChunk.Pile;
        public Chunk actualChunk;
        private bool AIRandom = false;
        private Player Winner { get; set; }
        private GameEnd victoryType { get; set; }
        //Actions
        //Notify phase change
        public Action<TurnPhase> ChangePhase { get; set; }
        private void OnChangePhase(TurnPhase phase) => ChangePhase?.Invoke(phase);

        //Notify end of game
        public Action<Player, GameEnd> NotifyEndGame { get; set; }
        private void OnEndGame(Player winner, GameEnd victory) => NotifyEndGame?.Invoke(winner, victory);

        //Notify where the AI places the chunk
        public Action<PointRotation> NotifyAIChunkPlacement { get; set; }
        private void OnAIChunkPlacement(PointRotation pr) => NotifyAIChunkPlacement?.Invoke(pr);
        //Notify where the AI places the building
        public Action<Building, Vector2Int> NotifyAIBuildingPlacement { get; set; }
        private void OnAIBuildingPlacement(Building b, Vector2Int pos) => NotifyAIBuildingPlacement?.Invoke(b, pos);

        //Notify player eliminated
        public Action<Player> NotifyPlayerEliminated { get; set; }
        private void OnPlayerElimination(Player p) => NotifyPlayerEliminated?.Invoke(p);

        public GameManagment(int nbPlayers)
        {
            historic = new();
            this.players = new Player[nbPlayers];
            this.actualTurn = 1;
            this.gameBoard = new();
            this.NbPlayers = nbPlayers;
            this.maxTurn = 12 * nbPlayers;
            for (int i = 0; i < this.NbPlayers; i++)
            {
                players[i] = new((PlayerColor)i);
                if (this.AIRandom && i == 1)
                {
                    players[i].playerIA = true;
                    break;
                }
            }
            actualPlayer = players[0];
        }

        public void setAI()
        {
            this.AIRandom = true;
            this.NbPlayers = 2;
        }
        public class Coup
        {
            //New Chunk or cells
            public Vector2Int[] positions;
            public Rotation? rotation;

            //Last Chunk or last cells
            public Cell[] cells;
            public Chunk chunk;
            public Player player;
            public Building[] building;

            private Coup(Vector2Int[] positions, Rotation? rotation, Player actualPlayer)
            {
                this.positions = positions;
                this.rotation = rotation;
                this.player = actualPlayer;
            }

            public Coup(Vector2Int[] positions, Rotation rotation, Player actualPlayer, Chunk chunk) : this(positions, rotation, actualPlayer)
            {
                this.chunk = chunk;
                this.cells = new Cell[1];
                cells[0] = null;
            }

            public Coup(Vector2Int[] positions, Rotation rotation, Player actualPlayer, Chunk chunk, Cell[] cells, Building[] b) : this(positions, rotation, actualPlayer, cells, b)
            {
                this.chunk = chunk;
            }

            public Coup(Vector2Int[] positions, Rotation? rotation, Player actualPlayer, Cell[] cells, Building[] b) : this(positions, rotation, actualPlayer)
            {
                this.cells = cells;
                building = b;
            }
        }

        /// <summary>
        /// Check if we can undo
        /// </summary>
        public bool CanUndo => historic.CanUndo;

        /// <summary>
        /// Check if we can redo
        /// </summary>
        public bool CanRedo => historic.CanRedo;

        public void Save(string path)
        {
            using (FileStream file = File.Open(savePath + path, FileMode.OpenOrCreate, FileAccess.Write))
            using (BinaryWriter writer = new(file))
            {
                writer.Write(NbPlayers);
                for (int i = 0; i < NbPlayers; i++)
                {
                    writer.Write((uint)players[i].ID);
                    writer.Write(players[i] is AI);
                    if (players[i] is AI)
                        writer.Write((int)((AI)players[i]).difficulty);
                }

                Chunk[] stackArray = pile._stack.ToArray();
                writer.Write(stackArray.Length + pile._played.Count - 2);

                for (int i = stackArray.Length - 1; i >= 0; i--)
                {
                    writer.Write((int)stackArray[i].Coords[1].ActualBiome);
                    writer.Write((int)stackArray[i].Coords[2].ActualBiome);
                }

                for (int i = pile._played.Count - 1; i >= 0; i--)
                {
                    writer.Write((int)pile._played[i].Coords[1].ActualBiome);
                    writer.Write((int)pile._played[i].Coords[2].ActualBiome);
                }

                writer.Write(historic.Count);
                for (int i = 0; i < historic.Count; i++)
                {
                    writer.Write(i == historic.Index);
                    if (i % 2 == 0)
                    {
                        writer.Write(historic[i].positions.Length);
                        for (int j = 0; j < historic[i].positions.Length; j++)
                        {
                            writer.Write(historic[i].positions[i].x);
                            writer.Write(historic[i].positions[i].y);
                        }
                        writer.Write((int)historic[i].rotation);
                        writer.Write((uint)historic[i].player.ID);
                        for (int j = 1; j < historic[i].chunk.Coords.Length; j++)
                        {
                            writer.Write((int)historic[i].chunk.Coords[j].ActualBiome);
                            writer.Write((int)historic[i].chunk.Coords[j].ActualBuildings);
                            if (historic[i].chunk.Coords[j].ActualBuildings != Building.None)
                                writer.Write((int)historic[i].chunk.Coords[j].Owner);
                        }
                        writer.Write((int)historic[i].chunk.rotation);
                        writer.Write((int)historic[i].chunk.Level);
                        writer.Write(historic[i].cells.Length > 0);
                        if (historic[i].cells.Length > 0)
                        {
                            for (int j = 0; j < historic[i].cells.Length; j++)
                            {
                                writer.Write((int)historic[i].cells[j].ActualBiome);
                                writer.Write((int)historic[i].cells[j].ActualBuildings);
                                if (historic[i].chunk.Coords[j].ActualBuildings != Building.None)
                                    writer.Write((int)historic[i].chunk.Coords[j].Owner);
                                writer.Write((int)historic[i].building[j]);
                            }
                        }
                    } else
                    {
                        writer.Write(historic[i].positions.Length);
                        for (int j = 0; j < historic[i].positions.Length; j++)
                        {
                            writer.Write(historic[i].positions[j].x);
                            writer.Write(historic[i].positions[j].y);
                        }
                        writer.Write((int)historic[i].player.ID);
                        for (int j = 0; j < historic[i].cells.Length; j++)
                        {
                            writer.Write((int)historic[i].cells[j].ActualBiome);
                            writer.Write((int)historic[i].cells[j].ActualBuildings);
                            writer.Write(historic[i].chunk.Coords[j].ActualBuildings != Building.None);
                            if (historic[i].chunk.Coords[j].ActualBuildings != Building.None)
                                writer.Write((int)historic[i].chunk.Coords[j].Owner);
                            writer.Write((int)historic[i].building[j]);
                        }
                    }
                }
            }
        }

        public GameManagment(string path)
        {
            using (FileStream file = File.Open(savePath + path, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new(file))
            {
                this.NbPlayers = reader.ReadInt16();
                this.maxTurn = 12 * NbPlayers;
                this.actualTurn = 1;
                this.gameBoard = new();
                actualPlayer = players[0];
                this.players = new Player[this.NbPlayers];
                for (int i = 0; i < NbPlayers; i++)
                {
                    PlayerColor id = (PlayerColor)reader.ReadInt16();
                    bool ia = reader.ReadBoolean();
                    if (ia)
                    {
                        Difficulty d = (Difficulty)reader.ReadInt16();
                        switch (d)
                        {
                            //a completer en declarant les nouvelles classes d'ia
                            case Difficulty.BadPlayer:
                                players[i] = new AIRandom(id, this);
                                break;
                            case Difficulty.Normal:
                                throw new NotImplementedException();
                                break;
                            case Difficulty.SkillIssue:
                                throw new NotImplementedException();
                                break;
                        }

                    } else
                    {
                        players[i] = new(id);
                    }
                }

                List<Chunk> chunks = new();

                for (int i = 0; i < reader.ReadInt16(); i++)
                {
                    Cell left = new((Biomes)reader.ReadInt16());
                    Cell right = new((Biomes)reader.ReadInt16());
                    Chunk c = new(1, left, right);
                    chunks.Add(c);
                }

                pile = new(chunks.ToArray());

                int historicCount = reader.ReadInt16();
                for (int i = 0; i < historicCount; i++)
                {
                    bool index = reader.ReadBoolean();
                    if (!index)
                    {
                        if(i % 2 == 0)
                        {
                            this.actualChunk = pile.Draw();
                            int nbTiles = reader.ReadInt16();
                            Vector2Int[] positions = new Vector2Int[nbTiles];
                            for(int j = 0; i < nbTiles; j++)
                            {
                                positions[i] = new(reader.ReadInt16(), reader.ReadInt16());
                            }
                            Rotation r = (Rotation)reader.ReadInt16();
                            ValidateTile(new(positions[0], r), r);
                        } else
                        {
                            int nbTiles = reader.ReadInt16();
                            Vector2Int[] positions = new Vector2Int[nbTiles];
                            for(int j = 0; i < nbTiles; j++)
                            {
                                positions[i] = new(reader.ReadInt16(), reader.ReadInt16());
                            }
                            int playerID = reader.ReadInt16();
                            Building[] b = new Building[nbTiles];
                            for(int j = 0;  j < nbTiles; j++)
                            {
                                reader.ReadInt16();
                                reader.ReadInt16();
                                bool bl = reader.ReadBoolean();
                                if(bl)
                                    reader.ReadInt16();
                                b[i] = (Building)reader.ReadInt16();
                            }
                            ValidateBuilding(gameBoard.WorldMap[positions[0]], b[0]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Use this when you're in phase 1 during the placement of the chunk.
        /// It will also store the possible cells under the chunk.
        /// </summary>
        /// <param name="position">Position of the chunk</param>
        /// <param name="rotation">Rotation of the chunk</param>
        /// <param name="chunk">The chunk played at the round</param>
        public void AddHistoric(Vector2Int position, Rotation rotation, Chunk chunk)
        {
            if (!gameBoard.WorldMap.IsVoid(position))
            {
                Vector2Int[] positions = gameBoard.GetChunksCoords(position, rotation);
                Cell[] newCells = new Cell[gameBoard.WorldMap[position].ParentCunk.Coords.Length];
                Building[] buildings = new Building[gameBoard.WorldMap[position].ParentCunk.Coords.Length];
                for (int i = 0; i < gameBoard.WorldMap[position].ParentCunk.Coords.Length; i++)
                {
                    newCells[i] = new(gameBoard.WorldMap[positions[i]].ParentCunk.Coords[i]);
                    buildings[i] = gameBoard.WorldMap[positions[i]].ActualBuildings;
                }
                historic.Add(new(gameBoard.GetChunksCoords(position, rotation), rotation, actualPlayer, new(chunk), newCells, buildings));
            } else
            {
                historic.Add(new(new[] { position }, rotation, actualPlayer, new(chunk)));
            }

        }

        /// <summary>
        /// Use this when you're in phase 2 during the placement of a building.
        /// It will store the different information on the cells.
        /// </summary>
        /// <param name="positions">Buildings positions</param>
        /// <param name="cells">Cells with the buildings</param>
        public void AddHistoric(Vector2Int[] positions, Cell[] cells, Building b)
        {
            Cell[] newCells = new Cell[cells.Length];
            Building[] buildings = new Building[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                newCells[i] = new(cells[i]);
                buildings[i] = b;
            }
            historic.Add(new(positions, null, actualPlayer, newCells, buildings));
        }

        public void PrecedentPhase()
        {
            if (actualPhase != TurnPhase.IAPlays)
            {
                int precedantPhaseValue = ((int)actualPhase - 1) % Enum.GetNames(typeof(TurnPhase)).Length - 1;
                actualPhase = (TurnPhase)precedantPhaseValue;
                OnChangePhase(actualPhase);
            } else
            {
                actualPhase = TurnPhase.NextPlayer; //Change Player ?
                OnChangePhase(actualPhase);
            }

        }

        public void NextPhase()
        {

            if (actualPhase != TurnPhase.IAPlays)
            {
                int nextPhaseValue = ((int)actualPhase + 1) % Enum.GetNames(typeof(TurnPhase)).Length - 1;
                actualPhase = (TurnPhase)nextPhaseValue;
                OnChangePhase(actualPhase);
            } else
            {
                actualPhase = TurnPhase.NextPlayer;
                OnChangePhase(actualPhase);
            }

        }

        public Coup Undo()
        {
            if (!historic.CanUndo)
                return null;

            Coup c = historic.Undo();
            if (c.chunk != null)
            {
                gameBoard.RemoveChunk(c.chunk);
                if (c.cells[0] != null)
                {
                    for (int i = 0; i < c.cells.Length; i++)
                    {
                        gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                        gameBoard.PlaceBuilding(c.cells[i], c.building[i], actualPlayer);
                    }
                }
                pile.Stack(c.chunk);
            } else
            {
                actualPlayer = c.player;
                for (int i = 0; i < c.cells.Length; i++)
                {
                    switch (c.building[i])
                    {
                        case Building.None:
                            break;
                        case Building.Temple:
                            actualPlayer.nbTemple++; ;
                            break;
                        case Building.Tower:
                            actualPlayer.nbTowers++;
                            break;
                        case Building.Barrack:
                            actualPlayer.nbBarrack += gameBoard.WorldMap[c.positions[i]].ParentCunk.Level;
                            break;
                    }
                    gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                }
            }
            PrecedentPhase();
            return c;
        }

        public Coup Redo()
        {
            if (!historic.CanRedo)
                return null;

            Coup c = historic.Redo();
            if (c.chunk == null)
            {
                for (int i = 0; i < c.cells.Length; i++)
                {
                    gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                    gameBoard.PlaceBuilding(c.cells[i], c.building[i], actualPlayer);
                }
            } else
            {
                gameBoard.AddChunk(c.chunk, c.player, new(c.positions[0], (Rotation)c.rotation), (Rotation)c.rotation);
                pile.Draw();
            }
            for (int i = 0; i < NbPlayers; i++)
                if (actualPlayer == players[i])
                    actualPlayer = players[i % NbPlayers];

            NextPhase();
            return c;
        }

        public Player CheckWinner()
        {
            Player p;
            if (maxTurn == 0)
            {
                p = NormalEnd;
                this.victoryType = GameEnd.NormalEnd;
                OnEndGame(p, GameEnd.NormalEnd);
            }

            Player tmp = EarlyEnd;
            if (tmp != null)
            {
                p = tmp;
                this.victoryType = GameEnd.EarlyEnd;
                OnEndGame(p, GameEnd.EarlyEnd);
            }
            var tmp2 = players.Where(p => !p.Eliminated);

            if (tmp2.Count() == 1)
            {
                OnEndGame(tmp2.First(), GameEnd.LastPlayerStanding);
            }
            return null;
        }

        private Player EarlyEnd
        {
            get
            {
                foreach (Player p in players)
                {
                    int completedBuildingTypes = 0;

                    if (gameBoard.GetTempleSlots(p).Length == p.nbTemple)
                        completedBuildingTypes++;

                    if (gameBoard.GetBarrackSlots(p).Length == p.nbBarrack)
                        completedBuildingTypes++;

                    if (gameBoard.GetTowerSlots(p).Length == p.nbTowers)
                        completedBuildingTypes++;

                    if (completedBuildingTypes >= 2)
                        return p;
                }

                throw new NotImplementedException();
            }
        }
        private Player NormalEnd
        {
            get
            {
                int maxTemple = 0;
                Player winner1 = null;
                foreach (Player p in players)
                {
                    if (p.nbBarrack == 0)
                        continue;

                    if (p.nbTemple > maxTemple)
                    {
                        maxTemple = p.nbTemple;
                        winner1 = p;
                    }
                }

                int egalityTemple = 0;
                foreach (Player p in players)
                {
                    if (p.nbBarrack == 0)
                        continue;
                    if (p.nbTemple == maxTemple)
                        egalityTemple++;
                }

                if (egalityTemple >= 2)
                {
                    int maxTower = 0;
                    Player winner2 = null;
                    foreach (Player p in players)
                    {
                        if (p.nbBarrack == 0)
                            continue;
                        if (p.nbTemple > maxTower)
                        {
                            maxTower = p.nbTemple;
                            winner2 = p;
                        }
                    }

                    int egalityTower = 0;
                    foreach (Player p in players)
                    {
                        if (p.nbBarrack == 0)
                            continue;
                        if (p.nbTemple == maxTemple)
                            egalityTower++;
                    }

                    if (egalityTower >= 2)
                    {
                        int maxBarrack = 0;
                        Player winner3 = null;
                        foreach (Player p in players)
                        {
                            if (p.nbBarrack == 0)
                                continue;
                            if (p.nbTemple > maxBarrack)
                            {
                                maxBarrack = p.nbTemple;
                                winner3 = p;
                            }
                        }

                        return winner3;
                    } else
                    {
                        return winner2;
                    }
                } else
                {
                    return winner1;
                }
            }
        }

        public void InitPlay()
        {
            if (CheckWinner() != null)
            {
                return;
            }
            PlayerEliminated();
            actualTurn++;
            this.actualChunk = pile.Draw();
            if (actualTurn + 1 > NbPlayers)
            {
                actualTurn = 0;
            }
            actualPlayer = players[actualTurn];

            if (actualPlayer.Eliminated)
            {
                actualTurn++;
                if (actualTurn + 1 > NbPlayers)
                {
                    actualTurn = 0;
                }
                actualPlayer = players[actualTurn];
            }

            if (actualPlayer is AI ai)
            {
                actualPhase = TurnPhase.IAPlays;
                AIMove(ai);
            } else
            {
                NextPhase();
            }
        }
        public void PlayerEliminated()
        {
            if (actualPlayer.nbBarrack == 0 && actualPlayer != this.Winner && TempleSlots(actualPlayer).Length == 0 && TowerSlots(actualPlayer).Length == 0)
            {
                actualPlayer.Eliminate();
                OnPlayerElimination(actualPlayer);
            }
        }

        public void Phase1(PointRotation pr, Rotation r)
        {
            if (ValidateTile(pr, r))
            {
                NextPhase();
                this.maxTurn--;
            }
        }

        //Place building
        public void Phase2(PointRotation pr, Building b)
        {
            Cell c = gameBoard.WorldMap[pr.point];
            if (ValidateBuilding(c, b))
            {
                NextPhase();
                InitPlay();
            }
        }

        //Placement chunk et buildings
        public void AIMove(AI ai)
        {
            OnChangePhase(TurnPhase.IAPlays);
            actualPhase = TurnPhase.IAPlays;
            PointRotation pr = ((AI)actualPlayer).PlayChunk();
            Rotation r = Rotation.N;
            for (int i = 0; i < 6; i++)
            {
                if (pr.rotations[i])
                    r = (Rotation)i;
            }
            OnAIChunkPlacement(pr);
            ValidateTile(pr, r);
            (Building b, Vector2Int pos) = ((AI)actualPlayer).PlayBuild();
            PointRotation p = new PointRotation(pos);
            Cell c = gameBoard.WorldMap[p.point];
            OnAIBuildingPlacement(b, pos);
            ValidateBuilding(c, b);
            NextPhase();
            InitPlay();
        }

        public List<Vector2Int> FindBiomesAroundVillage(Vector2Int cell) => gameBoard.FindBiomesAroundVillage(cell, actualPlayer);


        public bool ValidateTile(PointRotation pr, Rotation r)
        {
            AddHistoric(pr.point, r, actualChunk);
            return gameBoard.AddChunk(actualChunk, actualPlayer, pr, r);
        }

        public bool ValidateBuilding(Cell c, Building b)
        {
            bool building = false;

            List<Cell> cells = new();
            List<Vector2Int> sameBiomes = FindBiomesAroundVillage(gameBoard.GetCellCoord(c));
            if (sameBiomes.Count > 0)
            {
                foreach (Vector2Int cell in sameBiomes)
                {
                    cells.Add(new(gameBoard.WorldMap[cell]));
                }
            }
            building = gameBoard.PlaceBuilding(c, b, actualPlayer);

            if (building)
                AddHistoric(sameBiomes.Count > 0 ? sameBiomes.ToArray() : new[] { gameBoard.GetCellCoord(c) }, cells.Count > 0 ? cells.ToArray() : new[] { c }, b);

            return building;
        }

        public Vector2Int[] BarracksSlots()
        {
            return gameBoard.GetBarrackSlots(actualPlayer);
        }

        public Vector2Int[] TowerSlots(Player actualPlayer)
        {
            return gameBoard.GetTowerSlots(actualPlayer);
        }

        public Vector2Int[] TempleSlots(Player actualPlayer)
        {
            return gameBoard.GetTempleSlots(actualPlayer);
        }

        public PointRotation[] ChunkSlots()
        {
            return gameBoard.GetChunkSlots();
        }

        public void SetChunkLevel(PointRotation pr)
        {
            gameBoard.SetChunkLevel(pr);
        }

        public bool IsVoid(Vector2Int p)
        {
            return gameBoard.WorldMap.IsVoid(p);
        }

        public int NumberOfAI
        {
            set { throw new NotImplementedException(); }
        }

        public Difficulty AIDifficulty
        {
            set { throw new NotImplementedException(); }
        }

    }
}