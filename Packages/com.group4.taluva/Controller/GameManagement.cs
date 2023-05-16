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

namespace Taluva.Controller
{
    /// <summary>
    /// Manages the overall game logic and flow.
    /// </summary>
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

        // Actions
        public Action<TurnPhase> ChangePhase { get; set; }
        public Action<bool> NotifyEndGame { get; set; }
        public Action<PointRotation> NotifyAIChunkPlacement { get; set; }
        public Action<(Building, Vector2Int)> NotifyAIBuildingPlacement { get; set; }

        /// <summary>
        /// Initializes a new instance of the GameManagment class with the specified number of players.
        /// </summary>
        /// <param name="nbPlayers">The number of players in the game.</param>
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

        /// <summary>
        /// Sets the game to have AI players and randomizes the number of players to 2.
        /// </summary>
        public void setAI()
        {
            this.AIRandom = true;
            this.NbPlayers = 2;
        }

        /// <summary>
        /// Represents a move made by a player.
        /// </summary>
        public class Coup
        {
            public Vector2Int[] positions;
            public Rotation? rotation;
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

            /// <summary>
            /// Initializes a new instance of the Coup class for placing a chunk on the board.
            /// </summary>
            /// <param name="positions">The positions of the chunk on the board.</param>
            /// <param name="rotation">The rotation of the chunk.</param>
            /// <param name="actualPlayer">The player making the move.</param>
            /// <param name="chunk">The chunk being placed.</param>
            public Coup(Vector2Int[] positions, Rotation rotation, Player actualPlayer, Chunk chunk) : this(positions,
                rotation, actualPlayer)
            {
                this.chunk = chunk;
                this.cells = new Cell[1];
                cells[0] = null;
            }

            /// <summary>
            /// Initializes a new instance of the Coup class for placing buildings on the board.
            /// </summary>
            /// <param name="positions">The positions of the buildings on the board.</param>
            /// <param name="rotation">The rotation of the chunk.</param>
            /// <param name="actualPlayer">The player making the move.</param>
            /// <param name="chunk">The chunk associated with the placement.</param>
            /// <param name="cells">The cells affected by the placement.</param>
            /// <param name="b">The buildings being placed.</param>
            public Coup(Vector2Int[] positions, Rotation rotation, Player actualPlayer, Chunk chunk, Cell[] cells,
                Building[] b) : this(positions, rotation, actualPlayer, cells, b)
            {
                this.chunk = chunk;
            }

            /// <summary>
            /// Initializes a new instance of the Coup class for placing buildings on the board.
            /// </summary>
            /// <param name="positions">The positions of the buildings on the board.</param>
            /// <param name="rotation">The rotation of the chunk.</param>
            /// <param name="actualPlayer">The player making the move.</param>
            /// <param name="cells">The cells affected by the placement.</param>
            /// <param name="b">The buildings being placed.</param>
            public Coup(Vector2Int[] positions, Rotation? rotation, Player actualPlayer, Cell[] cells, Building[] b) :
                this(positions, rotation, actualPlayer)
            {
                this.cells = cells;
                building = b;
            }
        }

        /// <summary>
        /// Checks if the game can be undone.
        /// </summary>
        public bool CanUndo => historic.CanUndo;

        /// <summary>
        /// Checks if the game can be redone.
        /// </summary>
        public bool CanRedo => historic.CanRedo;

