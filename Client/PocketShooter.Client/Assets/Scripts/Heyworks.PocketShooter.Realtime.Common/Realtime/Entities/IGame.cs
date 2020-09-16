namespace Heyworks.PocketShooter.Realtime.Entities
{
    /// <summary>
    /// Base game interface for client and server.
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Gets generic player from collection of totally all players in game.
        /// </summary>
        /// <param name="id">The player id.</param>
        /// <returns>Generic player.</returns>
        IPlayer GetPlayer(EntityId id);

        /// <summary>
        /// Gets the first team.
        /// </summary>
        Team Team1 { get; }

        /// <summary>
        /// Gets the second team.
        /// </summary>
        Team Team2 { get; }

        /// <summary>
        /// Get the team by its number.
        /// </summary>
        /// <param name="number">The number of the team.</param>
        /// <returns>Team.</returns>
        Team GetTeam(TeamNo number);

        ConsumablesMatchState ConsumablesState { get; }
    }
}