using Taluva.Model;

namespace Taluva.Controller;

public class GameManagment
{
    private Player[] players;
    private Player actualPlayer;
    public int NbPlayers { get; set; }
    private int actualTurn;
    private int maxTurn;
    private Board gameBoard;
    private TurnPhase actualPhase;

    public GameManagment(int nbPlayers, int maxTurn)
    {
        this.players = new Player[nbPlayers];
        this.actualTurn = 1;
        this.gameBoard = new Board();
        this.NbPlayers = nbPlayers;
        this.maxTurn = maxTurn;
        for (int i = 0; i < this.NbPlayers; i++)
            players[i] = new Player((PlayerColor)i);
    }

    private Player Winner
    {
        get
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
    }

    [Obsolete($"Use {nameof(Winner)} property instead")]
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
    
    [Obsolete($"Use the {nameof(nbPlayers)} auto-property instead")]
    public void SetNumbersOfPlayers(int nbPlayers)
    {
        this.NbPlayers = nbPlayers;
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