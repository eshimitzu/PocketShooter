using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Entities;
using Orleans;

namespace Heyworks.PocketShooter.Meta.Services
{
    internal class ArmyFactory : IArmyFactory
    {
        private readonly IGrainFactory grainFactory;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IConfigurationsProvider configurationsProvider;

        public ArmyFactory(IGrainFactory grainFactory, IDateTimeProvider dateTimeProvider, IConfigurationsProvider configurationsProvider)
        {
            this.grainFactory = grainFactory;
            this.dateTimeProvider = dateTimeProvider;
            this.configurationsProvider = configurationsProvider;
        }

        public async Task<ServerArmy> Create(ServerPlayer player)
        {
            var armyConfiguration = await configurationsProvider.GetArmyConfiguration(player.Group);
            var trooperConfiguration = await configurationsProvider.GetTrooperConfiguration(player.Group);
            var weaponConfiguration = await configurationsProvider.GetWeaponConfiguration(player.Group);
            var helmetConfiguration = await configurationsProvider.GetHelmetConfiguration(player.Group);
            var armorConfiguration = await configurationsProvider.GetArmorConfiguration(player.Group);
            var skillConfiguration = await configurationsProvider.GetSkillConfiguration(player.Group);

            var armyGrain = grainFactory.GetGrain<IArmyGrain>(player.Id);
            var armyState = await armyGrain.GetState();

            return new ServerArmy(
                armyState,
                dateTimeProvider,
                armyConfiguration,
                trooperConfiguration,
                weaponConfiguration,
                helmetConfiguration,
                armorConfiguration,
                skillConfiguration);
        }
    }
}
