using System;

using UnityEngine;

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

        public PlayerColor ID { get; private set; }
        public Color IdColor => ID.GetColor();
        public int NbTowers = 2;
        public int NbTemple = 3;
        public int NbBarrack = 20;

        public bool Eliminated { get; set; }

        public Player(PlayerColor id)
        {
            ID = id;
        }

    public virtual Player Clone() => new(this);

        public Player(Player original) : this(original.ID)
        {
            Player clone = new(ID);
            clone.LastChunk = LastChunk;
            clone.NbTowers = NbTowers;
            clone.NbTemple = NbTemple;
            clone.NbBarrack = NbBarrack;
        }

        public void Eliminate() => Eliminated = true;
    }
}