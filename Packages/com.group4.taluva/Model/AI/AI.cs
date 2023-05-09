using UnityEngine;

namespace Taluva.Model.AI
{
    public class AI : Player
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

        public PointRotation PlayChunk()
        {
            return null;}

        public (Building buil, Vector2Int pos) PlayBuild()
        {
            return (Building.None, new());
        }

    }
}