using System;
using System.Collections.Generic;
using Taluva.Controller;
using UnityEngine;

namespace Taluva.Model.AI
{
    public class AIMonteCarlo : AITree
    {
        public AIMonteCarlo(PlayerColor id, GameManagment gm, Pile<Chunk> pile) : base(id, gm, pile)
        {
        }

        public AIMonteCarlo(AIMonteCarlo original) : base(original)
        {
        }

        public override Player Clone()
        {
            return new AIMonteCarlo(this);
        }

        public override PointRotation PlayChunk()
        {
            throw new System.NotImplementedException();
        }

        public override (Building buil, Vector2Int pos) PlayBuild()
        {
            throw new System.NotImplementedException();
        }

        protected override int Heuristic
        {
            get
            {
                //GameManagment currentState = new GameManagment(gm);
                throw new NotImplementedException();
            }
        }

        protected override int Compare(int a, int b)
        {
            throw new System.NotImplementedException();
        }
    }
}