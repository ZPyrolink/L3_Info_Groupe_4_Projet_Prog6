using System.Collections.Generic;
using Taluva.Controller;

namespace Taluva.Model.AI
{
    public class AITree2 : AITree
    {
        public AITree2(Color id, GameManagment gm) : base(id, gm)
        {
        }

        public AITree2(AITree orignal) : base(orignal)
        {
        }

        public override Player Clone()
        {
            return new AITree2(this);
        }

        protected override int Heuristic(GameManagment AI_gm, Player previousPlayer)
        {
            return 0;
            //TODO Create your heuristic here
        }
        protected override (Turn,int) GetBest(Dictionary<Turn,int> possible)
        {
            throw new System.NotImplementedException();
        }
    }
}