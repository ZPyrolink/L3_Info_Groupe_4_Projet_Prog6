using Taluva.Model;

namespace Taluva.Controller;

public class GameManagment
{
	private Player[] players;
	private Player actualPlayer;
	private int nbPlayers;
	private int actualTurn;
	private int maxTurn;
	private Board gameBoard;
	private TurnPhase actualPhase;

	public GameManagment(int nbPlayers , int actualTurn)
	{
		this.players = new Player[nbPlayers];
		this.actualTurn = actualTurn;
	}

	private Player GetWinner()
	{
		throw new NotImplementedException();
	}

	public void InitPlay()
	{
		throw new NotImplementedException();
	}

	public int NumberOfPlayers
	{
		set
		{
			throw new NotImplementedException();
		}
	}

	public int NumberOfAI
	{
		set
		{
			throw new NotImplementedException();
		}
	}

	public Difficulty AIDifficulty
	{
		set
		{
			throw new NotImplementedException();
		}
	}
}