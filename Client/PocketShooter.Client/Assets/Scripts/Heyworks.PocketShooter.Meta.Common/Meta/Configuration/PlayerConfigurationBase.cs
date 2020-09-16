using System;
using System.Collections.Generic;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    /// <summary>
    /// Configuration component that contains player related configuration methods shared between client and server.
    /// </summary>
    public class PlayerConfigurationBase : IPlayerConfigurationBase
    {
        #region [Private Members]

        private readonly GameConfig gameConfig;

        #endregion [Private Members]

        #region [Constructors and initialization]

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerConfigurationBase"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public PlayerConfigurationBase(GameConfig gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #endregion [Constructors and initialization]

        #region [Implementation of IPlayerConfigurationBase]

        /// <summary>
        /// Gets a player's max level.
        /// </summary>
        public int GetMaxLevel()
        {
            return
                gameConfig
                    .PlayerLevelsConfig
                    .Max(item => item.Level);
        }

        /// <summary>
        /// Gets an experience required to get next level.
        /// </summary>
        /// <param name="currentLevel">The current level.</param>
        /// <param name="expInLevel">The experience in current level.</param>
        public int GetExperienceForNextLevel(int currentLevel, int currentExpInLevel)
        {
            if (currentLevel == GetMaxLevel())
            {
                return 0;
            }

            var currentLevelConfig = GetPlayerLevelConfig(currentLevel);

            return currentLevelConfig.ExperienceInLevel - currentExpInLevel;
        }

        /// <summary>
        /// Gets a reward for player's level up.
        /// </summary>
        /// <param name="levelBefore">The initial player's level.</param>
        /// <param name="levelAfter">The new player's level.</param>
        public IEnumerable<IContentIdentity> GetLevelUpReward(int levelBefore, int levelAfter)
        {
            var totalLevelUpReward = Enumerable.Empty<IContentIdentity>();
            for (int i = 1; i <= levelAfter - levelBefore; i++)
            {
                var playerLevelConfig = GetPlayerLevelConfig(levelBefore + i);
                totalLevelUpReward = totalLevelUpReward.Concat(playerLevelConfig.Reward);
            }

            return totalLevelUpReward.ToList();
        }

        /// <summary>
        /// Gets an interval for providing repeating rewards.
        /// </summary>
        /// <param name="playerLevel">The player's level.</param>
        public TimeSpan GetRepeatingRewardInterval(int playerLevel)
        {
            var playerLevelConfig = GetPlayerLevelConfig(playerLevel);

            return playerLevelConfig.RepeatingRewardInterval;
        }

        /// <summary>
        /// Gets a repeating provided reward.
        /// </summary>
        /// <param name="playerLevel">The player's level.</param>
        public IEnumerable<IContentIdentity> GetRepeatingReward(int playerLevel)
        {
            var playerLevelConfig = GetPlayerLevelConfig(playerLevel);

            return playerLevelConfig.RepeatingReward;
        }

        #endregion [Implementation of IPlayerConfigurationBase]

        protected PlayerLevelConfig GetPlayerLevelConfig(int playerLevel)
        {
            var playerLevelConfig = gameConfig
                .PlayerLevelsConfig
                .SingleOrDefault(item => item.Level == playerLevel);

            if (playerLevelConfig == null)
            {
                throw new ConfigurationException($"Can't find player level config for level {playerLevel}");
            }

            return playerLevelConfig;
        }
    }
}
