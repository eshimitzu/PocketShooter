using System;
using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Meta.Utils;
using Heyworks.PocketShooter.Realtime.Data;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using Orleans.Providers;

namespace Heyworks.PocketShooter.Meta.Communication
{
    [StorageProvider(ProviderName = Server.Constants.GrainStorageProviderName)]
    public class PlayerGrain : Grain<ServerPlayerState>, IPlayerGrain, IPlayerContentGrain, IPlayerBalanceGrain
    {
        private readonly IPlayerFactory playerFactory;
        private readonly IArmyFactory armyFactory;
        private readonly IDateTimeProvider timeProvider;
        private readonly IConfigurationsProvider configurationsProvider;
        private readonly ILogger<PlayerGrain> logger;

        public PlayerGrain(IPlayerFactory playerFactory, IArmyFactory armyFactory, IDateTimeProvider timeProvider, IConfigurationsProvider configurationsProvider, ILogger<PlayerGrain> logger)
        {
            this.playerFactory = playerFactory;
            this.armyFactory = armyFactory;
            this.timeProvider = timeProvider;
            this.configurationsProvider = configurationsProvider;
            this.logger = logger;
        }

        public async Task Create(Immutable<CreatePlayerData> createPlayerData)
        {
            var data = createPlayerData.Value;
            var playerConfiguration = await configurationsProvider.GetPlayerConfiguration(data.Group);

            var player = new ServerPlayer(this.GetPrimaryKey(), data.Nickname, data.DeviceId, data.Group, timeProvider, playerConfiguration);

            player.Country = data.Country;
            player.UpdateClientData(data.BundleId, data.ApplicationStore, data.ClientVersion);
            player.SetRegistrationDate(DateTime.UtcNow);

            await SavePlayerState(player);
        }

        public Task<ServerPlayerState> GetState() => Task.FromResult(State);

        public Task<string> GetNickname() => Task.FromResult(State.Nickname);

        public Task<string> GetGroup() => Task.FromResult(State.Group);

        public async Task UpdateClientData(string bundleId, ApplicationStoreName applicationStore, string clientVersion)
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            player.UpdateClientData(bundleId, applicationStore, clientVersion);

            await SavePlayerState(player);
        }

        public async Task UpdateGroup(string group)
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            player.UpdateGroup(group);

            await SavePlayerState(player);
        }

        public Task<MatchMakingData> GetMatchMakingData() =>
            Task.FromResult(new MatchMakingData(State.LearningMeter, State.Level));

        public async Task StartMatch()
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            player.StartMatch();

            await SavePlayerState(player);
        }

        public async Task<PlayerReward> ApplyMatchResults(int teamPosition, int kills, bool isWin)
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            var levelBefore = player.Level;

            var reward = player.ApplyMatchResults(teamPosition, kills, isWin);
            await SavePlayerState(player);

            if (player.Level > levelBefore)
            {
                await ApplyLevelUpReward(levelBefore, player);
            }

            return reward;
        }

        public async Task<Immutable<(int accountLevel, PlayerInfo info)>> GetPlayerInfo()
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            var army = await armyFactory.Create(player);

            var info = CreatePlayerInfo(player.Id, player.Nickname, army);

            return (player.Level, info).AsImmutable();
        }

        public async Task<bool> ChangeNickname(string newNickname)
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());

            if (!NicknameValidator.IsValid(newNickname))
            {
                return false;
            }

            player.Nickname = newNickname;

            await SavePlayerState(player);

            return true;
        }

        public async Task<bool> CollectRepeatingReward()
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());

            if (!player.RewardProvider.CanGetReward())
            {
                return false;
            }

            var reward = player.RewardProvider.GetReward();

            await SavePlayerState(player);

            var gameContentGrain = GrainFactory.GetGrain<IGameContentGrain>(this.GetPrimaryKey());
            await gameContentGrain.ApplyContent(reward.AsImmutable());

            return true;
        }

        async Task IPlayerContentGrain.AddResource(Immutable<ResourceIdentity> resource)
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            player.AddResource(resource.Value);

            await SavePlayerState(player);
        }

        async Task IPlayerContentGrain.AddExperience(int experience)
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            player.AddExperience(experience);

            await SavePlayerState(player);
        }

        async Task IPlayerBalanceGrain.PayPrice(Immutable<Price> price)
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            player.PayPrice(price.Value);

            await SavePlayerState(player);
        }

        async Task IPlayerBalanceGrain.RegisterPurchase(Immutable<InAppPurchase> purchase)
        {
            var player = await playerFactory.Create(this.GetPrimaryKey());
            player.RegisterPurchase(purchase.Value);

            await SavePlayerState(player);
        }

        private static PlayerInfo CreatePlayerInfo(Guid id, string nickname, ServerArmy army) =>
            new PlayerInfo(
                id,
                nickname,
                army.Troopers.Select(item => item.GetTooperInfo()).ToArray(),
                new ConsumablesInfo(army.GetSelectedOffensivesCount(), army.GetSelectedSupportsCount()));

        private async Task ApplyLevelUpReward(int levelBefore, ServerPlayer player)
        {
            var playerConfiguration = await configurationsProvider.GetPlayerConfiguration(player.Group);

            var totalLevelUpReward = playerConfiguration.GetLevelUpReward(levelBefore, player.Level);

            var gameContentGrain = GrainFactory.GetGrain<IGameContentGrain>(this.GetPrimaryKey());
            await gameContentGrain.ApplyContent(totalLevelUpReward.AsImmutable());
        }

        private async Task SavePlayerState(ServerPlayer player)
        {
            State = player.GetState();
            await WriteStateAsync();
        }
    }
}