using System.Collections.Generic;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;
using Orleans.Concurrency;
using Orleans.Services;

namespace Heyworks.PocketShooter.Meta.Services
{
    public interface IConfigurationGrainService : IGrainService
    {
        Task<Immutable<ServerGameConfig>> GetGameConfig(string key);

        Task<Immutable<(DominationModeConfig, IList<DominationMapConfig>)>> GetDominationModeConfig();

        Task<Immutable<(MatchMakingConfiguration, IList<MapsSelectorConfig>)>> GetMatchMakingConfig();

        Task<Immutable<GradesDefaultsData>> GetDefaultGrades();

        Task<Immutable<IList<BotsTrainConfig>>> GetBotsTrain();
    }
}
