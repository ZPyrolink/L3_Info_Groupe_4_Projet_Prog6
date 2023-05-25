using System.Collections.Generic;
using Taluva.Controller;

using UnityEngine;

namespace Taluva.Model.AI
{
    public class AITree : AI
    {
        public AITree(Color id, GameManagment gm) : base(id, gm)
        {
            this.Difficulty = Difficulty.Normal;
        }

        public AITree(AITree orignal) : base(orignal)
        {
            this.Difficulty = Difficulty.Normal;
        }


        protected PointRotation ChunkPos;
        protected Vector2Int BuildPos;
        protected Building BuildType;

        protected Turn AITurn;
        protected class Turn
        {
            public PointRotation ChunkPos { get; }
            public Rotation ChunkRot { get; }
            public Vector2Int BuildPos { get; }
            public Building BuildType { get; }
            

            public Turn(PointRotation ChunkPos, Rotation ChunkRot, Vector2Int BuildPos,Building BuildType)
            {
                this.ChunkPos = ChunkPos;
                this.ChunkRot = ChunkRot;
                this.BuildPos = BuildPos;
                this.BuildType = BuildType;
                
            }
            
        }


        protected (Turn,int[]) TreeExplore(GameManagment AI_gm,List<Chunk> possibleDraw,int depth,Player previousPlayer)
        {
            if (depth <= 0 || AI_gm.CheckWinner() != null)
                return (null,Heuristic(AI_gm));
            PointRotation[] possibleChunk = AI_gm.gameBoard.GetChunkSlots();
            Dictionary<Turn, int[]> possiblePlay = new();
            foreach (PointRotation p in possibleChunk)
            {
                for (int i = 0; i < p.Rotations.Length; i++)
                {
                    if (!p.Rotations[i])
                        continue;

                    foreach (Chunk c in possibleDraw)
                    {
                        if (c != AI_gm.CurrentChunk)
                        {
                            AI_gm.Pile.Content.Clear();
                            foreach (var chunk in possibleDraw)
                            {
                                if(chunk != c)
                                    AI_gm.Pile.Content.Push(chunk);
                            }
                            AI_gm.Pile.Played.Remove(AI_gm.CurrentChunk);
                            AI_gm.Pile.Played.Add(c);
                            AI_gm.CurrentChunk = c;
                        }
                        AI_gm.Phase1(p, (Rotation)i, false, true);
                        
                        Player currentPlayer = AI_gm.CurrentPlayer;
                        Vector2Int[] possibleBarracks = AI_gm.gameBoard.GetBarrackSlots(AI_gm.CurrentPlayer);
                        foreach (Vector2Int pos in possibleBarracks)
                        {
                            AI_gm.Phase2(pos, Building.Barrack,true,true);
                            //AI_gm.Phase2IA(new(pos), Building.Barrack);
                            //AI_gm.InitPlay(true,true,true);
                            possiblePlay.Add(new(p,(Rotation)i,pos,Building.Barrack), TreeExplore(AI_gm,AI_gm.GetPossibleChunks(),depth-1,currentPlayer).Item2);
                            AI_gm.Undo(true);
                        }
                        Vector2Int[] possibleTower = AI_gm.gameBoard.GetTowerSlots(AI_gm.CurrentPlayer);
                        foreach (Vector2Int pos in possibleTower)
                        {
                            AI_gm.Phase2(pos, Building.Tower,true,true);
                            //AI_gm.Phase2IA(new(pos), Building.Tower);
                            //AI_gm.InitPlay(true,true,true);
                            possiblePlay.Add(new(p,(Rotation)i,pos,Building.Tower), TreeExplore(AI_gm,AI_gm.GetPossibleChunks(),depth-1,currentPlayer).Item2);
                            AI_gm.Undo(true);
                        }
                        
                        Vector2Int[] possibleTemple = AI_gm.gameBoard.GetTempleSlots(AI_gm.CurrentPlayer);
                        foreach (Vector2Int pos in possibleTemple)
                        {
                            
                            AI_gm.Phase2(pos, Building.Temple,true,true);
                            //AI_gm.Phase2IA(new(pos), Building.Temple);
                            //AI_gm.InitPlay(true,true,true);
                            possiblePlay.Add(new(p,(Rotation)i,pos,Building.Temple), TreeExplore(AI_gm,AI_gm.GetPossibleChunks(),depth-1,currentPlayer).Item2);
                            AI_gm.Undo(true);
                        }
                        AI_gm.Undo(true);

                    }
                }
            }
            
            return GetBest(AI_gm,possiblePlay,AI_gm.CurrentPlayer);
    }

        protected virtual void ComputeBestMove()
        {
            GameManagment AI_gm = new(Gm);
            List<Chunk> possibleChunk = new List<Chunk>(){AI_gm.CurrentChunk};
            AITurn = TreeExplore(AI_gm,possibleChunk,1,AI_gm.CurrentPlayer).Item1;
        }

        public override Player Clone()
        {
            return new AITree(this);
        }

        public override PointRotation PlayChunk()
        { 
            ComputeBestMove();
            return new(AITurn.ChunkPos.Point, AITurn.ChunkRot);
        }

        public override (Building buil, Vector2Int pos) PlayBuild()
        {
            return (AITurn.BuildType, AITurn.BuildPos);
        }

        protected virtual int[] Heuristic(GameManagment AI_gm)
        {
            int[] playerValues = new int[AI_gm.NbPlayers];
            Player player;
            for(int i = 0; i<AI_gm.NbPlayers;i++)
            {
                player = AI_gm.Players[i];
                playerValues[i] = (20 - player.NbBarrack) * 2 + (2 - player.NbTowers) * 100 + (3 - player.NbTemple);
            }
            return playerValues;
        }
        protected virtual (Turn, int[]) GetBest(GameManagment AI_gm,Dictionary<Turn, int[]> possible,Player previousPlayer)
        {
            KeyValuePair<Turn, int[]> max = new(null,new int[AI_gm.NbPlayers]);
            List<KeyValuePair<Turn,int[]>> possibleMax= new();
            int previousPlayerIndex = -1;
            for(int i = 0; i<AI_gm.NbPlayers;i++)
            {
                if(AI_gm.Players[i]==previousPlayer)
                    previousPlayerIndex = i;
            }
            foreach (var set in possible)
            {
                if (set.Value[previousPlayerIndex] > max.Value[previousPlayerIndex])
                {
                    max = set;
                    possibleMax.Clear();
                }

                if (set.Value == max.Value)
                    possibleMax.Add(set);
            }

            int rand = Random.Range(0, possibleMax.Count);
            return (possibleMax[rand].Key,possibleMax[rand].Value);
        }
    }
    
}