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

    public GameManagment(int nbPlayers, int maxTurn)
    {
        this.players = new Player[nbPlayers];
        this.actualTurn = 1;
        this.gameBoard = new Board();
        this.nbPlayers = nbPlayers;
        this.maxTurn = maxTurn;
        for (int i = 0; i < this.nbPlayers; i++)
            players[i] = new Player((PlayerColor)i);
    }

    private Player GetWinner()
    {
        foreach (Player p in players)
        {
            int completedBuildingTypes = 0;

            if (gameBoard.GetTempleSlots(p).Length> 0)
                completedBuildingTypes++;

            if (gameBoard.GetBarracksSlots(p).Length > 0)
                completedBuildingTypes++;

            if (gameBoard.GetTowerSlots(p).Length > 0)
                completedBuildingTypes++;

            if (completedBuildingTypes >= 2)
                return p;
        }

        throw new NotImplementedException();
    }

    public void InitPlay()
    {
        actualPhase = TurnPhase.PlaceBuilding;
        actualPhase = TurnPhase.RotateCell;
        actualPhase = TurnPhase.SelectCells;
        actualTurn++;
        if (actualTurn > maxTurn)
            actualTurn = 1;
    }

    public void SetNumbersOfPlayers(int nbPlayers)
    {
        this.nbPlayers = nbPlayers;
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