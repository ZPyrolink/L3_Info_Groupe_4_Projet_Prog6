using System;
using System.Collections.Generic;

using Taluva.Controller;

using UnityEngine;

namespace Taluva.Model.AI
{
    public abstract class AITree : AI
    {
        public AITree(PlayerColor id, GameManagment gm) : base(id, gm)
        {
            Computed = false;
        }

        public AITree(AITree orignal) : base(orignal)
        {
            Computed = false;
        }

        protected bool Computed;

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


        private (Turn,int) TreeExplore(int depth)
        {
            if (depth == 0 || Gm.CheckWinner() == null)
                return (null,Heuristic);

            Chunk[] possibleDraw = Gm.pile.GetRemaining();
            PointRotation[] possibleChunk = Gm.gameBoard.GetChunkSlots();
            int value = 0;
            Dictionary<Turn, int> possiblePlay = new Dictionary<Turn, int>();
            foreach (PointRotation p in possibleChunk)
            {
                for (int i = 0; i < p.rotations.Length; i++)
                {
                    if (!p.rotations[i])
                        continue;

                    foreach (Chunk c in possibleDraw)
                    {
                        Gm.Phase1IA(p, (Rotation)i);
                        Vector2Int[] possibleBarracks = Gm.gameBoard.GetBarrackSlots(Gm.actualPlayer);
                        foreach (Vector2Int pos in possibleBarracks)
                        {
                            Gm.Phase2IA(new PointRotation(pos), Building.Barrack);
                            possiblePlay.Add(new Turn(p,(Rotation)i,pos,Building.Barrack), TreeExplore(--depth).Item2);
                            Gm.Undo();
                        }
                        Vector2Int[] possibleTower = Gm.gameBoard.GetTowerSlots(Gm.actualPlayer);
                        foreach (Vector2Int pos in possibleTower)
                        {
                            Gm.Phase2IA(new PointRotation(pos), Building.Tower);
                            possiblePlay.Add(new Turn(p,(Rotation)i,pos,Building.Tower), TreeExplore(--depth).Item2);
                            Gm.Undo();
                        }
                        
                        Vector2Int[] possibleTemple = Gm.gameBoard.GetTempleSlots(Gm.actualPlayer);
                        foreach (Vector2Int pos in possibleTemple)
                        {
                            Gm.Phase2IA(new PointRotation(pos), Building.Temple);
                            possiblePlay.Add(new Turn(p,(Rotation)i,pos,Building.Temple), TreeExplore(--depth).Item2);
                            Gm.Undo();
                        }
                        Gm.Undo();

                    }
                }
            }
            
            KeyValuePair<Turn, int> max = new KeyValuePair<Turn, int>(null,0);
            foreach (var set in possiblePlay)
            {
                if (set.Value > max.Value)
                {
                    max = set;
                }
            }


            return (max.Key,max.Value);
    }

        public override PointRotation PlayChunk() => throw new NotImplementedException();

        public override (Building buil, Vector2Int pos) PlayBuild() => throw new NotImplementedException();
        
        
        protected abstract int Heuristic { get; }
        protected abstract (Turn,int) GetBest(Dictionary<Turn,int> possible);
    }
    
}