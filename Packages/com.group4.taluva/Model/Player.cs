using System;

using Taluva.Model;
using Taluva.Model.AI;
using UnityEngine;

public class Player
{
    public Chunk lastChunk { get; set; }
    bool b_played;
    public PlayerColor ID { get; private set; }
    public int nbTowers = 2;
    public int nbTemple = 3;
    public int nbBarrack = 20;
    public bool playerIA = false;

    public Player(PlayerColor id)
    {
        this.ID = id;
    }

    public void SetAi()
    {
        playerIA = true;
    }


    
    //Player play function
    public void PlaceChunk(Board gameBoard, Vector2Int p, Chunk chunk)
    {
        gameBoard.AddChunk(chunk, this, new PointRotation(p), chunk.rotation);
        
    }
    public void PlaceBuilding(Board gameBoard, Cell c, Building b)
    {
        gameBoard.PlaceBuilding(c, b, this);
    }
}
