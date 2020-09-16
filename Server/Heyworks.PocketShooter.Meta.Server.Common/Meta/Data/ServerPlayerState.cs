using System;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class ServerPlayerState : PlayerState
    {
        public ServerPlayerState()
        {
        }

        public ServerPlayerState(Guid id, string nickname, string deviceId, string group)
            : base(id, nickname, deviceId, group)
        {
        }

        /// <summary>
        /// Gets or sets the player's application bundle identifier.
        /// </summary>
        public string BundleId { get; set; }

        /// <summary>
        /// Gets or sets the current application store name.
        /// </summary>
        public ApplicationStoreName ApplicationStore { get; set; }

        /// <summary>
        /// Gets or sets a player's game client version.
        /// </summary>
        public string ClientVersion { get; set; }

        /// <summary>
        /// Gets or sets a player's learning meter value for education when he started to play game.
        /// </summary>
        public int LearningMeter { get; set; }

        /// <summary>
        /// Gets or sets a player's total experience value. 
        /// Semantically is <see cref="ExperienceInLevel" />.
        /// </summary>
        public int TotalExperience { get; set; }

        /// <summary>
        /// Gets or sets the max amount of real physical money in USD spent on in-app purchases.
        /// </summary>
        public double IAPMax { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="PlayerState"/> class.
        /// </summary>
        public ClientPlayerState ToClientState()
        {
            return new ClientPlayerState(Id, Nickname, DeviceId, Group)
            {
                Country = Country,
                IAPCount = IAPCount,
                IAPTotalUSD = IAPTotalUSD,
                Cash = Cash,
                Gold = Gold,
                Level = Level,
                ExperienceInLevel = ExperienceInLevel,
                MatchesCount = MatchesCount,
                IsAnalyticsDisabled = IsAnalyticsDisabled,
                RegisteredAt = RegisteredAt,
                RewardProvider = RewardProvider,
            };
        }
    }
}
