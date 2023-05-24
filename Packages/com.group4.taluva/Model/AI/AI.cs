using Taluva.Controller;
using UnityEngine;

namespace Taluva.Model.AI
{
    public abstract class AI : Player
    {
        protected readonly GameManagment Gm;
        public Difficulty Difficulty;

        public AI(Color id, GameManagment gm) : base(id)
        {
            this.Gm = gm;
        }

        public AI(AI original) :base(original.ID)
        {
            this.Gm = original.Gm;
            this.LastChunk = original.LastChunk;
            this.NbTowers = original.NbTowers;
            this.NbTemple = original.NbTemple;
            this.NbBarrack = original.NbBarrack;
        }

        public abstract Player Clone();

        public abstract PointRotation PlayChunk();

        public abstract (Building buil, Vector2Int pos) PlayBuild();
        
    }
}