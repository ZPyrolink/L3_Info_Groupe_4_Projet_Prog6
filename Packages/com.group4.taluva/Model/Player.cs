using System;

using Taluva.Model;
using Taluva.Model.AI;
using UnityEngine;

public class Player
{
    public Chunk lastChunk { get; set; }
    public bool b_played;
    public PlayerColor ID { get; private set; }
    public int nbTowers = 2;
    public int nbTemple = 3;
    public int nbBarrack = 20;
    public bool playerIA = false;
    public bool Eliminated { get; private set; }

    public Player(PlayerColor id)
    {
        this.ID = id;
    }

    public void SetAi()
    {
        playerIA = true;
    }

    public virtual Player Clone()
    {
        return new Player(this);
    }

    public Player(Player original) : this(original.ID)
    {
        Player clone = new Player(this.ID);
        clone.lastChunk = this.lastChunk;
        clone.b_played = this.b_played;
        clone.nbTowers = this.nbTowers;
        clone.nbTemple = this.nbTemple;
        clone.nbBarrack = this.nbBarrack;
        clone.playerIA = this.playerIA;
    }


    //Player placeChunk
    public void PlaceChunk(Board gameBoard, PointRotation p, Chunk chunk , Rotation r )
    {
        gameBoard.AddChunk(chunk, this, p, r);
        
    }
    public void PlaceBuilding(Board gameBoard, Cell c, Building b)
    {
        gameBoard.PlaceBuilding(c, b, this);
    }

    public void Eliminate() => Eliminated = true;
}
