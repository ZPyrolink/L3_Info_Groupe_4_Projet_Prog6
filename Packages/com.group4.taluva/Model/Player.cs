using System;

using Taluva.Utils;

using UnityEngine;

using UColor = UnityEngine.Color;

namespace Taluva.Model
{
    public class Player
    {
        public Chunk LastChunk { get; set; }

        [Obsolete("Use LastChunk instead", true)]
        public Chunk lastChunk
        {
            get => LastChunk;
            set => LastChunk = value;
        }

        public Color ID { get; private set; }
        public UColor IdColor => ID.GetColor();
        public int NbTowers = 2;
        public int NbTemple = 3;
        public int NbBarrack = 20;

        public bool Eliminated { get; set; }

        public Player(Color id)
        {
            ID = id;
        }

        public virtual Player Clone() => new(this);

        public Player(Player original) : this(original.ID)
        {
            Player clone = new(ID)
            {
                LastChunk = LastChunk,
                NbTowers = NbTowers,
                NbTemple = NbTemple,
                NbBarrack = NbBarrack
            };
        }

        public void Eliminate() => Eliminated = true;

        public enum Color : uint
        {
            Red = 0xFFD20F28,
            Green = 0xFF00FF00,
            Blue = 0xFF2E00FF,
            Yellow = 0xFFFFD700
        }
    }
    
    public static class PlayerColorExt
    {
        public static Color GetColor(this Player.Color pc) => ColorUtils.From((uint) pc);
    }
}