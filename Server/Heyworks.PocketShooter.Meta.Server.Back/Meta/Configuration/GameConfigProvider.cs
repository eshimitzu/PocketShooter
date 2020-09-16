using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Heyworks.PocketShooter.Meta.Configuration.Data;
using Heyworks.PocketShooter.Meta.Serialization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NJsonSchema.Validation;

namespace Heyworks.PocketShooter.Meta.Configuration
{
    // Read and runtime schema validate(currently running code) game mechanics config from file storage.
    // Any exceptions from this class are critical.
    // Does:
    // - provides default configs 
    // - allows to check new-latest configs appeared in file storage (like Mongo file storage or SCP directory)
    // - provides only schema(no check for whole semantics) valid configuration
    // Does NOT:
    // - allows upper layer to decide what to do if new bad config added and how to share new config with cluster
    // - does not touches database or state of running actors
    public class GameConfigProvider
    {
        // deployment and development wide constants, dot not change
        private const string gameConfigDirectory = ".gameconfigs";
        private const string defaultConfig = "pocketshooter.json";
        private const string configSuffix = ".pocketshooter.json";

        private readonly IHostingEnvironment environment;
        private readonly ILogger<GameConfigProvider> logger;

        private JsonSchema4 schema;

        public GameConfigProvider(IHostingEnvironment environment, ILogger<GameConfigProvider> logger)
        {
            this.environment = environment;
            this.logger = logger;
        }

        public async Task<IReadOnlyDictionary<string, ServerGameConfig>> ReadGameConfigs()
        {
            logger.LogInformation($"Will use {environment.ContentRootPath} to read game configs");

            if (this.schema == null)
            {
                // we use schema file generated during build
                var schemaFile = environment.ContentRootFileProvider.GetFileInfo(Path.Combine(gameConfigDirectory, "pocketshooter.schema.json"));

                // ensure no locks-leaks, `use` of C# 8 to short hand
                using (var ss = schemaFile.CreateReadStream())
                {
                    using (var reader = new StreamReader(ss))
                    {
                        var rawSchemaBody = await reader.ReadToEndAsync();
                        this.schema = await JsonSchema4.FromJsonAsync(rawSchemaBody);
                    }
                }
            }

            var gameConfigs = GetGameConfigsDirectory(gameConfigDirectory);

            // read all configs, validate, all or nothing
            var result = new Dictionary<string, ServerGameConfig>();
            foreach (var file in gameConfigs)
            {
                if (file.Name.EndsWith(configSuffix) || file.Name.Equals(defaultConfig))
                {
                    var validationErrors = await ValidateGameConfig(file);
                    if (validationErrors.Count > 0)
                    {
                        var exceptions = validationErrors
                                            .Select(x => (x.Kind, x.LineNumber, x.LinePosition, x.Path, x.Property))
                                            .Select(Except.Error);
                        Throw.Aggregate(exceptions);
                    }

                    using (var stream = file.CreateReadStream())
                    {
                        using (var reader = new JsonTextReader(new StreamReader(stream)))
                        {
                            var settings = new DefaultSerializerSettings();
                            settings.Converters.Add(new StringEnumConverter());

                            var serializer = JsonSerializer.Create(settings);
                            if (file.Name.Equals(defaultConfig, StringComparison.InvariantCulture))
                            {
                                var config = serializer.Deserialize<ServerGameConfig>(reader);
                                result.Add("default", config);
                                logger.LogInformation("Have read game {name} configuration of {version}", "default", config.Version);
                            }
                            else
                            {
                                var configPrefix = file.Name.Replace(configSuffix, string.Empty);
                                var config = serializer.Deserialize<ServerGameConfig>(reader);
                                result.Add(configPrefix, config);
                                logger.LogInformation("Have read game {name} configuration of {version}", configPrefix, config.Version);
                            }
                        }
                    }
                }
            }

            if (result.Count == 0)
            {
                throw new ConfigurationException($"Have not found any configurations in {Path.Combine(environment.ContentRootPath, gameConfigDirectory)}");
            }

            return result;
        }

        private async Task<ICollection<ValidationError>> ValidateGameConfig(IFileInfo configFile)
        {
            using (var stream = configFile.CreateReadStream())
            {
                using (var reader = new JsonTextReader(new StreamReader(stream)))
                {
                    var config = await JToken.ReadFromAsync(reader);
                    return schema.Validate(config);
                }
            }
        }

        private IDirectoryContents GetGameConfigsDirectory(string gameConfigDirectory)
        {
            var gameConfigs = environment.ContentRootFileProvider.GetDirectoryContents(gameConfigDirectory);
            if (!gameConfigs.Exists)
            {
                throw new ConfigurationException($"Failed to find game configuration in {Path.Combine(environment.ContentRootPath, gameConfigDirectory)}");
            }

            return gameConfigs;
        }

        public IDisposable SubscribeOnGameConfigsChanged(Action<object> callback, object state)
        {
            var pathToMonitor = Path.Combine(gameConfigDirectory, "*" + defaultConfig);
            logger.LogInformation("Will monitor {GameConfigurationsDirectoryWatcher} for game configuration changes", pathToMonitor);
            var changeToken = environment.ContentRootFileProvider.Watch(pathToMonitor);
            if (!changeToken.ActiveChangeCallbacks)
            {
                Throw.NotImplemented("You are on non NTFS or some *nix?");
            }

            return changeToken.RegisterChangeCallback(callback, state);
        }
    }
}