using Heyworks.PocketShooter.Communication;
using UnityEngine;

namespace Heyworks.PocketShooter.Configuration
{
    /// <summary>
    /// Provides game configurations.
    /// Initialized from code by default.
    /// Overridden by Assets\Settings\Resources if exists.
    /// Overridden by Editor if exists.
    /// Overridden by latest used if exists.
    /// </summary>
    public sealed class ConfigurationProvider
    {
        private const string DefaultConfigurationFilePostfix = ".default";

        private const string AppConfigurationFile = nameof(AppConfiguration);

        private static ServerAddress? debugServerAddress;

        /// <summary>
        /// Gets meta configuration.
        /// </summary>
        public IAppConfiguration AppConfiguration => LoadConfiguration();

        public static void SetDebugServerAddress(in ServerAddress address)
        {
            debugServerAddress = address;
        }

        private static AppConfiguration LoadConfiguration()
        {
            var config = LoadConfigurationFromFile(AppConfigurationFile);
            if (debugServerAddress != null)
            {
                config.SetDebugServerAddress(debugServerAddress.Value);
            }

            return config;
        }

        private static AppConfiguration LoadConfigurationFromFile(string configurationFile)
        {
            var file = Resources.Load<TextAsset>(configurationFile);
            if (file)
            {
                return JsonUtility.FromJson<AppConfiguration>(file.text);
            }

            file = Resources.Load<TextAsset>(configurationFile + DefaultConfigurationFilePostfix);
            if (file)
            {
                return JsonUtility.FromJson<AppConfiguration>(file.text);
            }

            throw new System.IO.FileNotFoundException($"Can't load configuration file {configurationFile} or {configurationFile + DefaultConfigurationFilePostfix}.");
        }
    }
}