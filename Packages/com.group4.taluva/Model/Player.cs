using System;

using UnityEngine;

namespace Taluva.Model
{
    public class Player
    {
        /// <summary>
        /// The last chunk placed by the player.
        /// </summary>
        public Chunk LastChunk { get; set; }

        [Obsolete("Use LastChunk instead")]
        public Chunk lastChunk
        {
            get => LastChunk;
            set => LastChunk = value;
        }

        /// <summary>
        /// The ID representing the player's color.
        /// </summary>
        public PlayerColor ID { get; private set; }

        /// <summary>
        /// The color of the player's ID.
        /// </summary>
        public Color IdColor => ID.GetColor();

        /// <summary>
        /// The number of remaining towers the player can place.
        /// </summary>
        public int NbTowers = 2;

        /// <summary>
        /// The number of remaining temples the player can place.
        /// </summary>
        public int NbTemple = 3;

        /// <summary>
        /// The number of remaining barracks the player can place.
        /// </summary>
        public int NbBarrack = 20;

        /// <summary>
        /// Indicates if the player has been eliminated from the game.
        /// </summary>
        public bool Eliminated { get; set; }

        /// <summary>
        /// Creates a new instance of the Player class with the specified ID.
        /// </summary>
        /// <param name="id">The ID representing the player's color.</param>
        public Player(PlayerColor id)
        {
            ID = id;
        }

        /// <summary>
        /// Creates a new instance of the Player class based on an existing player instance.
        /// </summary>
        /// <param name="original">The original player instance to clone.</param>
        public Player(Player original) : this(original.ID)
        {
            Player clone = new(ID);
            clone.LastChunk = LastChunk;
            clone.NbTowers = NbTowers;
            clone.NbTemple = NbTemple;
            clone.NbBarrack = NbBarrack;
        }

        /// <summary>
        /// Creates a clone of the player.
        /// </summary>
        /// <returns>A new instance of the Player class representing the clone.</returns>
        public Player Clone() => new(this);

        /// <summary>
        /// Eliminates the player from the game.
        /// </summary>
        public void Eliminate() => Eliminated = true;
    }
}
