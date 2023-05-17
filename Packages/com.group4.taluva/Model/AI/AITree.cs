using System;
using System.Collections.Generic;
using Taluva.Controller;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

namespace Taluva.Model.AI
{
    public abstract class AITree : AI
    {
        public AITree(PlayerColor id,GameManagment gm) : base(id, gm){}
        public AITree(AITree orignal) : base(orignal){}
        
        private int TreeExplore(int depth)
        {
            if (depth == 0 /*|| gm.gameEnd*/)
            {
                return Heuristic;
            }
            Chunk[] possibleDraw = gm.pile.GetRemaining();
            PointRotation[] possibleChunk = gm.gameBoard.GetChunkSlots();
            int value = 0;
            List<GameManagment.Coup> possiblePlay;
            foreach (PointRotation p in possibleChunk)
            {
                for (int i = 0; i < p.rotations.Length; i++)
                {
                    if (!p.rotations[i])
                        continue;
                    
                    foreach (Chunk c in possibleDraw)
                    {
                       //gm.AIPlay(c,new PointRotation(p.point,(Rotation)i));
                       value = TreeExplore(--depth);
                       gm.Undo();

                    }
                }
            }
            return 0;
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