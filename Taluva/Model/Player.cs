using System.Drawing;

namespace Taluva.Model;

public class Player
{
	private Chunk lastChunk;
	private bool bPlayed;
	public PlayerColor ID { get; private set; }
	private int nbTowers = 2;
	private int nbTemple = 3;
	private int nbBarrack = 20;

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