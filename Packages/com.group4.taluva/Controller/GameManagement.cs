using System;

using Taluva.Model;
using Taluva.Utils;
using Taluva.Model.AI;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Networking.PlayerConnection;

namespace Taluva.Controller
{
    public class GameManagment
    {
        private Player[] players;
        private Player actualPlayer;
        private AI ActualAi => (AI)actualPlayer;
        public int NbPlayers { get; set; }
        public  int actualTurn { get; private set; }
        private int maxTurn;
        public Board gameBoard;
        private TurnPhase actualPhase = TurnPhase.SelectCells;
        private Historic<Coup> historic;
        private Pile<Chunk> pile = ListeChunk.Pile;
        private Chunk actualChunk;
        private bool AIRandom = false;
        private Player Winner { get; set; }
        
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
                players[i] = new((PlayerColor) i);
                if (this.AIRandom && i==1)
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

            public Coup(Vector2Int[] positions, Rotation rotation, Player actualPlayer, Chunk chunk, Cell[] cells) : this(positions, rotation, actualPlayer, cells)
            {
                this.chunk = chunk;
            }

            public Coup(Vector2Int[] positions, Rotation? rotation, Player actualPlayer, Cell[] cells) : this(positions, rotation, actualPlayer)
            {
                this.cells = cells;
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
                historic.Add(new(new[] { position }, rotation, actualPlayer, chunk, gameBoard.WorldMap.GetValue(position).ParentCunk.Coords));
            } else {
                historic.Add(new(new[] { position }, rotation, actualPlayer, chunk));
            }
                
        }

        /// <summary>
        /// Use this when you're in phase 2 during the placement of a building.
        /// It will store the different information on the cells.
        /// </summary>
        /// <param name="positions">Buildings positions</param>
        /// <param name="cells">Cells with the buildings</param>
        public void AddHistoric(Vector2Int[] positions, Cell[] cells)
        {
            historic.Add(new(positions, null, actualPlayer, cells));
        }

        public Coup Undo()
        {
            if (!historic.CanUndo)
                return null;

            int nbActualPlayer = 0;
            for(int i = 0; i < players.Length; i++) {
                if (actualPlayer == players[i])
                    nbActualPlayer = i;
            }
            actualPlayer = players[(nbActualPlayer + NbPlayers - 1) % NbPlayers];

            Coup c = historic.Undo();
            if(c.chunk != null) {
                gameBoard.RemoveChunk(c.chunk);
                if(c.cells[0] != null)
                    for(int i = 0; i < c.cells.Length; i++)
                        gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                pile.Stack(c.chunk);
            }else {
                for (int i = 0; i < c.cells.Length; i++) {
                    gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                    switch (c.cells[i].ActualBuildings) {
                        case Building.None:
                            break;
                        case Building.Temple:
                            actualPlayer.nbTemple++; ;
                            break;
                        case Building.Tower:
                            actualPlayer.nbTowers++;
                            break;
                        case Building.Barrack:
                            actualPlayer.nbBarrack += gameBoard.WorldMap.GetValue(c.positions[i]).ParentCunk.Level;
                            break;
                    }
                }
            }
            return c;
        }

        public Coup Redo()
        {
            if (!historic.CanRedo)
                return null;

            Coup c = historic.Redo();
            if(c.chunk == null) {
                for(int i = 0; i < c.cells.Length; i++) {
                    gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                    switch (c.cells[i].ActualBuildings) {
                        case Building.None:
                            break;
                        case Building.Temple:
                            actualPlayer.nbTemple--; ;
                            break;
                        case Building.Tower:
                            actualPlayer.nbTowers--;
                            break;
                        case Building.Barrack:
                            actualPlayer.nbBarrack -= gameBoard.WorldMap.GetValue(c.positions[i]).ParentCunk.Level;
                            break;
                    }
                }
            } else {
                gameBoard.AddChunk(c.chunk, c.player, new(c.positions[0]),(Rotation) c.rotation);
                pile.Draw();
            }
            int nbActualPlayer = 0;
            for (int i = 0; i < players.Length; i++) {
                if (actualPlayer == players[i])
                    nbActualPlayer = i;
            }
            actualPlayer = players[(nbActualPlayer + 1) % NbPlayers];

            return c;
        }

        public Player? GetWinner()
        {
            if (maxTurn == 0)
                return NormalEnd;
            if (EarlyEnd != null)
                return EarlyEnd;
            return null;
        }

        private Player? EarlyEnd
        {
            get
            {
                foreach (Player p in players)
                {
                    int completedBuildingTypes = 0;

                    if (gameBoard.GetTempleSlots(p).Length == p.nbTemple)
                        completedBuildingTypes++;

                    if (gameBoard.GetBarrackSlots().Length == p.nbBarrack)
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
                    }
                    else
                    {
                        return winner2;
                    }
                }
                else
                {
                    return winner1;
                }
            }
        }

        public void InitPlay()
        {
            if (GetWinner() != null)
            {
                EndGame();
                return;
            }
            actualTurn++;
            //TODO : notify vieuw turn change
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
                //TODO : notify view phase change
                // Phase1();
            }
        }

        public void EndGame()
        {
            this.Winner = GetWinner();
            //TODO : notify view to end game
        }

        public void Phase1(PointRotation pr, Rotation r)
        {
            if (ValidateTile(pr, r))
            {
                //TODO : notify view chunk placed at the postion x, y
                actualPhase = TurnPhase.PlaceBuilding;
                this.maxTurn--;
            }
        }

        //Place building
        public void Phase2(PointRotation pr, Building b)
        {
            Cell c = gameBoard.WorldMap.GetValue(pr.point);
            if (ValidateBuilding(c, b))
            {
                //TODO : notify view building placed
                actualPhase = TurnPhase.SelectCells;
                InitPlay();
            }
        }

        public TurnPhase NotifyTurnPhaseChange{
            set { throw new NotImplementedException(); }
        }
        
        // public event NotifyEndGame()
        // {
        //     
        // }

        //Placement chunk et buildings
        public void AIMove(AI ai)
        {
            PointRotation pr = ((AI)actualPlayer).PlayChunk();
            Rotation r = Rotation.N;
            for (int i = 0; i < 6; i++)
            {
                if (pr.rotations[i])
                    r = (Rotation)i;
            }
            ValidateTile(pr, r);
            actualPhase = TurnPhase.PlaceBuilding;
            (Building b, Vector2Int pos) = ((AI)actualPlayer).PlayBuild();
            PointRotation p = new PointRotation(pos);
            Cell c = gameBoard.WorldMap.GetValue(p.point);
            ValidateBuilding(c,b);
            actualPhase = TurnPhase.SelectCells;
            InitPlay(); //TODO : AI move done, notify view, move to next player
        }
        
        public List<Vector2Int> FindBiomesAroundVillage(Vector2Int cell) => gameBoard.FindBiomesAroundVillage(cell, actualPlayer);


        public bool ValidateTile(PointRotation pr, Rotation r)     //Place
        {
            //Ajouter les cells si elles existent
            AddHistoric(pr.point, r, actualChunk);
            return gameBoard.AddChunk(actualChunk, actualPlayer, pr, r);
        }

        public bool ValidateBuilding(Cell c, Building b)
        {
            Cell cell = new(c);
            AddHistoric(new[] { gameBoard.GetCellCoord(c) }, new[] { cell });
            return gameBoard.PlaceBuilding(c, b, actualPlayer);
        }

        public Vector2Int[] BarracksSlots()
        {
            return gameBoard.GetBarrackSlots();
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