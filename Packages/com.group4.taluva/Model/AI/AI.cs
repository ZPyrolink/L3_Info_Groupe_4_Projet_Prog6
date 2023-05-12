using System;
using System.Collections.Generic;
using Taluva.Controller;
using UnityEngine;

namespace Taluva.Model.AI
{
    public abstract class AI : Player
    {
        protected readonly GameManagment gm;
        protected readonly Pile<Chunk> pile;

        public AI(PlayerColor id, GameManagment gm, Pile<Chunk> pile) : base(id)
        {
            this.gm = gm;
            this.pile = pile;
        }

        public AI(AI original) :base(original.ID)
        {
            this.gm = original.gm;
            this.pile = original.pile;
            this.lastChunk = original.lastChunk;
            this.b_played = original.b_played;
            this.nbTowers = original.nbTowers;
            this.nbTemple = original.nbTemple;
            this.nbBarrack = original.nbBarrack;
            this.playerIA = original.playerIA;
        }

        public abstract Player Clone();

        public abstract PointRotation PlayChunk();

        public abstract (Building buil, Vector2Int pos) PlayBuild();
        
    }
}