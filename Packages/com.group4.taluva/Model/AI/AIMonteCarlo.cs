using System.Collections.Generic;
using System.Linq;
using Taluva.Controller;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Taluva.Model.AI
{
    public class AIMonteCarlo : AITree
    {
        private Player virtualPawn = null;
        public AIMonteCarlo(Color id, GameManagment gm) : base(id, gm)
        {
            this.Difficulty = Difficulty.SkillIssue;
        }

        public AIMonteCarlo(AIMonteCarlo original) : base(original)
        {
            this.Difficulty = Difficulty.SkillIssue;
        }

        public override Player Clone()
        {
            return new AIMonteCarlo(this);
        }
        
        protected override void ComputeBestMove()
        {
            GameManagment AI_gm = new(Gm);
            List<Chunk> possibleChunk = new List<Chunk>(){AI_gm.CurrentChunk};
            AITurn = TreeExplore(AI_gm,possibleChunk,2,AI_gm.CurrentPlayer).Item1;
        }

        protected override int[] Heuristic(GameManagment AI_gm)
        {
                int[] val = new int[AI_gm.NbPlayers];
                GameManagment virtualGm = new(AI_gm);
                for (int i = 0; i < 100; i++)
                {
                    Player winner = AI_gm.CheckWinner();
                    while (winner == null)
                    {
                        (PointRotation chunkPos,Rotation chunkRot) = PlayRandomChunk();
                        virtualGm.Phase1(chunkPos,chunkRot,false,true);
                        (Building buildType,Vector2Int buildPos) = PlayRandomBuild();
                        virtualGm.Phase2(buildPos,buildType,true,true);
                        winner = AI_gm.CheckWinner();

                    }

                    for (int j = 0; j < virtualGm.NbPlayers; j++)
                    {
                        if (winner == virtualGm.Players[i])
                        {
                            val[i]++;
                        }
                    }
                }
                return val;
        }


        public (PointRotation,Rotation) PlayRandomChunk()
        {
            int rand;
            PointRotation[] possible = Gm.gameBoard.GetChunkSlots();
            
            List<PointRotation> possibleOcean = new List<PointRotation>();
            List<PointRotation> possibleVolcano =  new List<PointRotation>();
            foreach (var pr in possible)
            {
                if (Gm.gameBoard.WorldMap.IsVoid(pr.Point))
                    possibleOcean.Add(pr);
                else
                    possibleVolcano.Add(pr);
            }

            int max = 0;
            List<PointRotation> possiblePointRotations;
            if (possibleVolcano.Count > 0)
            {
                possiblePointRotations = possibleVolcano;
            }
            else
            {
                possiblePointRotations = possibleOcean;
            }
            max = possiblePointRotations.SelectMany(p => p.Rotations).Count(rot => rot);


            rand = Random.Range(1, max + 1);
            foreach (PointRotation p in possiblePointRotations)
                for (int i = 0; i < 6; i++)
                {
                    if (p.Rotations[i])
                        rand--;

                    if (rand == 0)
                        return (new(p.Point, (Rotation) i),(Rotation)i);
                }

            return (null,0);
        }

        public (Building buil, Vector2Int pos) PlayRandomBuild()
        {
            int rand;
            Vector2Int[] temples = Gm.gameBoard.GetTempleSlots(this);
            if(temples.Length>0)
            {
                rand = Random.Range(0, temples.Length);
                return (Building.Temple,temples[rand]);
            } 
            Vector2Int[] towers = Gm.gameBoard.GetTowerSlots(this);
            if(towers.Length>0)
            {
                rand = Random.Range(0, towers.Length);
                return (Building.Tower,towers[rand]);
            }
            Vector2Int[] barracks = Gm.gameBoard.GetBarrackSlots(this);
            int maxlevel = 0;
            List<Vector2Int> barracksAtMaxLevel = new List<Vector2Int>();
            foreach (var pos in barracks)
            {
                if (Gm.gameBoard.WorldMap[pos].ParentChunk.Level > maxlevel)
                    maxlevel = Gm.gameBoard.WorldMap[pos].ParentChunk.Level;
                barracksAtMaxLevel.Clear();
                if(Gm.gameBoard.WorldMap[pos].ParentChunk.Level==maxlevel)
                    barracksAtMaxLevel.Add(pos);
            }
                
            rand = Random.Range(0, barracksAtMaxLevel.Count);
            return (Building.Barrack,barracksAtMaxLevel[rand]);
        }
    }
    
    
    
    
    
    
    
}