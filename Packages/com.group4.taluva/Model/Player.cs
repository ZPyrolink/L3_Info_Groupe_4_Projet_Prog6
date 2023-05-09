using System;

using Taluva.Model;

using UnityEngine;

public class Player
{
    public Chunk lastChunk { get;  set ;  }
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

    public void Play(TurnPhase phase, Board gameBoard, Vector2 p, Building b, Chunk chunk)
    {
        throw new NotImplementedException();
    }

}
