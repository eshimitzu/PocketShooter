using Heyworks.PocketShooter.CharacterSetupUtility;
using Heyworks.PocketShooter.VersionUtility;
using Heyworks.PocketShooter.VersionUtility.Versions;
using UnityEditor;

namespace Heyworks.PocketShooter
{
    public static class HeyworksEditorMenu
    {
        [MenuItem("Heyworks/Version Utility/Settings", false, 0)]
        private static void ShowVersionsSettingsWindow()
        {
            VersionsEditorWindow.Init();
        }

        [MenuItem("Heyworks/Version Utility/Refresh All Version Config JSONs", false, 0)]
        private static void RefreshAllConfigJSONs()
        {
            VersionManager versionManager = new VersionManager();
            versionManager.SaveAllVersionConfigurations();
        }

        [MenuItem("Heyworks/Character Setup Utility/Set up new character", false, 0)]
        private static void ShowCharacterSetupWindow()
        {
            CharacterSetupWindow.Init();
        }

        [MenuItem("Heyworks/Character Setup Utility/Update Skinned Mesh Bones")]
        private static void ShowUpdateSkinnedMeshBonesWindow()
        {
            UpdateSkinnedMeshWindow.Init();
        }

    }
}