using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Entities;
using Heyworks.PocketShooter.Realtime.Data;
using MoreLinq;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Communication
{
    // no sure if this must be grain at all
    public class BotsGrain : Grain, IBotsGrain
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IConfigurationsProvider configurationsProvider;

        public BotsGrain(IDateTimeProvider dateTimeProvider, IConfigurationsProvider configurationsProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
            this.configurationsProvider = configurationsProvider;
        }

        public async Task<Immutable<PlayerInfo[]>> GetBotPrototypes(int numberOfBots, int level, bool useConfiguration)
        {
            // note: stream should provide config into all grains instead of costly calls (will be simplier - see so many layers above config as of now)
            var armyConfiguration = await configurationsProvider.GetArmyConfiguration("default");
            var trooperConfiguration = await configurationsProvider.GetTrooperConfiguration("default");
            var weaponConfiguration = await configurationsProvider.GetWeaponConfiguration("default");
            var helmetConfiguration = await configurationsProvider.GetHelmetConfiguration("default");
            var armorConfiguration = await configurationsProvider.GetArmorConfiguration("default");
            var skillConfiguration = await configurationsProvider.GetSkillConfiguration("default");
            var classGrades = await configurationsProvider.GetDefaultGrades();

            // not so much configs so linear search is fast
            var config = (await configurationsProvider.GetBotsTrain()).SingleOrDefault(x => x.Level == level) ?? new BotsTrainConfig();

            var list = new List<PlayerInfo>(numberOfBots);
            for (int i = 0; i < numberOfBots; i++)
            {
                var playerId = Guid.NewGuid();
                var armyState = new ArmyGenerator().Generate(playerId, level, config, classGrades);

                var army = new ServerArmy(
                    armyState,
                    dateTimeProvider,
                    armyConfiguration,
                    trooperConfiguration,
                    weaponConfiguration,
                    helmetConfiguration,
                    armorConfiguration,
                    skillConfiguration);

                var info = new PlayerInfo(playerId, DefaultNames.PlayerNames[i], army.Troopers.Select(_ => _.GetTooperInfo()).ToArray(), new ConsumablesInfo(25, 25));
                foreach (var trooper in info.Troopers)
                {
                    trooper.MaxArmor = (int)(config.ProtectionFactorPercent * trooper.MaxArmor);
                    trooper.MaxHealth = (int)(config.ProtectionFactorPercent * trooper.MaxHealth);
                    trooper.Weapon.Damage = config.DamageFactorPercent * trooper.Weapon.Damage;
                }

                list.Add(info);
            }

            return list.ToArray().AsImmutable();
        }
    }
}