using System;
using System.Collections.Generic;

using Taluva.Controller;

using UnityEngine;

namespace Taluva.Model.AI
{
    public abstract class AITree : AI
    {
        public AITree(PlayerColor id, GameManagment gm) : base(id, gm){}

        public AITree(AITree orignal) : base(orignal){}


        protected PointRotation ChunkPos;
        protected Vector2Int BuildPos;
        protected Building BuildType;

        protected Turn AITurn;
        protected class Turn
        {
            public PointRotation ChunkPos;
            public Rotation ChunkRot;
            public Vector2Int BuildPos;
            public Building BuildType;
            

            public Turn(PointRotation ChunkPos, Rotation ChunkRot, Vector2Int BuildPos,Building BuildType)
            {
                this.ChunkPos = ChunkPos;
                this.ChunkRot = ChunkRot;
                this.BuildPos = BuildPos;
                this.BuildType = BuildType;
                
            }
            
        }


        private (Turn,int) TreeExplore(GameManagment AI_gm,Chunk[] possibleDraw,int depth)
        {
            if (depth == 0 || AI_gm.CheckWinner() == null)
                return (null,Heuristic);
            PointRotation[] possibleChunk = AI_gm.gameBoard.GetChunkSlots();
            Dictionary<Turn, int> possiblePlay = new Dictionary<Turn, int>();
            foreach (PointRotation p in possibleChunk)
            {
                for (int i = 0; i < p.rotations.Length; i++)
                {
                    if (!p.rotations[i])
                        continue;

                    foreach (Chunk c in possibleDraw)
                    {
                        Gm.Phase1IA(c,p, (Rotation)i);
                        Vector2Int[] possibleBarracks = AI_gm.gameBoard.GetBarrackSlots(Gm.actualPlayer);
                        foreach (Vector2Int pos in possibleBarracks)
                        {
                            Gm.Phase2IA(new PointRotation(pos), Building.Barrack);
                            possiblePlay.Add(new Turn(p,(Rotation)i,pos,Building.Barrack), TreeExplore(AI_gm,AI_gm.pile.GetRemaining(),--depth).Item2);
                            Gm.Undo();
                        }
                        Vector2Int[] possibleTower = AI_gm.gameBoard.GetTowerSlots(Gm.actualPlayer);
                        foreach (Vector2Int pos in possibleTower)
                        {
                            Gm.Phase2IA(new PointRotation(pos), Building.Tower);
                            possiblePlay.Add(new Turn(p,(Rotation)i,pos,Building.Tower), TreeExplore(AI_gm,AI_gm.pile.GetRemaining(),--depth).Item2);
                            Gm.Undo();
                        }
                        
                        Vector2Int[] possibleTemple = AI_gm.gameBoard.GetTempleSlots(Gm.actualPlayer);
                        foreach (Vector2Int pos in possibleTemple)
                        {
                            Gm.Phase2IA(new PointRotation(pos), Building.Temple);
                            possiblePlay.Add(new Turn(p,(Rotation)i,pos,Building.Temple), TreeExplore(AI_gm,AI_gm.pile.GetRemaining(),--depth).Item2);
                            Gm.Undo();
                        }
                        Gm.Undo();

                    }
                }
            }
            
            return GetBest(possiblePlay);
    }

        private void ComputeBestMove()
        {
            GameManagment AI_gm = new GameManagment(Gm); //TODO May have to copy Players too to avoid issues with end game states. 
            AITurn = TreeExplore(AI_gm,new Chunk[]{AI_gm.ActualChunk},5).Item1;
        }

        public override PointRotation PlayChunk()
        { 
            ComputeBestMove();
            return new PointRotation(AITurn.ChunkPos.Point, AITurn.ChunkRot);
        }

        public override (Building buil, Vector2Int pos) PlayBuild()
        {
            return (AITurn.BuildType, AITurn.BuildPos);
        }
        
        protected abstract int Heuristic { get; }

        protected virtual (Turn, int) GetBest(Dictionary<Turn, int> possible)
        {
            KeyValuePair<Turn, int> max = new KeyValuePair<Turn, int>(null,0);
            foreach (var set in possible)
            {
                if (set.Value > max.Value)
                {
                    max = set;
                }
            }

            return (max.Key,max.Value);
        }
    }
    
}