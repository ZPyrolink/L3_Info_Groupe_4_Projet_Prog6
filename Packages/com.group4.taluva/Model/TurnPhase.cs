namespace Taluva.Model
{
    /// <summary>
    /// Represents the different phases of a player's turn in the game.
    /// </summary>
    public enum TurnPhase
    {
        /// <summary>
        /// The phase where the player selects cells for their turn.
        /// </summary>
        SelectCells = 0,

        /// <summary>
        /// The phase where the player places a building on the board.
        /// </summary>
        PlaceBuilding = 1,

        /// <summary>
        /// The phase where the turn ends and the next player's turn begins.
        /// </summary>
        NextPlayer = 2,

        /// <summary>
        /// The phase where the AI player makes its move.
        /// </summary>
        IAPlays = 3
    }
}