using System.Collections.Generic;
using Taluva.Controller;

namespace Taluva.Model.AI
{
    public class AIMinMax : AITree
    {
        public AIMinMax(PlayerColor id, GameManagment gm) : base(id, gm)
        {
        }

        public AIMinMax(AITree orignal) : base(orignal)
        {
        }

        public override Player Clone()
        {
            return new AIMinMax(this);
        }

        protected override int Heuristic { get; }
        protected override GameManagment.Coup GetBest(Dictionary<int, GameManagment.Coup> possible)
        {
            throw new System.NotImplementedException();
        }
    }
}