using System;
using System.Collections.Generic;
using Taluva.Controller;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace Taluva.Model.AI
{
    public abstract class AITree : AI
    {
        public AITree(PlayerColor id,GameManagment gm) : base(id, gm){}
        public AITree(AITree orignal) : base(orignal){}

        //private AIMove; 

        private int TreeExplore(int depth)
        {
            if (depth == 0 /*|| TODO gm.gameEnd*/)
            {
                return Heuristic;
            }

            Chunk[] possibleDraw = gm.pile.GetRemaining();
            PointRotation[] possibleChunk = gm.gameBoard.GetChunkSlots();
            int value = 0;
            Dictionary<PointRotation, Dictionary<Building, Dictionary<Vector2Int, int>>> possiblePlay =
                new Dictionary<PointRotation, Dictionary<Building, Dictionary<Vector2Int, int>>>();
            foreach (PointRotation p in possibleChunk)
            {
                for (int i = 0; i < p.rotations.Length; i++)
                {
                    if (!p.rotations[i])
                        continue;

                    foreach (Chunk c in possibleDraw)
                    {
                        gm.Phase1IA(p, (Rotation)i);
                        Dictionary<Building, Dictionary<Vector2Int, int>> possibleBuild =
                            new Dictionary<Building, Dictionary<Vector2Int, int>>();

                        Dictionary<Vector2Int, int> possibleBuildLocation = new Dictionary<Vector2Int, int>();
                        Vector2Int[] possibleBarracks = gm.gameBoard.GetBarrackSlots(gm.actualPlayer);
                        foreach (Vector2Int pos in possibleBarracks)
                        {
                            gm.Phase2IA(new PointRotation(pos), Building.Barrack);
                            possibleBuildLocation.Add(pos, TreeExplore(--depth));
                            gm.Undo();
                        }

                        possibleBuild.Add(Building.Barrack, possibleBuildLocation);

                        possibleBuildLocation = new Dictionary<Vector2Int, int>();
                        Vector2Int[] possibleTower = gm.gameBoard.GetTowerSlots(gm.actualPlayer);
                        foreach (Vector2Int pos in possibleTower)
                        {
                            gm.Phase2IA(new PointRotation(pos), Building.Tower);
                            possibleBuildLocation.Add(pos, TreeExplore(--depth));
                            gm.Undo();
                        }

                        possibleBuild.Add(Building.Tower, possibleBuildLocation);

                        possibleBuildLocation = new Dictionary<Vector2Int, int>();
                        Vector2Int[] possibleTemple = gm.gameBoard.GetTempleSlots(gm.actualPlayer);
                        foreach (Vector2Int pos in possibleTemple)
                        {
                            gm.Phase2IA(new PointRotation(pos), Building.Temple);
                            possibleBuildLocation.Add(pos, TreeExplore(--depth));
                            gm.Undo();
                        }

                        possibleBuild.Add(Building.Temple, possibleBuildLocation);
                        possiblePlay.Add(new PointRotation(p.point, (Rotation)i), possibleBuild);

                        gm.Undo();

                    }
                }
            }

            int max = 0;
            foreach (var a in possiblePlay)
            {
                foreach (var b in a.Value)
                {
                    foreach (var c in b.Value)
                    {
                        if (c.Value > max)
                        {
                            max = c.Value;
                        }
                    }
                }
            }


            return (max);
    }

        public override PointRotation PlayChunk()
        {
            throw new System.NotImplementedException();
        }

        public override (Building buil, Vector2Int pos) PlayBuild()
        {
            throw new System.NotImplementedException();
        }
        
        
        protected abstract int Heuristic { get; }
        protected abstract GameManagment.Coup GetBest(Dictionary<int,GameManagment.Coup> possible);
    }
    
}