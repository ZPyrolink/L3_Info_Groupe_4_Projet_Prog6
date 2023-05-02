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

                if (gameBoard.GetTempleSlots(p).Length ==  p.nbTemple)
                    completedBuildingTypes++;

                if (gameBoard.GetBarracksSlots(p).Length == p.nbBarrack)
                    completedBuildingTypes++;

                if (gameBoard.GetTowerSlots(p).Length == p.nbTowers)
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

            if (gameBoard.GetTempleSlots(p).Length ==  p.nbTemple)
                completedBuildingTypes++;

            if (gameBoard.GetBarracksSlots(p).Length == p.nbBarrack)
                completedBuildingTypes++;

            if (gameBoard.GetTowerSlots(p).Length == p.nbTowers)
                completedBuildingTypes++;

            if (completedBuildingTypes >= 2)
                return p;
        }

        throw new NotImplementedException();
    }

    public void InitPlay()
    {
        while (actualTurn <= maxTurn)
        {
            actualPhase = TurnPhase.PlaceBuilding;
            actualPhase = TurnPhase.RotateCell;
            actualPhase = TurnPhase.SelectCells;

            actualTurn++;
            if (actualTurn > NbPlayers)
            {
                actualTurn = 1;
            }

            actualPlayer = players[actualTurn - 1];
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