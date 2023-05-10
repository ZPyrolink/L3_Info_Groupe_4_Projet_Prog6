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


    public void Play(TurnPhase phase, Board gameBoard, Vector2Int p, Building b, Chunk chunk, Cell c)
    {
        if (this is AI)
        {
            PointRotation pro = ((AI)this).PlayChunk();
            if (phase == TurnPhase.SelectCells)
                gameBoard.AddChunk(chunk, this, pro, chunk.rotation);
            else if (phase == TurnPhase.PlaceBuilding)
            {
                Building bAI;
                Vector2Int vAI;
                (bAI, vAI) = ((AI)this).PlayBuild();
                Cell cAI = gameBoard.WorldMap.GetValue(vAI);
                gameBoard.PlaceBuilding(cAI, bAI, this);
            }
        }
        else
        {
            if (phase == TurnPhase.SelectCells)
                gameBoard.AddChunk(chunk, this, new PointRotation(p), chunk.rotation);
            else if (phase == TurnPhase.PlaceBuilding)
            {
                gameBoard.PlaceBuilding(c, b, this);
            }
        }

    }
}
