using System;
using System.Collections.Generic;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public sealed class RewardProvider
    {
        private readonly RewardProviderState state;
        private readonly int playerLevel;
        private readonly IDateTimeProvider timeProvider;
        private readonly IPlayerConfigurationBase playerConfiguration;

        public RewardProvider(RewardProviderState state, int playerLevel, IDateTimeProvider timeProvider, IPlayerConfigurationBase playerConfiguration)
        {
            this.state = state;
            this.playerLevel = playerLevel;
            this.timeProvider = timeProvider;
            this.playerConfiguration = playerConfiguration;
        }

        public TimeSpan RemainingTime => NextRewardAvailableAt - timeProvider.UtcNow;

        public bool CanGetReward() => IsRewardAvailable;

        public IEnumerable<IContentIdentity> GetReward()
        {
            if (!CanGetReward())
            {
                throw new InvalidOperationException("Can't get reward now.");
            }

            NextRewardAvailableAt = timeProvider.UtcNow + playerConfiguration.GetRepeatingRewardInterval(playerLevel);

            return playerConfiguration.GetRepeatingReward(playerLevel);
        }

        private DateTime NextRewardAvailableAt
        {
            get => state.NextRewardAvailableAt;
            set => state.NextRewardAvailableAt = value;
        }

        private bool IsRewardAvailable => timeProvider.UtcNow >= NextRewardAvailableAt;
    }
}
