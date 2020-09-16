using System;
using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Communication;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Data;
using Orleans;
using Orleans.Concurrency;

namespace Heyworks.PocketShooter.Meta.Services
{
    public class ConfigGrain : Grain, IConfigGrain, IDominationModeInfoGrain
    {
        private readonly IConfigurationServiceClient configurationService;

        public ConfigGrain(IConfigurationServiceClient configurationService)
        {
            this.configurationService = configurationService;
        }

        public async Task<Version> GetGameConfigVersion(string key)
        {
            var gameConfig = await GetGameConfig(key);
            return gameConfig.Value.Version;
        }

        public Task<Immutable<ServerGameConfig>> GetGameConfig(string key) => configurationService.GetGameConfig(key);

        async Task<Immutable<DominationModeInfo>> IDominationModeInfoGrain.GetDominationModeInfo(MapNames mapName)
        {
            var (dominationModeConfig, maps) = (await configurationService.GetDominationModeConfig()).Value;
            var map = maps.SingleOrError(_ => _.MapName == mapName, $"{nameof(maps)} does not contain one {mapName}");
            var teamsInfo =
                    map.TeamsConfig
                        .Select(t => new TeamInfo(t.TeamNo,
                                                   t.SpawnPoints
                                                    .Select(_ => new SpawnPointInfo(_.X, _.Y, _.Z, _.Yaw))
                                                    .ToArray()))
                        .ToList();

            var zonesInfo = map.ZonesConfig
                                .Select(_ => new DominationZoneInfo(_.Id, _.X, _.Y, _.Z, _.Radius))
                                .ToList();

            var dominationModeInfo = new DominationModeInfo(
                dominationModeConfig.MaxPlayers,
                Realtime.Constants.ToTicks(dominationModeConfig.RespawnTimeMs),
                dominationModeConfig.TimeplayersToCapture,
                Realtime.Constants.ToTicks(dominationModeConfig.CheckIntervalMs),
                dominationModeConfig.WinScore,
                Realtime.Constants.ToTicks(dominationModeConfig.GameDurationMs),
                new GameArmorInfo(dominationModeConfig.GameArmorConfig.Impact, dominationModeConfig.GameArmorConfig.DamageFactor),
                new DominationMapInfo(teamsInfo.ToArray(), zonesInfo.ToArray()));

            return dominationModeInfo.AsImmutable();
        }

        public async Task<int> GetModeLimits()
        {
            var (dominationModeConfig, _) = (await configurationService.GetDominationModeConfig()).Value;
            return dominationModeConfig.MaxPlayers;
        }
    }
}
