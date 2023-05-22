namespace Taluva.Model
{
    /// <summary>
    /// Represents different game end conditions.
    /// </summary>
    public enum GameEnd
    {
        /// <summary>
        /// The game ended prematurely.
        /// </summary>
        EarlyEnd,

        /// <summary>
        /// The game ended under normal circumstances.
        /// </summary>
        NormalEnd,

        /// <summary>
        /// The last player standing wins the game.
        /// </summary>
        LastPlayerStanding
    }
}