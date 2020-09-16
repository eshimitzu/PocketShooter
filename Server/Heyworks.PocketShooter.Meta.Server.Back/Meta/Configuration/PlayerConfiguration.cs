using System;
using System.Linq;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    /// <summary>
    /// Represents a player configuration.
    /// </summary>
    public class PlayerConfiguration : PlayerConfigurationBase, IPlayerConfiguration
    {
        private readonly ServerGameConfig gameConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerConfiguration"/> class.
        /// </summary>
        /// <param name="gameConfig">The game config.</param>
        public PlayerConfiguration(ServerGameConfig gameConfig)
            : base(gameConfig)
        {
            this.gameConfig = gameConfig;
        }

        #region [IPlayerConfiguration Implementation]

        /// <summary>
        /// Gets a battle reward for a player.
        /// </summary>
        /// <param name="playerLevel">The player's level.</param>
        /// <param name="teamPosition">The player's position in team.</param>
        /// <param name="kills">The player's kills.</param>
        /// <param name="isWin">The value indicating whether the player wins this battle.</param>
        public PlayerReward GetBattleReward(int playerLevel, int teamPosition, int kills, bool isWin)
        {
            var playerLevelConfig = GetPlayerLevelConfig(playerLevel);

            var positionRewardConfig = gameConfig.BattlePositionsRewardConfig.SingleOrDefault(item => item.TeamPosition == teamPosition);
            if (positionRewardConfig == null)
            {
                throw new ConfigurationException($"Can't find team position reward config for position {teamPosition}");
            }

            var battleRewardFactors = gameConfig.BattleRewardFactorsConfig;

            var goldReward = teamPosition == 1 ? playerLevelConfig.GoldBattleRewardBase : 0;

            var cashReward = (int)Math.Round(
                battleRewardFactors.CashPlayerLevelFactor * playerLevelConfig.CashBattleRewardBase * kills +
                battleRewardFactors.CashTeamPositionFactor * (isWin ? positionRewardConfig.CashRewardWin : positionRewardConfig.CashRewardLose));

            var expReward = (int)Math.Round(
                battleRewardFactors.ExpPlayerLevelFactor * playerLevelConfig.ExpBattleRewardBase *
                battleRewardFactors.ExpTeamPositionFactor * (isWin ? positionRewardConfig.ExpRewardWin : positionRewardConfig.ExpRewardLose));

            return new PlayerReward(cashReward, goldReward, expReward);
        }

        /// <summary>
        /// Gets a learning meter reward for battle.
        /// </summary>
        /// <param name="teamPosition">The player's position in team.</param>
        /// <param name="isWin">The value indicating whether the player wins this battle.</param>
        public int GetLearningMeterReward(int teamPosition, bool isWin)
        {
            var positionRewardConfig = gameConfig.BattlePositionsRewardConfig.SingleOrDefault(item => item.TeamPosition == teamPosition);
            if (positionRewardConfig == null)
            {
                throw new ConfigurationException($"Can't find team position reward config for position {teamPosition}");
            }

            return isWin ? positionRewardConfig.LearningMeterRewardWin : positionRewardConfig.LearningMeterRewardLose;
        }

        #endregion [IPlayerConfiguration Implementation]
    }
}
