using System;
using System.Drawing;

public class Player
{
	Chunk lastChunk;
	bool b_played;
    public PlayerColor ID { get; private set; }
	int nbTowers = 2;
	int nbTemple = 3;
	int nbBarrack = 20;

	public Player(PlayerColor ID)
	{
		this.ID = ID;
	}
	public void Play(TurnPhase Phase, Board GameBoard,Point p,Building b)
	{
		switch (Phase)
		{
			case TurnPhase.PlaceBuilding:
				GameBoard.PlaceBuilding(p,b,this);
				break;
			case TurnPhase.RotateCell:
				GameBoard.RotateChunk(null);
				break;
			case TurnPhase.SelectCells:
				
				break;
			default:
				throw new ArgumentException($"no phase: {Phase}");
		}
	}
	
	
}
