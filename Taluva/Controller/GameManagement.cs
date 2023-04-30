using System;
using static Board;
public class GameManagment
{
    Player[] players;
    Player ActualPlayer;
    int nbPlayers;
    int ActualTurn;
    int MaxTurn;
    Board GameBoard;
    TurnPhase ActualPhase;

    public GameManagment(int nbPlayers, int maxTurn)
    {
        this.players = new Player[nbPlayers];
        this.ActualTurn = 1;
        this.GameBoard = new Board();
        this.nbPlayers = nbPlayers;
        this.MaxTurn = maxTurn;
        for (int i = 0; i < this.nbPlayers; i++)
            players[i] = new Player((PlayerColor)i);
    }

    private Player GetWinner()
    {
        foreach (Player p in players)
        {
            int completedBuildingTypes = 0;

            if (GameBoard.GetTempleSlots(p).Length> 0)
                completedBuildingTypes++;

            if (GameBoard.GetBarracksSlots(p).Length > 0)
                completedBuildingTypes++;

            if (GameBoard.GetTowerSlots(p).Length > 0)
                completedBuildingTypes++;

            if (completedBuildingTypes >= 2)
                return p;
        }

        return null;
    }

    public void InitPlay()
    {
        ActualPhase = TurnPhase.PlaceBuilding;
        ActualPhase = TurnPhase.RotateCell;
        ActualPhase = TurnPhase.SelectCells;
        ActualTurn++;
        if (ActualTurn > MaxTurn)
            ActualTurn = 1;
    }

    public void SetNumbersOfPlayers(int nbPlayers)
    {
        this.nbPlayers = nbPlayers;
    }

    public void SetNumbersOfAI(int nbAI)
    {

    }

    public void SetAIDifficulty(Difficulty d)
    {

    }
}