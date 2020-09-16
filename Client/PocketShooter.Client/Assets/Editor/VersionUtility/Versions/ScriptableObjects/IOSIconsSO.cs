using Heyworks.PocketShooter.PropertyAttributesAndDrawers;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.Versions.ScriptableObjects
{
    [CreateAssetMenu(fileName = "IOSIconsSO", menuName = "Heyworks/Versions Utility/Create IOSIconsSO")]
    public class IOSIconsSO : ScriptableObject
    {
        [TexturePath]
        public string ApplicationiPhoneIcon180x180;
        [TexturePath]
        public string ApplicationiPhoneIcon120x120;
        [TexturePath]
        public string ApplicationiPadIcon167x167;
        [TexturePath]
        public string ApplicationiPadIcon152x152;
        [TexturePath]
        public string ApplicationiPadIcon76x76;

        [TexturePath]
        public string SpotlightiPhoneIcon120x120;
        [TexturePath]
        public string SpotlightiPhoneIcon80x80;
        [TexturePath]
        public string SpotlightiPadIcon80x80;
        [TexturePath]
        public string SpotlightiPadIcon40x40;

        [TexturePath]
        public string SettingsiPhoneIcon87x87;
        [TexturePath]
        public string SettingsiPhoneIcon58x58;
        [TexturePath]
        public string SettingsiPhoneIcon29x29;
        [TexturePath]
        public string SettingsiPadIcon58x58;
        [TexturePath]
        public string SettingsiPadIcon29x29;

        [TexturePath]
        public string NotificationsiPhoneIcon60x60;
        [TexturePath]
        public string NotificationsiPhoneIcon40x40;
        [TexturePath]
        public string NotificationsiPadIcon40x40;
        [TexturePath]
        public string NotificationsiPadIcon20x20;

        [TexturePath]
        public string AppStoreIcon1024x1024;
    }
}