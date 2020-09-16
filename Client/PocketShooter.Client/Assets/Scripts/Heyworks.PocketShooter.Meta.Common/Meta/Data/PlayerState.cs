using System;

namespace Heyworks.PocketShooter.Meta.Data
{
    public class PlayerState
    {
        const int MinLevel = 1;

        protected PlayerState()
        {
            Level = MinLevel;
            RewardProvider = new RewardProviderState();
        }

        public PlayerState(Guid id, string nickname, string deviceId, string group)
            : this()
        {
            Id = id;
            Nickname = nickname;
            DeviceId = deviceId;
            Group = group;
        }

        /// <summary>
        /// Gets or sets a player's id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the player's nickname.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or set a player's device id.
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets a player's group.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Gets or sets the player's country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the total number of in-app purchases.
        /// </summary>
        public int IAPCount { get; set; }

        /// <summary>
        /// Gets or sets the total amount of real physical money in USD spent on in-app purchases.
        /// </summary>
        public double IAPTotalUSD { get; set; }

        /// <summary>
        /// Gets or sets the current amount of cash.
        /// </summary>
        public int Cash { get; set; }

        /// <summary>
        /// Gets or sets the current amount of gold.
        /// </summary>
        public int Gold { get; set; }

        /// <summary>
        /// Gets or sets a player's level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the player's experience in current level.
        /// </summary>
        public int ExperienceInLevel { get; set; }

        /// <summary>
        /// Gets or sets a number of matches.
        /// </summary>
        public int MatchesCount { get; set; }

        /// <summary>
        /// Gets or sets a player's registration date.
        /// </summary>
        public DateTime RegisteredAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the game analytics gathering is disabled for the given player.
        /// </summary>
        public bool IsAnalyticsDisabled { get; set; }

        /// <summary>
        /// Gets or sets a reward provider.
        /// </summary>
        public RewardProviderState RewardProvider { get; set; }
    }
}
