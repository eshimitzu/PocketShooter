using Heyworks.PocketShooter.VersionUtility.Versions;
using Heyworks.PocketShooter.VersionUtility.Versions.ScriptableObjects;
using UnityEditor;
#if UNITY_ANDROID
using UnityEditor.Android;
using GooglePlayGames.Editor;
#endif
#if UNITY_IOS
using UnityEditor.iOS;
#endif
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility
{
    public class ProjectSettingsManager
    {
        private const string AppMetricaPrefabPath = "Assets/ThirdParties/AppMetrica/AppMetrica.prefab";

        public static void WriteSettingsToEditor(Version version)
        {
            WriteCommonSettingsToEditor(version);
            if (version.PlatformName == PlatformName.Android)
            {
                WriteAndroidSettingsToEditor(version);
            }

            if (version.PlatformName == PlatformName.iOS)
            {
                WriteIOSSettingsToEditor(version);
            }
        }

        private static void WriteCommonSettingsToEditor(Version version)
        {
            if (version.CommonSettings == null)
            {
                version.CommonSettings = new CommonSettings();
            }

            var common = version.CommonSettings;
            PlayerSettings.companyName = common.CompanyName;
            PlayerSettings.productName = common.ProductName;

            if (version.CommonSettings.Identification == null)
            {
                version.CommonSettings.Identification = new Identification();
            }

            var identification = version.CommonSettings.Identification;
            PlayerSettings.bundleVersion = identification.BundleVersion;

            SetBundleIdentifier(version.CommonSettings.AppConfiguration);

            SetAppMetricaApiKeyToPrefab(common);
            
        }

        private static void SetBundleIdentifier(AppConfig appConfig)
        {
            var buildTargetGroups = System.Enum.GetValues(typeof(BuildTargetGroup));
            foreach (var buildTargetGroup in buildTargetGroups)
            {
                var group = (BuildTargetGroup)buildTargetGroup;

                if (group == BuildTargetGroup.Android
                    || group == BuildTargetGroup.iOS
                    || group == BuildTargetGroup.Standalone)
                {
                    PlayerSettings.SetApplicationIdentifier(group, appConfig.BundleId);
                }
            }
        }

        private static void SetAppMetricaApiKeyToPrefab(CommonSettings commonSettings)
        {
            GameObject appMetricaPrefab = PrefabUtility.LoadPrefabContents(AppMetricaPrefabPath);
            appMetricaPrefab.GetComponent<AppMetrica>().ApiKey = commonSettings.AppMetricaApiKey;
            PrefabUtility.SaveAsPrefabAsset(appMetricaPrefab, AppMetricaPrefabPath);
            PrefabUtility.UnloadPrefabContents(appMetricaPrefab);
        }

        private static void WriteAndroidSettingsToEditor(Version version)
        {
            if (version.AndroidSettings == null)
            {
                version.AndroidSettings = new AndroidSettings();
            }

            var androidSettings = version.AndroidSettings;
            WriteAndroidIconsToEditor(androidSettings);

            PlayerSettings.Android.splashScreenScale = AndroidSplashScreenScale.ScaleToFill;

            SetUpGooglePlayGameServices(androidSettings);
        }

        private static void WriteAndroidIconsToEditor(AndroidSettings androidSettings)
        {
#if UNITY_ANDROID
            AndroidIconsSO androidIconsSO = AssetDatabase.LoadAssetAtPath<AndroidIconsSO>(androidSettings.IconsScriptableObject);

            if (androidIconsSO == null)
            {
                return;
            }

            var androidPlatform = BuildTargetGroup.Android;

            var roundAndroidIcons = PlayerSettings.GetPlatformIcons(androidPlatform, AndroidPlatformIconKind.Round);
            roundAndroidIcons[0].SetTexture(GetTextureByPath(androidIconsSO.RoundIcon192x192));
            roundAndroidIcons[1].SetTexture(GetTextureByPath(androidIconsSO.RoundIcon144x144));
            roundAndroidIcons[2].SetTexture(GetTextureByPath(androidIconsSO.RoundIcon96x96));
            roundAndroidIcons[3].SetTexture(GetTextureByPath(androidIconsSO.RoundIcon72x72));
            roundAndroidIcons[4].SetTexture(GetTextureByPath(androidIconsSO.RoundIcon48x48));
            roundAndroidIcons[5].SetTexture(GetTextureByPath(androidIconsSO.RoundIcon36x36));
            PlayerSettings.SetPlatformIcons(androidPlatform, AndroidPlatformIconKind.Round, roundAndroidIcons);

            var legacyAndroidIcons = PlayerSettings.GetPlatformIcons(androidPlatform, AndroidPlatformIconKind.Legacy);
            legacyAndroidIcons[0].SetTexture(GetTextureByPath(androidIconsSO.LegacyIcon192x192));
            legacyAndroidIcons[1].SetTexture(GetTextureByPath(androidIconsSO.LegacyIcon144x144));
            legacyAndroidIcons[2].SetTexture(GetTextureByPath(androidIconsSO.LegacyIcon96x96));
            legacyAndroidIcons[3].SetTexture(GetTextureByPath(androidIconsSO.LegacyIcon72x72));
            legacyAndroidIcons[4].SetTexture(GetTextureByPath(androidIconsSO.LegacyIcon48x48));
            legacyAndroidIcons[5].SetTexture(GetTextureByPath(androidIconsSO.LegacyIcon36x36));
            PlayerSettings.SetPlatformIcons(androidPlatform, AndroidPlatformIconKind.Legacy, legacyAndroidIcons);

            PlatformIcon[] adaptiveAndroidIcons = PlayerSettings.GetPlatformIcons(androidPlatform, AndroidPlatformIconKind.Adaptive);
            Texture2D[][] textures = new Texture2D[][]
            {
                new Texture2D[] { GetTextureByPath(androidIconsSO.AdaptiveIconBackground432x432), GetTextureByPath(androidIconsSO.AdaptiveIconForeground432x432) },
                new Texture2D[] { GetTextureByPath(androidIconsSO.AdaptiveIconBackground324x324), GetTextureByPath(androidIconsSO.AdaptiveIconForeground324x324) },
                new Texture2D[] { GetTextureByPath(androidIconsSO.AdaptiveIconBackground216x216), GetTextureByPath(androidIconsSO.AdaptiveIconForeground216x216) },
                new Texture2D[] { GetTextureByPath(androidIconsSO.AdaptiveIconBackground162x162), GetTextureByPath(androidIconsSO.AdaptiveIconForeground162x162) },
                new Texture2D[] { GetTextureByPath(androidIconsSO.AdaptiveIconBackground108x108), GetTextureByPath(androidIconsSO.AdaptiveIconForeground108x108) },
                new Texture2D[] { GetTextureByPath(androidIconsSO.AdaptiveIconBackground81x81), GetTextureByPath(androidIconsSO.AdaptiveIconForeground81x81) },
            };
            for (var i = 0; i < adaptiveAndroidIcons.Length; i++)
            {
                adaptiveAndroidIcons[i].SetTextures(textures[i]);
            }

            PlayerSettings.SetPlatformIcons(androidPlatform, AndroidPlatformIconKind.Adaptive, adaptiveAndroidIcons);
#endif
        }

        private static void SetUpGooglePlayGameServices(AndroidSettings androidSettings)
        {
 #if UNITY_ANDROID
            try
            {
                GPGSProjectSettings settings = GPGSProjectSettings.Instance;

                settings.Set(GPGSUtil.APPIDKEY, androidSettings.GooglePlayAppId);
                settings.Set(GPGSUtil.WEBCLIENTIDKEY, androidSettings.GooglePlayWebClientId);
                settings.Set(GPGSUtil.ANDROIDRESOURCEKEY, androidSettings.GooglePlayResourcesXML);
                settings.Set(GPGSUtil.ANDROIDBUNDLEIDKEY, PlayerSettings.applicationIdentifier);
                settings.Set(GPGSUtil.ANDROIDSETUPDONEKEY, true);

                GPGSAndroidSetupUI.PerformSetup(androidSettings.GooglePlayWebClientId, "/Assets/Temp/", "GooglePlayConstants", androidSettings.GooglePlayResourcesXML, null);
            }
            catch
            {
                throw new System.Exception("Error applying GPGS settings to editor!");
            }
#endif
        }

        private static void WriteIOSSettingsToEditor(Version version)
        {
            if (version.IOSSettings == null)
            {
                version.IOSSettings = new IOSSettings();
            }

            WriteIOSIconsToEditor(version.IOSSettings);
        }

        private static void WriteIOSIconsToEditor(IOSSettings iosSettings)
        {
#if UNITY_IOS
            IOSIconsSO iOSIconsSO = AssetDatabase.LoadAssetAtPath<IOSIconsSO>(iosSettings.IconsScriptableObject);
            if (iOSIconsSO == null)
            {
                return;
            }

            var iOSPlatform = BuildTargetGroup.iOS;

            var applicationIOSIcons = PlayerSettings.GetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Application);
            applicationIOSIcons[0].SetTexture(GetTextureByPath(iOSIconsSO.ApplicationiPhoneIcon180x180));
            applicationIOSIcons[1].SetTexture(GetTextureByPath(iOSIconsSO.ApplicationiPhoneIcon120x120));
            applicationIOSIcons[2].SetTexture(GetTextureByPath(iOSIconsSO.ApplicationiPadIcon167x167));
            applicationIOSIcons[3].SetTexture(GetTextureByPath(iOSIconsSO.ApplicationiPadIcon152x152));
            applicationIOSIcons[4].SetTexture(GetTextureByPath(iOSIconsSO.ApplicationiPadIcon76x76));
            PlayerSettings.SetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Application, applicationIOSIcons);

            var spotlightIOSIcons = PlayerSettings.GetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Spotlight);
            spotlightIOSIcons[0].SetTexture(GetTextureByPath(iOSIconsSO.SpotlightiPhoneIcon120x120));
            spotlightIOSIcons[1].SetTexture(GetTextureByPath(iOSIconsSO.SpotlightiPhoneIcon80x80));
            spotlightIOSIcons[2].SetTexture(GetTextureByPath(iOSIconsSO.SpotlightiPadIcon80x80));
            spotlightIOSIcons[3].SetTexture(GetTextureByPath(iOSIconsSO.SpotlightiPadIcon40x40));
            PlayerSettings.SetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Spotlight, spotlightIOSIcons);

            var settingsIOSIcons = PlayerSettings.GetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Settings);
            settingsIOSIcons[0].SetTexture(GetTextureByPath(iOSIconsSO.SettingsiPhoneIcon87x87));
            settingsIOSIcons[1].SetTexture(GetTextureByPath(iOSIconsSO.SettingsiPhoneIcon58x58));
            settingsIOSIcons[2].SetTexture(GetTextureByPath(iOSIconsSO.SettingsiPhoneIcon29x29));
            settingsIOSIcons[3].SetTexture(GetTextureByPath(iOSIconsSO.SettingsiPadIcon58x58));
            settingsIOSIcons[4].SetTexture(GetTextureByPath(iOSIconsSO.SettingsiPadIcon29x29));
            PlayerSettings.SetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Settings, settingsIOSIcons);

            var notificationIOSIcons = PlayerSettings.GetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Notification);
            notificationIOSIcons[0].SetTexture(GetTextureByPath(iOSIconsSO.NotificationsiPhoneIcon60x60));
            notificationIOSIcons[1].SetTexture(GetTextureByPath(iOSIconsSO.NotificationsiPhoneIcon40x40));
            notificationIOSIcons[2].SetTexture(GetTextureByPath(iOSIconsSO.NotificationsiPadIcon40x40));
            notificationIOSIcons[3].SetTexture(GetTextureByPath(iOSIconsSO.NotificationsiPadIcon20x20));
            PlayerSettings.SetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Notification, notificationIOSIcons);

            var appStoreIcon = PlayerSettings.GetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Marketing);
            appStoreIcon[0].SetTexture(GetTextureByPath(iOSIconsSO.AppStoreIcon1024x1024));
            PlayerSettings.SetPlatformIcons(iOSPlatform, iOSPlatformIconKind.Marketing, appStoreIcon);
#endif
        }

        private static Texture2D GetTextureByPath(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
            return asset as Texture2D;
        }
    }
}