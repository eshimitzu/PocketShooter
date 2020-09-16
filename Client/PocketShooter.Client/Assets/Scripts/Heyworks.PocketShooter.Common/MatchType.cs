namespace Heyworks.PocketShooter
{
    /// <summary>
    /// Match type shared cross all game.
    /// </summary>
    public enum MatchType
    {
        // production match making
        Domination = 0,

        // play only with bots
        DominationBots,

        // play with any currently open room with players
        DominationFast,
    }
}