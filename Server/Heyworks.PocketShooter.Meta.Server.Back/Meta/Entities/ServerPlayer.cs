using System;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;

namespace Heyworks.PocketShooter.Meta.Entities
{
    public class ServerPlayer : PlayerBase
    {
        private readonly ServerPlayerState playerState;
        private readonly IPlayerConfiguration playerConfiguration;

        public ServerPlayer(Guid id, string nickname, string deviceId, string group, IDateTimeProvider timeProvider, IPlayerConfiguration playerConfiguration)
            : this(new ServerPlayerState(id, nickname, deviceId, group), timeProvider, playerConfiguration)
        {
        }

        public ServerPlayer(ServerPlayerState playerState, IDateTimeProvider timeProvider, IPlayerConfiguration playerConfiguration)
            : base(playerState, timeProvider, playerConfiguration)
        {
            this.playerState = playerState;
            this.playerConfiguration = playerConfiguration;
        }

        /// <summary>
        /// Gets the player's application bundle identifier.
        /// </summary>
        public string BundleId
        {
            get => playerState.BundleId;
            private set => playerState.BundleId = value;
        }

        /// <summary>
        /// Gets the current application store name.
        /// </summary>
        public ApplicationStoreName ApplicationStore
        {
            get => playerState.ApplicationStore;
            private set => playerState.ApplicationStore = value;
        }

        /// <summary>
        /// Gets  a player's game client version.
        /// </summary>
        public string ClientVersion
        {
            get => playerState.ClientVersion;
            private set => playerState.ClientVersion = value;
        }

        /// <summary>
        /// Gets a player's learning meter value.
        /// </summary>
        public int LearningMeter
        {
            get => playerState.LearningMeter;
            private set => playerState.LearningMeter = value;
        }

        /// <summary>
        /// Gets the max amount of real physical money in USD spent on in-app purchases.
        /// </summary>
        public double IAPMax
        {
            get => playerState.IAPMax;
            private set => playerState.IAPMax = value;
        }

        /// <summary>
        /// Updates a client data.
        /// </summary>
        /// <param name="bundleId">The application bundle id.</param>
        /// <param name="applicationStore">The application store name.</param>
        /// <param name="clientVersion">The client version.</param>
        public void UpdateClientData(string bundleId, ApplicationStoreName applicationStore, string clientVersion)
        {
            BundleId = bundleId;
            ApplicationStore = applicationStore;
            ClientVersion = clientVersion;
        }

        /// <summary>
        /// Updates a player's group.
        /// </summary>
        /// <param name="newGroup">The new group.</param>
        public void UpdateGroup(string newGroup)
        {
            playerState.Group = newGroup;
        }

        /// <summary>
        /// Register's an in-app purchase on a player.
        /// </summary>
        /// <param name="purchase">The in-app purchase to register.</param>
        public void RegisterPurchase(InAppPurchase purchase)
        {
            IAPCount++;
            IAPTotalUSD += purchase.PriceUSD;
            IAPMax = Math.Max(IAPMax, purchase.PriceUSD);
        }

        /// <summary>
        /// Starts a match.
        /// </summary>
        public void StartMatch()
        {
            MatchesCount++;
        }

        /// <summary>
        /// Applies a match results to a player.
        /// </summary>
        /// <param name="teamPosition">The player's position in team.</param>
        /// <param name="kills">The player's kills.</param>
        /// <param name="isWin">The value indicating whether a player wins battle.</param>
        public PlayerReward ApplyMatchResults(int teamPosition, int kills, bool isWin)
        {
            var reward = playerConfiguration.GetBattleReward(Level, teamPosition, kills, isWin);
            ApplyReward(reward);

            LearningMeter += playerConfiguration.GetLearningMeterReward(teamPosition, isWin);

            return reward;
        }

        /// <summary>
        /// Sets a player's registration date.
        /// </summary>
        /// <param name="date">The date to set.</param>
        public void SetRegistrationDate(DateTime date)
        {
            playerState.RegisteredAt = date;
        }

        public ServerPlayerState GetState()
        {
            // TODO: Think about state cloning to prevent modifications from outside.
            return playerState;
        }

        protected override void OnExperienceChanged(int addValue)
        {
            playerState.TotalExperience += addValue;
        }
    }
}
