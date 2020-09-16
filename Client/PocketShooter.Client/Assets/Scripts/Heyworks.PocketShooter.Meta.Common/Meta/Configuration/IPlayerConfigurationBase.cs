using Heyworks.PocketShooter.Meta.Entities;
using System;
using System.Collections.Generic;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IPlayerConfigurationBase
    {
        /// <summary>
        /// Gets a player's max level.
        /// </summary>
        int GetMaxLevel();

        /// <summary>
        /// Gets an experience required to get next level.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="expInLevel">The experience in current level.</param>
        /// <returns></returns>
        int GetExperienceForNextLevel(int currentLevel, int currentExpInLevel);

        /// <summary>
        /// Gets a reward for player's level up.
        /// </summary>
        /// <param name="levelBefore">The initial player's level.</param>
        /// <param name="levelAfter">The new player's level.</param>
        IEnumerable<IContentIdentity> GetLevelUpReward(int levelBefore, int levelAfter);

        /// <summary>
        /// Gets an interval for providing repeating rewards.
        /// </summary>
        /// <param name="playerLevel">The player's level.</param>
        TimeSpan GetRepeatingRewardInterval(int playerLevel);

        /// <summary>
        /// Gets a repeating provided reward.
        /// </summary>
        /// <param name="playerLevel">The player's level.</param>
        IEnumerable<IContentIdentity> GetRepeatingReward(int playerLevel);
    }
}
