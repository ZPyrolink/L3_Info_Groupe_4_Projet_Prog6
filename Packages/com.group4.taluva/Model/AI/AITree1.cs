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

        protected override int Heuristic
        {
            get
            {
                return this.nbBarrack * 5 + this.nbTowers * 100 + this.nbTemple * 1000;
            }
        }

        protected override GameManagment.Coup GetBest(Dictionary<int, GameManagment.Coup> possible)
        {
            possible.Max();
            throw new NotImplementedException();
        }
    }
}