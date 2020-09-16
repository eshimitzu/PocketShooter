using System.Collections.Generic;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration.Data;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    public interface IConfigurationsProvider
    {
        Task<IPlayerConfiguration> GetPlayerConfiguration(string key);

        Task<IArmyConfigurationBase> GetArmyConfiguration(string key);

        Task<ITrooperConfiguration> GetTrooperConfiguration(string key);

        Task<IWeaponConfiguration> GetWeaponConfiguration(string key);

        Task<IHelmetConfiguration> GetHelmetConfiguration(string key);

        Task<IArmorConfiguration> GetArmorConfiguration(string key);

        Task<ISkillConfiguration> GetSkillConfiguration(string key);

        Task<IShopConfigurationBase> GetShopConfiguration(string key);

        Task<ICheatsConfiguration> GetCheatsConfiguration(string key);

        Task<IDominationModeConfiguration> GetDominationModeConfiguration();

        Task<(MatchMakingConfiguration, IList<MapsSelectorConfig>)> GetMatchMakingConfiguration();

        Task<GradesDefaultsData> GetDefaultGrades();

        Task<IList<BotsTrainConfig>> GetBotsTrain();
    }
}
