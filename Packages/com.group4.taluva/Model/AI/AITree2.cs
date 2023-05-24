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

        protected override int[] Heuristic(GameManagment AI_gm)
        {
            throw new System.NotImplementedException();
            //TODO Create your heuristic here
        }
        protected override (Turn,int[]) GetBest(GameManagment AI_gm,Dictionary<Turn,int[]> possible,Player previousPlayer)
        {
            throw new System.NotImplementedException();
        }
    }
}