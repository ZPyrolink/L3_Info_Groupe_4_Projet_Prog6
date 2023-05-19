using System;

namespace Taluva.Model
{
    
    public class Player
    {
        public Chunk LastChunk { get; set; }

        [Obsolete("Use LastChunk instead")]
        public Chunk lastChunk
        {
            get => LastChunk;
            set => LastChunk = value;
        }

        public bool BPlayed;
        [Obsolete("Use BPlayed instead")] public bool b_played => BPlayed;
        public PlayerColor ID { get; private set; }
        public int NbTowers = 2;

        public int NbTemple = 3;

        public int NbBarrack = 20;

        public bool PlayerIA;
        public bool Eliminated { get; set; }

        public Player(PlayerColor id)
        {
            this.ID = id;
        }

        public void SetAi()
        {
            PlayerIA = true;
        }

        public Player Clone()
        {
            return new Player(this);
        }

        public Player(Player original) : this(original.ID)
        {
            Player clone = new Player(this.ID);
            clone.LastChunk = this.LastChunk;
            clone.BPlayed = this.BPlayed;
            clone.NbTowers = this.NbTowers;
            clone.NbTemple = this.NbTemple;
            clone.NbBarrack = this.NbBarrack;
            clone.PlayerIA = this.PlayerIA;
        }


        //Player placeChunk
        public void PlaceChunk(Board gameBoard, PointRotation p, Chunk chunk, Rotation r)
        {
            gameBoard.AddChunk(chunk, this, p, r);

        }

        public void PlaceBuilding(Board gameBoard, Cell c, Building b)
        {
            gameBoard.PlaceBuilding(c, b, this);
        }

        public void Eliminate() => Eliminated = true;
    }
}