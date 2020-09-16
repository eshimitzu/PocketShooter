using System;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public abstract class PlayerBase
    {
        private readonly IDateTimeProvider timeProvider;
        private readonly IPlayerConfigurationBase playerConfiguration;

        private PlayerState playerState;

        protected PlayerBase(Guid id, string nickname, string deviceId, string group, IDateTimeProvider timeProvider, IPlayerConfigurationBase playerConfiguration)
            : this(new PlayerState(id, nickname, deviceId, group), timeProvider, playerConfiguration)
        {
        }

        protected PlayerBase(PlayerState playerState, IDateTimeProvider timeProvider, IPlayerConfigurationBase playerConfiguration)
        {
            this.Id = playerState.Id;
            this.playerState = playerState;
            this.timeProvider = timeProvider;
            this.playerConfiguration = playerConfiguration;
        }

        public Guid Id { get; }

        /// <summary>
        /// Gets a player's nickname.
        /// </summary>
        public virtual string Nickname
        {
            get => playerState.Nickname;
            set => playerState.Nickname = value;
        }

        /// <summary>
        /// Gets or sets the player's device id.
        /// </summary>
        public string DeviceId => playerState.DeviceId;

        /// <summary>
        /// Gets a player's group.
        /// </summary>
        public string Group => playerState.Group;

        /// <summary>
        /// Gets or sets a player's country.
        /// </summary>
        public string Country
        {
            get => playerState.Country;
            set => playerState.Country = value;
        }

        /// <summary>
        /// Gets the total number of in-app purchases.
        /// </summary>
        public int IAPCount
        {
            get => playerState.IAPCount;
            protected set => playerState.IAPCount = value;
        }

        /// <summary>
        /// Gets the total amount of real physical money in USD spent on in-app purchases.
        /// </summary>
        public double IAPTotalUSD
        {
            get => playerState.IAPTotalUSD;
            protected set => playerState.IAPTotalUSD = value;
        }

        /// <summary>
        /// Gets or sets the current amount of cash.
        /// </summary>
        public virtual int Cash
        {
            get => playerState.Cash;
            protected set => playerState.Cash = value;
        }

        /// <summary>
        /// Gets or sets the current amount of gold.
        /// </summary>
        public virtual int Gold
        {
            get => playerState.Gold;
            protected set => playerState.Gold = value;
        }

        /// <summary>
        /// Gets the current player's level.
        /// </summary>
        public virtual int Level
        {
            get => playerState.Level;
            protected set => playerState.Level = value;
        }

        /// <summary>
        /// Gets or sets the player's experience in current level.
        /// </summary>
        public virtual int ExperienceInLevel
        {
            get => playerState.ExperienceInLevel;
            protected set => playerState.ExperienceInLevel = value;
        }

        /// <summary>
        /// Gets a max experience value in current level.
        /// </summary>
        public int MaxExperienceInLevel => playerConfiguration.GetExperienceForNextLevel(Level, 0);

        /// <summary>
        /// Gets a number of matches.
        /// </summary>
        public int MatchesCount
        {
            get => playerState.MatchesCount;
            protected set => playerState.MatchesCount = value;
        }

        /// <summary>
        /// Gets a player's registration date.
        /// </summary>
        public DateTime RegisteredAt => playerState.RegisteredAt;

        /// <summary>
        /// Gets a value indicating whether the game analytics gathering is disabled for the given player.
        /// </summary>
        public bool IsAnalyticsDisabled => playerState.IsAnalyticsDisabled;

        /// <summary>
        /// Gets a value indicating whether a player has a max level.
        /// </summary>
        public bool HasMaxLevel => Level == playerConfiguration.GetMaxLevel();

        /// <summary>
        /// Gets a reward provider.
        /// </summary>
        public RewardProvider RewardProvider =>
            new RewardProvider(playerState.RewardProvider, Level, timeProvider, playerConfiguration);

        public void UpdateState(PlayerState playerState)
        {
            this.playerState = playerState;
        }

        /// <summary>
        /// Applies a reward to the player.
        /// </summary>
        /// <param name="reward">The reward to apply.</param>
        public void ApplyReward(PlayerReward reward)
        {
            AddResource(new ResourceIdentity(reward.Cash, reward.Gold));
            AddExperience(reward.Experience);
        }

        /// <summary>
        /// Determines whether a specified price can be payed.
        /// </summary>
        /// <param name="price">The price to check.</param>
        public bool CanPayPrice(Price price)
        {
            price.NotNull();

            switch (price.Type)
            {
                case PriceType.Gold:
                    return Gold >= price.GoldAmount;
                case PriceType.Cash:
                    return Cash >= price.CashAmount;
                case PriceType.RealCurrency:
                    return true;
                default:
                    throw new NotImplementedException($"The price type {price.Type} is not supported.");
            }
        }

        /// <summary>
        /// Pays a specified price.
        /// </summary>
        /// <param name="price">The price to pay.</param>
        public void PayPrice(Price price)
        {
            price.NotNull();

            if (!CanPayPrice(price))
            {
                throw new InvalidOperationException("Player can't pay provided price now.");
            }

            switch (price.Type)
            {
                case PriceType.Gold:
                    Gold -= price.GoldAmount;
                    break;
                case PriceType.Cash:
                    Cash -= price.CashAmount;
                    break;
                case PriceType.RealCurrency:
                    throw new InvalidOperationException($"The price in real currency can't be provided.");
                default:
                    throw new NotImplementedException($"The price type {price.Type} is not supported.");
            }
        }

        /// <summary>
        /// Add resources.
        /// </summary>
        public void AddResource(ResourceIdentity resource)
        {
            resource.NotNull();

            Cash += resource.Cash;
            Gold += resource.Gold;
        }

        /// <summary>
        /// Adds an experience to this player.
        /// </summary>
        /// <param name="addValue">The experience value to add.</param>
        public void AddExperience(int addValue)
        {
            var tmpAddValue = addValue;
            var experienceForNextLevel = playerConfiguration.GetExperienceForNextLevel(Level, ExperienceInLevel);

            if (addValue < experienceForNextLevel)
            {
                ExperienceInLevel += addValue;
            }
            else
            {
                while (!HasMaxLevel && (addValue - experienceForNextLevel >= 0))
                {
                    Level++;

                    addValue -= experienceForNextLevel;
                    experienceForNextLevel = MaxExperienceInLevel;
                }

                ExperienceInLevel = !HasMaxLevel ? addValue : 0;
            }

            OnExperienceChanged(tmpAddValue);
        }

        /// <summary>
        /// Returns a string that represents the current <see cref="PlayerBase"/> object.
        /// </summary>
        public override string ToString()
        {
            return string.Format(@"""Id : {0}, Nickname : {1}""", Id, Nickname);
        }

        protected virtual void OnExperienceChanged(int addValue) { }
    }
}
