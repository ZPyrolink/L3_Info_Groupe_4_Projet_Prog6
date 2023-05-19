using Taluva.Controller;
using UnityEngine;

namespace Taluva.Model.AI
{
    public abstract class AI : Player
    {
        protected readonly GameManagment Gm;
        protected readonly Pile<Chunk> Pile;
        public Difficulty Difficulty;

        public AI(PlayerColor id, GameManagment gm, Pile<Chunk> pile) : base(id)
        {
            this.Gm = gm;
            this.Pile = pile;
        }

        public AI(AI original) :base(original.ID)
        {
            this.Gm = original.Gm;
            this.Pile = original.Pile;
            this.LastChunk = original.LastChunk;
            this.BPlayed = original.BPlayed;
            this.NbTowers = original.NbTowers;
            this.NbTemple = original.NbTemple;
            this.NbBarrack = original.NbBarrack;
            this.PlayerIA = original.PlayerIA;
        }

        public abstract Player Clone();

        public abstract PointRotation PlayChunk();

        public abstract (Building buil, Vector2Int pos) PlayBuild();
        
    }
}