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
                gameBoard.RotateChunk(null);
                break;
            case TurnPhase.SelectCells:
                break;
            default:
                throw new ArgumentException($"no phase: {phase}");
        }
    }
}