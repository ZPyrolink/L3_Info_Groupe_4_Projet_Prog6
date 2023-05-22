using System.Collections.Generic;
using Taluva.Controller;

namespace Taluva.Model.AI
{
    public class AITree2 : AITree
    {
        public AITree2(PlayerColor id, GameManagment gm) : base(id, gm)
        {
        }

        public AITree2(AITree orignal) : base(orignal)
        {
        }

        public override Player Clone()
        {
            return new AITree2(this);
        }

        protected override int Heuristic { get; }
        protected override GameManagment.Coup GetBest(Dictionary<int, GameManagment.Coup> possible)
        {
            throw new System.NotImplementedException();
        }
    }
}