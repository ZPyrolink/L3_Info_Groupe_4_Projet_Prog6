using System;


public class GameManagment
{
	Player[] players;
	Player ActualPlayer;
	int nbPlayers;
	int ActualTurn;
	int MaxTurn;
	Board GameBoard;
	TurnPhase ActualPhase;

	public GameManagment(int nbPlayers , int ActualTurn)
	{
		this.Players = new Player[nbPlayers];
		this.ActualTurn = ActualTurn;

	}

	private Player GetWinner()
	{

	}

	public void InitPlay()
	{

	}

	public void SetNumbersOfPlayers(int nbPlayers)
	{

	}

	public void SetNumbersOfAI(int nbAI)
	{

	}

	public void SetAIDifficulty(Difficulty d)
	{
		
	}


}
