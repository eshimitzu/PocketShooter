using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IPlayerConfiguration : IPlayerConfigurationBase
    {
        /// <summary>
        /// Gets a battle reward for a player.
        /// </summary>
        /// <param name="playerLevel">The player's level.</param>
        /// <param name="teamPosition">The player's position in team.</param>
        /// <param name="kills">The player's kills.</param>
        /// <param name="isWin">The value indicating whether the player wins this battle.</param>
        PlayerReward GetBattleReward(int playerLevel, int teamPosition, int kills, bool isWin);

        /// <summary>
        /// Gets a learning meter reward for battle.
        /// </summary>
        /// <param name="teamPosition">The player's position in team.</param>
        /// <param name="isWin">The value indicating whether the player wins this battle.</param>
        int GetLearningMeterReward(int teamPosition, bool isWin);
    }
}
