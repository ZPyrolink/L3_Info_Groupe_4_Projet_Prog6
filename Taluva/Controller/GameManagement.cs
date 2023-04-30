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

	public void SetNumbersOfPlayers(int nbPlayers)
	{
		throw new NotImplementedException();
	}

	public void SetNumbersOfAI(int nbAI)
	{
		throw new NotImplementedException();
	}

	public void SetAIDifficulty(Difficulty d)
	{
		throw new NotImplementedException();
	}
}
