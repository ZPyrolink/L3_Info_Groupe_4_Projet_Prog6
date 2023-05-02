using System;
using System.Drawing;
using Taluva.Model;

public class Player
{
	Chunk lastChunk;
	bool b_played;
    public PlayerColor ID { get; private set; }
	int nbTowers = 2;
	int nbTemple = 3;
	int nbBarrack = 20;

    public Player(PlayerColor id)
    {
        this.ID = id;
    }

    public void Play(TurnPhase phase, Board gameBoard, Point p, Building b)
    {
        switch (phase)
        {
            case TurnPhase.PlaceBuilding:
                gameBoard.PlaceBuilding(p, b, this);
                break;
            case TurnPhase.RotateCell:
                break;
            case TurnPhase.SelectCells:
                break;
            default:
                throw new ArgumentException($"no phase: {phase}");
        }
    }
}
