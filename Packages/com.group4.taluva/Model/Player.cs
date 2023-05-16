using System;

using Taluva.Model;
using Taluva.Model.AI;
using UnityEngine;

public class Player
{
    /// <summary>
    /// The last chunk placed by the player.
    /// </summary>
    public Chunk lastChunk { get; set; }

    private bool b_played;

    /// <summary>
    /// The ID representing the player's color.
    /// </summary>
    public PlayerColor ID { get; private set; }

    /// <summary>
    /// The number of remaining towers the player can place.
    /// </summary>
    public int nbTowers = 2;

    /// <summary>
    /// The number of remaining temples the player can place.
    /// </summary>
    public int nbTemple = 3;

    /// <summary>
    /// The number of remaining barracks the player can place.
    /// </summary>
    public int nbBarrack = 20;

    /// <summary>
    /// Indicates if the player is controlled by the AI.
    /// </summary>
    public bool playerIA = false;

    /// <summary>
    /// Creates a new instance of the Player class with the specified ID.
    /// </summary>
    /// <param name="id">The ID representing the player's color.</param>
    public Player(PlayerColor id)
    {
        this.ID = id;
    }

    /// <summary>
    /// Sets the player as controlled by the AI.
    /// </summary>
    public void SetAi()
    {
        playerIA = true;
    }

    /// <summary>
    /// Places a chunk on the game board.
    /// </summary>
    /// <param name="gameBoard">The game board.</param>
    /// <param name="p">The position and rotations of the chunk.</param>
    /// <param name="chunk">The chunk to place.</param>
    /// <param name="r">The rotation of the chunk.</param>
    public void PlaceChunk(Board gameBoard, PointRotation p, Chunk chunk, Rotation r)
    {
        gameBoard.AddChunk(chunk, this, p, r);
    }

    /// <summary>
    /// Places a building on the game board.
    /// </summary>
    /// <param name="gameBoard">The game board.</param>
    /// <param name="c">The cell where the building will be placed.</param>
    /// <param name="b">The building to place.</param>
    public void PlaceBuilding(Board gameBoard, Cell c, Building b)
    {
        gameBoard.PlaceBuilding(c, b, this);
    }

    /// <summary>
    /// Performs the player's turn based on the specified phase.
    /// </summary>
    /// <param name="phase">The current turn phase.</param>
    public void Play(TurnPhase phase)
    {
        if (phase == TurnPhase.SelectCells)
        {
            // Code for selecting cells
        }
        else
        {
            // Code for placing buildings
        }
    }
}