using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;
using Orleans.Concurrency;
using Orleans.Runtime.Services;

namespace Heyworks.PocketShooter.Meta.Services
{
    public class ConfigurationServiceClient : GrainServiceClient<IConfigurationGrainService>,
        IConfigurationServiceClient,
        IConfigurationsProvider
    {
        public ConfigurationServiceClient(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public Task<Immutable<ServerGameConfig>> GetGameConfig(string key) => GrainService.GetGameConfig(key);

        public Task<Immutable<(DominationModeConfig, IList<DominationMapConfig>)>> GetDominationModeConfig() => GrainService.GetDominationModeConfig();

        async Task<IPlayerConfiguration> IConfigurationsProvider.GetPlayerConfiguration(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return new PlayerConfiguration(gameConfig.Value);
        }

        async Task<IArmyConfigurationBase> IConfigurationsProvider.GetArmyConfiguration(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return new ArmyConfigurationBase(gameConfig.Value);
        }

        async Task<ITrooperConfiguration> IConfigurationsProvider.GetTrooperConfiguration(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return new TrooperConfiguration(gameConfig.Value);
        }

        async Task<IWeaponConfiguration> IConfigurationsProvider.GetWeaponConfiguration(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return new WeaponConfiguration(gameConfig.Value);
        }

        async Task<IHelmetConfiguration> IConfigurationsProvider.GetHelmetConfiguration(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return new HelmetConfiguration(gameConfig.Value);
        }

        async Task<IArmorConfiguration> IConfigurationsProvider.GetArmorConfiguration(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return new ArmorConfiguration(gameConfig.Value);
        }

        async Task<ISkillConfiguration> IConfigurationsProvider.GetSkillConfiguration(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return new SkillConfiguration(gameConfig.Value);
        }

        async Task<IShopConfigurationBase> IConfigurationsProvider.GetShopConfiguration(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return new ShopConfigurationBase(gameConfig.Value);
        }

        async Task<ICheatsConfiguration> IConfigurationsProvider.GetCheatsConfiguration(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return new CheatsConfiguration(gameConfig.Value);
        }

        async Task<IDominationModeConfiguration> IConfigurationsProvider.GetDominationModeConfiguration()
        {
            var dominationModeConfig = await GetDominationModeConfig();
            return new DominationModeConfiguration(dominationModeConfig.Value.Item1);
        }

        public async Task<(MatchMakingConfiguration,  IList<MapsSelectorConfig>)> GetMatchMakingConfiguration()
        {
            var config = await GetMatchMakingConfig();
            return config.Value;
        }

        public Task<Immutable<(MatchMakingConfiguration, IList<MapsSelectorConfig>)>> GetMatchMakingConfig() =>
            GrainService.GetMatchMakingConfig();

        Task<Immutable<IList<BotsTrainConfig>>> GetBotsTrain() => GrainService.GetBotsTrain();

        Task<Immutable<GradesDefaultsData>> IConfigurationGrainService.GetDefaultGrades() =>
            GrainService.GetDefaultGrades();

        public async Task<GradesDefaultsData> GetDefaultGrades() =>
            (await GrainService.GetDefaultGrades()).Value;

        Task<Immutable<IList<BotsTrainConfig>>> IConfigurationGrainService.GetBotsTrain() => GrainService.GetBotsTrain();

        async Task<IList<BotsTrainConfig>> IConfigurationsProvider.GetBotsTrain() =>
            (await GrainService.GetBotsTrain()).Value;
    }
}
