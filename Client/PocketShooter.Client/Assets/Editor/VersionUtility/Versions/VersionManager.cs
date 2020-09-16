using System;

namespace Heyworks.PocketShooter.VersionUtility.Versions
{
    public class VersionManager
    {
        public VersionConfigSerializable CurrentVersionConfigSerializable { get; private set; }

        public string CurrentVersionConfigFilePath { get; private set; }

        public VersionName CurrentVersionName { get; private set; }

        public PlatformName CurrentPlatformName { get; private set; }

        private const string AppConfigurationPath = "/Settings/Resources/AppConfiguration.json";
        private const string TestingConfigPath = "/Settings/Versions/testing_versions_config.txt";
        private const string StagingConfigPath = "/Settings/Versions/staging_versions_config.txt";
        private const string DevConfigPath = "/Settings/Versions/demo_versions_config.txt";
        private const string ProdConfigPath = "/Settings/Versions/prod_versions_config.txt";

        public event Action CurrentVersionChanged;

        public VersionManager()
        {
            ReadCurrentVersionConfigFromFile(CurrentVersionName);
        }

        public void SetCurrentVersion(VersionName versionName, PlatformName platformName)
        {
            CurrentVersionName = versionName;
            CurrentPlatformName = platformName;

            ReadCurrentVersionConfigFromFile(CurrentVersionName);

            OnCurrentVersionChanged();
        }

        public Version GetCurrentVersion()
        {
           return GetCurrentVersion(CurrentVersionName, CurrentPlatformName);
        }

        private Version GetCurrentVersion(VersionName versionName, PlatformName currentPlatformName)
        {
            return CurrentVersionConfigSerializable.GetVersion(versionName, currentPlatformName);
        }

        public void SaveVersionConfiguration()
        {
            CurrentVersionConfigSerializable.Save(CurrentVersionConfigFilePath);
        }

        public void SaveAllVersionConfigurations()
        {
            foreach(VersionName versionName in (VersionName[])Enum.GetValues(typeof(VersionName)))
            {
                ReadCurrentVersionConfigFromFile(versionName);
                SaveVersionConfiguration();
            }
        }

        public void SaveAppConfiguration()
        {
            var currentAppConfig = GetCurrentVersion().CommonSettings.AppConfiguration;
            var serializable = new AppConfigSerializable(currentAppConfig);
            serializable.Save(AppConfigurationPath);
        }

        private void ReadCurrentVersionConfigFromFile(VersionName versionName)
        {
            CurrentVersionName = versionName;
            CurrentVersionConfigFilePath = GetConfigPath(versionName);
            CurrentVersionConfigSerializable = VersionConfigSerializable.GetFromFile(CurrentVersionConfigFilePath);

            if (CurrentVersionConfigSerializable == null)
            {
                CreateEmptyVersionConfigSerializable();
            }
        }

        private void CreateEmptyVersionConfigSerializable()
        {
            CurrentVersionConfigSerializable = new VersionConfigSerializable(CurrentVersionName, (PlatformName[])Enum.GetValues(typeof(PlatformName)));
            CurrentVersionConfigSerializable.Save(CurrentVersionConfigFilePath);
        }

        private string GetConfigPath(VersionName versionName)
        {
            switch (versionName)
            {
                case VersionName.Testing:
                    return TestingConfigPath;
                case VersionName.Staging:
                    return StagingConfigPath;
                case VersionName.Demo:
                    return DevConfigPath;
                case VersionName.Prod:
                    return ProdConfigPath;
                default:
                    throw new NotSupportedException($"Version {versionName} not supported.");
            }
        }

        public string GetAppConfigPath()
        {
            return AppConfigurationPath;
        }

        private void OnCurrentVersionChanged()
        {
            Action handler = CurrentVersionChanged;
            if (handler != null) handler();
        }
    }
}