        /// <summary>
        /// Saves the game state.
        /// </summary>
        public void Save()
        {
            using (FileStream file = File.Open(savePath + DateTime.Now.ToString(new CultureInfo("de-DE")),
                       FileMode.Create, FileAccess.Write))
            using (BinaryWriter writer = new(file))
            {
                writer.Write(NbPlayers);
                for (int i = 0; i < NbPlayers; i++)
                {
                    writer.Write((uint)players[i].ID);
                    writer.Write(players[i] is AI);
                }

                writer.Write(historic.Count);
                for (int i = 0; i < historic.Count; i++)
                {
                    writer.Write(i == historic.Index);
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

                    i++;
                    if (i >= historic.Count)
                        break;
                    writer.Write(i == historic.Index);
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
                        if (historic[i].chunk.Coords[j].ActualBuildings != Building.None)
                            writer.Write((int)historic[i].chunk.Coords[j].Owner);
                        writer.Write((int)historic[i].building[j]);
                    }
                }
            }
        }

        /// <summary>
        /// Initiates the play of the game.
        /// </summary>
        public void InitPlay()
        {
            if (GetWinner() != null)
            {
                EndGame();
                OnEndGame(true);
                return;
            }

            actualTurn++;
            this.actualChunk = pile.Draw();
            if (actualTurn + 1 > NbPlayers)
            {
                actualTurn = 0;
            }

            actualPlayer = players[actualTurn];

            if (actualPlayer.nbBarrack == 0 && actualPlayer != Winner)
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
                AIMove(ai);
            }
            else
            {
                OnChangePhase(TurnPhase.SelectCells);
            }
        }

        /// <summary>
        /// Ends the game and determines the winner.
        /// </summary>
        public void EndGame()
        {
            this.Winner = GetWinner();
            OnEndGame(true);
        }

        /// <summary>
        /// Handles Phase 1 of the game, which involves placing a chunk on the board.
        /// </summary>
        /// <param name="pr">The PointRotation representing the chunk position and rotation.</param>
        /// <param name="r">The rotation of the chunk.</param>
        public void Phase1(PointRotation pr, Rotation r)
        {
            if (ValidateTile(pr, r))
            {
                actualPhase = TurnPhase.PlaceBuilding;
                OnChangePhase(TurnPhase.PlaceBuilding);
                this.maxTurn--;
            }
        }

        /// <summary>
        /// Handles Phase 2 of the game, which involves placing a building on the board.
        /// </summary>
        /// <param name="pr">The PointRotation representing the building position.</param>
        /// <param name="b">The building to be placed.</param>
        public void Phase2(PointRotation pr, Building b)
        {
            Cell c = gameBoard.WorldMap[pr.point];
            if (ValidateBuilding(c, b))
            {
                actualPhase = TurnPhase.NextPlayer;
                OnChangePhase(TurnPhase.NextPlayer);
                InitPlay();
            }
        }

        /// <summary>
        /// Performs the AI move in the game.
        /// </summary>
        /// <param name="ai">The AI player making the move.</param>
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
            actualPhase = TurnPhase.NextPlayer;
            OnChangePhase(TurnPhase.NextPlayer);
            InitPlay();
        }

        /// <summary>
        /// Finds the biomes surrounding a village cell.
        /// </summary>
        /// <param name="cell">The village cell.</param>
        /// <returns>A list of vectors representing the positions of the surrounding biomes.</returns>
        public List<Vector2Int> FindBiomesAroundVillage(Vector2Int cell) =>
            gameBoard.FindBiomesAroundVillage(cell, actualPlayer);

        /// <summary>
        /// Validates the placement of a tile (chunk) on the board.
        /// </summary>
        /// <param name="pr">The PointRotation representing the chunk position and rotation.</param>
        /// <param name="r">The rotation of the chunk.</param>
        /// <returns>True if the placement is valid, false otherwise.</returns>
        public bool ValidateTile(PointRotation pr, Rotation r)
        {
            AddHistoric(pr.point, r, actualChunk);
            return gameBoard.AddChunk(actualChunk, actualPlayer, pr, r);
        }

        /// <summary>
        /// Validates the placement of a building on the board.
        /// </summary>
        /// <param name="c">The cell on which the building is to be placed.</param>
        /// <param name="b">The building to be placed.</param>
        /// <returns>True if the placement is valid, false otherwise.</returns>
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
                AddHistoric(sameBiomes.Count > 0 ? sameBiomes.ToArray() : new[] { gameBoard.GetCellCoord(c) },
                    cells.Count > 0 ? cells.ToArray() : new[] { c }, b);

            return building;
        }

        /// <summary>
        /// Gets the slots available for placing barracks.
        /// </summary>
        /// <returns>An array of Vector2Int representing the available slots.</returns>
        public Vector2Int[] BarracksSlots()
        {
            return gameBoard.GetBarrackSlots(actualPlayer);
        }

        /// <summary>
        /// Gets the slots available for placing towers.
        /// </summary>
        /// <param name="actualPlayer">The current player.</param>
        /// <returns>An array of Vector2Int representing the available slots.</returns>
        public Vector2Int[] TowerSlots(Player actualPlayer)
        {
            return gameBoard.GetTowerSlots(actualPlayer);
        }

        /// <summary>
        /// Gets the slots available for placing temples.
        /// </summary>
        /// <param name="actualPlayer">The current player.</param>
        /// <returns>An array of Vector2Int representing the available slots.</returns>
        public Vector2Int[] TempleSlots(Player actualPlayer)
        {
            return gameBoard.GetTempleSlots(actualPlayer);
        }

        /// <summary>
        /// Gets the slots available for placing chunks.
        /// </summary>
        /// <returns>An array of PointRotation representing the available slots.</returns>
        public PointRotation[] ChunkSlots()
        {
            return gameBoard.GetChunkSlots();
        }

        /// <summary>
        /// Sets the level of a chunk.
        /// </summary>
        /// <param name="pr">The PointRotation representing the chunk position.</param>
        public void SetChunkLevel(PointRotation pr)
        {
            gameBoard.SetChunkLevel(pr);
        }

        /// <summary>
        /// Checks if a position on the board is void.
        /// </summary>
        /// <param name="p">The position to check.</param>
        /// <returns>True if the position is void, false otherwise.</returns>
        public bool IsVoid(Vector2Int p)
        {
            return gameBoard.WorldMap.IsVoid(p);
        }

        /// <summary>
        /// Gets or sets the number of AI players.
        /// </summary>
        public int NumberOfAI
        {
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets the difficulty of the AI players.
        /// </summary>
        public Difficulty AIDifficulty
        {
            set { throw new NotImplementedException(); }
        }
    }
}




