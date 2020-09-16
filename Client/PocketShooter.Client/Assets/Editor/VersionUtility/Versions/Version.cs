using Heyworks.PocketShooter.PropertyAttributesAndDrawers;
using Heyworks.PocketShooter.VersionUtility.PropertyAttributesAndDrawers;
using System;

namespace Heyworks.PocketShooter.VersionUtility.Versions
{
    public enum VersionName
    {
        Testing,
        Staging,
        Demo,
        Prod
    }

    public enum PlatformName
    {
        iOS,
        Android
    }

    [Serializable]
    public class Version
    {
        public VersionName VersionName;
        public PlatformName PlatformName;
        public CommonSettings CommonSettings;
        public AndroidSettings AndroidSettings;
        public IOSSettings IOSSettings;

        public Version()
        {
            CommonSettings = new CommonSettings();
            AndroidSettings = new AndroidSettings();
            IOSSettings = new IOSSettings();
        }
    }

    [Serializable]
    public class CommonSettings
    {
        public string CompanyName;
        public string ProductName;
        public Identification Identification;
        public AppConfig AppConfiguration;
        public string StoreLink;
        public string AppMetricaApiKey;
        public string DataDogApiKey;

        public CommonSettings()
        {
            Identification = new Identification();
            AppConfiguration = new AppConfig();
        }
    }

    [Serializable]
    public class Identification
    {
        public string BundleVersion;
        public string ShortBundleVersion;
    }

    [Serializable]
    public class AppConfig
    {
        public string Version;
        public string MetaServerIp;
        public string MetaServerPort;
        public string BundleId;
    }

    [Serializable]
    public class AndroidSettings
    {
        [ScriptableObjectPath]
        public string IconsScriptableObject;
        public string GooglePlayAppId;
        public string GooglePlayWebClientId;
        [LongString]
        public string GooglePlayResourcesXML;
    }

    [Serializable]
    public class IOSSettings
    {
        [ScriptableObjectPath]
        public string IconsScriptableObject;
        public string GameCenter;
    }
}