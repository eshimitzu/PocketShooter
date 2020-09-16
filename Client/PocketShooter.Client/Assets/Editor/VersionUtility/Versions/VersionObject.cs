using UnityEngine;

namespace Heyworks.PocketShooter.VersionUtility.Versions
{
    public class VersionObject : ScriptableObject
    {
        public CommonSettings CommonSettings;
        public AndroidSettings AndroidSettings;
        public IOSSettings IOSSettings;
    }
}