using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Taluva.Controller;

namespace Taluva.Model.AI
{
    public abstract class AITree : AI
    {
        public AITree(PlayerColor id,GameManagment gm,Pile<Chunk> pile) : base(id, gm,pile){}
        public AITree(AITree orignal) : base(orignal){}
        
        private int TreeExplore(GameManagment gm)
        {
            Chunk[] possibleDraw = pile.GetRemaining();
            PointRotation[] possibleChunk = gm.gameBoard.GetChunkSlots();
            foreach (PointRotation p in possibleChunk)
            {
                for (int i = 0; i < p.rotations.Length; i++)
                {
                    if (!p.rotations[i])
                        continue;
                    
                    foreach (Chunk c in possibleDraw)
                    {
                       // tempBoard = new Board(tempBoard);
                    }
                }
            }
            return 0;
        }

        protected abstract int Heuristic { get; }
        protected abstract int Compare(int a, int b);
    }
    
}