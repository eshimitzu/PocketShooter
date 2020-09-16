using Heyworks.PocketShooter.PropertyAttributesAndDrawers;
using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.Versions.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AndroidIconsSO", menuName = "Heyworks/Versions Utility/Create AndroidIconsSO")]
    public class AndroidIconsSO : ScriptableObject
    {
        [TexturePath]
        public string AdaptiveIconForeground432x432;
        [TexturePath]
        public string AdaptiveIconBackground432x432;
        [TexturePath]
        public string AdaptiveIconForeground324x324;
        [TexturePath]
        public string AdaptiveIconBackground324x324;
        [TexturePath]
        public string AdaptiveIconForeground216x216;
        [TexturePath]
        public string AdaptiveIconBackground216x216;
        [TexturePath]
        public string AdaptiveIconForeground162x162;
        [TexturePath]
        public string AdaptiveIconBackground162x162;
        [TexturePath]
        public string AdaptiveIconForeground108x108;
        [TexturePath]
        public string AdaptiveIconBackground108x108;
        [TexturePath]
        public string AdaptiveIconForeground81x81;
        [TexturePath]
        public string AdaptiveIconBackground81x81;

        [TexturePath]
        public string RoundIcon192x192;
        [TexturePath]
        public string RoundIcon144x144;
        [TexturePath]
        public string RoundIcon96x96;
        [TexturePath]
        public string RoundIcon72x72;
        [TexturePath]
        public string RoundIcon48x48;
        [TexturePath]
        public string RoundIcon36x36;

        [TexturePath]
        public string LegacyIcon192x192;
        [TexturePath]
        public string LegacyIcon144x144;
        [TexturePath]
        public string LegacyIcon96x96;
        [TexturePath]
        public string LegacyIcon72x72;
        [TexturePath]
        public string LegacyIcon48x48;
        [TexturePath]
        public string LegacyIcon36x36;
    }
}