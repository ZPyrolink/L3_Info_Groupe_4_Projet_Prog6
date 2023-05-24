using Taluva.Controller;
using UnityEngine;

namespace Taluva.Model.AI
{
    public abstract class AI : Player
    {
        protected readonly GameManagment Gm;
        public Difficulty Difficulty { get; }

        public AI(Color id, GameManagment gm) : base(id)
        {
            Gm = gm;
        }

        public AI(AI original) :base(original.ID)
        {
            Gm = original.Gm;
            LastChunk = original.LastChunk;
            NbTowers = original.NbTowers;
            NbTemple = original.NbTemple;
            NbBarrack = original.NbBarrack;
        }

        public abstract Player Clone();

        public abstract PointRotation PlayChunk();

        public abstract (Building buil, Vector2Int pos) PlayBuild();
        
    }
}