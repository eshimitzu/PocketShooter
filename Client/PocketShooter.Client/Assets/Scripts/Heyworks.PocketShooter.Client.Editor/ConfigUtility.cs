using Heyworks.PocketShooter.Modules.GameEnvironment;
using UnityEditor;
using UnityEngine;

namespace Heyworks.PocketShooter
{
    public static class ConfigUtility
    {
        [MenuItem("Tools/Update Config")]
        public static void Execute()
        {
            var spawner = Object.FindObjectOfType<Spawner>();
            spawner.UpdateConfig();

            var zoneConfig = Object.FindObjectOfType<ZoneConfig>();
            zoneConfig.UpdateConfig();
        }
    }
}