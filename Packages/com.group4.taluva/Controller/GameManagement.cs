using System;

using Taluva.Model;
using Taluva.Utils;
using UnityEngine;

namespace Taluva.Controller
{
    public class GameManagment
    {
        private Player[] players;
        private Player actualPlayer { get; }
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
            //New Chunk
            public Vector2Int position;
            public Rotation rotation;

            //Last Chunk or last cells
            public Cell[] cells;
            public Player player;

            //Use it when there is nothing at the position
            public Coup(Vector2Int position, Rotation rotation)
            {
                this.position = position;
                this.rotation = rotation;
                this.player = actualPlayer;
            }

            public Coup(Vector2Int position, Rotation rotation, Chunk cells) : this(position, rotation)
            {
                this.cells = (Cell[])cells.Coords.Clone();
            }

            public Coup(Vector2Int position, Rotation rotation, Cell[] cells) : this(position, rotation)
            {
                this.cells = cells;
            }
        }

        public void Undo()
        {
            Coup c = historic.Undo();
            Chunk chunk = gameBoard.WorldMap.GetValue(c.position).ParentCunk;
            if (c.cells == null) {
                gameBoard.RemoveChunk(chunk);
                return;
            }

            gameBoard.AddChunk(chunk, c.player, new(c.position), c.rotation);
        }

        public void Redo()
        {
            Coup c = historic.Redo();
            Chunk chunk = gameBoard.WorldMap.GetValue(c.position).ParentCunk;
            gameBoard.AddChunk(chunk, c.player, new(c.position), c.rotation);
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

                    if (gameBoard.GetTempleSlot(p).Length == p.nbTemple)
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