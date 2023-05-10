using UnityEngine;

namespace Taluva.Model.AI
{
    public abstract class AI : Player
    {
        //ArrayList<Village> Villages;
        protected Board board;

        public AI(PlayerColor id) : base(id)
        {
        }
        
        public AI(PlayerColor id,Board board) : base(id)
        {
            this.board = board;
        }

        public abstract PointRotation PlayChunk();

        public abstract (Building buil, Vector2Int pos) PlayBuild();
    }
}