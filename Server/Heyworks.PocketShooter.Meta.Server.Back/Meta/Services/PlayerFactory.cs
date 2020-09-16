using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    internal class PlayerFactory : IPlayerFactory
    {
        private readonly IGrainFactory grainFactory;
        private readonly IDateTimeProvider timeProvider;
        private readonly IConfigurationsProvider configurationsProvider;

        public PlayerFactory(IGrainFactory grainFactory, IDateTimeProvider timeProvider, IConfigurationsProvider configurationsProvider)
        {
            this.grainFactory = grainFactory;
            this.timeProvider = timeProvider;
            this.configurationsProvider = configurationsProvider;
        }

        public async Task<ServerPlayer> Create(Guid playerId)
        {
            var playerGrain = grainFactory.GetGrain<IPlayerGrain>(playerId);
            var playerState = await playerGrain.GetState();

            var playerConfiguration = await configurationsProvider.GetPlayerConfiguration(playerState.Group);

            return new ServerPlayer(playerState, timeProvider, playerConfiguration);
        }
    }
}
