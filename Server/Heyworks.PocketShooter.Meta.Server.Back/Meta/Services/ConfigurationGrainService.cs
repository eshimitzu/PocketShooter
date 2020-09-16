using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Realtime.Configuration.Data;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Orleans;
using Orleans.Concurrency;
using Orleans.Core;
using Orleans.Runtime;

namespace Heyworks.PocketShooter.Meta.Services
{
    // does procedures of version update of configs (only new or newer version configs can be saved in shared storage)
    // provides cached values from config
    // updates configs upon modification event in source (publish by game designers)
    public class ConfigurationGrainService : GrainService, IConfigurationGrainService
    {
        private const string ConfigsCollectionName = "GameConfig";

        private IReadOnlyDictionary<string, ServerGameConfig> cache = new Dictionary<string, ServerGameConfig>();
        private IGrainFactory grainFactory;
        private ILogger<ConfigurationGrainService> logger;
        private GameConfigProvider gameConfigProvider;
        private IDisposable onConfigsChanged;
        private readonly IMongoDatabase database;

        public ConfigurationGrainService(
            IGrainIdentity id,
            Silo silo,
            ILoggerFactory loggerFactory,
            GameConfigProvider gameConfigProvider,
            IGrainFactory grainFactory,
            IMongoDatabase database)
            : base(id, silo, loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<ConfigurationGrainService>();
            this.gameConfigProvider = gameConfigProvider;
            this.grainFactory = grainFactory;
            this.database = database;
        }

        public override async Task Start()
        {
            onConfigsChanged = gameConfigProvider.SubscribeOnGameConfigsChanged(async x => await OnGameConfigChanged(x), null);
            await UpdateConfigs();

            await base.Start();
        }

        public override Task Stop()
        {
            onConfigsChanged?.Dispose();
            return base.Stop();
        }

        public Task<Immutable<ServerGameConfig>> GetGameConfig(string key) =>
            Task.FromResult(cache[key].AsImmutable());

        public Task<Immutable<(DominationModeConfig, IList<DominationMapConfig>)>> GetDominationModeConfig()
        {
            var mode = cache["default"].RealtimeConfig.DominationModeConfig;
            var maps = cache["default"].RealtimeConfig.Maps;
            return Task.FromResult((mode, maps).AsImmutable());
        }

        // https://stackoverflow.com/questions/25017219/how-to-check-if-collection-exists-in-mongodb-using-c-sharp-driver
        private static async Task<bool> CollectionExistsAsync(IMongoDatabase self, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = await self.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
            return await collections.AnyAsync();
        }

        private async Task UpdateConfigs()
        {
            var configsCollection = database.GetCollection<StoredServerGameConfig>(ConfigsCollectionName);
            var configsFromFiles = await gameConfigProvider.ReadGameConfigs();
            var configsToStore = configsFromFiles.Select(x => new StoredServerGameConfig(x.Key, x.Value));

            // if data does not exists in database - store all configs
            // if exists - add new or overwrite with newer version
            if (!await CollectionExistsAsync(database, ConfigsCollectionName))
            {
                await configsCollection.InsertManyAsync(configsToStore);
            }
            else
            {
                foreach (var newConfig in configsToStore)
                {
                    var oldVersion = await configsCollection
                                            .Find(x => x.Id == newConfig.Id)
                                            .Project(x => new { version = x.Config.Version }).SingleOrDefaultAsync();
                    if (oldVersion == null)
                        await configsCollection.InsertOneAsync(newConfig);
                    else if (newConfig.Config.Version > oldVersion.version)
                        await configsCollection.ReplaceOneAsync(x => x.Id == newConfig.Id, newConfig);
                    else
                        logger.LogInformation("Trying to update with already existing {Version} of {ConfigName}", newConfig.Config.Version, newConfig.Id);
                }
            }

            // read from database
            // - the future path with multiple machines in cluster
            // - ensure we have classes cross compatible across while stack (if no - then decide what to do)
            // - allow to introspect current runtime via database
            var reRead = await (await configsCollection.FindAsync(x => true)).ToListAsync();
            reRead.ForEach(_ => _.Config.BuildIndexes());

            cache = reRead
                .Select(x => new KeyValuePair<string, ServerGameConfig>(x.Id, x.Config))
                .ToImmutableDictionary();

            Validate(cache);
        }

        // ensure mongo stores valid config (in future can make schema work in mongo)
        [Conditional("DEBUG")]
        private void Validate(IReadOnlyDictionary<string, ServerGameConfig> configs)
        {
            foreach (var item in configs)
            {
                item.Value.RealtimeConfig.DominationModeConfig.NotNull();
            }
        }

        private Task previousUpdate = Task.CompletedTask;
        private CancellationTokenSource previousTokenSource = new CancellationTokenSource();

        private async Task OnGameConfigChanged(object obj)
        {
            logger.LogInformation("Game configuration source was changed");
            // if allow to run SCP to copy files it hands - locks on files
            // so until files are coming we delay and cancel previous files handling (usual timer or other means do not help)
            // could find such pattern implemented in Orleans or handle file one by one and only new (diff with database-cache)
            // other means (store id database or via cluster need more stable environment and longer to achieve)
            await previousUpdate;

            previousUpdate = this.ScheduleTask(async () =>
                            {
                                previousTokenSource?.Cancel();
                                previousTokenSource?.Dispose();
                                previousTokenSource = new CancellationTokenSource();
                                var token = previousTokenSource.Token;
                                await Task.Delay(1000);
                                if (!token.IsCancellationRequested)
                                {
                                    var newNotification = gameConfigProvider.SubscribeOnGameConfigsChanged(async x => await OnGameConfigChanged(x), null);
                                    onConfigsChanged?.Dispose();
                                    onConfigsChanged = newNotification;
                                    try
                                    {
                                        await UpdateConfigs();
                                    }
                                    catch (AggregateException ex)
                                    {
                                        logger.LogCritical(ex.Flatten(), "Failed to update game configuration during runtime");
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogCritical(ex, "Failed to update game configuration during runtime");
                                    }
                                }
                            });
        }

        public Task<Immutable<(MatchMakingConfiguration, IList<MapsSelectorConfig>)>> GetMatchMakingConfig()
        {
            var match = cache["default"].MatchMaking;
            var selectors = cache["default"].MapSelectors;
            return Task.FromResult((match, selectors).AsImmutable());
        }

        // ISSUE: having task here indicated wrong design
        public Task<Immutable<IList<BotsTrainConfig>>> GetBotsTrain()
        {
            var data = cache["default"].BotsTrainsConfig;
            return Task.FromResult(data.AsImmutable());
        }

        public Task<Immutable<GradesDefaultsData>> GetDefaultGrades()
        {
            var defaultConfig = cache["default"];
            // as of now there are several troopers of same class but different grade, but may need to take lowest grade possible and talk to game designed about this
            var troopers = defaultConfig.TrooperGradesConfig.Select(x => (x.Class, x.Grade)).ToList();
            var weapons = defaultConfig.WeaponGradesConfig.Select(x => (x.Name, x.Grade)).ToList();
            var helmets = defaultConfig.HelmetGradesConfig.Select(x => (x.Name, x.Grade)).ToList();
            var armors = defaultConfig.ArmorGradesConfig.Select(x => (x.Name, x.Grade)).ToList();

            return Task.FromResult(new GradesDefaultsData { Troopers = troopers, Weapons = weapons, Helmets = helmets, Armors = armors }.AsImmutable());
        }
    }
}
