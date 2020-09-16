using System;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;

namespace Heyworks.PocketShooter.Meta.Services
{
    internal class GameFactory : IGameFactory
    {
        private readonly IPlayerFactory playerFactory;
        private readonly IArmyFactory armyFactory;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IConfigurationsProvider configurationsProvider;

        public GameFactory(IPlayerFactory playerFactory, IArmyFactory armyFactory, IDateTimeProvider dateTimeProvider, IConfigurationsProvider configurationsProvider)
        {
            this.playerFactory = playerFactory;
            this.armyFactory = armyFactory;
            this.dateTimeProvider = dateTimeProvider;
            this.configurationsProvider = configurationsProvider;
        }

        public async Task<Game> Create(Guid playerId)
        {
            var player = await playerFactory.Create(playerId);
            var army = await armyFactory.Create(player);

            var shopConfiguration = await configurationsProvider.GetShopConfiguration(player.Group);

            return new Game(player, army, new Shop(army, shopConfiguration), dateTimeProvider);
        }
    }
}
