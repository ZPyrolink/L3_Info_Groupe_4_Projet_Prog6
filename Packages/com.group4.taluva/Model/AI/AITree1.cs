using System;
using System.Collections.Generic;
using System.Linq;
using Taluva.Controller;

namespace Taluva.Model.AI
{
    public class AITree1 : AITree
    {
        public AITree1(PlayerColor id, GameManagment gm) : base(id, gm)
        {
        }

        public AITree1(AITree orignal) : base(orignal)
        {
        }

        public override Player Clone()
        {
            return new AITree1(this);
        }

        protected override int Heuristic => this.NbBarrack * 5 + this.NbTowers * 100 + this.NbTemple * 1000;

        protected override (Turn,int) GetBest(Dictionary<Turn,int> possible)
        {
            possible.Max();
            throw new NotImplementedException();
        }
    }
}