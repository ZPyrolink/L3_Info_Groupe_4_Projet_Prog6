using System;

using Taluva.Model;
using Taluva.Utils;
using UnityEngine;

namespace Taluva.Controller
{
    public class GameManagment
    {
        private Player[] players;
        private Player actualPlayer;
        public int NbPlayers { get; set; }
        private int actualTurn;
        private int maxTurn;
        private Board gameBoard;
        private TurnPhase actualPhase;
        private Historic<Coup> historic;

        public GameManagment(int nbPlayers, int maxTurn)
        {
            this.players = new Player[nbPlayers];
            this.actualTurn = 1;
            this.gameBoard = new();
            this.NbPlayers = nbPlayers;
            this.maxTurn = 12 * nbPlayers;
            actualPlayer = players[0];
            for (int i = 0; i < this.NbPlayers; i++)
                players[i] = new((PlayerColor) i);
        }

        private class Coup
        {
            //New Chunk or cells
            public Vector2Int[] positions;
            public Rotation? rotation;

            //Last Chunk or last cells
            public Cell[] cells;
            public Chunk chunk;
            public Player player;

            //Use it when there is nothing at the position
            public Coup(Vector2Int[] positions, Rotation? rotation, Player actualPlayer)
            {
                this.positions = positions;
                this.rotation = rotation;
                this.player = actualPlayer;
            }

            public Coup(Vector2Int[] positions, Rotation rotation, Player actualPlayer, Chunk chunk) : this(positions, rotation, actualPlayer)
            {
                this.chunk = chunk;
            }

            public Coup(Vector2Int[] positions, Rotation? rotation, Player actualPlayer, Cell[] cells) : this(positions, rotation, actualPlayer)
            {
                this.cells = cells;
            }
        }

        public void AddHistoric(Vector2Int[] positions, Rotation rotation)
        {
            historic.Add(new(positions, rotation, actualPlayer));
        }

        public void AddHistoric(Vector2Int position, Rotation rotation, Chunk chunk)
        {
            historic.Add(new(new[] { position }, rotation, actualPlayer, chunk));
        }

        public void AddHistoric(Vector2Int[] positions, Cell[] cells)
        {
            historic.Add(new(positions, null, actualPlayer, cells));
        }

        public void Undo()
        {
            //ne pas oublier le changement de player
            Coup c = historic.Undo();
            if(c.chunk == null && c.cells == null) {
                foreach (Vector2Int position in c.positions)
                    gameBoard.WorldMap.Remove(position);
            }
        }

        public void Redo()
        {
            Coup c = historic.Redo();
            if(c.chunk == null) {
                for(int i = 0; i < c.cells.Length; i++) {
                    gameBoard.WorldMap.Add(c.cells[i], c.positions[i]);
                }
            } else {
                gameBoard.AddChunk(c.chunk, c.player, new(c.positions[0]), c.rotation);
            }
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
                    if (p.nbTemple > maxTemple)
                    {
                        maxTemple = p.nbTemple;
                        winner1 = p;
                    }
                }

                int egalityTemple = 0;
                foreach (Player p in players)
                {
                    if (p.nbTemple == maxTemple)
                        egalityTemple++;
                }

                if (egalityTemple >= 2)
                {
                    int maxTower = 0;
                    Player winner2 = null;
                    foreach (Player p in players)
                    {
                        if (p.nbTemple > maxTower)
                        {
                            maxTower = p.nbTemple;
                            winner2 = p;
                        }
                    }

                    int egalityTower = 0;
                    foreach (Player p in players)
                    {
                        if (p.nbTemple == maxTemple)
                            egalityTower++;
                    }

                    if (egalityTower >= 2)
                    {
                        int maxBarrack = 0;
                        Player winner3 = null;
                        foreach (Player p in players)
                        {
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
            while (actualTurn <= maxTurn)
            {
                actualPhase = TurnPhase.PlaceBuilding;
                actualPhase = TurnPhase.RotateCell;
                actualPhase = TurnPhase.SelectCells;

                actualTurn++;
                if (actualTurn > NbPlayers)
                {
                    actualTurn = 1;
                }

                actualPlayer = players[actualTurn - 1];
            }
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